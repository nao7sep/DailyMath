namespace DailyMath.Core.Rendering;

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Represents a platform-agnostic, mutable raster image (grid of pixels).
/// Provides methods for pixel-level manipulation and platform-specific I/O.
/// </summary>
public interface IImage : IDisposable
{
    // --- Properties ---

    /// <summary>
    /// Gets the width of the image in pixels.
    /// </summary>
    int Width { get; }

    /// <summary>
    /// Gets the height of the image in pixels.
    /// </summary>
    int Height { get; }

    /// <summary>
    /// Gets the pixel format of the raw image data.
    /// </summary>
    PixelFormat PixelFormat { get; }

    // --- Pixel Access ---

    /// <summary>
    /// Gets the color of the pixel at the specified coordinates.
    /// </summary>
    Color GetPixel(int x, int y);

    /// <summary>
    /// Sets the color of the pixel at the specified coordinates.
    /// </summary>
    void SetPixel(int x, int y, Color color);

    /// <summary>
    /// Copies the raw pixel data to the specified destination span.
    /// The destination span must be at least Width * Height * 4 bytes.
    /// </summary>
    /// <remarks>
    /// Pixel data is typically in RGBA format (4 bytes per pixel). Callers must ensure sufficient buffer size to prevent overflow.
    /// </remarks>
    void CopyPixels(Span<byte> destination);

    /// <summary>
    /// Writes raw pixel data from the specified source span into the image.
    /// The source span must be at least Width * Height * 4 bytes.
    /// </summary>
    /// <remarks>
    /// This operation directly overwrites all pixel data. The source data layout must match the image's PixelFormat.
    /// </remarks>
    void WritePixels(ReadOnlySpan<byte> source);

    // --- Transformation ---

    /// <summary>
    /// Rescales the image to the specified dimensions.
    /// </summary>
    void Resize(int newWidth, int newHeight);

    /// <summary>
    /// Rescales the image to fit/cover the specified target dimensions based on the scaling mode.
    /// </summary>
    public void Resize(int targetWidth, int targetHeight, ImageScaling scaling)
    {
        // Calculate new dimensions using layout calculator; delegates scaling algorithm to LayoutCalculator
        var (newW, newH) = DailyMath.Core.Layout.LayoutCalculator.Scale(Width, Height, targetWidth, targetHeight, scaling);

        // Avoid redundant resize operations if calculated dimensions are effectively equal (tolerance: 0.5px)
        if (Math.Abs(newW - Width) < 0.5 && Math.Abs(newH - Height) < 0.5)
            return;

        // Ensure minimum 1x1 dimensions to prevent zero-sized allocations; round floating-point results to nearest integer
        int finalWidth = Math.Max(1, (int)Math.Round(newW));
        int finalHeight = Math.Max(1, (int)Math.Round(newH));

        Resize(finalWidth, finalHeight);
    }

    // --- Rendering ---

    /// <summary>
    /// Creates a renderer for drawing directly onto this image.
    /// The returned renderer implements IDisposable and should be disposed after use to release platform-specific resources.
    /// </summary>
    IRenderer CreateRenderer();

    // --- I/O ---

    /// <summary>
    /// Encodes the image to a byte array.
    /// </summary>
    /// <remarks>
    /// The caller receives ownership of the returned byte array. Callers should dispose/clear the array if it contains sensitive data.
    /// </remarks>
    byte[] Encode(ImageFormat format, int? quality = null);

    /// <summary>
    /// Encodes the image to the specified stream.
    /// </summary>
    /// <remarks>
    /// The caller retains ownership and responsibility for the stream (e.g., flushing, disposal). This method does not close or dispose the stream.
    /// </remarks>
    void Encode(Stream stream, ImageFormat format, int? quality = null);

    /// <summary>
    /// Asynchronously encodes the image to a byte array.
    /// </summary>
    /// <remarks>
    /// Encoding is CPU-bound; use cancellationToken to allow cancellation of long-running operations on large images.
    /// </remarks>
    Task<byte[]> EncodeAsync(ImageFormat format, int? quality = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously encodes the image to the specified stream.
    /// </summary>
    /// <remarks>
    /// The caller retains ownership of the stream. Like the synchronous variant, this method does not dispose or flush the stream.
    /// </remarks>
    Task EncodeAsync(Stream stream, ImageFormat format, int? quality = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Encodes and saves the image to the specified file path.
    /// </summary>
    /// <remarks>
    /// This method creates or overwrites the file. The file is flushed and closed before the method returns.
    /// </remarks>
    void Save(string path, ImageFormat format, int? quality = null);

    /// <summary>
    /// Asynchronously encodes and saves the image to the specified file path.
    /// </summary>
    /// <remarks>
    /// The file is flushed and closed asynchronously. Use cancellationToken to cancel the operation.
    /// </remarks>
    Task SaveAsync(string path, ImageFormat format, int? quality = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Generic extension of IImage to support static factory methods (C# 11+).
/// </summary>
/// <remarks>
/// This interface enables factory methods that return the concrete type (TSelf) rather than the interface type.
/// Useful for immutable or specialized image implementations.
/// </remarks>
public interface IImage<TSelf> : IImage where TSelf : IImage<TSelf>
{
    /// <summary>
    /// Creates a new empty image with the specified dimensions.
    /// </summary>
    static abstract TSelf Create(int width, int height);

    /// <summary>
    /// Loads an image from a file.
    /// </summary>
    static abstract TSelf Load(string path);

    /// <summary>
    /// Asynchronously loads an image from a file.
    /// </summary>
    static abstract Task<TSelf> LoadAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads an image from encoded byte data.
    /// </summary>
    /// <remarks>
    /// The method takes ownership of the byte data during decoding; the caller retains responsibility for the original span.
    /// </remarks>
    static abstract TSelf Load(ReadOnlySpan<byte> data);

    /// <summary>
    /// Asynchronously loads an image from a stream.
    /// </summary>
    /// <remarks>
    /// The caller retains ownership and responsibility for stream disposal. This method does not close or dispose the stream.
    /// </remarks>
    static abstract Task<TSelf> LoadAsync(Stream stream, CancellationToken cancellationToken = default);
}
