namespace DailyMath.Core.Rendering;

using System;
using DailyMath.Core.Layout;

/// <summary>
/// Abstraction for drawing content.
/// Implementations (Skia, WPF) manage the underlying graphics context.
/// This interface does not directly manage resources; implementations must properly
/// dispose any native resources in their Dispose() implementation to prevent memory leaks.
/// </summary>
public interface IRenderer : IDisposable
{
    // --- Element-Based Drawing (High Level) ---

    /// <summary>
    /// Draws text aligned within the target element's content area.
    ///
    /// NOTE: This method expects single-line text. Behavior with multiline text is undefined.
    /// If the text contains newlines or is wider than the content area, the rendering result
    /// may be clipped, wrapped, or displayed incorrectly depending on the implementation.
    /// Pre-process text to ensure it is a single line before calling this method.
    /// </summary>
    void DrawText(Element target, string text, FontSpec font, ContentAlignment alignment, Color color);

    /// <summary>
    /// Draws text scaled to fit within the target element's content area.
    ///
    /// NOTE: This method expects single-line text. Behavior with multiline text is undefined.
    /// Font size is adjusted within the specified bounds (minSizePoints to maxSizePoints)
    /// to maximize visibility while keeping the text within the element's boundaries.
    /// </summary>
    void DrawTextToFit(Element target, string text, FontSpec baseFont, ContentAlignment alignment, Color color,
        TextFitMode fitMode = TextFitMode.HeightOnly, double minSizePoints = 6, double maxSizePoints = 72);

    /// <summary>
    /// Draws a border around the element and/or fills its background.
    ///
    /// IMPORTANT: Strokes are centered to the bounds of the element. If you draw a 2px border
    /// around an element that is exactly the size of the printable area, the outer 1px of the
    /// stroke will extend beyond the boundary and will not be rendered. This is by design because
    /// centered strokes allow proper border overlap and collapse for adjacent elements like
    /// table cells—without this, borders would appear doubled at cell boundaries.
    /// </summary>
    void DrawRectangle(Element target, Color? borderColor, Color? fillColor, double borderThickness = 1.0);

    /// <summary>
    /// Draws a canvas within the target element's content area.
    /// </summary>
    void DrawImage(Element target, ICanvas image, ContentAlignment alignment, ImageScaling scaling = ImageScaling.Fit);

    // --- Region-Based Drawing (Low Level) ---

    /// <summary>
    /// Draws text aligned within an absolute region using device-independent coordinates.
    ///
    /// NOTE: This method expects single-line text. Behavior with multiline text is undefined.
    ///
    /// The DPI parameter is required to convert between logical points (used in FontSpec) and
    /// physical pixels. This allows proper font rendering at the target output resolution.
    /// Without DPI information, the implementation cannot accurately scale fonts for printing
    /// or high-resolution displays.
    /// </summary>
    void DrawText(Region region, double dpi, string text, FontSpec font, ContentAlignment alignment, Color color);

    /// <summary>
    /// Draws text scaled to fit within an absolute region using device-independent coordinates.
    ///
    /// NOTE: This method expects single-line text. Behavior with multiline text is undefined.
    /// Font size is adjusted to maximize text visibility within the specified region bounds.
    ///
    /// The DPI parameter is required for the same reason as the single-line DrawText overload:
    /// to convert between font sizes in points and physical pixels. The scaling calculation
    /// must account for the actual output resolution to produce correct results across
    /// different display densities and print resolutions.
    /// </summary>
    void DrawTextToFit(Region region, double dpi, string text, FontSpec baseFont, ContentAlignment alignment, Color color,
        TextFitMode fitMode = TextFitMode.HeightOnly, double minSizePoints = 6, double maxSizePoints = 72);

    /// <summary>
    /// Draws a rectangle using absolute coordinates.
    ///
    /// IMPORTANT: Strokes are centered to the bounds of the region. See DrawRectangle(Element, ...)
    /// for details on stroke centering and its implications for layouts with adjacent elements.
    /// </summary>
    void DrawRectangle(Region region, Color? strokeColor, Color? fillColor, double strokeThickness = 1.0);

    /// <summary>
    /// Draws a line between two absolute coordinates.
    /// </summary>
    void DrawLine(Point start, Point end, Color color, double thickness = 1.0);

    /// <summary>
    /// Draws a canvas within an absolute region.
    /// </summary>
    void DrawImage(Region region, ICanvas image, ContentAlignment alignment, ImageScaling scaling = ImageScaling.Fit);
}
