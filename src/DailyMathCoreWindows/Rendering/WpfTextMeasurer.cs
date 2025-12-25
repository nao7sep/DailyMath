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
    /// <summary>
    /// WPF's standard DIP (Device Independent Pixel) resolution.
    /// WPF internally uses 96 DPI as the baseline for device-independent units.
    /// </summary>
    private const double WpfDipResolution = 96.0;

    /// <summary>
    /// Converts FontWeight enum (100-900) to WPF FontWeight.
    /// </summary>
    public static System.Windows.FontWeight ConvertToFontWeight(Core.Rendering.FontWeight weight)
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
    public static System.Windows.FontStyle ConvertToFontStyle(Core.Rendering.FontStyle style)
    {
        if (style.HasFlag(Core.Rendering.FontStyle.Italic))
            return FontStyles.Italic;

        return FontStyles.Normal;
    }

    /// <summary>
    /// Converts font size from points to DIPs (Device Independent Pixels).
    /// FormattedText expects size in DIPs (WPF's 96 DPI baseline).
    /// Formula: DIPs = Points * (WpfDipResolution / PointsPerInch)
    /// </summary>
    public static double CalculateDipSize(double sizeInPoints)
    {
        return sizeInPoints * (WpfDipResolution / FontSpec.PointsPerInch);
    }

    /// <summary>
    /// Creates a Typeface from FontSpec (family, weight, style).
    /// Reusable for multiple font sizes.
    /// </summary>
    public static Typeface CreateTypeface(FontSpec spec)
    {
        return new Typeface(
            new FontFamily(spec.Family),
            ConvertToFontStyle(spec.Style),
            ConvertToFontWeight(spec.Weight),
            FontStretches.Normal
        );
    }

    /// <summary>
    /// Creates a FormattedText instance with the specified text and font.
    /// Converts Points to Pixels using DPI for accurate measurement.
    /// </summary>
    public static FormattedText CreateFormattedText(string text, FontSpec spec, double dpi)
    {
        var typeface = CreateTypeface(spec);
        double emSize = CalculateDipSize(spec.SizeInPoints);
        double pixelsPerDip = dpi / WpfDipResolution;

        return CreateFormattedText(text, typeface, emSize, pixelsPerDip, spec.Style);
    }

    /// <summary>
    /// Creates a FormattedText instance with pre-created typeface and DPI settings.
    /// Overload for performance when creating multiple FormattedText instances with the same typeface.
    /// </summary>
    public static FormattedText CreateFormattedText(string text, Typeface typeface, double emSize,
        double pixelsPerDip, Core.Rendering.FontStyle style)
    {
        var formattedText = new FormattedText(
            text,
            CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            typeface,
            emSize,
            Brushes.Black,
            pixelsPerDip
        );

        ApplyTextDecorations(formattedText, style);
        return formattedText;
    }

    /// <inheritdoc />
    public Measure MeasureText(string text, FontSpec font, double dpi)
    {
        if (string.IsNullOrEmpty(text))
            return new Measure(0.AsPixels(), 0.AsPixels());

        if (dpi <= 0 || double.IsNaN(dpi) || double.IsInfinity(dpi))
            throw new ArgumentOutOfRangeException(nameof(dpi), $"DPI must be a positive finite number. Got: {dpi}");

        var formattedText = CreateFormattedText(text, font, dpi);

        // Use Height (tight bounding box: ascent to descent) instead of Extent (includes line gap/leading).
        // This matches SkiaSharp's behavior where leading is intentionally excluded.
        // Spacing should be controlled via Element padding, not built into measurements.
        return new Measure(
            Math.Ceiling(formattedText.Width).AsPixels(),
            Math.Ceiling(formattedText.Height).AsPixels()
        );
    }

    /// <inheritdoc />
    public double GetMaxFontSize(string text, FontSpec baseFont, Measure bounds, double dpi,
        double minSizeInPoints = 6, double maxSizeInPoints = 72)
    {
        // Empty string has no size - return maximum since it fits in any bounds
        if (string.IsNullOrEmpty(text))
            return maxSizeInPoints;

        if (minSizeInPoints > maxSizeInPoints)
            throw new ArgumentException($"Minimum size ({minSizeInPoints}) cannot be greater than maximum size ({maxSizeInPoints}).");

        if (dpi <= 0 || double.IsNaN(dpi) || double.IsInfinity(dpi))
            throw new ArgumentOutOfRangeException(nameof(dpi), $"DPI must be a positive finite number. Got: {dpi}");

        // 1. Resolve bounds to absolute pixels for fast comparison
        double maxWidthPx = bounds.Width.ToPixels(dpi: dpi);
        double maxHeightPx = bounds.Height.ToPixels(dpi: dpi);

        // Validate resolved bounds
        if (maxWidthPx <= 0 || maxHeightPx <= 0 ||
            double.IsNaN(maxWidthPx) || double.IsNaN(maxHeightPx) ||
            double.IsInfinity(maxWidthPx) || double.IsInfinity(maxHeightPx))
        {
            throw new ArgumentException($"Bounds must resolve to positive finite pixels. Got: Width={maxWidthPx}px, Height={maxHeightPx}px");
        }

        double low = minSizeInPoints;
        double high = maxSizeInPoints;

        // Create typeface once for reuse (same family/weight/style throughout)
        var typeface = CreateTypeface(baseFont);
        double pixelsPerDip = dpi / WpfDipResolution;

        // 2. Binary Search (typically 12-13 iterations for 6-72pt range)
        // Continue until precision is better than 0.01pt (sub-pixel accuracy at typical DPIs)
        while (high - low > 0.01)
        {
            double mid = (low + high) / 2.0;
            double emSize = CalculateDipSize(mid);

            // Create FormattedText using helper (reusing typeface and pixelsPerDip for performance)
            var formattedText = CreateFormattedText(text, typeface, emSize, pixelsPerDip, baseFont.Style);

            // Use Height (tight bounding box) instead of Extent (includes leading) for consistency
            if (FitsInBounds(formattedText.Width, formattedText.Height, maxWidthPx, maxHeightPx))
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
    /// Applies text decorations (underline, strikethrough, overline) to FormattedText.
    /// </summary>
    private static void ApplyTextDecorations(FormattedText formattedText, Core.Rendering.FontStyle style)
    {
        var decorations = new TextDecorationCollection();
        if (style.HasFlag(Core.Rendering.FontStyle.Underline))
        {
            decorations.Add(TextDecorations.Underline);
        }
        if (style.HasFlag(Core.Rendering.FontStyle.Strikethrough))
        {
            decorations.Add(TextDecorations.Strikethrough);
        }
        if (style.HasFlag(Core.Rendering.FontStyle.Overline))
        {
            decorations.Add(TextDecorations.OverLine);
        }
        if (decorations.Count > 0)
        {
            formattedText.SetTextDecorations(decorations);
        }
    }

    /// <summary>
    /// Checks if measured text dimensions fit within specified bounds.
    /// </summary>
    private static bool FitsInBounds(double width, double height, double maxWidth, double maxHeight)
    {
        return Math.Ceiling(width) <= maxWidth && Math.Ceiling(height) <= maxHeight;
    }
}
