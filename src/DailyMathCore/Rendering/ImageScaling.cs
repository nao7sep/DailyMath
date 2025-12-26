namespace DailyMath.Core.Rendering;

/// <summary>
/// Specifies how an image should be scaled relative to a target area.
/// </summary>
public enum ImageScaling
{
    /// <summary>
    /// No scaling is applied. The image is used at its original pixel dimensions.
    /// It may be larger or smaller than the target area.
    /// </summary>
    None,

    /// <summary>
    /// Scales the image (up or down) to be as large as possible while fitting entirely 
    /// within the target area. Aspect ratio is preserved.
    /// (Also known as "Contain" or "Uniform").
    /// </summary>
    Fit,

    /// <summary>
    /// Scales the image DOWN if it is larger than the target area, but keeps it at 
    /// original size if it is smaller. Aspect ratio is preserved.
    /// Useful for preventing pixelation of small icons.
    /// </summary>
    FitDownOnly,

    /// <summary>
    /// Scales the image (up or down) to fill the entire target area.
    /// If aspect ratios differ, the image will exceed the bounds in one dimension.
    /// Aspect ratio is preserved.
    /// (Also known as "Cover" or "UniformToFill").
    /// </summary>
    Cover,

    /// <summary>
    /// Stretches the image to exactly match the target width and height.
    /// Aspect ratio is NOT preserved (content may look distorted).
    /// </summary>
    Stretch
}
