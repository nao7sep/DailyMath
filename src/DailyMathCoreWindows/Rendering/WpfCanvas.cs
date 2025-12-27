using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DailyMath.Core.Rendering;
using DailyMath.Core.Layout;

// Alias to resolve ambiguity
using CoreColor = DailyMath.Core.Rendering.Color;
using WpfColor = System.Windows.Media.Color;

namespace DailyMath.Core.Windows;

/// <summary>
/// WPF implementation of a mutable drawable canvas.
/// Wraps a WriteableBitmap for pixel manipulation.
/// </summary>
public sealed class WpfCanvas : ICanvas<WpfCanvas>
{
    private WriteableBitmap _bitmap;
    private bool _disposed;

    // --- Properties ---

    /// <summary>
    /// Gets the underlying WPF WriteableBitmap.
    /// </summary>
    public WriteableBitmap Bitmap
    {
        get
        {
            ThrowIfDisposed();
            return _bitmap;
        }
    }

    public int Width => _bitmap.PixelWidth;
    public int Height => _bitmap.PixelHeight;

    public DailyMath.Core.Rendering.PixelFormat PixelFormat => DailyMath.Core.Rendering.PixelFormat.Bgra8888;

    // --- Constructor ---

    private WpfCanvas(WriteableBitmap bitmap)
    {
        _bitmap = bitmap ?? throw new ArgumentNullException(nameof(bitmap));
    }

    // --- Factory Methods ---

    public static WpfCanvas Create(int width, int height)
    {
        if (width <= 0 || height <= 0)
            throw new ArgumentException("Dimensions must be positive.");

        var bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
        return new WpfCanvas(bitmap);
    }

    public static WpfCanvas Load(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException("Canvas file not found.", path);

        using var stream = File.OpenRead(path);
        return LoadFromStreamInternal(stream);
    }

    public static async Task<WpfCanvas> LoadAsync(string path, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException("Canvas file not found.", path);

        byte[] data;
        using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
        {
            data = new byte[stream.Length];
            await stream.ReadExactlyAsync(data, cancellationToken);
        }
        return Load(data);
    }

    public static WpfCanvas Load(ReadOnlySpan<byte> data)
    {
        using var stream = new MemoryStream(data.ToArray());
        return LoadFromStreamInternal(stream);
    }

    public static async Task<WpfCanvas> LoadAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;
        return LoadFromStreamInternal(memoryStream);
    }

    // --- Pixel Access ---

    public CoreColor GetPixel(int x, int y)
    {
        ThrowIfDisposed();
        CheckBounds(x, y);

        byte[] pixelData = new byte[4];
        int stride = 4;

        _bitmap.CopyPixels(new Int32Rect(x, y, 1, 1), pixelData, stride, 0);

        byte b = pixelData[0];
        byte g = pixelData[1];
        byte r = pixelData[2];
        byte a = pixelData[3];

        return new CoreColor(r, g, b, a);
    }

    public void SetPixel(int x, int y, CoreColor color)
    {
        ThrowIfDisposed();
        CheckBounds(x, y);

        byte[] pixelData = new byte[4];
        pixelData[0] = color.B;
        pixelData[1] = color.G;
        pixelData[2] = color.R;
        pixelData[3] = color.A;

        int stride = 4;
        _bitmap.WritePixels(new Int32Rect(x, y, 1, 1), pixelData, stride, 0);
    }

    public void CopyPixels(Span<byte> destination)
    {
        ThrowIfDisposed();
        ValidatePixelBufferSize(destination.Length, nameof(destination));

        _bitmap.Lock();
        try
        {
            int stride = _bitmap.BackBufferStride;
            int widthBytes = Width * 4;
            int expectedSize = GetExpectedPixelBufferSize();

            // Use unsafe for direct memory access to the WriteableBitmap back buffer.
            // This avoids intermediate allocations and provides optimal copy performance.
            // The fixed block pins the destination span while we copy from unmanaged memory.
            unsafe
            {
                byte* srcPtr = (byte*)_bitmap.BackBuffer.ToPointer();
                fixed (byte* dstPtr = destination)
                {
                    if (stride == widthBytes)
                    {
                        Buffer.MemoryCopy(srcPtr, dstPtr, destination.Length, expectedSize);
                    }
                    else
                    {
                        for (int row = 0; row < Height; row++)
                        {
                            Buffer.MemoryCopy(srcPtr + (row * stride), dstPtr + (row * widthBytes), widthBytes, widthBytes);
                        }
                    }
                }
            }
        }
        finally
        {
            _bitmap.Unlock();
        }
    }

    public void WritePixels(ReadOnlySpan<byte> source)
    {
        ThrowIfDisposed();
        ValidatePixelBufferSize(source.Length, nameof(source));

        _bitmap.Lock();
        try
        {
            int stride = _bitmap.BackBufferStride;
            int widthBytes = Width * 4;
            int expectedSize = GetExpectedPixelBufferSize();

            // Use unsafe for direct memory access to the WriteableBitmap back buffer.
            // This avoids intermediate allocations and provides optimal copy performance.
            // The fixed block pins the source span while we copy to unmanaged memory.
            unsafe
            {
                byte* dstPtr = (byte*)_bitmap.BackBuffer.ToPointer();
                fixed (byte* srcPtr = source)
                {
                    if (stride == widthBytes)
                    {
                        Buffer.MemoryCopy(srcPtr, dstPtr, expectedSize, expectedSize);
                    }
                    else
                    {
                        for (int row = 0; row < Height; row++)
                        {
                            Buffer.MemoryCopy(srcPtr + (row * widthBytes), dstPtr + (row * stride), widthBytes, widthBytes);
                        }
                    }
                }
            }
            _bitmap.AddDirtyRect(new Int32Rect(0, 0, Width, Height));
        }
        finally
        {
            _bitmap.Unlock();
        }
    }

    // --- Transformation ---

    public void Resize(int newWidth, int newHeight)
    {
        ThrowIfDisposed();
        if (newWidth <= 0 || newHeight <= 0)
            throw new ArgumentException("Dimensions must be positive.");

        var scaleX = (double)newWidth / Width;
        var scaleY = (double)newHeight / Height;

        var transform = new ScaleTransform(scaleX, scaleY);
        var transformed = new TransformedBitmap(_bitmap, transform);

        _bitmap = new WriteableBitmap(transformed);
    }

    // --- Rendering ---

    public IRenderer CreateRenderer()
    {
        ThrowIfDisposed();
        return new WpfRenderer(_bitmap);
    }

    // --- I/O ---

    public byte[] Encode(ExportFormat format, ExportOptions? options = null)
    {
        using var memoryStream = new MemoryStream();
        Encode(memoryStream, format, options);
        return memoryStream.ToArray();
    }

    public void Encode(Stream stream, ExportFormat format, ExportOptions? options = null)
    {
        ThrowIfDisposed();
        options ??= ExportOptions.Default;
        BitmapEncoder encoder = format switch
        {
            ExportFormat.Png => new PngBitmapEncoder(),
            ExportFormat.Jpeg => new JpegBitmapEncoder(),
            ExportFormat.Bmp => new BmpBitmapEncoder(),
            ExportFormat.Gif => new GifBitmapEncoder(),
            ExportFormat.Tiff => new TiffBitmapEncoder(),
            ExportFormat.Pdf => throw new NotSupportedException("PDF format is not supported by WpfCanvas implementation. Consider using SkiaCanvas for PDF support."),
            _ => throw new NotSupportedException($"Format {format} is not supported by WpfCanvas implementation.")
        };

        if (encoder is JpegBitmapEncoder jpegEncoder)
        {
            jpegEncoder.QualityLevel = options.JpegQuality;
        }

        encoder.Frames.Add(BitmapFrame.Create(_bitmap));
        encoder.Save(stream);
    }

    public async Task<byte[]> EncodeAsync(ExportFormat format, ExportOptions? options = null, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => Encode(format, options), cancellationToken);
    }

    public async Task EncodeAsync(Stream stream, ExportFormat format, ExportOptions? options = null, CancellationToken cancellationToken = default)
    {
        await Task.Run(() => Encode(stream, format, options), cancellationToken);
    }

    public void Save(string path, ExportFormat format, ExportOptions? options = null)
    {
        ThrowIfDisposed();
        using var stream = File.OpenWrite(path);
        Encode(stream, format, options);
    }

    public async Task SaveAsync(string path, ExportFormat format, ExportOptions? options = null, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        using var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
        await EncodeAsync(stream, format, options, cancellationToken);
    }

    // --- Dispose ---

    public void Dispose()
    {
        if (!_disposed)
        {
            _bitmap = null!;
            _disposed = true;
        }
    }

    // --- Helpers ---

    private static WpfCanvas LoadFromStreamInternal(Stream stream)
    {
        var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.OnLoad);
        var frame = decoder.Frames[0];

        BitmapSource source = frame;

        if (source.Format != PixelFormats.Bgra32)
        {
            source = new FormatConvertedBitmap(source, PixelFormats.Bgra32, null, 0);
        }

        return new WpfCanvas(new WriteableBitmap(source));
    }

    private void CheckBounds(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            throw new ArgumentOutOfRangeException("Coordinates are out of bounds.");
    }

    private int GetExpectedPixelBufferSize() => Width * Height * 4;

    private void ValidatePixelBufferSize(int bufferLength, string paramName)
    {
        int expectedSize = GetExpectedPixelBufferSize();
        if (bufferLength < expectedSize)
            throw new ArgumentException($"Buffer is too small. Expected {expectedSize} bytes.", paramName);
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(WpfCanvas));
    }
}