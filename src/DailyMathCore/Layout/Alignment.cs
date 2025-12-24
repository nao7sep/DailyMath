namespace DailyMath.Core.Layout;

/// <summary>
/// Specifies how an element is positioned within its parent's content area.
/// </summary>
public enum Alignment
{
    /// <summary>
    /// Positioned using Margin.Left and Margin.Top from parent's top-left corner.
    /// </summary>
    TopLeft,

    /// <summary>
    /// Anchored to parent's right edge using Margin.Right, positioned from top using Margin.Top.
    /// </summary>
    TopRight,

    /// <summary>
    /// Anchored to parent's bottom edge using Margin.Bottom, positioned from left using Margin.Left.
    /// </summary>
    BottomLeft,

    /// <summary>
    /// Anchored to parent's bottom-right corner using Margin.Right and Margin.Bottom.
    /// </summary>
    BottomRight,

    /// <summary>
    /// Ignores Size and stretches to fill parent's content area (inside parent's Padding),
    /// leaving space for this element's own Margin on all sides.
    /// </summary>
    Fill
}
