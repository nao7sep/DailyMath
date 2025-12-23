namespace DailyMath.Core.Layout;

public static class LengthConverter
{
    public const double MillimetersPerInch = 25.4;

    public static double ToMillimeters(double value, LengthUnit fromUnit)
    {
        return fromUnit switch
        {
            LengthUnit.Millimeter => value,
            LengthUnit.Inch => value * MillimetersPerInch,
            LengthUnit.Pixel => throw new InvalidOperationException("Cannot convert pixels to millimeters without DPI"),
            _ => throw new ArgumentException($"Unsupported LengthUnit: {fromUnit}", nameof(fromUnit))
        };
    }

    public static double ToInches(double value, LengthUnit fromUnit)
    {
        return fromUnit switch
        {
            LengthUnit.Millimeter => value / MillimetersPerInch,
            LengthUnit.Inch => value,
            LengthUnit.Pixel => throw new InvalidOperationException("Cannot convert pixels to inches without DPI"),
            _ => throw new ArgumentException($"Unsupported LengthUnit: {fromUnit}", nameof(fromUnit))
        };
    }

    public static double ToPixels(double value, LengthUnit fromUnit, double dpi)
    {
        return fromUnit switch
        {
            LengthUnit.Millimeter => value * dpi / MillimetersPerInch,
            LengthUnit.Inch => value * dpi,
            LengthUnit.Pixel => value,
            _ => throw new ArgumentException($"Unsupported LengthUnit: {fromUnit}", nameof(fromUnit))
        };
    }

    public static double FromPixels(double pixels, LengthUnit toUnit, double dpi)
    {
        return toUnit switch
        {
            LengthUnit.Millimeter => pixels * MillimetersPerInch / dpi,
            LengthUnit.Inch => pixels / dpi,
            LengthUnit.Pixel => pixels,
            _ => throw new ArgumentException($"Unsupported LengthUnit: {toUnit}", nameof(toUnit))
        };
    }
}
