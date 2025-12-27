namespace DailyMath.Core.Rendering;

/// <summary>
/// Specifies the file format for canvas export and serialization.
/// </summary>
public enum ExportFormat
{
    /// <summary>
    /// Portable Network Graphics (PNG).
    /// Lossless compression supporting 8-bit, 24-bit, and 32-bit (alpha) color.
    /// Best for line art, text, and graphics requiring transparency.
    /// </summary>
    Png,

    /// <summary>
    /// WebP Image Format.
    /// A modern format developed by Google that provides superior lossless and lossy compression.
    /// Supports transparency (alpha channel) in both lossy and lossless modes.
    /// Supported by all modern browsers.
    /// </summary>
    Webp,

    /// <summary>
    /// AV1 Image File Format (AVIF).
    /// A next-generation format based on the AV1 video codec.
    /// Offers significantly better compression than WebP and JPEG with high quality.
    /// </summary>
    Avif,

    /// <summary>
    /// Joint Photographic Experts Group (JPEG).
    /// Lossy compression optimized for photographs and realistic scenes.
    /// Does not support transparency.
    /// </summary>
    Jpeg,

    /// <summary>
    /// Graphics Interchange Format (GIF).
    /// Legacy format limited to a 256-color palette.
    /// Supports simple transparency (1-bit alpha) and animation.
    /// Generally inferior to PNG/WebP for static images but widely supported.
    /// </summary>
    Gif,

    /// <summary>
    /// High Efficiency Image File Format (HEIF).
    /// The standard format for photos on modern iOS devices.
    /// Based on HEVC (H.265) video compression.
    /// </summary>
    Heif,

    /// <summary>
    /// Windows Icon Format (ICO).
    /// A container format that can store multiple images of different sizes and color depths.
    /// Used specifically for Windows executable icons and website favicons.
    /// </summary>
    Ico,

    /// <summary>
    /// Windows Bitmap (BMP).
    /// An uncompressed raster graphics format.
    /// Large file sizes but extremely fast to read/write as no decoding is required.
    /// </summary>
    Bmp,

    /// <summary>
    /// Portable Document Format (PDF).
    /// A vector-based document format supporting text, images, and graphics.
    /// Ideal for archival, distribution, and printing with consistent appearance across devices.
    /// </summary>
    Pdf,

    /// <summary>
    /// Tagged Image File Format (TIFF).
    /// A flexible format widely used in professional photography and desktop publishing.
    /// Supports lossless compression (LZW, Zip) and CMYK color spaces.
    /// </summary>
    Tiff
}