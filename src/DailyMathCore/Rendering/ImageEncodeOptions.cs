namespace DailyMath.Core.Rendering;

/// <summary>
/// Options for encoding/saving an image.
/// Currently controls JPEG quality; other metadata properties are not yet supported.
/// </summary>
/// <remarks>
/// <para>
/// <strong>Metadata Handling:</strong>
/// When an image is loaded with EXIF orientation metadata, SkiaImage and WpfImage automatically apply the orientation
/// transformation to the pixel data itself (via ApplyOrientation/TransformedBitmap). This means the in-memory bitmap
/// is already visually correct. However, when encoding, the original EXIF and metadata are not copied to the output file.
/// </para>
/// <para>
/// <strong>Why no EXIF preservation:</strong>
/// - SkiaSharp has minimal metadata support; extracting/re-encoding EXIF requires external libraries.
/// - WPF BitmapMetadata is platform-specific and doesn't work well cross-format.
/// - For generated images (the common case), metadata is unnecessary and increases file size.
/// - If you need to preserve EXIF, consider external tools (ImageMagick, ExifTool) as post-processing.
/// </para>
/// <para>
/// <strong>Color Profile:</strong>
/// ICC color profiles are also omitted when saving. This is safe for generated/synthetic images (web, UI, math rendering)
/// but risky for photos or color-critical work. If the source image has a color profile embedded, colors may render differently
/// on devices with different default profiles. This limitation is acceptable for generated/synthetic images; professional color work
/// should use dedicated tools (Lightroom, Photoshop, GIMP).
/// </para>
/// <para>
/// <strong>Result after save:</strong>
/// The saved file contains correctly-oriented pixel data with no EXIF orientation tag (or default TopLeft), no embedded ICC profile,
/// and no other metadata. Re-opening the file will display correctly without double-rotation. All EXIF (camera, date, GPS) and
/// color profile data is lost.
/// </para>
/// </remarks>
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
