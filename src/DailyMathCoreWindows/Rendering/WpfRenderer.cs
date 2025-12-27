using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Globalization;
using System.Collections.Concurrent;
using DailyMath.Core.Layout;
using DailyMath.Core.Rendering;
using Point = DailyMath.Core.Layout.Point;
using WPoint = System.Windows.Point;

// Resolve Color Ambiguity
using CoreColor = DailyMath.Core.Rendering.Color;
using WpfColor = System.Windows.Media.Color;

namespace DailyMath.Core.Windows;

/// <summary>
/// WPF-based renderer implementation.
/// Draws directly to a DrawingContext.
/// </summary>
public sealed class WpfRenderer : IRenderer
{
    // --- Caching ---

    private static readonly ConcurrentDictionary<CoreColor, Brush> _brushCache = new();
    private static readonly ConcurrentDictionary<(string, System.Windows.FontWeight, bool), Typeface> _typefaceCache = new();

    // --- Fields ---

    private readonly DrawingContext _dc;
    private readonly bool _ownsContext;
    private readonly WriteableBitmap? _targetBitmap;
    private readonly DrawingVisual? _visual;
    private bool _disposed;

    // --- Constructors ---

    /// <summary>
    /// Initializes a new instance of the <see cref="WpfRenderer"/> class.
    /// </summary>
    /// <param name="dc">The drawing context to render into.</param>
    /// <param name="ownsContext">If true, closes the context when disposed.</param>
    public WpfRenderer(DrawingContext dc, bool ownsContext = false)
    {
        _dc = dc ?? throw new ArgumentNullException(nameof(dc));
        _ownsContext = ownsContext;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WpfRenderer"/> class targeting a WriteableBitmap.
    /// </summary>
    internal WpfRenderer(WriteableBitmap target)
    {
        _targetBitmap = target ?? throw new ArgumentNullException(nameof(target));
        _visual = new DrawingVisual();
        _dc = _visual.RenderOpen();
        _ownsContext = true;
    }

    // --- Element-Based Drawing (High Level) ---

    public void DrawText(Element target, string text, FontSpec font, ContentAlignment alignment, CoreColor color)
    {
        var region = target.GetAbsoluteRegion();
        var contentRegion = LayoutCalculator.ApplyPadding(region, target.Padding, target.GetEffectiveDpi());
        DrawText(contentRegion, target.GetEffectiveDpi(), text, font, alignment, color);
    }

    public void DrawTextToFit(Element target, string text, FontSpec baseFont, ContentAlignment alignment, CoreColor color, TextFitMode fitMode = TextFitMode.HeightOnly, double minSizePoints = 6, double maxSizePoints = 72)
    {
        var region = target.GetAbsoluteRegion();
        var contentRegion = LayoutCalculator.ApplyPadding(region, target.Padding, target.GetEffectiveDpi());
        DrawTextToFit(contentRegion, target.GetEffectiveDpi(), text, baseFont, alignment, color, fitMode, minSizePoints, maxSizePoints);
    }

    public void DrawRectangle(Element target, CoreColor? borderColor, CoreColor? fillColor, double borderThickness = 1.0)
    {
        var region = target.GetAbsoluteRegion();
        DrawRectangle(region, borderColor, fillColor, borderThickness);
    }

    public void DrawImage(Element target, ICanvas image, ContentAlignment alignment, ImageScaling scaling = ImageScaling.Fit)
    {
        var region = target.GetAbsoluteRegion();
        var contentRegion = LayoutCalculator.ApplyPadding(region, target.Padding, target.GetEffectiveDpi());
        DrawImage(contentRegion, image, alignment, scaling);
    }

    // --- Region-Based Drawing (Low Level) ---

    public void DrawText(Region region, double dpi, string text, FontSpec font, ContentAlignment alignment, CoreColor color)
    {
        if (string.IsNullOrEmpty(text)) return;

        double emSize = font.SizeInPoints * (96.0 / 72.0);

        var formattedText = new FormattedText(
            text,
            CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            GetTypefaceCached(font),
            emSize,
            GetBrushCached(color),
            dpi / 96.0);

        var pos = LayoutCalculator.Align(region, formattedText.Width, formattedText.Height, alignment);
        _dc.DrawText(formattedText, new WPoint(pos.X, pos.Y));
    }

    public void DrawTextToFit(Region region, double dpi, string text, FontSpec baseFont, ContentAlignment alignment, CoreColor color, TextFitMode fitMode = TextFitMode.HeightOnly, double minSizePoints = 6, double maxSizePoints = 72)
    {
        if (string.IsNullOrEmpty(text)) return;

        double low = minSizePoints;
        double high = maxSizePoints;
        double optimalSize = minSizePoints;
        var typeface = GetTypefaceCached(baseFont);
        double pixelsPerDip = dpi / 96.0;

        while (high - low > 0.5)
        {
            double mid = (low + high) / 2.0;
            double sizeInDips = mid * (96.0 / 72.0);

            var testText = new FormattedText(
                text,
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                typeface,
                sizeInDips,
                Brushes.Black,
                pixelsPerDip);

            bool fitsWidth = fitMode == TextFitMode.HeightOnly || testText.Width <= region.Width;
            bool fitsHeight = fitMode == TextFitMode.WidthOnly || testText.Height <= region.Height;

            if (fitsWidth && fitsHeight)
            {
                optimalSize = mid;
                low = mid;
            }
            else
            {
                high = mid;
            }
        }

        DrawText(region, dpi, text, baseFont.WithSize(optimalSize), alignment, color);
    }

    public void DrawRectangle(Region region, CoreColor? strokeColor, CoreColor? fillColor, double strokeThickness = 1.0)
    {
        Brush? fill = fillColor.HasValue ? GetBrushCached(fillColor.Value) : null;
        Pen? pen = null;

        if (strokeColor.HasValue && strokeThickness > 0)
        {
            pen = new Pen(GetBrushCached(strokeColor.Value), strokeThickness);
            // WPF centers pens on the stroke path by default.
            // This means for a 2px stroke on a rect at {0,0,100,100}, the outer 1px extends outside the logical bounds.
            // This is intentional—it prevents double borders when adjacent rects share edges (e.g., table cells).
            pen.Freeze();
        }

        if (fill != null || pen != null)
        {
            _dc.DrawRectangle(fill, pen, ToRect(region));
        }
    }

    public void DrawLine(Point start, Point end, CoreColor color, double thickness = 1.0)
    {
        var pen = new Pen(GetBrushCached(color), thickness);
        pen.Freeze();
        _dc.DrawLine(pen, new WPoint(start.X, start.Y), new WPoint(end.X, end.Y));
    }

    public void DrawImage(Region region, ICanvas image, ContentAlignment alignment, ImageScaling scaling = ImageScaling.Fit)
    {
        if (image == null) return;

        BitmapSource bitmapSource;
        if (image is WpfCanvas wpfCanvas)
        {
            bitmapSource = wpfCanvas.Bitmap;
        }
        else
        {
            int width = image.Width;
            int height = image.Height;
            int stride = width * 4;
            byte[] pixels = new byte[height * stride];
            image.CopyPixels(pixels);

            if (image.PixelFormat == DailyMath.Core.Rendering.PixelFormat.Rgba8888)
            {
                for (int i = 0; i < pixels.Length; i += 4)
                {
                    byte r = pixels[i];
                    byte b = pixels[i + 2];
                    pixels[i] = b;
                    pixels[i + 2] = r;
                }
            }

            bitmapSource = BitmapSource.Create(
                width, height, 96, 96,
                PixelFormats.Bgra32,
                null, pixels, stride);
            bitmapSource.Freeze();
        }

        var (newW, newH) = LayoutCalculator.Scale(bitmapSource.PixelWidth, bitmapSource.PixelHeight, region.Width, region.Height, scaling);
        var pos = LayoutCalculator.Align(region, newW, newH, alignment);

        var destRect = new Rect(pos.X, pos.Y, newW, newH);
        _dc.DrawImage(bitmapSource, destRect);
    }

    // --- Dispose ---

    public void Dispose()
    {
        if (!_disposed)
        {
            if (_ownsContext)
            {
                _dc.Close();
            }

            if (_targetBitmap != null && _visual != null)
            {
                var rtb = new RenderTargetBitmap(
                    _targetBitmap.PixelWidth, _targetBitmap.PixelHeight,
                    96, 96, PixelFormats.Pbgra32);
                rtb.Render(_visual);

                _targetBitmap.Lock();
                try
                {
                    int stride = _targetBitmap.BackBufferStride;
                    int height = _targetBitmap.PixelHeight;
                    rtb.CopyPixels(new Int32Rect(0, 0, _targetBitmap.PixelWidth, height), _targetBitmap.BackBuffer, stride * height, stride);
                    _targetBitmap.AddDirtyRect(new Int32Rect(0, 0, _targetBitmap.PixelWidth, height));
                }
                finally
                {
                    _targetBitmap.Unlock();
                }
            }

            _disposed = true;
        }
    }

    // --- Helpers ---

    private static Rect ToRect(Region region) => new Rect(region.Left, region.Top, region.Width, region.Height);

    private static Brush GetBrushCached(CoreColor color)
    {
        return _brushCache.GetOrAdd(color, c =>
        {
            var brush = new SolidColorBrush(WpfColor.FromArgb(c.A, c.R, c.G, c.B));
            brush.Freeze();
            return brush;
        });
    }

    private static Typeface GetTypefaceCached(FontSpec spec)
    {
        bool isItalic = spec.Style.HasFlag(DailyMath.Core.Rendering.FontStyle.Italic);
        var key = (spec.Family, System.Windows.FontWeight.FromOpenTypeWeight((int)spec.Weight), isItalic);

        return _typefaceCache.GetOrAdd(key, k =>
        {
            var (family, weight, italic) = k;
            var style = italic ? FontStyles.Italic : FontStyles.Normal;
            return new Typeface(new FontFamily(family), style, weight, FontStretches.Normal);
        });
    }
}