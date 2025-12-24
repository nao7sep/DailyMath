using System;
using SkiaSharp;
using DailyMath.Core.Layout;
using DailyMath.Core.Rendering;

namespace DailyMath.Core.Skia;

/// <summary>
/// SkiaSharp implementation of text measurement for layout calculations.
/// Cross-platform text metrics using Skia rendering engine.
/// </summary>
public class SkiaTextMeasurer : ITextMeasurer
{
    // Skia uses an SKPaint object to define text properties (font, size, style)
    // We create a temporary one for measuring.

    /// <inheritdoc />
    public Measure MeasureText(string text, FontSpec font, double dpi)
    {
        if (string.IsNullOrEmpty(text))
            return new Measure(0.AsPixels(), 0.AsPixels());

        using var skFont = CreateFont(font, dpi);
        using var paint = new SKPaint();

        // 1. Measure Width
        // Skia measures accurate horizontal advance
        float width = skFont.MeasureText(text);

        // 2. Measure Height
        // We use FontMetrics to get the standard line height (Ascent + Descent)
        // This ensures the box is tall enough for the tallest characters in the font
        SKFontMetrics metrics = skFont.Metrics;
        float height = metrics.Descent - metrics.Ascent;
        // Note: Ascent is usually negative (up), Descent is positive (down).
        // So (Descent - Ascent) = (Positive - Negative) = Total Height.

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
        // 1. Resolve bounds to absolute pixels
        double maxWidthPx = bounds.Width.ToPixels(dpi: dpi);
        double maxHeightPx = bounds.Height.ToPixels(dpi: dpi);

        double low = minSizeInPoints;
        double high = maxSizeInPoints;

        // 2. Binary Search
        // Continue until precision is better than 0.01pt (sub-pixel accuracy at typical DPIs)
        while (high - low > 0.01)
        {
            double mid = (low + high) / 2.0;

            // Reuse MeasureText logic
            Measure measured = MeasureText(text, baseFont.WithSize(mid), dpi);

            // Note: measured.Width.Value and measured.Height.Value are in pixels
            // because MeasureText returns Measure with pixel Lengths
            if (measured.Width.Value <= maxWidthPx && measured.Height.Value <= maxHeightPx)
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
    /// Creates an SKFont configured with the correct font family, style, and size.
    /// Critical: Converts Points (Physical) to Pixels (Digital) using DPI.
    /// Note: Underline and Strikethrough decorations must be drawn manually in rendering code.
    /// </summary>
    private SKFont CreateFont(FontSpec spec, double dpi)
    {
        // 1. Convert 100-900 Weight to Skia Weight
        // Skia uses the same 100-900 scale, so we can cast directly!
        var skWeight = (SKFontStyleWeight)(int)spec.Weight;

        // 2. Convert Style to Skia Slant
        var skSlant = spec.Style.HasFlag(Core.Rendering.FontStyle.Italic)
            ? SKFontStyleSlant.Italic
            : SKFontStyleSlant.Upright;

        // 3. Load Typeface
        // SKFontStyle.Width.Normal is standard width (not condensed/expanded)
        var skStyle = new SKFontStyle(skWeight, SKFontStyleWidth.Normal, skSlant);
        var typeface = SKTypeface.FromFamilyName(spec.Family, skStyle);

        // 4. Convert Size (Points -> Pixels)
        // Formula: Pixels = Points * (DPI / 72)
        // SKFont expects Size in PIXELS.
        float pixelSize = (float)(spec.SizeInPoints * (dpi / 72.0));

        // 5. Create Font
        var font = new SKFont(typeface, pixelSize)
        {
            Subpixel = true
        };

        return font;
    }
}
