namespace DailyMath.Core.Rendering;

using System;
using DailyMath.Core.Layout;

/// <summary>
/// Abstraction for drawing content.
/// Implementations (Skia, WPF) manage the underlying graphics context.
/// </summary>
public interface IRenderer : IDisposable
{
    // --- Element-Based Drawing (High Level) ---

    /// <summary>
    /// Draws text aligned within the target element's content area.
    /// </summary>
    void DrawText(Element target, string text, FontSpec font, ContentAlignment alignment, Color color);

    /// <summary>
    /// Draws text scaled to fit within the target element's content area.
    /// </summary>
    void DrawTextToFit(Element target, string text, FontSpec baseFont, ContentAlignment alignment, Color color,
        TextFitMode fitMode = TextFitMode.HeightOnly, double minSizePoints = 6, double maxSizePoints = 72);

    /// <summary>
    /// Draws a border around the element and/or fills its background.
    /// </summary>
    void DrawRectangle(Element target, Color? borderColor, Color? fillColor, double borderThickness = 1.0);

    /// <summary>
    /// Draws an image within the target element's content area.
    /// </summary>
    void DrawImage(Element target, IImage image, ContentAlignment alignment, ImageScaling scaling = ImageScaling.Fit);

    // --- Region-Based Drawing (Low Level) ---

    /// <summary>
    /// Draws text aligned within an absolute region.
    /// </summary>
    void DrawText(Region region, double dpi, string text, FontSpec font, ContentAlignment alignment, Color color);

    /// <summary>
    /// Draws text scaled to fit within an absolute region.
    /// </summary>
    void DrawTextToFit(Region region, double dpi, string text, FontSpec baseFont, ContentAlignment alignment, Color color,
        TextFitMode fitMode = TextFitMode.HeightOnly, double minSizePoints = 6, double maxSizePoints = 72);

    /// <summary>
    /// Draws a rectangle using absolute coordinates.
    /// </summary>
    void DrawRectangle(Region region, Color? strokeColor, Color? fillColor, double strokeThickness = 1.0);

    /// <summary>
    /// Draws a line between two absolute coordinates.
    /// </summary>
    void DrawLine(Point start, Point end, Color color, double thickness = 1.0);

    /// <summary>
    /// Draws an image within an absolute region.
    /// </summary>
    void DrawImage(Region region, IImage image, ContentAlignment alignment, ImageScaling scaling = ImageScaling.Fit);
}
