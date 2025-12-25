using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using DailyMath.Core.Layout;
using DailyMath.Core.Rendering;

namespace DailyMath.Core.Windows;

/// <summary>
/// Windows WPF implementation of text measurement for layout calculations.
/// Uses FormattedText for precise DirectWrite-based text metrics.
/// </summary>
public class WpfTextMeasurer : ITextMeasurer
{
    /// <inheritdoc />
    public Measure MeasureText(string text, FontSpec font, double dpi)
    {
        if (string.IsNullOrEmpty(text))
            return new Measure(0.AsPixels(), 0.AsPixels());

        var formattedText = CreateFormattedText(text, font, dpi);

        // Use Extent for tight bounding box (no leading/line gap)
        // This matches Skia's Descent - Ascent approach
        double width = formattedText.Width;
        double height = formattedText.Extent;

        // Round up to ensure the layout box is never smaller than the text
        return new Measure(
            Math.Ceiling(width).AsPixels(),
            Math.Ceiling(height).AsPixels()
        );
    }

    /// <inheritdoc />
    public double GetMaxFontSize(
        string text,
        FontSpec baseFont,
        Measure bounds,
        double dpi,
        double minSizeInPoints = 6,
        double maxSizeInPoints = 72)
    {
        // 1. Resolve bounds to absolute pixels for fast comparison
        double maxWidthPx = bounds.Width.ToPixels(dpi: dpi);
        double maxHeightPx = bounds.Height.ToPixels(dpi: dpi);

        double low = minSizeInPoints;
        double high = maxSizeInPoints;

        // 2. Binary Search (typically 12-13 iterations for 6-72pt range)
        // Continue until precision is better than 0.01pt (sub-pixel accuracy at typical DPIs)
        while (high - low > 0.01)
        {
            double mid = (low + high) / 2.0;

            // Measure with the candidate size
            var testFont = baseFont.WithSize(mid);
            var formattedText = CreateFormattedText(text, testFont, dpi);

            // Check if it fits (using tight bounds)
            bool fitsWidth = Math.Ceiling(formattedText.Width) <= maxWidthPx;
            bool fitsHeight = Math.Ceiling(formattedText.Extent) <= maxHeightPx;

            if (fitsWidth && fitsHeight)
            {
                // It fits! Try going larger.
                low = mid;
            }
            else
            {
                // Too big! Go smaller.
                high = mid;
            }
        }

        // Return the lower bound (guaranteed to fit)
        return low;
    }

    /// <summary>
    /// Creates a FormattedText instance with the specified text and font.
    /// Converts Points to Pixels using DPI for accurate measurement.
    /// </summary>
    private FormattedText CreateFormattedText(string text, FontSpec spec, double dpi)
    {
        // 1. Create Typeface from FontSpec
        var typeface = new Typeface(
            new FontFamily(spec.Family),
            ConvertToFontStyle(spec.Style),
            ConvertToFontWeight(spec.Weight),
            FontStretches.Normal
        );

        // 2. Convert Points to Pixels
        // FormattedText expects size in device-independent pixels (1/96 inch)
        // Formula: DIPs = Points * (96 / 72)
        double emSize = spec.SizeInPoints * (96.0 / FontSpec.PointsPerInch);

        // 3. Scale for actual DPI
        // pixelsPerDip converts from 96 DPI to target DPI
        double pixelsPerDip = dpi / 96.0;

        // 4. Create FormattedText
        var formattedText = new FormattedText(
            text,
            CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            typeface,
            emSize,
            Brushes.Black,
            pixelsPerDip
        );

        // 5. Apply text decorations
        if (spec.Style.HasFlag(Core.Rendering.FontStyle.Underline))
        {
            formattedText.SetTextDecorations(TextDecorations.Underline);
        }
        if (spec.Style.HasFlag(Core.Rendering.FontStyle.Strikethrough))
        {
            formattedText.SetTextDecorations(TextDecorations.Strikethrough);
        }

        return formattedText;
    }

    /// <summary>
    /// Converts FontWeight enum (100-900) to WPF FontWeight.
    /// </summary>
    private System.Windows.FontWeight ConvertToFontWeight(Core.Rendering.FontWeight weight)
    {
        return weight switch
        {
            Core.Rendering.FontWeight.Thin => FontWeights.Thin,
            Core.Rendering.FontWeight.ExtraLight => FontWeights.ExtraLight,
            Core.Rendering.FontWeight.Light => FontWeights.Light,
            Core.Rendering.FontWeight.Normal => FontWeights.Normal,
            Core.Rendering.FontWeight.Medium => FontWeights.Medium,
            Core.Rendering.FontWeight.SemiBold => FontWeights.SemiBold,
            Core.Rendering.FontWeight.Bold => FontWeights.Bold,
            Core.Rendering.FontWeight.ExtraBold => FontWeights.ExtraBold,
            Core.Rendering.FontWeight.Black => FontWeights.Black,
            _ => FontWeights.Normal
        };
    }

    /// <summary>
    /// Converts FontStyle flags to WPF FontStyle.
    /// </summary>
    private System.Windows.FontStyle ConvertToFontStyle(Core.Rendering.FontStyle style)
    {
        if (style.HasFlag(Core.Rendering.FontStyle.Italic))
            return FontStyles.Italic;

        return FontStyles.Normal;
    }
}
