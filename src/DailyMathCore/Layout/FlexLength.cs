using System;
using System.Globalization;

namespace DailyMathCore.Layout;

/// <summary>
/// Represents a length that can be expressed in various units.
/// Supports DPI-aware and context-aware conversions.
/// 
/// Terminology:
/// Base Size: The basis for resolving relative units (Percentage). 
/// When resolving Margin or Element Size, the Base Size is the Container (Parent Size - Parent Padding).
/// When resolving an element's own Padding, the Base Size is the element's own Size.
/// </summary>
public readonly struct FlexLength : IEquatable<FlexLength>
{
    /// <summary>
    /// A shared instance representing a zero length in pixels.
    /// </summary>
    public static readonly FlexLength Zero = new FlexLength(0, Unit.Pixels);

    private const double MillimetersPerInch = 25.4;
    private const double CentimetersPerInch = 2.54;

    /// <summary>
    /// The numeric value of the length.
    /// </summary>
    public double Value { get; }

    /// <summary>
    /// The unit of measurement.
    /// </summary>
    public Unit Unit { get; }

    public FlexLength(double value, Unit unit)
    {
        Value = value;
        Unit = unit;
    }

    #region Conversion Methods

    /// <summary>
    /// Converts the length to pixels.
    /// </summary>
    /// <param name="dpi">The Dots Per Inch of the target display. Required for absolute units.</param>
    /// <param name="baseSize">The basis for resolving relative units. Required for percentage units.</param>
    public double ToPixels(double? dpi, double? baseSize)
    {
        if (Value == 0) return 0;

        return Unit switch
        {
            Unit.Pixels => Value,
            Unit.Percentage => (Value / 100.0) * ValidateBaseSize(baseSize),
            Unit.Millimeters => (Value / MillimetersPerInch) * ValidateDpi(dpi),
            Unit.Centimeters => (Value / CentimetersPerInch) * ValidateDpi(dpi),
            Unit.Inches => Value * ValidateDpi(dpi),
            _ => throw new InvalidOperationException($"Unsupported unit: {Unit}")
        };
    }

    public double ToInches(double? dpi, double? baseSize)
    {
        if (Value == 0) return 0;

        return Unit switch
        {
            Unit.Pixels => Value / ValidateDpi(dpi),
            Unit.Percentage => ((Value / 100.0) * ValidateBaseSize(baseSize)) / ValidateDpi(dpi),
            Unit.Millimeters => Value / MillimetersPerInch,
            Unit.Centimeters => Value / CentimetersPerInch,
            Unit.Inches => Value,
            _ => throw new InvalidOperationException($"Unsupported unit: {Unit}")
        };
    }

    public double ToMillimeters(double? dpi, double? baseSize)
    {
        if (Value == 0) return 0;

        return Unit switch
        {
            Unit.Pixels => (Value / ValidateDpi(dpi)) * MillimetersPerInch,
            Unit.Percentage => (((Value / 100.0) * ValidateBaseSize(baseSize)) / ValidateDpi(dpi)) * MillimetersPerInch,
            Unit.Millimeters => Value,
            Unit.Centimeters => Value * 10.0,
            Unit.Inches => Value * MillimetersPerInch,
            _ => throw new InvalidOperationException($"Unsupported unit: {Unit}")
        };
    }

    public double ToCentimeters(double? dpi, double? baseSize)
    {
        if (Value == 0) return 0;

        return Unit switch
        {
            Unit.Pixels => (Value / ValidateDpi(dpi)) * CentimetersPerInch,
            Unit.Percentage => (((Value / 100.0) * ValidateBaseSize(baseSize)) / ValidateDpi(dpi)) * CentimetersPerInch,
            Unit.Millimeters => Value / 10.0,
            Unit.Centimeters => Value,
            Unit.Inches => Value * CentimetersPerInch,
            _ => throw new InvalidOperationException($"Unsupported unit: {Unit}")
        };
    }

    public double ToPercentage(double? dpi, double? baseSize)
    {
        if (Value == 0) return 0;
        if (Unit == Unit.Percentage) return Value;

        double bs = ValidateBaseSize(baseSize);
        if (bs == 0) throw new InvalidOperationException("Cannot convert a non-zero length to a percentage of a zero-length base.");

        return Unit switch
        {
            Unit.Pixels => (Value / bs) * 100.0,
            Unit.Millimeters => (((Value / MillimetersPerInch) * ValidateDpi(dpi)) / bs) * 100.0,
            Unit.Centimeters => (((Value / CentimetersPerInch) * ValidateDpi(dpi)) / bs) * 100.0,
            Unit.Inches => ((Value * ValidateDpi(dpi)) / bs) * 100.0,
            _ => throw new InvalidOperationException($"Unsupported unit: {Unit}")
        };
    }

    #endregion

    #region Operators

    public static FlexLength operator +(FlexLength length) => length;
    public static FlexLength operator -(FlexLength length) => new FlexLength(-length.Value, length.Unit);

    public static FlexLength operator +(FlexLength left, FlexLength right)
    {
        if (left.Value == 0) return right;
        if (right.Value == 0) return left;

        if (!AreAgnosticCompatible(left.Unit, right.Unit))
            throw new InvalidOperationException($"Units {left.Unit} and {right.Unit} are not compatible for context-free arithmetic.");

        double convertedValue = ConvertAgnostic(right.Value, right.Unit, left.Unit);
        return new FlexLength(left.Value + convertedValue, left.Unit);
    }

    public static FlexLength operator -(FlexLength left, FlexLength right)
    {
        if (right.Value == 0) return left;
        if (left.Value == 0) return -right;

        if (!AreAgnosticCompatible(left.Unit, right.Unit))
            throw new InvalidOperationException($"Units {left.Unit} and {right.Unit} are not compatible for context-free arithmetic.");

        double convertedValue = ConvertAgnostic(right.Value, right.Unit, left.Unit);
        return new FlexLength(left.Value - convertedValue, left.Unit);
    }

    public static bool operator ==(FlexLength left, FlexLength right) => left.Equals(right);
    public static bool operator !=(FlexLength left, FlexLength right) => !left.Equals(right);

    #endregion

    #region Equality and Overrides

    public bool Equals(FlexLength other)
    {
        if (Value == 0 && other.Value == 0) return true;
        return Value.Equals(other.Value) && Unit == other.Unit;
    }

    public override bool Equals(object? obj) => obj is FlexLength other && Equals(other);

    public override int GetHashCode()
    {
        if (Value == 0) return 0; // Use a constant for all zero-value variants
        return HashCode.Combine(Value, Unit);
    }

    public override string ToString() => ToString(null);

    public string ToString(string? format)
    {
        string fmt = format ?? LayoutConstants.DefaultNumericFormat;
        string unitStr = LayoutEnumConverter.ToShortString(Unit);
        return $"{Value.ToString(fmt, CultureInfo.InvariantCulture)}{unitStr}";
    }

    #endregion

    #region Private Helpers

    private static double ValidateDpi(double? dpi)
    {
        if (!dpi.HasValue) throw new InvalidOperationException("DPI context is required for this conversion.");
        if (dpi.Value <= 0) throw new InvalidOperationException("DPI must be a positive non-zero value.");
        return dpi.Value;
    }

    private static double ValidateBaseSize(double? baseSize)
    {
        return baseSize ?? throw new InvalidOperationException("Base size context is required for relative resolution (e.g. Percentage).");
    }

    private static bool AreAgnosticCompatible(Unit u1, Unit u2)
    {
        if (u1 == u2) return true;
        return IsAbsolute(u1) && IsAbsolute(u2);
    }

    private static bool IsAbsolute(Unit u) => u == Unit.Millimeters || u == Unit.Centimeters || u == Unit.Inches;

    private static double ConvertAgnostic(double value, Unit from, Unit to)
    {
        if (from == to) return value;
        
        if (from == Unit.Millimeters && to == Unit.Centimeters) return value / 10.0;
        if (from == Unit.Centimeters && to == Unit.Millimeters) return value * 10.0;

        double inches = from switch
        {
            Unit.Millimeters => value / MillimetersPerInch,
            Unit.Centimeters => value / CentimetersPerInch,
            Unit.Inches => value,
            _ => throw new InvalidOperationException("Conversion is not context-agnostic.")
        };

        return to switch
        {
            Unit.Millimeters => inches * MillimetersPerInch,
            Unit.Centimeters => inches * CentimetersPerInch,
            Unit.Inches => inches,
            _ => throw new InvalidOperationException("Conversion is not context-agnostic.")
        };
    }

    #endregion
}