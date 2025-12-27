namespace DailyMath.Core.Rendering;

/// <summary>
/// Options for encoding/saving an image.
/// Currently controls JPEG quality; other metadata and EXIF properties are not preserved.
/// </summary>
public record ImageEncodeOptions
{
    /// <summary>
    /// JPEG quality level (0-100). Ignored for non-JPEG formats (PNG, BMP, etc.).
    /// </summary>
    public required int JpegQuality { get; init; }

    // --- Presets ---

    /// <summary>
    /// Standard quality (75): good for regular use and web distribution. Balances quality and file size.
    /// </summary>
    public static ImageEncodeOptions Default => new()
    {
        JpegQuality = 75
    };

    /// <summary>
    /// Better quality (85): suitable for most professional workflows and high-quality display.
    /// </summary>
    public static ImageEncodeOptions Better => new()
    {
        JpegQuality = 85
    };

    /// <summary>
    /// Best quality (95): for printing, archival, and preservation where quality is paramount.
    /// </summary>
    public static ImageEncodeOptions Best => new()
    {
        JpegQuality = 95
    };
}
