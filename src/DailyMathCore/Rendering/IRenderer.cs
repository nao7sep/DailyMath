namespace DailyMath.Core.Rendering;

using DailyMath.Core.Layout;

/// <summary>
/// Abstraction for drawing content.
/// Implementations (Skia, WPF) manage the underlying graphics context.
/// 
/// <para>
/// <strong>Architecture Note:</strong><br/>
/// This interface provides two layers of drawing methods:
/// <list type="bullet">
/// <item><strong>Element-Based (High Level):</strong> Take an <see cref="Element"/>, automatically calculate 
/// its absolute content region (respecting padding), resolve DPI, and delegate to the Region-based method.</item>
/// <item><strong>Region-Based (Low Level):</strong> Take explicit <see cref="Region"/> and <see cref="Point"/> 
/// coordinates. Used for custom layouts or when drawing across multiple elements.</item>
/// </list>
/// </para>
/// </summary>
public interface IRenderer : IDisposable
{
    /// <summary>
    /// Draws text aligned within the target element's content area.
    /// Delegates to <see cref="DrawText(Region, double, string, FontSpec, ContentAlignment, Color)"/>.
    /// </summary>
    void DrawText(Element target, string text, FontSpec font, ContentAlignment alignment, Color color);

    /// <summary>
    /// Draws text aligned within an absolute region.
    /// </summary>
    /// <param name="region">Absolute coordinates in pixels.</param>
    /// <param name="dpi">DPI context for font scaling (Points to Pixels).</param>
    void DrawText(Region region, double dpi, string text, FontSpec font, ContentAlignment alignment, Color color);

    /// <summary>
    /// Draws text scaled to fit within the target element's content area.
    /// Delegates to <see cref="DrawTextToFit(Region, double, string, FontSpec, ContentAlignment, Color, TextFitMode, double, double)"/>.
    /// </summary>
    void DrawTextToFit(Element target, string text, FontSpec baseFont, ContentAlignment alignment, Color color,
        TextFitMode fitMode = TextFitMode.HeightOnly, double minSizePoints = 6, double maxSizePoints = 72);

    /// <summary>
    /// Draws text scaled to fit within an absolute region.
    /// </summary>
    void DrawTextToFit(Region region, double dpi, string text, FontSpec baseFont, ContentAlignment alignment, Color color,
        TextFitMode fitMode = TextFitMode.HeightOnly, double minSizePoints = 6, double maxSizePoints = 72);

    /// <summary>
    /// Draws a border around the element and/or fills its background.
    /// Delegates to <see cref="DrawRectangle(Region, Color?, Color?, double)"/>.
    /// </summary>
    void DrawRectangle(Element target, Color? borderColor, Color? fillColor, double borderThickness = 1.0);

    /// <summary>
    /// Draws a rectangle using absolute coordinates.
    /// 
    /// <para>
    /// <strong>Border Collapsing Behavior:</strong><br/>
    /// The border is drawn centered on the region's boundary lines.
    /// For example, if <paramref name="strokeThickness"/> is 4.0, the border extends 2.0 pixels
    /// outside the region and 2.0 pixels inside.
    /// </para>
    /// </summary>
    void DrawRectangle(Region region, Color? strokeColor, Color? fillColor, double strokeThickness = 1.0);

    /// <summary>
    /// Draws a line between two absolute coordinates.
    /// </summary>
    void DrawLine(Point start, Point end, Color color, double thickness = 1.0);

    /// <summary>
    /// Draws an image within the target element's content area.
    /// Delegates to <see cref="DrawImage(Region, IImage, ContentAlignment, ImageScaling)"/>.
    /// </summary>
    void DrawImage(Element target, IImage image, ContentAlignment alignment, ImageScaling scaling = ImageScaling.Fit);

    /// <summary>
    /// Draws an image within an absolute region.
    /// </summary>
    void DrawImage(Region region, IImage image, ContentAlignment alignment, ImageScaling scaling = ImageScaling.Fit);
}
