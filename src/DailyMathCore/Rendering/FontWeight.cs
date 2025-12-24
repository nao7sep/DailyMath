namespace DailyMath.Core.Rendering;

/// <summary>
/// Specifies the density of a typeface, in terms of the lightness or heaviness of the strokes.
/// Values correspond to the standard web and OpenType weight scale (100-900).
/// Platform-agnostic font weight representation for cross-platform rendering.
/// </summary>
public enum FontWeight
{
    /// <summary>
    /// Thin weight (100) - very light strokes.
    /// </summary>
    Thin = 100,

    /// <summary>
    /// Extra light weight (200) - lighter than Light.
    /// </summary>
    ExtraLight = 200,

    /// <summary>
    /// Light weight (300) - lighter than Normal.
    /// </summary>
    Light = 300,

    /// <summary>
    /// Normal weight (400) - standard 'Regular' weight.
    /// </summary>
    Normal = 400,

    /// <summary>
    /// Medium weight (500) - between Normal and SemiBold.
    /// </summary>
    Medium = 500,

    /// <summary>
    /// Semi-bold weight (600) - between Medium and Bold.
    /// </summary>
    SemiBold = 600,

    /// <summary>
    /// Bold weight (700) - standard 'Bold' weight.
    /// </summary>
    Bold = 700,

    /// <summary>
    /// Extra bold weight (800) - heavier than Bold.
    /// </summary>
    ExtraBold = 800,

    /// <summary>
    /// Black weight (900) - heaviest weight.
    /// </summary>
    Black = 900
}
