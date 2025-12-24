namespace DailyMath.Core.Layout;

/// <summary>
/// Provides fluent extension methods for creating Length instances.
/// </summary>
public static class LengthExtensions
{
    public static Length AsPixels(this double value) => new(value, Unit.Pixels);
    public static Length AsPixels(this int value) => new(value, Unit.Pixels);

    public static Length AsInches(this double value) => new(value, Unit.Inches);
    public static Length AsInches(this int value) => new(value, Unit.Inches);

    public static Length AsMillimeters(this double value) => new(value, Unit.Millimeters);
    public static Length AsMillimeters(this int value) => new(value, Unit.Millimeters);

    public static Length AsPercent(this double value) => new(value, Unit.Percent);
    public static Length AsPercent(this int value) => new(value, Unit.Percent);
}
