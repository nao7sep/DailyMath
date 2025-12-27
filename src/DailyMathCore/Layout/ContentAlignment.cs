namespace DailyMath.Core.Layout;

/// <summary>
/// Specifies how content is aligned within an Element or Region.
/// Used during rendering to position text, images, etc.
/// </summary>
public enum ContentAlignment
{
    /// <summary>
    /// Aligns content to the top-left corner.
    /// </summary>
    TopLeft,

    /// <summary>
    /// Aligns content to the top-center.
    /// </summary>
    TopCenter,

    /// <summary>
    /// Aligns content to the top-right corner.
    /// </summary>
    TopRight,

    /// <summary>
    /// Aligns content to the middle-left.
    /// </summary>
    MiddleLeft,

    /// <summary>
    /// Aligns content to the center.
    /// </summary>
    MiddleCenter,

    /// <summary>
    /// Aligns content to the middle-right.
    /// </summary>
    MiddleRight,

    /// <summary>
    /// Aligns content to the bottom-left corner.
    /// </summary>
    BottomLeft,

    /// <summary>
    /// Aligns content to the bottom-center.
    /// </summary>
    BottomCenter,

    /// <summary>
    /// Aligns content to the bottom-right corner.
    /// </summary>
    BottomRight
}
