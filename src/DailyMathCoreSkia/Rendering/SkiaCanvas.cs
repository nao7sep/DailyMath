using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SkiaSharp;
using DailyMath.Core.Rendering;
using DailyMath.Core.Layout;

namespace DailyMath.Core.Skia;

/// <summary>
/// SkiaSharp implementation of a mutable drawable canvas.
/// Wraps an SKBitmap for pixel manipulation and storage.
/// Implements IDisposable via ICanvas interface - ensure to Dispose() to release unmanaged Skia resources.
/// </summary>
public sealed class SkiaCanvas : ICanvas<SkiaCanvas>
{
    private SKBitmap _bitmap;
    private bool _disposed;

    // --- Properties ---

    public SKBitmap Bitmap
    {
        get
        {
            ThrowIfDisposed();
            return _bitmap;
        }
    }

    public int Width => Bitmap.Width;
    public int Height => Bitmap.Height;

    public PixelFormat PixelFormat => PixelFormat.Rgba8888;

    // --- Constructor ---

    private SkiaCanvas(SKBitmap bitmap)
    {
        _bitmap = bitmap ?? throw new ArgumentNullException(nameof(bitmap));
    }

    // --- Factory Methods ---

    public static SkiaCanvas Create(int width, int height)
    {
        if (width <= 0 || height <= 0)
            throw new ArgumentException("Dimensions must be positive.");

        var info = new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Premul);
        var bitmap = new SKBitmap(info);
        return new SkiaCanvas(bitmap);
    }

    public static SkiaCanvas Load(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException("Canvas file not found.", path);

        using var stream = File.OpenRead(path);
        using var codec = SKCodec.Create(stream);
        return LoadFromCodec(codec);
    }

    public static async Task<SkiaCanvas> LoadAsync(string path, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException("Canvas file not found.", path);

        return await Task.Run(() =>
        {
            using var stream = File.OpenRead(path);
            using var codec = SKCodec.Create(stream);
            return LoadFromCodec(codec);
        }, cancellationToken);
    }

    public static SkiaCanvas Load(ReadOnlySpan<byte> data)
    {
        // Use unsafe to pin span memory for interop with SkiaSharp's unmanaged SKData API.
        // The fixed block ensures the GC won't move the data during the SKData.CreateCopy call.
        unsafe
        {
            fixed (byte* ptr = data)
            {
                using var skData = SKData.CreateCopy((IntPtr)ptr, data.Length);
                using var codec = SKCodec.Create(skData);
                return LoadFromCodec(codec);
            }
        }
    }

    public static async Task<SkiaCanvas> LoadAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        if (stream.CanSeek)
        {
            return await Task.Run(() =>
            {
                using var codec = SKCodec.Create(stream);
                return LoadFromCodec(codec);
            }, cancellationToken);
        }

        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;

        return await Task.Run(() =>
        {
            using var codec = SKCodec.Create(memoryStream);
            return LoadFromCodec(codec);
        }, cancellationToken);
    }

    // --- Pixel Access ---

    public Color GetPixel(int x, int y)
    {
        ThrowIfDisposed();
        var skColor = _bitmap.GetPixel(x, y);
        return new Color(skColor.Red, skColor.Green, skColor.Blue, skColor.Alpha);
    }

    public void SetPixel(int x, int y, Color color)
    {
        ThrowIfDisposed();
        _bitmap.SetPixel(x, y, new SKColor(color.R, color.G, color.B, color.A));
    }

    public void CopyPixels(Span<byte> destination)
    {
        ThrowIfDisposed();
        ValidatePixelBufferSize(destination.Length, nameof(destination));

        var srcSpan = _bitmap.GetPixelSpan();
        int rowBytes = _bitmap.RowBytes;
        int widthBytes = Width * 4;

        if (rowBytes == widthBytes)
        {
            srcSpan.CopyTo(destination);
        }
        else
        {
            for (int y = 0; y < Height; y++)
            {
                var rowSrc = srcSpan.Slice(y * rowBytes, widthBytes);
                var rowDst = destination.Slice(y * widthBytes, widthBytes);
                rowSrc.CopyTo(rowDst);
            }
        }
    }

    public void WritePixels(ReadOnlySpan<byte> source)
    {
        ThrowIfDisposed();
        ValidatePixelBufferSize(source.Length, nameof(source));

        var dstSpan = _bitmap.GetPixelSpan();
        int rowBytes = _bitmap.RowBytes;
        int widthBytes = Width * 4;

        if (rowBytes == widthBytes)
        {
            source.CopyTo(dstSpan);
        }
        else
        {
            for (int y = 0; y < Height; y++)
            {
                var rowSrc = source.Slice(y * widthBytes, widthBytes);
                var rowDst = dstSpan.Slice(y * rowBytes, widthBytes);
                rowSrc.CopyTo(rowDst);
            }
        }
    }

    // --- Transformation ---

    public void Resize(int newWidth, int newHeight)
    {
        ThrowIfDisposed();
        if (newWidth <= 0 || newHeight <= 0)
            throw new ArgumentException("Dimensions must be positive.");

        var info = new SKImageInfo(newWidth, newHeight, _bitmap.ColorType, _bitmap.AlphaType);
        var sampling = new SKSamplingOptions(SKCubicResampler.Mitchell);
        var resized = _bitmap.Resize(info, sampling);

        if (resized == null)
            throw new InvalidOperationException("Failed to resize image.");

        _bitmap.Dispose();
        _bitmap = resized;
    }

    // --- Rendering ---

    public IRenderer CreateRenderer()
    {
        ThrowIfDisposed();
        return new SkiaRenderer(new SKCanvas(_bitmap), true);
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
        var skFormat = MapFormat(format);
        int qualityValue = options.JpegQuality;

        using var image = SKImage.FromBitmap(_bitmap);
        using var data = image.Encode(skFormat, qualityValue);
        data.SaveTo(stream);
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
            _bitmap.Dispose();
            _disposed = true;
        }
    }

    // --- Helpers ---

    private static SkiaCanvas LoadFromCodec(SKCodec codec)
    {
        if (codec == null)
            throw new InvalidOperationException("Failed to create codec from input.");

        var info = codec.Info;
        var bitmap = new SKBitmap(info.Width, info.Height, SKColorType.Rgba8888, SKAlphaType.Premul);

        var result = codec.GetPixels(bitmap.Info, bitmap.GetPixels());
        if (result != SKCodecResult.Success)
        {
            bitmap.Dispose();
            throw new InvalidOperationException($"Failed to decode canvas: {result}");
        }

        return new SkiaCanvas(bitmap);
    }

    private int GetExpectedPixelBufferSize() => Width * Height * 4;

    private void ValidatePixelBufferSize(int bufferLength, string paramName)
    {
        int expectedSize = GetExpectedPixelBufferSize();
        if (bufferLength < expectedSize)
            throw new ArgumentException($"Buffer is too small. Expected {expectedSize} bytes.", paramName);
    }

    private static SKEncodedImageFormat MapFormat(ExportFormat format)
    {
        return format switch
        {
            ExportFormat.Png => SKEncodedImageFormat.Png,
            ExportFormat.Jpeg => SKEncodedImageFormat.Jpeg,
            ExportFormat.Webp => SKEncodedImageFormat.Webp,
            ExportFormat.Gif => SKEncodedImageFormat.Gif,
            ExportFormat.Bmp => SKEncodedImageFormat.Bmp,
            ExportFormat.Ico => SKEncodedImageFormat.Ico,
            ExportFormat.Heif => SKEncodedImageFormat.Heif,
            ExportFormat.Pdf => SKEncodedImageFormat.Pdf,
            _ => throw new NotSupportedException($"Format {format} is not supported by SkiaCanvas implementation.")
        };
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(SkiaCanvas));
    }
}