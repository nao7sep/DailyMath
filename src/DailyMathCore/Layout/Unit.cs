namespace DailyMath.Core.Layout;

/// <summary>
/// Specifies the unit of measurement for a length value.
/// </summary>
/// <remarks>
/// Note: Centimeters are intentionally not included. While 1 inch = 2.54 cm might suggest
/// including centimeters, cm and mm are highly compatible and easily convertible (1 cm = 10 mm).
/// In printing and precise layout work, millimeters are almost always simpler and more convenient
/// than centimeters, so we removed the less commonly used unit for simplicity.
/// </remarks>
public enum Unit
{
    /// <summary>
    /// Absolute pixels. Does not scale with DPI.
    /// </summary>
    Pixels,

    /// <summary>
    /// Physical inches. Scales with DPI.
    /// </summary>
    Inches,

    /// <summary>
    /// Physical millimeters. Scales with DPI.
    /// </summary>
    Millimeters,

    /// <summary>
    /// Percentage of parent's content area (0-100).
    /// Enables relative sizing without a separate "relative mode".
    /// </summary>
    Percent
}
