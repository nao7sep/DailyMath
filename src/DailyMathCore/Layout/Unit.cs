namespace DailyMath.Core.Layout;

/// <summary>
/// Specifies the unit of measurement for a length value.
/// </summary>
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
    /// Physical centimeters. Scales with DPI.
    /// </summary>
    Centimeters,

    /// <summary>
    /// Percentage of parent's content area (0-100).
    /// Enables relative sizing without a separate "relative mode".
    /// </summary>
    Percent
}
