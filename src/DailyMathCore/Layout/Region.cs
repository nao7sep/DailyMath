namespace DailyMath.Core.Layout;

/// <summary>
/// Represents the calculated absolute position and size of an element in pixels.
/// Stores edges (Left, Top, Right, Bottom) as source of truth to ensure perfect alignment
/// when adjacent elements share boundaries. Width and Height are calculated properties.
/// Immutable result of layout calculation.
/// </summary>
public readonly struct Region
{
    public double Left { get; }
    public double Top { get; }
    public double Right { get; }
    public double Bottom { get; }

    /// <summary>
    /// Gets the width (Right - Left).
    /// </summary>
    public double Width => Right - Left;

    /// <summary>
    /// Gets the height (Bottom - Top).
    /// </summary>
    public double Height => Bottom - Top;

    /// <summary>
    /// Creates a new region using edge coordinates.
    /// </summary>
    /// <param name="left">The left edge position.</param>
    /// <param name="top">The top edge position.</param>
    /// <param name="right">The right edge position.</param>
    /// <param name="bottom">The bottom edge position.</param>
    public Region(double left, double top, double right, double bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }
}
