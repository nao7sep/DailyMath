namespace DailyMath.Core.Rendering;

/// <summary>
/// Specifies the file format for canvas export and serialization.
/// Includes formats supported by at least one implementation (SkiaCanvas or WpfCanvas), plus PDF.
/// </summary>
public enum ExportFormat
{
    /// <summary>
    /// Portable Document Format (PDF).
    /// A vector-based document format supporting text, images, and graphics.
    /// Ideal for archival, distribution, and printing with consistent appearance across devices.
    /// Supported by SkiaCanvas only.
    /// </summary>
    Pdf,

    /// <summary>
    /// Portable Network Graphics (PNG).
    /// Lossless compression supporting 8-bit, 24-bit, and 32-bit (alpha) color.
    /// Best for line art, text, and graphics requiring transparency.
    /// Supported by both SkiaCanvas and WpfCanvas.
    /// </summary>
    Png,

    /// <summary>
    /// Joint Photographic Experts Group (JPEG).
    /// Lossy compression optimized for photographs and realistic scenes.
    /// Does not support transparency.
    /// Supported by both SkiaCanvas and WpfCanvas.
    /// </summary>
    Jpeg,

    /// <summary>
    /// Graphics Interchange Format (GIF).
    /// Legacy format supporting simple transparency (1-bit alpha) and animation.
    /// Supported by both SkiaCanvas and WpfCanvas.
    /// </summary>
    Gif,

    /// <summary>
    /// Windows Bitmap (BMP).
    /// An uncompressed raster graphics format.
    /// Large file sizes but extremely fast to read/write as no decoding is required.
    /// Supported by both SkiaCanvas and WpfCanvas.
    /// </summary>
    Bmp,

    /// <summary>
    /// WebP Image Format.
    /// A modern format developed by Google that provides superior lossless and lossy compression.
    /// Supports transparency (alpha channel) in both lossy and lossless modes.
    /// Supported by SkiaCanvas only.
    /// </summary>
    Webp,

    /// <summary>
    /// High Efficiency Image File Format (HEIF).
    /// The standard format for photos on modern iOS devices.
    /// Based on HEVC (H.265) video compression.
    /// Supported by SkiaCanvas only.
    /// </summary>
    Heif,

    /// <summary>
    /// Tagged Image File Format (TIFF).
    /// A flexible format widely used in professional photography and desktop publishing.
    /// Supports lossless compression (LZW, Zip) and CMYK color spaces.
    /// Supported by WpfCanvas only.
    /// </summary>
    Tiff
}