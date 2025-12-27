namespace DailyMath.Core.Layout;

using System.Globalization;

/// <summary>
/// Represents the desired width and height of an element.
/// </summary>
public struct Measure
{
    /// <summary>
    /// Desired width. Supports Pixels, Inches, Millimeters, and Percent.
    /// Percent is evaluated against the parent's content width.
    /// </summary>
    public Length Width { get; set; }

    /// <summary>
    /// Desired height. Supports Pixels, Inches, Millimeters, and Percent.
    /// Percent is evaluated against the parent's content height.
    /// </summary>
    public Length Height { get; set; }

    public Measure(Length width, Length height)
    {
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Returns a string representation of the measure with the specified format.
    /// </summary>
    /// <param name="format">The numeric format string (e.g., "0.##", "F2").</param>
    /// <param name="includeTypeName">If true, prefixes the result with "Measure: ".</param>
    /// <returns>A string representation of the measure.</returns>
    public string ToString(string format, bool includeTypeName = false)
    {
        string sizes = $"{Width.ToString(format)}, {Height.ToString(format)}";
        return includeTypeName ? $"Measure: {sizes}" : sizes;
    }

    /// <summary>
    /// Returns a string representation of the measure (e.g., "100px, 200px").
    /// </summary>
    public override string ToString() => ToString("0.##", false);
}
