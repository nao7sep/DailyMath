using System;
using SkiaSharp;
using DailyMath.Core.Layout;
using DailyMath.Core.Rendering;

namespace DailyMath.Core.Skia;

/// <summary>
/// Text measurement implementation using SkiaSharp for accurate font metrics.
///
/// This class handles the calculation of text dimensions using SkiaSharp's font rendering engine.
/// It provides reliable text measurement and font size optimization for layout calculations.
///
/// Design Note: Text decorations (Underline, Strikethrough, Overline) are intentionally not
/// handled here because they do not affect the tight bounding box (Ascent + Descent).
/// The Measurer's responsibility is to calculate dimensions; decorations are purely visual
/// and are fully handled during rendering in the Renderer classes.
/// </summary>
public class SkiaTextMeasurer : ITextMeasurer
{
    /// <summary>
    /// Measures the width and height of text when rendered with a specific font.
    ///
    /// This method:
    /// - Uses SkiaSharp to accurately calculate text dimensions
    /// - Applies font hinting and antialiased rendering for consistent, crisp results
    /// - Returns measurements in pixels based on screen DPI
    ///
    /// Font rendering configuration:
    /// - Subpixel positioning: Allows text to be placed at fractional pixel coordinates
    /// - Antialias edging: Provides smooth edges without color fringing on any background
    /// - Normal hinting: Balances grid-fitting for sharpness while maintaining font design
    /// </summary>
    public Measure MeasureText(string text, FontSpec font, double dpi)
    {
        if (string.IsNullOrEmpty(text))
        {
            return new Measure(0.AsPixels(), 0.AsPixels());
        }

        using var typeface = CreateTypeface(font);
        // Convert font size from points (pt) to pixels (px) using the DPI value
        // Formula: pixels = points * DPI / 72 (since 72 points = 1 inch = DPI pixels)
        float sizeInPixels = (float)UnitConverter.PointsToPixels(font.SizeInPoints, dpi);
        using var skFont = new SKFont(typeface, sizeInPixels);

        // Configure the font with optimal rendering settings
        ConfigureSKFont(skFont);

        var width = skFont.MeasureText(text);
        var height = GetFontHeight(skFont);

        return new Measure(Math.Ceiling(width).AsPixels(), Math.Ceiling(height).AsPixels());
    }

    /// <summary>
    /// Determines the maximum font size that allows text to fit within specified bounds.
    ///
    /// This method uses binary search to efficiently find the largest font size where:
    /// - The text width fits within the bounds' width
    /// - The text height fits within the bounds' height
    ///
    /// Binary search approach:
    /// - Starts with a range between minSizeInPoints and maxSizeInPoints
    /// - Iteratively narrows the range until convergence (within 0.1 point precision)
    /// - Converges faster than linear search, especially for large size ranges
    /// </summary>
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
        using var skFont = new SKFont(typeface);

        // Configure the font with optimal rendering settings
        ConfigureSKFont(skFont);

        // Binary search for the optimal font size
        while (high - low > 0.1)
        {
            double mid = (low + high) / 2.0;
            // Convert font size from points to pixels
            skFont.Size = (float)UnitConverter.PointsToPixels(mid, dpi);

            var width = skFont.MeasureText(text);
            var height = GetFontHeight(skFont);

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

    /// <summary>
    /// Creates an SKTypeface from a FontSpec specification.
    ///
    /// SKTypeface represents the actual font file/outline data in SkiaSharp.
    /// This method maps from our FontSpec abstraction to SkiaSharp's type system:
    /// - Weight: Determines boldness (from Thin to Black)
    /// - Slant: Determines style (Upright or Italic)
    /// - Width: Always set to Normal (can be extended for condensed/expanded fonts)
    ///
    /// Fallback handling:
    /// If the requested font family is not installed on the system, this method gracefully
    /// falls back to the system default font. This prevents crashes and ensures the application
    /// can always render text, even if the user's machine is missing specific font families.
    /// </summary>
    private static SKTypeface CreateTypeface(FontSpec spec)
    {
        var weight = (SKFontStyleWeight)(int)spec.Weight;
        var slant = spec.Style.HasFlag(Core.Rendering.FontStyle.Italic)
            ? SKFontStyleSlant.Italic
            : SKFontStyleSlant.Upright;

        // If the requested font family is not found, fall back to the system default font
        // to prevent crashes due to missing fonts on the user's machine.
        return SKTypeface.FromFamilyName(spec.Family, new SKFontStyle(weight, SKFontStyleWidth.Normal, slant))
               ?? SKTypeface.Default;
    }

    /// <summary>
    /// Configures an SKFont instance with optimal rendering properties for consistent text display.
    ///
    /// Configuration details:
    /// - Subpixel = true: Enables subpixel positioning.
    ///   This allows the font to be drawn at fractional pixel coordinates (e.g., 10.4px) rather
    ///   than snapping to whole integers, resulting in smoother spacing and better alignment.
    /// - Edging = Antialias: Applies standard grayscale antialiasing.
    ///   This provides smooth character edges without color fringing, making it safe for any background
    ///   including transparent layers. (Note: SubpixelAntialias would use LCD subpixels for sharper text
    ///   but can cause color artifacts on non-opaque backgrounds).
    /// - Hinting = Normal: Applies moderate grid-fitting to align glyphs with the pixel grid.
    ///   This improves sharpness and contrast while maintaining the font's design.
    /// </summary>
    private static void ConfigureSKFont(SKFont skFont)
    {
        skFont.Subpixel = true;
        skFont.Edging = SKFontEdging.Antialias;
        skFont.Hinting = SKFontHinting.Normal;
    }

    /// <summary>
    /// Extracts the line height from an SKFont instance using its metrics.
    ///
    /// Font metrics explanation:
    /// - Ascent: The distance from the baseline up to the top of most characters (negative value).
    ///   For example, if ascent = -10, characters extend 10 pixels above the baseline.
    /// - Descent: The distance from the baseline down to the bottom of characters like 'g' or 'p' (positive value).
    ///   For example, if descent = 3, descenders extend 3 pixels below the baseline.
    /// - Height calculation: height = descent - ascent = descent + |ascent|
    ///   This gives the total vertical space needed for a line of text.
    ///
    /// Why negative ascent?
    /// SkiaSharp uses the convention that up is negative and down is positive from the baseline,
    /// which is standard in typography and coordinate systems where Y increases downward.
    /// </summary>
    private static float GetFontHeight(SKFont skFont)
    {
        var metrics = skFont.Metrics;
        // Ascent is negative, so subtracting it adds the absolute value
        return metrics.Descent - metrics.Ascent;
    }
}