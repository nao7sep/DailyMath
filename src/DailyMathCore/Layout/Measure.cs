namespace DailyMath.Core.Layout;

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
}
