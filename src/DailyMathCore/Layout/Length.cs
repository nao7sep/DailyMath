namespace DailyMath.Core.Layout;

public readonly struct Length
{
    public double Value { get; }
    public LengthUnit Unit { get; }

    public Length(double value, LengthUnit unit)
    {
        Value = value;
        Unit = unit;
    }

    public override string ToString() => $"{Value}{Unit switch
    {
        LengthUnit.Millimeter => "mm",
        LengthUnit.Inch => "in",
        LengthUnit.Pixel => "px",
        _ => throw new ArgumentException($"Unsupported LengthUnit: {Unit}")
    }}";

    public static Length Zero(LengthUnit unit) => new(0, unit);
}
