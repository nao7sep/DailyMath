namespace DailyMath.Core.Layout;

/// <summary>
/// Provides fluent extension methods for creating Length instances.
/// </summary>
public static class LengthExtensions
{
    /// <summary>
    /// Creates a pixel-based <see cref="Length"/> from a double value.
    /// </summary>
    public static Length AsPixels(this double value) => new(value, Unit.Pixels);

    /// <summary>
    /// Creates a pixel-based <see cref="Length"/> from an integer value.
    /// </summary>
    public static Length AsPixels(this int value) => new(value, Unit.Pixels);

    /// <summary>
    /// Creates an inch-based <see cref="Length"/> from a double value.
    /// Physical unit that converts to pixels using DPI.
    /// </summary>
    public static Length AsInches(this double value) => new(value, Unit.Inches);

    /// <summary>
    /// Creates an inch-based <see cref="Length"/> from an integer value.
    /// Physical unit that converts to pixels using DPI.
    /// </summary>
    public static Length AsInches(this int value) => new(value, Unit.Inches);

    /// <summary>
    /// Creates a millimeter-based <see cref="Length"/> from a double value.
    /// Physical unit that converts to pixels using DPI.
    /// </summary>
    public static Length AsMillimeters(this double value) => new(value, Unit.Millimeters);

    /// <summary>
    /// Creates a millimeter-based <see cref="Length"/> from an integer value.
    /// Physical unit that converts to pixels using DPI.
    /// </summary>
    public static Length AsMillimeters(this int value) => new(value, Unit.Millimeters);

    /// <summary>
    /// Creates a centimeter-based <see cref="Length"/> from a double value.
    /// Physical unit that converts to pixels using DPI.
    /// </summary>
    public static Length AsCentimeters(this double value) => new(value, Unit.Centimeters);

    /// <summary>
    /// Creates a centimeter-based <see cref="Length"/> from an integer value.
    /// Physical unit that converts to pixels using DPI.
    /// </summary>
    public static Length AsCentimeters(this int value) => new(value, Unit.Centimeters);

    /// <summary>
    /// Creates a percent-based <see cref="Length"/> from a double value.
    /// Percent is evaluated against the parent's content area at usage time.
    /// </summary>
    public static Length AsPercent(this double value) => new(value, Unit.Percent);

    /// <summary>
    /// Creates a percent-based <see cref="Length"/> from an integer value.
    /// Percent is evaluated against the parent's content area at usage time.
    /// </summary>
    public static Length AsPercent(this int value) => new(value, Unit.Percent);
}
