namespace DailyMath.Core.Layout;

using System.Globalization;

/// <summary>
/// Represents the calculated absolute position and size of an element in pixels.
/// Stores edges (Left, Top, Right, Bottom) as source of truth to ensure perfect alignment
/// when adjacent elements share boundaries. Width and Height are calculated properties.
/// Immutable result of layout calculation.
///
/// Design Rationale:
/// - Edges as source of truth prevent floating-point drift in border collapse.
/// - When two adjacent cells share an edge, they get the exact same coordinate value (bit-wise identical).
/// - This ensures pixel-perfect borders when rendering: drawing all element borders naturally produces
///   collapsed borders without gaps or overlaps.
/// - Example: Left cell's Right = 100.0, right cell's Left = 100.0 (guaranteed identical).
/// </summary>
public readonly struct Region
{
    /// <summary>
    /// The left edge position in pixels.
    /// Represents the X coordinate of the region's left boundary.
    /// </summary>
    public double Left { get; }

    /// <summary>
    /// The top edge position in pixels.
    /// Represents the Y coordinate of the region's top boundary.
    /// </summary>
    public double Top { get; }

    /// <summary>
    /// The right edge position in pixels.
    /// Represents the X coordinate of the region's right boundary.
    /// </summary>
    public double Right { get; }

    /// <summary>
    /// The bottom edge position in pixels.
    /// Represents the Y coordinate of the region's bottom boundary.
    /// </summary>
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

    /// <summary>
    /// Returns a string representation of the region edges with the specified format.
    /// </summary>
    /// <param name="format">The numeric format string (e.g., "0.##", "F2").</param>
    /// <param name="includeTypeName">If true, prefixes the result with "Region: ".</param>
    /// <returns>A string representation of the region edges.</returns>
    public string ToString(string format, bool includeTypeName = false)
    {
        string edges = $"{Left.ToString(format, CultureInfo.InvariantCulture)}px, {Top.ToString(format, CultureInfo.InvariantCulture)}px, " +
                       $"{Right.ToString(format, CultureInfo.InvariantCulture)}px, {Bottom.ToString(format, CultureInfo.InvariantCulture)}px";
        string size = $"{Width.ToString(format, CultureInfo.InvariantCulture)}px, {Height.ToString(format, CultureInfo.InvariantCulture)}px";
        return includeTypeName ? $"Region: {edges} ({size})" : $"{edges} ({size})";
    }

    /// <summary>
    /// Returns a string representation of the region edges and size (e.g., "10px, 20px, 30px, 40px (20px, 20px)").
    /// </summary>
    public override string ToString() => ToString("0.##", false);
}
