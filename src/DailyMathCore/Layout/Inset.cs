namespace DailyMath.Core.Layout;

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
    /// Returns an Inset with all edges set to zero pixels.
    /// </summary>
    public static Inset Zero => new(0.AsPixels(), 0.AsPixels(), 0.AsPixels(), 0.AsPixels());
}
