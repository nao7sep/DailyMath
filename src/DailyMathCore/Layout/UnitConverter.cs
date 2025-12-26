namespace DailyMath.Core.Layout;

/// <summary>
/// Provides utility methods and constants for converting between absolute units used in rendering and layout.
/// Centralizes all unit conversion logic to ensure consistency across the codebase.
///
/// Design Note: This class handles conversions between absolute units only (Points, Pixels, Inches, Millimeters, Centimeters).
/// Percent is intentionally not included here because it is a relative unit that requires parent/container context to convert.
/// Percent-based calculations are handled directly in Length.cs where the parent context is available.
/// </summary>
public static class UnitConverter
{
    #region Unit Conversion Constants

    /// <summary>
    /// Typographic points per inch. Standard in typography: 72pt = 1 inch.
    /// </summary>
    public const double PointsPerInch = 72.0;

    /// <summary>
    /// Millimeters per inch. Used for metric conversions: 1" = 25.4mm.
    /// </summary>
    public const double MillimetersPerInch = 25.4;

    /// <summary>
    /// Centimeters per inch. Used for metric conversions: 1" = 2.54cm.
    /// </summary>
    public const double CentimetersPerInch = 2.54;

    #endregion

    #region Points ↔ Inches (Typography Base)

    /// <summary>
    /// Converts typographic points to inches.
    /// </summary>
    public static double PointsToInches(double points)
    {
        return points / PointsPerInch;
    }

    /// <summary>
    /// Converts inches to typographic points.
    /// </summary>
    public static double InchesToPoints(double inches)
    {
        return inches * PointsPerInch;
    }

    #endregion

    #region Points ↔ Pixels (Typography via DPI)

    /// <summary>
    /// Converts font size from points to pixels based on DPI.
    /// </summary>
    /// <param name="sizeInPoints">Font size in points (72pt = 1 inch).</param>
    /// <param name="dpi">Dots per inch (pixel density).</param>
    /// <returns>Font size in pixels.</returns>
    public static double PointsToPixels(double sizeInPoints, double dpi)
    {
        // Divide first (dpi / PointsPerInch) before multiplying to minimize rounding errors.
        // This keeps intermediate values smaller and more numerically stable than computing
        // (sizeInPoints * dpi) first, then dividing by PointsPerInch.
        return sizeInPoints * (dpi / PointsPerInch);
    }

    /// <summary>
    /// Converts font size from pixels to points based on DPI.
    /// </summary>
    /// <param name="sizeInPixels">Font size in pixels.</param>
    /// <param name="dpi">Dots per inch (pixel density).</param>
    /// <returns>Font size in points.</returns>
    public static double PixelsToPoints(double sizeInPixels, double dpi)
    {
        // Divide first (PointsPerInch / dpi) before multiplying to minimize rounding errors.
        // This keeps intermediate values smaller and more numerically stable than computing
        // (sizeInPixels * PointsPerInch) first, then dividing by dpi.
        return sizeInPixels * (PointsPerInch / dpi);
    }

    #endregion

    #region Inches ↔ Pixels (Physical via DPI)

    /// <summary>
    /// Converts inches to pixels based on DPI.
    /// </summary>
    /// <param name="inches">Length in inches.</param>
    /// <param name="dpi">Dots per inch (pixel density).</param>
    /// <returns>Length in pixels.</returns>
    public static double InchesToPixels(double inches, double dpi)
    {
        return inches * dpi;
    }

    /// <summary>
    /// Converts pixels to inches based on DPI.
    /// </summary>
    /// <param name="pixels">Length in pixels.</param>
    /// <param name="dpi">Dots per inch (pixel density).</param>
    /// <returns>Length in inches.</returns>
    public static double PixelsToInches(double pixels, double dpi)
    {
        return pixels / dpi;
    }

    #endregion

    #region Millimeters ↔ Pixels (Metric via DPI)

    /// <summary>
    /// Converts millimeters to pixels based on DPI.
    /// </summary>
    /// <param name="millimeters">Length in millimeters.</param>
    /// <param name="dpi">Dots per inch (pixel density).</param>
    /// <returns>Length in pixels.</returns>
    public static double MillimetersToPixels(double millimeters, double dpi)
    {
        // Divide first (dpi / MillimetersPerInch) before multiplying to minimize rounding errors.
        return millimeters * (dpi / MillimetersPerInch);
    }

    /// <summary>
    /// Converts pixels to millimeters based on DPI.
    /// </summary>
    /// <param name="pixels">Length in pixels.</param>
    /// <param name="dpi">Dots per inch (pixel density).</param>
    /// <returns>Length in millimeters.</returns>
    public static double PixelsToMillimeters(double pixels, double dpi)
    {
        // Divide first (MillimetersPerInch / dpi) before multiplying to minimize rounding errors.
        return pixels * (MillimetersPerInch / dpi);
    }

    #endregion

    #region Centimeters ↔ Pixels (Metric via DPI)

    /// <summary>
    /// Converts centimeters to pixels based on DPI.
    /// </summary>
    /// <param name="centimeters">Length in centimeters.</param>
    /// <param name="dpi">Dots per inch (pixel density).</param>
    /// <returns>Length in pixels.</returns>
    public static double CentimetersToPixels(double centimeters, double dpi)
    {
        // Divide first (dpi / CentimetersPerInch) before multiplying to minimize rounding errors.
        return centimeters * (dpi / CentimetersPerInch);
    }

    /// <summary>
    /// Converts pixels to centimeters based on DPI.
    /// </summary>
    /// <param name="pixels">Length in pixels.</param>
    /// <param name="dpi">Dots per inch (pixel density).</param>
    /// <returns>Length in centimeters.</returns>
    public static double PixelsToCentimeters(double pixels, double dpi)
    {
        // Divide first (CentimetersPerInch / dpi) before multiplying to minimize rounding errors.
        return pixels * (CentimetersPerInch / dpi);
    }

    #endregion

    #region Inches ↔ Metric (Absolute Conversions)

    /// <summary>
    /// Converts inches to millimeters.
    /// </summary>
    public static double InchesToMillimeters(double inches)
    {
        return inches * MillimetersPerInch;
    }

    /// <summary>
    /// Converts millimeters to inches.
    /// </summary>
    public static double MillimetersToInches(double millimeters)
    {
        return millimeters / MillimetersPerInch;
    }

    /// <summary>
    /// Converts inches to centimeters.
    /// </summary>
    public static double InchesToCentimeters(double inches)
    {
        return inches * CentimetersPerInch;
    }

    /// <summary>
    /// Converts centimeters to inches.
    /// </summary>
    public static double CentimetersToInches(double centimeters)
    {
        return centimeters / CentimetersPerInch;
    }

    #endregion

    #region Millimeters ↔ Centimeters (Metric Internal)

    /// <summary>
    /// Converts millimeters to centimeters.
    /// </summary>
    public static double MillimetersToCentimeters(double millimeters)
    {
        return millimeters / 10.0;
    }

    /// <summary>
    /// Converts centimeters to millimeters.
    /// </summary>
    public static double CentimetersToMillimeters(double centimeters)
    {
        return centimeters * 10.0;
    }

    #endregion
}

