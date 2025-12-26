namespace DailyMath.Core.Layout;

using System;
using DailyMath.Core.Rendering;

/// <summary>
/// Provides pure mathematical calculations for layout, alignment, and scaling.
/// Used by renderers and image processors to ensure consistent geometry.
/// </summary>
public static class LayoutCalculator
{
    /// <summary>
    /// Calculates the top-left coordinate to place an object of (width, height) 
    /// within a region, obeying the specified content alignment.
    /// </summary>
    public static Point Align(Region region, double objectWidth, double objectHeight, ContentAlignment alignment)
    {
        double x = region.Left;
        double y = region.Top;

        // Horizontal
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
        }

        // Vertical
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
        }

        return new Point(x, y);
    }

    /// <summary>
    /// Calculates the target dimensions of an object scaling to fit a container.
    /// </summary>
    public static (double Width, double Height) Scale(double objectWidth, double objectHeight, double containerWidth, double containerHeight, ImageScaling scaling)
    {
        if (scaling == ImageScaling.None)
            return (objectWidth, objectHeight);

        if (scaling == ImageScaling.Stretch)
            return (containerWidth, containerHeight);

        double widthRatio = containerWidth / objectWidth;
        double heightRatio = containerHeight / objectHeight;
        double scale = 1.0;

        switch (scaling)
        {
            case ImageScaling.Fit:
                scale = Math.Min(widthRatio, heightRatio);
                break;
            case ImageScaling.FitDownOnly:
                scale = Math.Min(1.0, Math.Min(widthRatio, heightRatio));
                break;
            case ImageScaling.Cover:
                scale = Math.Max(widthRatio, heightRatio);
                break;
        }

        return (objectWidth * scale, objectHeight * scale);
    }
    
    /// <summary>
    /// Calculates the inner content region after applying padding to a bounding box.
    /// </summary>
    public static Region ApplyPadding(Region region, Inset padding, double dpi)
    {
        double pl = padding.Left.ToPixels(dpi, region.Width);
        double pt = padding.Top.ToPixels(dpi, region.Height);
        double pr = padding.Right.ToPixels(dpi, region.Width);
        double pb = padding.Bottom.ToPixels(dpi, region.Height);
        
        return new Region(
            region.Left + pl, 
            region.Top + pt, 
            region.Right - pr, 
            region.Bottom - pb
        );
    }
}
