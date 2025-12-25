using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using DailyMath.Core.Layout;
using DailyMath.Core.Rendering;

namespace DailyMath.Core.Windows;

public class WpfTextMeasurer : ITextMeasurer
{
    private const double WpfDpi = 96.0;

    public Measure MeasureText(string text, FontSpec font, double dpi)
    {
        if (string.IsNullOrEmpty(text))
        {
            return new Measure(0.AsPixels(), 0.AsPixels());
        }

        // Setup scaling and font
        double pixelsPerDip = dpi / WpfDpi;
        double emSizeDip = font.SizeInPoints * (WpfDpi / 72.0);
        var typeface = CreateTypeface(font);

        // Measure Width
        var formattedText = new FormattedText(
            text,
            CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            typeface,
            emSizeDip,
            Brushes.Black,
            pixelsPerDip);

        // Apply basic decorations if present (for consistency, though rarely affects size)
        ApplyDecorations(formattedText, font.Style);

        // Measure Height (Tight bounding box to match Skia)
        double heightDip;
        if (typeface.TryGetGlyphTypeface(out var glyphTypeface))
        {
            heightDip = emSizeDip * glyphTypeface.Height;
        }
        else
        {
            heightDip = emSizeDip * typeface.FontFamily.LineSpacing;
        }

        return new Measure(
            Math.Ceiling(formattedText.Width * pixelsPerDip).AsPixels(),
            Math.Ceiling(heightDip * pixelsPerDip).AsPixels());
    }

    public double GetMaxFontSize(string text, FontSpec baseFont, Measure bounds, double dpi, double minSizeInPoints = 6, double maxSizeInPoints = 72)
    {
        if (string.IsNullOrEmpty(text))
        {
            return maxSizeInPoints;
        }

        double maxWidthPx = bounds.Width.ToPixels(dpi);
        double maxHeightPx = bounds.Height.ToPixels(dpi);
        double pixelsPerDip = dpi / WpfDpi;

        double low = minSizeInPoints;
        double high = maxSizeInPoints;

        var typeface = CreateTypeface(baseFont);

        // Pre-calculate height factor to avoid repeated lookups
        double heightFactor;
        if (typeface.TryGetGlyphTypeface(out var glyphTypeface))
        {
            heightFactor = glyphTypeface.Height;
        }
        else
        {
            heightFactor = typeface.FontFamily.LineSpacing;
        }

        // Binary Search
        while (high - low > 0.1)
        {
            double mid = (low + high) / 2.0;
            double emSizeDip = mid * (WpfDpi / 72.0);

            var formattedText = new FormattedText(
                text,
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                typeface,
                emSizeDip,
                Brushes.Black,
                pixelsPerDip);

            double widthPx = formattedText.Width * pixelsPerDip;
            double heightPx = (emSizeDip * heightFactor) * pixelsPerDip;

            if (widthPx <= maxWidthPx && heightPx <= maxHeightPx)
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

    private static Typeface CreateTypeface(FontSpec spec)
    {
        var style = spec.Style.HasFlag(Core.Rendering.FontStyle.Italic) ? FontStyles.Italic : FontStyles.Normal;
        var weight = System.Windows.FontWeight.FromOpenTypeWeight((int)spec.Weight);
        
        return new Typeface(new FontFamily(spec.Family), style, weight, FontStretches.Normal);
    }

    private static void ApplyDecorations(FormattedText text, Core.Rendering.FontStyle style)
    {
        var decorations = new TextDecorationCollection();
        if (style.HasFlag(Core.Rendering.FontStyle.Underline)) decorations.Add(TextDecorations.Underline);
        if (style.HasFlag(Core.Rendering.FontStyle.Strikethrough)) decorations.Add(TextDecorations.Strikethrough);
        
        if (decorations.Count > 0)
        {
            text.SetTextDecorations(decorations);
        }
    }
}