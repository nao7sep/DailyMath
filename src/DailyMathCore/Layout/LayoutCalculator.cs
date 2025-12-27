namespace DailyMath.Core.Layout;

using System;
using DailyMath.Core.Rendering;

/// <summary>
/// Provides pure mathematical calculations for layout, alignment, and scaling.
/// Used by renderers and image processors across all platforms to ensure consistent geometry.
///
/// Three main responsibilities:
/// - Position objects within regions using 9-point alignment
/// - Scale objects to fit/cover containers (aspect-ratio-preserving or distortion modes)
/// - Contract regions inward via padding
/// </summary>
public static class LayoutCalculator
{
    /// <summary>
    /// Positions an object within a region using a 9-point alignment grid (corners, edges, center).
    /// Solves the problem of placing a sized object at a specific anchor point within a bounding box.
    /// </summary>
    /// <param name="region">The bounding region.</param>
    /// <param name="objectWidth">The width of the object to position.</param>
    /// <param name="objectHeight">The height of the object to position.</param>
    /// <param name="alignment">The anchor point (ContentAlignment enum).</param>
    /// <returns>The top-left point where the object should be drawn.</returns>
    /// <exception cref="NotSupportedException">Thrown if an unsupported ContentAlignment value is provided.</exception>
    public static Point Align(Region region, double objectWidth, double objectHeight, ContentAlignment alignment)
    {
        double x = region.Left;
        double y = region.Top;

        // Horizontal: Distribute free space based on alignment (Left, Center, Right).
        switch (alignment)
        {
            case ContentAlignment.TopCenter:
            case ContentAlignment.MiddleCenter:
            case ContentAlignment.BottomCenter:
                x += (region.Width - objectWidth) / 2.0;
                break;
            case ContentAlignment.TopRight:
            case ContentAlignment.MiddleRight:
            case ContentAlignment.BottomRight:
                x += (region.Width - objectWidth);
                break;
            default:
                throw new NotSupportedException($"Unsupported alignment: {alignment}");
        }

        // Vertical: Distribute free space based on alignment (Top, Middle, Bottom).
        switch (alignment)
        {
            case ContentAlignment.MiddleLeft:
            case ContentAlignment.MiddleCenter:
            case ContentAlignment.MiddleRight:
                y += (region.Height - objectHeight) / 2.0;
                break;
            case ContentAlignment.BottomLeft:
            case ContentAlignment.BottomCenter:
            case ContentAlignment.BottomRight:
                y += (region.Height - objectHeight);
                break;
            default:
                throw new NotSupportedException($"Unsupported alignment: {alignment}");
        }

        return new Point(x, y);
    }

    /// <summary>
    /// Calculates new dimensions for an object to fit or fill a container using the specified scaling strategy.
    ///
    /// Modes:
    /// - None: Return unchanged dimensions
    /// - Stretch: Distort to fill container entirely
    /// - Fit: Maintain aspect ratio, scale to fit within container
    /// - FitDownOnly: Like Fit but never scale up
    /// - Cover: Maintain aspect ratio, scale to cover entire container (may crop)
    /// </summary>
    /// <param name="objectWidth">The original width (must be > 0).</param>
    /// <param name="objectHeight">The original height (must be > 0).</param>
    /// <param name="containerWidth">The target container width.</param>
    /// <param name="containerHeight">The target container height.</param>
    /// <param name="scaling">The scaling strategy to apply.</param>
    /// <returns>The scaled dimensions (width, height).</returns>
    /// <exception cref="ArgumentException">Thrown if object dimensions are <= 0.</exception>
    /// <exception cref="NotSupportedException">Thrown if an unsupported ImageScaling value is provided.</exception>
    public static (double Width, double Height) Scale(double objectWidth, double objectHeight, double containerWidth, double containerHeight, ImageScaling scaling)
    {
        // Guard against division by zero.
        if (objectWidth <= 0 || objectHeight <= 0)
            throw new ArgumentException("Object dimensions must be positive.");

        if (scaling == ImageScaling.None)
            return (objectWidth, objectHeight);

        if (scaling == ImageScaling.Stretch)
            return (containerWidth, containerHeight);

        // For aspect-ratio-preserving modes, calculate the scale factor for each dimension.
        double widthRatio = containerWidth / objectWidth;
        double heightRatio = containerHeight / objectHeight;
        double scale = 1.0;

        switch (scaling)
        {
            case ImageScaling.Fit:
                // Use the smaller ratio to ensure the object fits entirely.
                scale = Math.Min(widthRatio, heightRatio);
                break;
            case ImageScaling.FitDownOnly:
                // Like Fit, but clamp to 1.0 (never scale up).
                scale = Math.Min(1.0, Math.Min(widthRatio, heightRatio));
                break;
            case ImageScaling.Cover:
                // Use the larger ratio to ensure the object covers the entire container.
                scale = Math.Max(widthRatio, heightRatio);
                break;
            default:
                throw new NotSupportedException($"Unsupported scaling: {scaling}");
        }

        return (objectWidth * scale, objectHeight * scale);
    }

    /// <summary>
    /// Contracts a region inward by applying padding on all sides.
    /// Supports both absolute (point-based) and relative (percentage-based) padding units.
    /// </summary>
    /// <param name="region">The region to apply padding to.</param>
    /// <param name="padding">The inset padding values (may be absolute or percentage-based).</param>
    /// <param name="dpi">The DPI used to convert relative padding units to pixels.</param>
    /// <returns>The contracted region.</returns>
    public static Region ApplyPadding(Region region, Inset padding, double dpi)
    {
        // Convert padding to pixels (horizontal relative to width, vertical to height).
        double left = padding.Left.ToPixels(dpi, region.Width);
        double top = padding.Top.ToPixels(dpi, region.Height);
        double right = padding.Right.ToPixels(dpi, region.Width);
        double bottom = padding.Bottom.ToPixels(dpi, region.Height);

        // Move edges inward.
        return new Region(
            region.Left + left,
            region.Top + top,
            region.Right - right,
            region.Bottom - bottom
        );
    }
}
