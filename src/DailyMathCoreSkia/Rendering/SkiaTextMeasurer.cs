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

        // Create Typeface and Font explicitly to ensure both are Disposed
        using var typeface = CreateTypeface(font);

        // Convert Size (Points -> Pixels)
        // Formula: Pixels = Points * (DPI / PointsPerInch)
        // SKFont expects Size in PIXELS.
        float pixelSize = (float)(font.SizeInPoints * (dpi / FontSpec.PointsPerInch));

        using var skFont = new SKFont(typeface, pixelSize)
        {
            Subpixel = true
        };

        // 1. Measure Width
        // Skia measures accurate horizontal advance
        float width = skFont.MeasureText(text);

        // 2. Measure Height
        // We use Descent - Ascent for tight bounding box (no leading/line gap).
        // This gives precise glyph height, better for layout engines with explicit spacing control.
        // Note: Ascent is negative (up), Descent is positive (down).
        // Leading is intentionally excluded - spacing should be controlled via Element padding.
        SKFontMetrics metrics = skFont.Metrics;
        float height = metrics.Descent - metrics.Ascent;

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
        // Empty string has no size - return maximum since it fits in any bounds
        if (string.IsNullOrEmpty(text))
            return maxSizeInPoints;

        if (minSizeInPoints > maxSizeInPoints)
            throw new ArgumentException($"Minimum size ({minSizeInPoints}) cannot be greater than maximum size ({maxSizeInPoints}).");

        // 1. Resolve bounds to absolute pixels
        double maxWidthPx = bounds.Width.ToPixels(dpi: dpi);
        double maxHeightPx = bounds.Height.ToPixels(dpi: dpi);

        double low = minSizeInPoints;
        double high = maxSizeInPoints;

        // Create font once - we'll update its size in the loop
        using var typeface = CreateTypeface(baseFont);
        using var skFont = new SKFont(typeface)
        {
            Subpixel = true
        };

        // 2. Binary Search (typically 12-13 iterations for 6-72pt range)
        // Continue until precision is better than 0.01pt (sub-pixel accuracy at typical DPIs)
        while (high - low > 0.01)
        {
            double mid = (low + high) / 2.0;

            // Update font size for this iteration
            skFont.Size = (float)(mid * (dpi / FontSpec.PointsPerInch));

            // Measure with current size
            float width = skFont.MeasureText(text);
            SKFontMetrics metrics = skFont.Metrics;
            float height = metrics.Descent - metrics.Ascent;

            // Check if it fits
            bool fits = (Math.Ceiling(width) <= maxWidthPx) && (Math.Ceiling(height) <= maxHeightPx);

            if (fits)
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
    /// Creates an SKTypeface from FontSpec (family, weight, style).
    /// Reusable for multiple font sizes.
    /// </summary>
    public static SKTypeface CreateTypeface(FontSpec spec)
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
        return SKTypeface.FromFamilyName(spec.Family, skStyle);
    }
}
