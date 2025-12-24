namespace DailyMath.Core.Rendering;

/// <summary>
/// Represents a color with red, green, blue, and alpha components.
/// Platform-agnostic color representation for cross-platform rendering.
/// </summary>
public readonly struct Color
{
    public byte R { get; }
    public byte G { get; }
    public byte B { get; }
    public byte A { get; }

    public Color(byte r, byte g, byte b, byte a = 255)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    // Basic colors
    public static Color Black => new(0, 0, 0);
    public static Color White => new(255, 255, 255);
    public static Color Transparent => new(0, 0, 0, 0);

    // Highly distinct debug colors for border visualization
    public static Color Red => new(255, 0, 0);
    public static Color Green => new(0, 255, 0);
    public static Color Blue => new(0, 0, 255);
    public static Color Yellow => new(255, 255, 0);
    public static Color Cyan => new(0, 255, 255);
    public static Color Magenta => new(255, 0, 255);
    public static Color Orange => new(255, 165, 0);
    public static Color Purple => new(128, 0, 128);

    public override string ToString() => A == 255
        ? $"#{R:X2}{G:X2}{B:X2}"
        : $"#{R:X2}{G:X2}{B:X2}{A:X2}";
}
