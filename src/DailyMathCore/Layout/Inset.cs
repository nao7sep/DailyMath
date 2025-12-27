namespace DailyMath.Core.Layout;

using System.Globalization;

/// <summary>
/// Represents spacing on all four edges of an element.
/// Used for both Margin (space outside) and Padding (space inside).
/// </summary>
public struct Inset
{
    /// <summary>
    /// Left edge spacing. For Margin, this is space outside; for Padding, space inside.
    /// Percent is evaluated against the parent's content width.
    /// </summary>
    public Length Left { get; set; }

    /// <summary>
    /// Top edge spacing. For Margin, this is space outside; for Padding, space inside.
    /// Percent is evaluated against the parent's content height.
    /// </summary>
    public Length Top { get; set; }

    /// <summary>
    /// Right edge spacing. For Margin, this is space outside; for Padding, space inside.
    /// Percent is evaluated against the parent's content width.
    /// </summary>
    public Length Right { get; set; }

    /// <summary>
    /// Bottom edge spacing. For Margin, this is space outside; for Padding, space inside.
    /// Percent is evaluated against the parent's content height.
    /// </summary>
    public Length Bottom { get; set; }

    public Inset(Length left, Length top, Length right, Length bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    /// <summary>
    /// Returns a string representation of the inset edges with the specified format.
    /// </summary>
    /// <param name="format">The numeric format string (e.g., "0.##", "F2").</param>
    /// <param name="includeTypeName">If true, prefixes the result with "Inset: ".</param>
    /// <returns>A string representation of the inset edges.</returns>
    public string ToString(string format, bool includeTypeName = false)
    {
        string edges = $"{Left.ToString(format)}, {Top.ToString(format)}, {Right.ToString(format)}, {Bottom.ToString(format)}";
        return includeTypeName ? $"Inset: {edges}" : edges;
    }

    /// <summary>
    /// Returns a string representation of the inset edges (e.g., "10px, 20px, 30px, 40px").
    /// </summary>
    public override string ToString() => ToString("0.##", false);

    /// <summary>
    /// Returns an Inset with all edges set to zero pixels.
    /// </summary>
    public static Inset Zero => new(0.AsPixels(), 0.AsPixels(), 0.AsPixels(), 0.AsPixels());
}
