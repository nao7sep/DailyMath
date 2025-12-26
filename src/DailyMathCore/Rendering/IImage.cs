namespace DailyMath.Core.Rendering;

using System;

/// <summary>
/// Represents a platform-agnostic, mutable raster image (grid of pixels).
/// Provides methods for pixel-level manipulation and platform-specific I/O.
/// </summary>
public interface IImage : IDisposable
{
    /// <summary>
    /// Gets the width of the image in pixels.
    /// </summary>
    int Width { get; }

    /// <summary>
    /// Gets the height of the image in pixels.
    /// </summary>
    int Height { get; }

    /// <summary>
    /// Gets the color of the pixel at the specified coordinates.
    /// </summary>
    Color GetPixel(int x, int y);

    /// <summary>
    /// Sets the color of the pixel at the specified coordinates.
    /// </summary>
    void SetPixel(int x, int y, Color color);

    /// <summary>
    /// Rescales the image to the specified dimensions.
    /// Implementation should use high-quality interpolation suitable for scaling.
    /// </summary>
    void Resize(int newWidth, int newHeight);

    /// <summary>
    /// Rescales the image to fit/cover the specified target dimensions based on the scaling mode.
    /// Calculates the correct aspect-conserving dimensions and calls <see cref="Resize(int, int)"/>.
    /// 
    /// <para>
    /// <strong>Implementation Note:</strong> This is a Default Interface Method (C# 8.0+).
    /// It provides shared logic for aspect ratio calculation so implementing classes only need 
    /// to provide the raw <see cref="Resize(int, int)"/> logic.
    /// </para>
    /// </summary>
    /// <param name="targetWidth">The width of the bounding box.</param>
    /// <param name="targetHeight">The height of the bounding box.</param>
    /// <param name="scaling">The scaling logic to apply.</param>
    public void Resize(int targetWidth, int targetHeight, ImageScaling scaling)
    {
        if (scaling == ImageScaling.None) return;
        if (scaling == ImageScaling.Stretch)
        {
            Resize(targetWidth, targetHeight);
            return;
        }

        double widthRatio = (double)targetWidth / Width;
        double heightRatio = (double)targetHeight / Height;
        double scaleFactor = 1.0;

        switch (scaling)
        {
            case ImageScaling.Fit:
                scaleFactor = Math.Min(widthRatio, heightRatio);
                break;
            case ImageScaling.FitDownOnly:
                scaleFactor = Math.Min(1.0, Math.Min(widthRatio, heightRatio));
                break;
            case ImageScaling.Cover:
                scaleFactor = Math.Max(widthRatio, heightRatio);
                break;
        }

        if (Math.Abs(scaleFactor - 1.0) < 0.001) return;

        int finalWidth = (int)Math.Round(Width * scaleFactor);
        int finalHeight = (int)Math.Round(Height * scaleFactor);
        
        // Ensure at least 1px
        finalWidth = Math.Max(1, finalWidth);
        finalHeight = Math.Max(1, finalHeight);

        Resize(finalWidth, finalHeight);
    }

    /// <summary>
    /// Encodes and saves the image to the specified file path.
    /// </summary>
    void Save(string path, ImageFormat format = ImageFormat.Png);

    /// <summary>
    /// Encodes the image to a byte array.
    /// </summary>
    byte[] Encode(ImageFormat format = ImageFormat.Png);
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
    /// Loads an image from encoded byte data.
    /// </summary>
    static abstract TSelf Load(ReadOnlySpan<byte> data);
}
