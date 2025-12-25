using System;
using SkiaSharp;
using DailyMath.Core.Layout;
using DailyMath.Core.Rendering;

namespace DailyMath.Core.Skia;

public class SkiaTextMeasurer : ITextMeasurer
{
    public Measure MeasureText(string text, FontSpec font, double dpi)
    {
        if (string.IsNullOrEmpty(text))
        {
            return new Measure(0.AsPixels(), 0.AsPixels());
        }

        using var typeface = CreateTypeface(font);
        using var skFont = new SKFont(typeface, (float)(font.SizeInPoints * dpi / 72.0))
        {
            Subpixel = true,
            Edging = SKFontEdging.SubpixelAntialias
        };

        var width = skFont.MeasureText(text);
        var metrics = skFont.Metrics;
        // Calculate tight height (Ascent is negative)
        var height = metrics.Descent - metrics.Ascent;

        return new Measure(Math.Ceiling(width).AsPixels(), Math.Ceiling(height).AsPixels());
    }

    public double GetMaxFontSize(string text, FontSpec baseFont, Measure bounds, double dpi, double minSizeInPoints = 6, double maxSizeInPoints = 72)
    {
        if (string.IsNullOrEmpty(text))
        {
            return maxSizeInPoints;
        }

        double maxWidthPx = bounds.Width.ToPixels(dpi);
        double maxHeightPx = bounds.Height.ToPixels(dpi);

        double low = minSizeInPoints;
        double high = maxSizeInPoints;

        using var typeface = CreateTypeface(baseFont);
        using var skFont = new SKFont(typeface) { Subpixel = true };

        // Binary search for the optimal font size
        while (high - low > 0.1)
        {
            double mid = (low + high) / 2.0;
            skFont.Size = (float)(mid * dpi / 72.0);

            var width = skFont.MeasureText(text);
            var metrics = skFont.Metrics;
            var height = metrics.Descent - metrics.Ascent;

            if (width <= maxWidthPx && height <= maxHeightPx)
            {
                low = mid;
            }
            else
            {
                high = mid;
            }
        }

        return Math.Floor(low * 10) / 10;
    }

    private static SKTypeface CreateTypeface(FontSpec spec)
    {
        var weight = (SKFontStyleWeight)(int)spec.Weight;
        var slant = spec.Style.HasFlag(Core.Rendering.FontStyle.Italic) 
            ? SKFontStyleSlant.Italic 
            : SKFontStyleSlant.Upright;

        return SKTypeface.FromFamilyName(spec.Family, new SKFontStyle(weight, SKFontStyleWidth.Normal, slant));
    }
}