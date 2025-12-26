using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using DailyMath.Core.Layout;
using DailyMath.Core.Rendering;

namespace DailyMath.Core.Windows;

/// <summary>
/// Text measurement implementation using WPF for accurate font metrics.
///
/// This class measures how much space text will occupy when rendered with a specific font.
/// It uses WPF's FormattedText to calculate width and font metrics (Ascent/Descent) to
/// calculate height. The measurements are returned in pixels, accounting for the target DPI.
///
/// Design approach:
/// - Uses "tight bounding box" (excludes internal line spacing) to match SkiaSharp measurements
/// - Handles DPI-aware scaling for screens, printers, and other output devices
/// - Provides font size optimization via binary search to fit text in constrained bounds
///
/// Rendering quality (no manual configuration needed):
/// Unlike SkiaSharp, WPF defaults to high-quality rendering and does NOT require manual
/// configuration of antialiasing or hinting. Here's why:
/// - Subpixel Positioning: WPF's FormattedText defaults to Ideal mode, allowing text at
///   fractional pixels (e.g., x=10.4), which matches our Skia config (Subpixel=true)
/// - Antialiasing: WPF automatically applies ClearType/Grayscale antialiasing
/// - Hinting/Grid Alignment: Handled automatically via the pixelsPerDip parameter we pass
///   to FormattedText, which tells the font engine the exact display density
/// SkiaSharp requires manual configuration because it's lower-level; WPF abstracts
/// these details for you.
///
/// Note: This measurer intentionally ignores text decorations (underline, strikethrough, etc.)
/// because decorations don't affect the actual width/height bounds. They are applied only
/// during rendering, not measurement.
/// </summary>
public class WpfTextMeasurer : ITextMeasurer
{
    /// <summary>
    /// WPF's unit definition: 1 Device Independent Pixel (DIP) = 1/96 inch.
    /// This is a coordinate system constant, not a device resolution.
    ///
    /// Background: WPF abstracts away physical screen pixels and uses DIPs to be DPI-independent.
    /// All WPF measurements (FormattedText width/height, font sizes, etc.) are in DIPs.
    ///
    /// Why 96.0 always?
    /// - This is a hard-coded constant in the WPF graphics engine
    /// - It represents how WPF defines its logical coordinate system
    /// - It does NOT change based on actual screen DPI
    /// - Even when rendering to a 600 DPI printer, you still convert through 96.0
    ///
    /// Conversion flow:
    /// 1. User specifies font size in Points (1/72 inch)
    /// 2. Convert Points to DIPs using: points * (96.0 / 72.0)
    /// 3. Pass DIPs to FormattedText
    /// 4. Get result in DIPs from FormattedText
    /// 5. Convert DIPs to device pixels using: dips * (targetDPI / 96.0)
    ///
    /// Example: Font size 12pt on 300 DPI display
    /// - 12pt * (96/72) = 16 DIPs
    /// - FormattedText measures in 16 DIPs
    /// - 16 DIPs * (300/96) = 50 pixels at 300 DPI output
    /// </summary>
    private const double DipsPerInch = 96.0;

    /// <summary>
    /// Measures the width and height of text when rendered with a specific font.
    ///
    /// Process:
    /// 1. Converts font size from Points (typography) to DIPs (WPF's internal unit)
    /// 2. Creates a Typeface object with the specified family, style, and weight
    /// 3. Uses WPF's FormattedText to measure the text width
    /// 4. Extracts glyph metrics (Ascent + Descent) to calculate the tight height
    /// 5. Converts all measurements to pixels based on the target DPI
    ///
    /// Height calculation (the "tight box" approach):
    /// - Ascent: Distance from baseline up to the top of characters (e.g., top of 'H')
    /// - Descent: Distance from baseline down to descenders (e.g., bottom of 'g', 'y')
    /// - Height = Ascent + Descent (tight vertical space around character ink)
    /// - NOT the same as LineHeight, which includes line spacing gaps
    ///
    /// Why this matters: Ascent + Descent gives the actual visual height of the text,
    /// excluding empty space. This matches SkiaSharp's approach (Descent - Ascent,
    /// where Ascent is negative).
    /// </summary>
    public Measure MeasureText(string text, FontSpec font, double dpi)
    {
        if (string.IsNullOrEmpty(text))
            return new Measure(0.AsPixels(), 0.AsPixels());

        // Convert points (72 DPI) to WPF DIPs (96 DPI)
        double emSizeInDips = font.SizeInPoints * (DipsPerInch / UnitConverter.PointsPerInch);
        double pixelsPerDip = dpi / DipsPerInch;

        var typeface = CreateTypeface(font);

        // Measure width using FormattedText
        var formattedText = CreateFormattedText(text, typeface, emSizeInDips, pixelsPerDip);

        // Calculate tight height (excluding internal leading)
        double heightInDips = GetTightHeightDip(typeface, emSizeInDips);

        return new Measure(
            Math.Ceiling(formattedText.Width * pixelsPerDip).AsPixels(),
            Math.Ceiling(heightInDips * pixelsPerDip).AsPixels());
    }

    /// <summary>
    /// Determines the maximum font size that allows text to fit within specified bounds.
    ///
    /// Algorithm: Binary Search (also called "bisection search")
    /// - Starts with range [minSizeInPoints, maxSizeInPoints]
    /// - Repeatedly tests the middle value and narrows the range
    /// - Converges in O(log n) time, much faster than testing every size
    /// - Stops when the range is narrower than 0.1 points (precise enough)
    ///
    /// Example: Finding max size for text that fits in 100x50 pixel box
    /// - Iteration 1: Test 36pt (mid of 6-72) - too big? test lower
    /// - Iteration 2: Test 21pt (mid of 6-36) - fits? test higher
    /// - Iteration 3: Test 28pt (mid of 21-36) - too big? test lower
    /// - ... continues until convergence
    ///
    /// Performance optimization:
    /// Height calculation is just multiplication (very fast), but width requires
    /// creating a FormattedText object (slower). We calculate height first and
    /// skip the expensive width check if height already exceeds bounds.
    /// </summary>
    public double GetMaxFontSize(string text, FontSpec baseFont, Measure bounds, double dpi, double minSizeInPoints = 6, double maxSizeInPoints = 72)
    {
        if (string.IsNullOrEmpty(text))
            return maxSizeInPoints;

        double maxWidthPx = bounds.Width.ToPixels(dpi);
        double maxHeightPx = bounds.Height.ToPixels(dpi);
        // Calculate the scaling factor from DIPs to target device pixels
        double pixelsPerDip = dpi / DipsPerInch;

        double low = minSizeInPoints;
        double high = maxSizeInPoints;

        var typeface = CreateTypeface(baseFont);

        // Pre-calculate the normalized height factor (0..1).
        // This allows us to calculate height inside the loop without creating objects.
        double heightFactor = GetTightHeightFactor(typeface);

        // Binary search for the optimal font size
        while (high - low > 0.1)
        {
            double mid = (low + high) / 2.0;
            // Convert points to DIPs for this iteration
            double emSizeInDips = mid * (DipsPerInch / UnitConverter.PointsPerInch);

            // Fast height check (no object allocation)
            double heightPx = (emSizeInDips * heightFactor) * pixelsPerDip;

            if (heightPx > maxHeightPx)
            {
                high = mid;
                continue; // Skip expensive width check if height already fails
            }

            // Width check (requires FormattedText allocation)
            var formattedText = CreateFormattedText(text, typeface, emSizeInDips, pixelsPerDip);
            double widthPx = formattedText.Width * pixelsPerDip;

            if (widthPx <= maxWidthPx)
                low = mid;
            else
                high = mid;
        }

        return Math.Floor(low * 10) / 10;
    }

    /// <summary>
    /// Creates a Typeface object from a FontSpec specification.
    ///
    /// In WPF, a Typeface is the combination of:
    /// - FontFamily: The font name (e.g., "Arial", "Segoe UI", "Courier New")
    /// - FontStyle: Normal (upright) or Italic (slanted)
    /// - FontWeight: Boldness from 100 (Thin) to 900 (Black), in 100-unit increments
    ///   (Common values: 400=Normal, 700=Bold)
    /// - FontStretch: Character width (Normal, Condensed, Expanded, etc.)
    ///   This implementation always uses Normal.
    ///
    /// Mapping from FontSpec to WPF Typeface:
    /// - FontSpec.Family (string) → FontFamily by name
    /// - FontSpec.Style (flags) → FontStyle.Italic or FontStyle.Normal
    /// - FontSpec.Weight (enum) → Uses OpenType standard weights (100-900)
    ///
    /// Font Fallback:
    /// If the requested font family is not installed on the system, WPF automatically
    /// substitutes the closest available font (more forgiving than SkiaSharp).
    /// </summary>
    private static Typeface CreateTypeface(FontSpec spec)
    {
        var style = spec.Style.HasFlag(Core.Rendering.FontStyle.Italic)
            ? FontStyles.Italic
            : FontStyles.Normal;

        var weight = System.Windows.FontWeight.FromOpenTypeWeight((int)spec.Weight);

        return new Typeface(new FontFamily(spec.Family), style, weight, FontStretches.Normal);
    }

    /// <summary>
    /// Creates a FormattedText instance for measuring text dimensions.
    ///
    /// FormattedText is WPF's text measurement engine. It:
    /// - Lays out text using the specified Typeface and size
    /// - Measures the width needed to display the text
    /// - Provides access to font metrics (through GlyphTypeface)
    /// - Handles multi-line text, but we use it for single-line measurements
    ///
    /// Constructor parameters explanation:
    /// - text: The string to measure
    /// - CultureInfo.InvariantCulture: Uses invariant rules (no locale-specific formatting)
    /// - FlowDirection.LeftToRight: Text direction (left-to-right for English, etc.)
    /// - typeface: The font to use
    /// - emSizeInDips: Font size in Device Independent Pixels (WPF's internal unit)
    /// - Brushes.Black: Brush color (doesn't affect measurement, just required)
    /// - pixelsPerDip: Scaling factor for anti-aliasing calculations
    ///
    /// Maintenance note: This constructor signature may change in future .NET versions.
    /// If you update the .NET target framework and get compilation errors, check WPF docs
    /// and update the parameters here (only one place to fix).
    /// </summary>
    private static FormattedText CreateFormattedText(string text, Typeface typeface, double emSizeInDips, double pixelsPerDip)
    {
        return new FormattedText(
            text,
            CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            typeface,
            emSizeInDips,
            Brushes.Black,
            pixelsPerDip);
    }

    /// <summary>
    /// Calculates the normalized height factor (Ascent + Descent) that excludes internal leading.
    ///
    /// What is "tight height"?
    /// - It's the actual vertical space occupied by text, excluding line spacing gaps
    /// - Useful for laying out text without extra whitespace above/below
    ///
    /// Font metrics (from the font file):
    /// - Ascent: Height from baseline to the top of tall characters (e.g., 'H', 'T')
    ///   Typical range: +0.8 to +0.9 (normalized to font size)
    /// - Descent: Height from baseline to the bottom of descenders (e.g., 'g', 'y')
    ///   Typical range: -0.2 to -0.3 (negative because it goes down from baseline)
    /// - LineGap/Leading: Extra space added between lines (we intentionally ignore this)
    ///
    /// Example: 12pt font with Ascent=0.85, Descent=0.15
    /// - Height = (0.85 + 0.15) * 12 = 1.0 * 12 = 12 pixels
    ///
    /// Why use GlyphTypeface?
    /// - It accesses the actual font file metrics, which are the most accurate
    /// - Fallback to FontFamily.LineSpacing for composite fonts (fonts that don't
    ///   have complete glyph data)
    ///
    /// Comparison with SkiaSharp:
    /// - Skia: Height = Descent - Ascent (where Ascent is negative)
    /// - WPF: Height = Ascent + Descent (both positive)
    /// - Result: Both give the same height value, just different sign conventions
    /// </summary>
    private static double GetTightHeightFactor(Typeface typeface)
    {
        // Use a probe text to determine the actual rendering metrics used by WPF.
        // We use "Hg" to ensure we capture both the Ascender (H) and Descender (g).
        var probeText = new FormattedText(
            "Hg",
            CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            typeface,
            100.0, // Large EM size for precision
            Brushes.Black,
            1.0);  // 1.0 pixels per DIP

        // Calculate metrics from the probe
        // Ascent: Distance from Top to Baseline
        // Descent: Distance from Baseline to Bottom (derived from total Height)
        double ascent = probeText.Baseline;
        double descent = probeText.Height - probeText.Baseline;

        // Normalized factor (Ratio of Total Height to Em Size)
        return (ascent + descent) / 100.0;
    }

    /// <summary>
    /// Calculates the actual height in Device Independent Pixels (DIPs) for a given font size.
    ///
    /// Formula: heightInDips = emSizeInDips * heightFactor
    ///
    /// Where:
    /// - emSizeInDips: The font size in WPF's DIP units
    /// - heightFactor: The normalized ratio of (Ascent + Descent) to em-square
    ///
    /// Example: 12pt font with heightFactor = 1.0
    /// - 12pt → 16 DIPs (12 * 96/72)
    /// - heightInDips = 16 DIPs * 1.0 = 16 DIPs
    /// - At 96 DPI: 16 DIPs = 16 pixels
    /// - At 300 DPI: 16 DIPs = 50 pixels (16 * 300/96)
    /// </summary>
    private static double GetTightHeightDip(Typeface typeface, double emSizeInDips)
    {
        return emSizeInDips * GetTightHeightFactor(typeface);
    }
}