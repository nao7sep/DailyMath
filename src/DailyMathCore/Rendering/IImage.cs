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
    void CopyPixels(Span<byte> destination);

    /// <summary>
    /// Writes raw pixel data from the specified source span into the image.
    /// The source span must be at least Width * Height * 4 bytes.
    /// </summary>
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
        var (newW, newH) = DailyMath.Core.Layout.LayoutCalculator.Scale(Width, Height, targetWidth, targetHeight, scaling);

        if (Math.Abs(newW - Width) < 0.5 && Math.Abs(newH - Height) < 0.5) 
            return;

        int finalWidth = Math.Max(1, (int)Math.Round(newW));
        int finalHeight = Math.Max(1, (int)Math.Round(newH));

        Resize(finalWidth, finalHeight);
    }

    // --- Rendering ---

    /// <summary>
    /// Creates a renderer for drawing directly onto this image.
    /// </summary>
    IRenderer CreateRenderer();

    // --- I/O ---

    /// <summary>
    /// Encodes the image to a byte array.
    /// </summary>
    byte[] Encode(ImageFormat format, int? quality = null);

    /// <summary>
    /// Encodes the image to the specified stream.
    /// </summary>
    void Encode(Stream stream, ImageFormat format, int? quality = null);

    /// <summary>
    /// Asynchronously encodes the image to a byte array.
    /// </summary>
    Task<byte[]> EncodeAsync(ImageFormat format, int? quality = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously encodes the image to the specified stream.
    /// </summary>
    Task EncodeAsync(Stream stream, ImageFormat format, int? quality = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Encodes and saves the image to the specified file path.
    /// </summary>
    void Save(string path, ImageFormat format, int? quality = null);

    /// <summary>
    /// Asynchronously encodes and saves the image to the specified file path.
    /// </summary>
    Task SaveAsync(string path, ImageFormat format, int? quality = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Generic extension of IImage to support static factory methods (C# 11+).
/// </summary>
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
    static abstract TSelf Load(ReadOnlySpan<byte> data);
    
    /// <summary>
    /// Asynchronously loads an image from a stream.
    /// </summary>
    static abstract Task<TSelf> LoadAsync(Stream stream, CancellationToken cancellationToken = default);
}
