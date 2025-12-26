namespace DailyMath.Core.Rendering;

/// <summary>
/// Specifies which dimensions constrain the font size calculation when fitting text.
/// </summary>
public enum TextFitMode
{
    /// <summary>
    /// The text size is constrained by both the width and height of the element.
    /// The resulting font size ensures the text fits completely within the content area
    /// without overflowing in any direction.
    /// </summary>
    Both,

    /// <summary>
    /// The text size is constrained only by the height of the element.
    /// The width is ignored, allowing the text to potentially overflow horizontally.
    /// Commonly used when the element represents a line of text (like a header) 
    /// where the height defines the "line height" but the text length varies.
    /// </summary>
    HeightOnly,

    /// <summary>
    /// The text size is constrained only by the width of the element.
    /// The height is ignored, allowing the text to potentially overflow vertically.
    /// </summary>
    WidthOnly
}
