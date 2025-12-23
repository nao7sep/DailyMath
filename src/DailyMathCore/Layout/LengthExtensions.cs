namespace DailyMath.Core.Layout;

public static class LengthExtensions
{
    public static Length mm(this double value) => new(value, LengthUnit.Millimeter);
    public static Length mm(this int value) => new(value, LengthUnit.Millimeter);

    public static Length inch(this double value) => new(value, LengthUnit.Inch);
    public static Length inch(this int value) => new(value, LengthUnit.Inch);

    public static Length px(this double value) => new(value, LengthUnit.Pixel);
    public static Length px(this int value) => new(value, LengthUnit.Pixel);
}
