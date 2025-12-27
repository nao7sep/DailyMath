namespace DailyMathCore.Layout;

public static class FlexLengthExtensions
{
    #region Pixels

    public static FlexLength AsPixels(this double value) => new FlexLength(value, Unit.Pixels);
    public static FlexLength AsPixels(this float value) => new FlexLength(value, Unit.Pixels);
    public static FlexLength AsPixels(this int value) => new FlexLength(value, Unit.Pixels);

    #endregion

    #region Percentage

    public static FlexLength AsPercent(this double value) => new FlexLength(value, Unit.Percentage);
    public static FlexLength AsPercent(this float value) => new FlexLength(value, Unit.Percentage);
    public static FlexLength AsPercent(this int value) => new FlexLength(value, Unit.Percentage);

    #endregion

    #region Millimeters

    public static FlexLength AsMillimeters(this double value) => new FlexLength(value, Unit.Millimeters);
    public static FlexLength AsMillimeters(this float value) => new FlexLength(value, Unit.Millimeters);
    public static FlexLength AsMillimeters(this int value) => new FlexLength(value, Unit.Millimeters);

    #endregion

    #region Centimeters

    public static FlexLength AsCentimeters(this double value) => new FlexLength(value, Unit.Centimeters);
    public static FlexLength AsCentimeters(this float value) => new FlexLength(value, Unit.Centimeters);
    public static FlexLength AsCentimeters(this int value) => new FlexLength(value, Unit.Centimeters);

    #endregion

    #region Inches

    public static FlexLength AsInches(this double value) => new FlexLength(value, Unit.Inches);
    public static FlexLength AsInches(this float value) => new FlexLength(value, Unit.Inches);
    public static FlexLength AsInches(this int value) => new FlexLength(value, Unit.Inches);

    #endregion
}