using System;
using System.Collections.Concurrent;
using SkiaSharp;
using DailyMath.Core.Layout;
using DailyMath.Core.Rendering;
using Point = DailyMath.Core.Layout.Point;

namespace DailyMath.Core.Skia;

/// <summary>
/// SkiaSharp-based renderer implementation.
/// Draws directly to an SKCanvas.
/// </summary>
public sealed class SkiaRenderer : IRenderer
{
    // --- Caching ---
    
    private static readonly ConcurrentDictionary<(string?, FontWeight, bool), SKTypeface> _typefaceCache = new();

    // --- Fields ---

    private readonly SKCanvas _canvas;
    private readonly bool _ownsCanvas;
    private readonly SKPaint _paint; 
    private readonly SKFont _font;   
    private bool _disposed;

    // --- Constructor ---

    /// <summary>
    /// Initializes a new instance of the <see cref="SkiaRenderer"/> class.
    /// </summary>
    /// <param name="canvas">The canvas to draw on.</param>
    /// <param name="ownsCanvas">If true, disposes the canvas when the renderer is disposed.</param>
    public SkiaRenderer(SKCanvas canvas, bool ownsCanvas = false)
    {
        _canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
        _ownsCanvas = ownsCanvas;
        
        _paint = new SKPaint { IsAntialias = true };
        _font = new SKFont { Subpixel = true, Edging = SKFontEdging.SubpixelAntialias };
    }

    // --- Element-Based Drawing (High Level) ---

    public void DrawText(Element target, string text, FontSpec font, ContentAlignment alignment, Color color)
    {
        var region = target.GetAbsoluteRegion();
        var contentRegion = LayoutCalculator.ApplyPadding(region, target.Padding, target.GetEffectiveDpi());
        DrawText(contentRegion, target.GetEffectiveDpi(), text, font, alignment, color);
    }

    public void DrawTextToFit(Element target, string text, FontSpec baseFont, ContentAlignment alignment, Color color, TextFitMode fitMode = TextFitMode.HeightOnly, double minSizePoints = 6, double maxSizePoints = 72)
    {
        var region = target.GetAbsoluteRegion();
        var contentRegion = LayoutCalculator.ApplyPadding(region, target.Padding, target.GetEffectiveDpi());
        DrawTextToFit(contentRegion, target.GetEffectiveDpi(), text, baseFont, alignment, color, fitMode, minSizePoints, maxSizePoints);
    }

    public void DrawRectangle(Element target, Color? borderColor, Color? fillColor, double borderThickness = 1.0)
    {
        var region = target.GetAbsoluteRegion();
        DrawRectangle(region, borderColor, fillColor, borderThickness);
    }

    public void DrawImage(Element target, IImage image, ContentAlignment alignment, ImageScaling scaling = ImageScaling.Fit)
    {
        var region = target.GetAbsoluteRegion();
        var contentRegion = LayoutCalculator.ApplyPadding(region, target.Padding, target.GetEffectiveDpi());
        DrawImage(contentRegion, image, alignment, scaling);
    }

    // --- Region-Based Drawing (Low Level) ---

    public void DrawText(Region region, double dpi, string text, FontSpec font, ContentAlignment alignment, Color color)
    {
        if (string.IsNullOrEmpty(text)) return;

        ResetPaint();
        _paint.Color = ToSkColor(color);
        
        _font.Typeface = GetTypefaceCached(font);
        _font.Size = (float)(font.SizeInPoints * (dpi / 72.0));

        _font.MeasureText(text, out var textBounds, _paint);

        var pos = LayoutCalculator.Align(region, textBounds.Width, -textBounds.Top + textBounds.Bottom, alignment);
        float baselineY = (float)pos.Y - textBounds.Top;

        _canvas.DrawText(text, (float)pos.X, baselineY, _font, _paint);
    }

    public void DrawTextToFit(Region region, double dpi, string text, FontSpec baseFont, ContentAlignment alignment, Color color, TextFitMode fitMode = TextFitMode.HeightOnly, double minSizePoints = 6, double maxSizePoints = 72)
    {
        if (string.IsNullOrEmpty(text)) return;

        double low = minSizePoints;
        double high = maxSizePoints;
        double optimalSize = minSizePoints;

        _font.Typeface = GetTypefaceCached(baseFont);
        
        while (high - low > 0.5)
        {
            double mid = (low + high) / 2.0;
            float pixelSize = (float)(mid * (dpi / 72.0));
            
            _font.Size = pixelSize;
            _font.MeasureText(text, out var bounds, _paint);
            
            float textHeight = _font.Spacing; 
            float textWidth = bounds.Width;

            bool fitsWidth = fitMode == TextFitMode.HeightOnly || textWidth <= region.Width;
            bool fitsHeight = fitMode == TextFitMode.WidthOnly || textHeight <= region.Height;

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

    public void DrawRectangle(Region region, Color? strokeColor, Color? fillColor, double strokeThickness = 1.0)
    {
        ResetPaint();
        var rect = ToSkRect(region);

        if (fillColor.HasValue)
        {
            _paint.Style = SKPaintStyle.Fill;
            _paint.Color = ToSkColor(fillColor.Value);
            _canvas.DrawRect(rect, _paint);
        }

        if (strokeColor.HasValue && strokeThickness > 0)
        {
            _paint.Style = SKPaintStyle.Stroke;
            _paint.Color = ToSkColor(strokeColor.Value);
            _paint.StrokeWidth = (float)strokeThickness;
            _canvas.DrawRect(rect, _paint);
        }
    }

    public void DrawLine(Point start, Point end, Color color, double thickness = 1.0)
    {
        ResetPaint();
        _paint.Style = SKPaintStyle.Stroke;
        _paint.Color = ToSkColor(color);
        _paint.StrokeWidth = (float)thickness;
        
        _canvas.DrawLine((float)start.X, (float)start.Y, (float)end.X, (float)end.Y, _paint);
    }

    public void DrawImage(Region region, IImage image, ContentAlignment alignment, ImageScaling scaling = ImageScaling.Fit)
    {
        if (image == null) return;

        SKBitmap? tempBitmap = null;
        SKBitmap sourceBitmap;

        if (image is SkiaImage skiaImage)
        {
            sourceBitmap = skiaImage.Bitmap;
        }
        else
        {
            int width = image.Width;
            int height = image.Height;
            byte[] pixels = new byte[width * height * 4];
            image.CopyPixels(pixels);

            var info = new SKImageInfo(width, height, 
                image.PixelFormat == PixelFormat.Bgra8888 ? SKColorType.Bgra8888 : SKColorType.Rgba8888, 
                SKAlphaType.Premul); 
                
            tempBitmap = new SKBitmap(info);
            var ptr = tempBitmap.GetPixels();
            System.Runtime.InteropServices.Marshal.Copy(pixels, 0, ptr, pixels.Length);
            
            sourceBitmap = tempBitmap;
        }

        try
        {
            ResetPaint();

            var (newW, newH) = LayoutCalculator.Scale(sourceBitmap.Width, sourceBitmap.Height, region.Width, region.Height, scaling);
            var pos = LayoutCalculator.Align(region, newW, newH, alignment);
            
            var srcRect = new SKRect(0, 0, sourceBitmap.Width, sourceBitmap.Height);
            var destRect = new SKRect((float)pos.X, (float)pos.Y, (float)(pos.X + newW), (float)(pos.Y + newH));

            using var skImage = SKImage.FromBitmap(sourceBitmap);
            var sampling = new SKSamplingOptions(SKCubicResampler.Mitchell);
            _canvas.DrawImage(skImage, srcRect, destRect, sampling, _paint);
        }
        finally
        {
            tempBitmap?.Dispose();
        }
    }

    // --- Dispose ---

    public void Dispose()
    {
        if (!_disposed)
        {
            _paint.Dispose();
            _font.Dispose(); 
            if (_ownsCanvas)
            {
                _canvas.Dispose(); 
            }
            _disposed = true;
        }
    }

    // --- Helpers ---

    private void ResetPaint()
    {
        _paint.Reset();
        _paint.IsAntialias = true;
    }

    private static SKRect ToSkRect(Region r) => new SKRect((float)r.Left, (float)r.Top, (float)r.Right, (float)r.Bottom);

    private static SKColor ToSkColor(Color c) => new SKColor(c.R, c.G, c.B, c.A);

    private static SKTypeface GetTypefaceCached(FontSpec spec)
    {
        var key = (spec.Family, spec.Weight, spec.Style.HasFlag(DailyMath.Core.Rendering.FontStyle.Italic));
        
        return _typefaceCache.GetOrAdd(key, k => 
        {
            var (family, weightEnum, isItalic) = k;
            var weight = (SKFontStyleWeight)(int)weightEnum;
            var slant = isItalic ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright;

            return SKTypeface.FromFamilyName(family, weight, SKFontStyleWidth.Normal, slant) 
                   ?? SKTypeface.FromFamilyName(null, weight, SKFontStyleWidth.Normal, slant);
        });
    }
}
