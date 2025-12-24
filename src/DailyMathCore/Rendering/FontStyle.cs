using System;

namespace DailyMath.Core.Rendering;

/// <summary>
/// Specifies style information applied to text.
/// Supports bitwise combinations (e.g., Italic | Underline).
/// Platform-agnostic text styling for cross-platform rendering.
/// </summary>
[Flags]
public enum FontStyle
{
    /// <summary>
    /// Normal text with no styling.
    /// </summary>
    None = 0,

    /// <summary>
    /// Slanted text.
    /// </summary>
    Italic = 1,

    /// <summary>
    /// A line drawn under the text.
    /// </summary>
    Underline = 2,

    /// <summary>
    /// A line drawn through the center of the text.
    /// Also known as 'Strikeout' or 'Line-Through'.
    /// </summary>
    Strikethrough = 4,

    /// <summary>
    /// A line drawn above the text.
    /// Supported by CSS and modern UI frameworks.
    /// (Note: GDI+ renderers may ignore this if manual drawing is not implemented).
    /// </summary>
    Overline = 8
}
