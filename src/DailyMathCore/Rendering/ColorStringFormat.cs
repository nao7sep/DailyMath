namespace DailyMath.Core.Rendering;

/// <summary>
/// Specifies the component order used when converting colors to and from strings.
/// Intended for textual hex representations (e.g., "#RRGGBB", "#AARRGGBB").
/// It is distinct from <see cref="PixelFormat"/>, which describes the layout of raw pixel data in memory.
/// </summary>
public enum ColorStringFormat
{
    /// <summary>
    /// Red-Green-Blue-Alpha order. Hex forms: #RRGGBB (alpha omitted) or #RRGGBBAA.
    /// Common in CSS-style hex where alpha is optional and often omitted when fully opaque.
    /// </summary>
    RGBA,

    /// <summary>
    /// Alpha-Red-Green-Blue order. Hex form: #AARRGGBB.
    /// Common in some APIs (e.g., ARGB in .NET color notations).
    /// </summary>
    ARGB
}
