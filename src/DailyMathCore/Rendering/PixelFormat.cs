namespace DailyMath.Core.Rendering;

/// <summary>
/// Specifies the layout of pixel data in memory.
/// This describes the raw binary representation used for image rendering and interop.
/// It is distinct from <see cref="ColorStringFormat"/>, which describes textual serialization.
/// </summary>
public enum PixelFormat
{
    /// <summary>
    /// Red, Green, Blue, Alpha (8 bits each).
    /// </summary>
    Rgba8888,

    /// <summary>
    /// Blue, Green, Red, Alpha (8 bits each).
    /// Common in Windows/WPF/GDI+.
    /// </summary>
    Bgra8888
}
