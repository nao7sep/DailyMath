namespace DailyMath.Core.Rendering;

/// <summary>
/// Controls whether the alpha component is included in color string output.
/// </summary>
public enum ColorAlphaInclusion
{
    /// <summary>
    /// Always include alpha in the output.
    /// </summary>
    Always,

    /// <summary>
    /// Include alpha only when the color is not fully opaque (A != 255).
    /// </summary>
    WhenNotOpaque,

    /// <summary>
    /// Never include alpha in the output.
    /// </summary>
    Never
}
