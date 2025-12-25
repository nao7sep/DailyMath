namespace DailyMath.Core.Rendering;

using DailyMath.Core.Layout;

/// <summary>
/// Provides text measurement capabilities for layout calculations.
/// Platform-agnostic interface for cross-platform rendering.
/// </summary>
public interface ITextMeasurer
{
    /// <summary>
    /// Measures the exact bounding box of the text if rendered with the specified font and DPI.
    /// DPI is required because font sizes are specified in Points (physical units),
    /// but measurements must be returned in pixels for layout calculations.
    /// Supports single-line text only - newlines are treated as regular characters.
    /// </summary>
    /// <param name="text">The text to measure.</param>
    /// <param name="font">The font specification.</param>
    /// <param name="dpi">The rendering context DPI (converts Points to pixels).</param>
    /// <returns>The width and height of the text.</returns>
    Measure MeasureText(string text, FontSpec font, double dpi);

    /// <summary>
    /// Calculates the largest possible font size (in Points) that allows the text
    /// to fit completely within the specified bounds.
    /// DPI is required because bounds are in pixels, but font sizes are in Points.
    /// Implementations should use binary search for efficiency.
    /// Supports single-line text only - newlines are treated as regular characters.
    /// </summary>
    /// <param name="text">The text to fit.</param>
    /// <param name="baseFont">The font configuration (Family, Weight, Style). Size is ignored.</param>
    /// <param name="bounds">The maximum available width and height. To constrain only one dimension, set the other to a very large value (e.g., 10000 pixels).</param>
    /// <param name="dpi">The rendering context DPI (converts Points to pixels).</param>
    /// <param name="minSizeInPoints">The minimum font size to check (default 6pt - typical minimum readable size).</param>
    /// <param name="maxSizeInPoints">The maximum font size to check (default 72pt - 1 inch, typical document maximum).</param>
    /// <returns>The optimized font size in Points.</returns>
    double GetMaxFontSize(
        string text,
        FontSpec baseFont,
        Measure bounds,
        double dpi,
        double minSizeInPoints = 6,
        double maxSizeInPoints = 72);
}
