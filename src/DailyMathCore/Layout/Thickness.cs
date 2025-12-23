namespace DailyMath.Core.Layout;

public readonly struct Thickness
{
    public Length Left { get; }
    public Length Top { get; }
    public Length Right { get; }
    public Length Bottom { get; }

    public Thickness(Length left, Length top, Length right, Length bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    public Thickness(Length uniform)
        : this(uniform, uniform, uniform, uniform)
    {
    }

    public Thickness(Length horizontal, Length vertical)
        : this(horizontal, vertical, horizontal, vertical)
    {
    }

    public static Thickness Zero(LengthUnit unit) => new(Length.Zero(unit));

    public override string ToString() => $"({Left}, {Top}, {Right}, {Bottom})";
}
