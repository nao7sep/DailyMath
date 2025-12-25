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
    /// <summary>
    /// Converts font size from points to pixels based on DPI.
    /// Formula: Pixels = Points * (DPI / PointsPerInch)
    /// </summary>
    public static float CalculatePixelSize(double sizeInPoints, double dpi)
    {
        return (float)(sizeInPoints * (dpi / FontSpec.PointsPerInch));
    }

    /// <summary>
    /// Calculates tight bounding box height (no leading/line gap).
    /// Uses Descent - Ascent for precise glyph height.
    /// Note: Ascent is negative (up), Descent is positive (down).
    /// Leading is intentionally excluded - spacing should be controlled via Element padding.
    /// </summary>
    public static float CalculateHeight(SKFont font)
    {
        SKFontMetrics metrics = font.Metrics;
        return metrics.Descent - metrics.Ascent;
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

    /// <summary>
    /// Creates an SKFont with proper settings for text measurement.
    /// SKFont expects Size in PIXELS.
    /// </summary>
    public static SKFont CreateSKFont(SKTypeface typeface, double sizeInPoints, double dpi)
    {
        float pixelSize = CalculatePixelSize(sizeInPoints, dpi);
        return new SKFont(typeface, pixelSize) { Subpixel = true };
    }

    /// <inheritdoc />
    public Measure MeasureText(string text, FontSpec font, double dpi)
    {
        if (string.IsNullOrEmpty(text))
            return new Measure(0.AsPixels(), 0.AsPixels());

        if (dpi <= 0 || double.IsNaN(dpi) || double.IsInfinity(dpi))
            throw new ArgumentOutOfRangeException(nameof(dpi), $"DPI must be a positive finite number. Got: {dpi}");

        using var typeface = CreateTypeface(font);
        using var skFont = CreateSKFont(typeface, font.SizeInPoints, dpi);

        float width = skFont.MeasureText(text);
        float height = CalculateHeight(skFont);

        return new Measure(
            Math.Ceiling(width).AsPixels(),
            Math.Ceiling(height).AsPixels()
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

        // 1. Resolve bounds to absolute pixels
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

        // Create font once - we'll update its size in the loop
        using var typeface = CreateTypeface(baseFont);
        using var skFont = new SKFont(typeface) { Subpixel = true };

        // 2. Binary Search (typically 12-13 iterations for 6-72pt range)
        // Continue until precision is better than 0.01pt (sub-pixel accuracy at typical DPIs)
        while (high - low > 0.01)
        {
            double mid = (low + high) / 2.0;

            // Update font size for this iteration
            skFont.Size = CalculatePixelSize(mid, dpi);

            // Measure with current size
            float width = skFont.MeasureText(text);
            float height = CalculateHeight(skFont);

            // Check if it fits
            if (FitsInBounds(width, height, maxWidthPx, maxHeightPx))
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
    /// Checks if measured text dimensions fit within specified bounds.
    /// </summary>
    private static bool FitsInBounds(float width, float height, double maxWidth, double maxHeight)
    {
        return Math.Ceiling(width) <= maxWidth && Math.Ceiling(height) <= maxHeight;
    }
}
