namespace DailyMath.Core.Layout;

/// <summary>
/// Represents spacing on all four edges of an element.
/// Used for both Margin (space outside) and Padding (space inside).
/// </summary>
public struct Inset
{
    public Length Left { get; set; }
    public Length Top { get; set; }
    public Length Right { get; set; }
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
