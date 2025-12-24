using System;
using System.Drawing;
using DailyMath.Core.Layout;
using DailyMath.Core.Rendering;
using GdiFontStyle = System.Drawing.FontStyle;

namespace DailyMath.Core.Windows;

/// <summary>
/// Windows GDI+ implementation of text measurement for layout calculations.
/// Uses System.Drawing for precise text metrics.
/// </summary>
public class GdiTextMeasurer : ITextMeasurer
{
    // A reusable context for measurements.
    // We use a static 1x1 Bitmap to access the GDI+ graphics engine without overhead.
    private static readonly Bitmap _refBmp = new(1, 1);
    private static readonly Graphics _refGraphics = Graphics.FromImage(_refBmp);

    /// <inheritdoc />
    public Measure MeasureText(string text, FontSpec font, double dpi)
    {
        if (string.IsNullOrEmpty(text))
            return new Measure(0.AsPixels(), 0.AsPixels());

        using var gdiFont = CreateGdiFont(font);

        // 1. Measure string using GDI+
        // Note: _refGraphics usually runs at 96 DPI (Screen resolution)
        SizeF rawSize = _refGraphics.MeasureString(text, gdiFont);

        // 2. Scale to target DPI
        // If target is 300 DPI and reference is 96 DPI, we scale up by ~3.125
        double scaleFactor = dpi / _refGraphics.DpiX;

        // 3. Round up to ensure the layout box is never smaller than the text
        double finalWidth = Math.Ceiling(rawSize.Width * scaleFactor);
        double finalHeight = Math.Ceiling(rawSize.Height * scaleFactor);

        return new Measure(finalWidth.AsPixels(), finalHeight.AsPixels());
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
        // We assume 'bounds' is concrete (Pixels/Inches/MM), not Percent.
        double maxWidthPx = bounds.Width.ToPixels(dpi: dpi);
        double maxHeightPx = bounds.Height.ToPixels(dpi: dpi);

        double low = minSizeInPoints;
        double high = maxSizeInPoints;

        // 2. Binary Search
        // Continue until precision is better than 0.01pt (sub-pixel accuracy at typical DPIs)
        while (high - low > 0.01)
        {
            double mid = (low + high) / 2.0;

            // Measure with the candidate size
            // We reuse MeasureText to ensure logic (bolding, DPI scaling) is identical
            Measure measured = MeasureText(text, baseFont.WithSize(mid), dpi);

            // Check if it fits
            // Note: measured.Width.Value and measured.Height.Value are in pixels
            // because MeasureText returns Measure with pixel Lengths
            bool fitsWidth = measured.Width.Value <= maxWidthPx;
            bool fitsHeight = measured.Height.Value <= maxHeightPx;

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
    /// Maps the platform-agnostic FontSpec to a Windows GDI+ Font.
    /// Handles Weight mapping (100-900 -> Bold flag) and Style flags.
    /// </summary>
    private System.Drawing.Font CreateGdiFont(FontSpec spec)
    {
        // 1. Map Weight -> GDI Bold
        // Standard threshold: 700 (Bold) and above gets the Bold flag.
        bool isBold = spec.Weight >= FontWeight.Bold;

        // 2. Map Style Flags -> GDI FontStyle
        GdiFontStyle gdiStyle = GdiFontStyle.Regular;

        if (isBold)
            gdiStyle |= GdiFontStyle.Bold;

        if (spec.Style.HasFlag(Core.Rendering.FontStyle.Italic))
            gdiStyle |= GdiFontStyle.Italic;

        if (spec.Style.HasFlag(Core.Rendering.FontStyle.Underline))
            gdiStyle |= GdiFontStyle.Underline;

        if (spec.Style.HasFlag(Core.Rendering.FontStyle.Strikethrough))
            gdiStyle |= GdiFontStyle.Strikeout;

        // 3. Create Font
        // GDI+ Font constructor expects size in Points, which matches spec.SizeInPoints
        return new System.Drawing.Font(spec.Family, (float)spec.SizeInPoints, gdiStyle);
    }
}
