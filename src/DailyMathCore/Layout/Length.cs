namespace DailyMath.Core.Layout;

/// <summary>
/// Represents a length measurement with a value and unit.
/// Immutable struct that can be converted to various units given context (parent size, DPI).
///
/// Design Philosophy:
/// - Supports absolute physical units (Inches, Millimeters) for print-oriented layouts where paper dimensions are known.
/// - Supports Percent for flexible sizing relative to parent content area.
/// - Arithmetic operators (+, -) work only on physical units, enabling calculations like:
///   bodyHeight = paperHeight - headerHeight - footerHeight
/// - This approach is suitable for deterministic print layouts, not real-time dynamic/responsive layouts.
/// </summary>
public readonly struct Length
{
    /// <summary>
    /// Millimeters per inch conversion constant.
    /// </summary>
    public const double MillimetersPerInch = 25.4;

    public double Value { get; }
    public Unit Unit { get; }

    public Length(double value, Unit unit)
    {
        Value = value;
        Unit = unit;
    }

    /// <summary>
    /// Converts this length to pixels.
    /// </summary>
    /// <param name="dpi">Dots per inch for physical unit conversion. Required for Inches and Millimeters.</param>
    /// <param name="parentContentSize">The size of the parent's content area in pixels. Required for Percent.</param>
    /// <returns>The length in pixels.</returns>
    /// <exception cref="ArgumentNullException">Thrown when a required parameter is null.</exception>
    public double ToPixels(double? dpi = null, double? parentContentSize = null)
    {
        return Unit switch
        {
            Unit.Pixels => Value,
            Unit.Inches => dpi.HasValue
                ? Value * dpi.Value
                : throw new ArgumentNullException(nameof(dpi), "DPI is required to convert inches to pixels"),
            Unit.Millimeters => dpi.HasValue
                ? (Value / MillimetersPerInch) * dpi.Value
                : throw new ArgumentNullException(nameof(dpi), "DPI is required to convert millimeters to pixels"),
            Unit.Percent => parentContentSize.HasValue
                ? parentContentSize.Value * (Value / 100.0)
                : throw new ArgumentNullException(nameof(parentContentSize), "Parent content size is required to convert percent to pixels"),
            _ => throw new ArgumentException($"Unsupported unit: {Unit}")
        };
    }

    /// <summary>
    /// Converts this length to inches.
    /// </summary>
    /// <param name="dpi">Dots per inch for physical unit conversion. Required for Pixels.</param>
    /// <param name="parentContentSize">The size of the parent's content area in inches. Required for Percent.</param>
    /// <returns>The length in inches.</returns>
    /// <exception cref="ArgumentNullException">Thrown when a required parameter is null.</exception>
    public double ToInches(double? dpi = null, double? parentContentSize = null)
    {
        return Unit switch
        {
            Unit.Pixels => dpi.HasValue
                ? Value / dpi.Value
                : throw new ArgumentNullException(nameof(dpi), "DPI is required to convert pixels to inches"),
            Unit.Inches => Value,
            Unit.Millimeters => Value / MillimetersPerInch,
            Unit.Percent => parentContentSize.HasValue
                ? parentContentSize.Value * (Value / 100.0)
                : throw new ArgumentNullException(nameof(parentContentSize), "Parent content size is required to convert percent to inches"),
            _ => throw new ArgumentException($"Unsupported unit: {Unit}")
        };
    }

    /// <summary>
    /// Converts this length to millimeters.
    /// </summary>
    /// <param name="dpi">Dots per inch for physical unit conversion. Required for Pixels.</param>
    /// <param name="parentContentSize">The size of the parent's content area in millimeters. Required for Percent.</param>
    /// <returns>The length in millimeters.</returns>
    /// <exception cref="ArgumentNullException">Thrown when a required parameter is null.</exception>
    public double ToMillimeters(double? dpi = null, double? parentContentSize = null)
    {
        return Unit switch
        {
            Unit.Pixels => dpi.HasValue
                ? (Value / dpi.Value) * MillimetersPerInch
                : throw new ArgumentNullException(nameof(dpi), "DPI is required to convert pixels to millimeters"),
            Unit.Inches => Value * MillimetersPerInch,
            Unit.Millimeters => Value,
            Unit.Percent => parentContentSize.HasValue
                ? parentContentSize.Value * (Value / 100.0)
                : throw new ArgumentNullException(nameof(parentContentSize), "Parent content size is required to convert percent to millimeters"),
            _ => throw new ArgumentException($"Unsupported unit: {Unit}")
        };
    }

    /// <summary>
    /// Converts this length to a percentage.
    /// </summary>
    /// <param name="dpi">Dots per inch for physical unit conversion. Required for Pixels.</param>
    /// <param name="parentContentSize">The size of the parent's content area in the same unit as this length.
    /// Required for all units except Percent. Must not be zero.</param>
    /// <returns>The length as a percentage (0-100).</returns>
    /// <exception cref="ArgumentNullException">Thrown when a required parameter is null.</exception>
    /// <exception cref="ArgumentException">Thrown when parentContentSize is zero.</exception>
    public double ToPercent(double? dpi = null, double? parentContentSize = null)
    {
        return Unit switch
        {
            Unit.Pixels => dpi.HasValue && parentContentSize.HasValue
                ? parentContentSize.Value != 0
                    ? (Value / parentContentSize.Value) * 100.0
                    : throw new ArgumentException("Parent content size cannot be zero when converting to percent", nameof(parentContentSize))
                : !dpi.HasValue
                    ? throw new ArgumentNullException(nameof(dpi), "DPI is required to convert pixels to percent")
                    : throw new ArgumentNullException(nameof(parentContentSize), "Parent content size is required to convert to percent"),
            Unit.Inches => parentContentSize.HasValue
                ? parentContentSize.Value != 0
                    ? (Value / parentContentSize.Value) * 100.0
                    : throw new ArgumentException("Parent content size cannot be zero when converting to percent", nameof(parentContentSize))
                : throw new ArgumentNullException(nameof(parentContentSize), "Parent content size is required to convert to percent"),
            Unit.Millimeters => parentContentSize.HasValue
                ? parentContentSize.Value != 0
                    ? (Value / parentContentSize.Value) * 100.0
                    : throw new ArgumentException("Parent content size cannot be zero when converting to percent", nameof(parentContentSize))
                : throw new ArgumentNullException(nameof(parentContentSize), "Parent content size is required to convert to percent"),
            Unit.Percent => Value,
            _ => throw new ArgumentException($"Unsupported unit: {Unit}")
        };
    }

    // Arithmetic Operations
    // Enables pre-calculation of lengths for print layouts, e.g.:
    // Length bodyHeight = paperHeight - headerHeight - footerHeight;

    /// <summary>
    /// Adds two lengths together. Both operands must use absolute physical units (Inches or Millimeters).
    /// The result uses the unit of the left operand.
    ///
    /// Use case: Calculate total dimensions, e.g., totalMargin = leftMargin + rightMargin
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when either operand uses Percent or Pixels.</exception>
    public static Length operator +(Length a, Length b)
    {
        ValidateArithmeticOperands(a, b);
        double bInAUnits = ConvertToUnit(b, a.Unit);
        return new Length(a.Value + bInAUnits, a.Unit);
    }

    /// <summary>
    /// Subtracts one length from another. Both operands must use absolute physical units (Inches or Millimeters).
    /// The result uses the unit of the left operand.
    ///
    /// Use case: Calculate remaining space, e.g., bodyHeight = paperHeight - headerHeight - footerHeight
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when either operand uses Percent or Pixels.</exception>
    public static Length operator -(Length a, Length b)
    {
        ValidateArithmeticOperands(a, b);
        double bInAUnits = ConvertToUnit(b, a.Unit);
        return new Length(a.Value - bInAUnits, a.Unit);
    }

    /// <summary>
    /// Validates that both operands use absolute physical units (Inches or Millimeters only).
    /// </summary>
    private static void ValidateArithmeticOperands(Length a, Length b)
    {
        if (a.Unit == Unit.Percent || b.Unit == Unit.Percent)
            throw new InvalidOperationException("Cannot perform arithmetic with percentage-based lengths. Percentages are relative and require parent context.");
        if (a.Unit == Unit.Pixels || b.Unit == Unit.Pixels)
            throw new InvalidOperationException("Cannot perform arithmetic with pixel-based lengths. Pixels are DPI-dependent and ambiguous without context.");
    }

    /// <summary>
    /// Converts a length to the target unit. Only works for absolute physical units (Inches and Millimeters).
    /// </summary>
    private static double ConvertToUnit(Length length, Unit targetUnit)
    {
        if (length.Unit == targetUnit)
            return length.Value;

        // Convert between inches and millimeters
        if (length.Unit == Unit.Inches && targetUnit == Unit.Millimeters)
            return length.Value * MillimetersPerInch;
        if (length.Unit == Unit.Millimeters && targetUnit == Unit.Inches)
            return length.Value / MillimetersPerInch;

        throw new InvalidOperationException($"Cannot convert {length.Unit} to {targetUnit} in arithmetic context.");
    }

    // String Representation

    /// <summary>
    /// Converts this length to a string representation with custom number formatting.
    /// </summary>
    /// <param name="format">The numeric format string (e.g., "0.##", "0.###", "F2").
    /// See standard .NET numeric format strings.</param>
    /// <returns>A string representation of this length with the specified format.</returns>
    public string ToString(string format)
    {
        return Unit switch
        {
            Unit.Pixels => $"{Value.ToString(format)}px",
            Unit.Inches => $"{Value.ToString(format)}in",
            Unit.Millimeters => $"{Value.ToString(format)}mm",
            Unit.Percent => $"{Value.ToString(format)}%",
            _ => throw new ArgumentException($"Unsupported unit: {Unit}")
        };
    }

    /// <summary>
    /// Converts this length to a string representation with default formatting (up to 2 decimal places).
    /// </summary>
    public override string ToString() => ToString("0.##");
}
