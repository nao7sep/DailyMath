namespace DailyMath.Core.Layout;

/// <summary>
/// Represents the desired width and height of an element.
/// </summary>
public struct Measure
{
    public Length Width { get; set; }
    public Length Height { get; set; }

    public Measure(Length width, Length height)
    {
        Width = width;
        Height = height;
    }
}
