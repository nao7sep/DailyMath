using System;

namespace DailyMathCore.Layout;

public static class LayoutEnumConverter
{
    public static string ToShortString(Unit unit)
    {
        return unit switch
        {
            Unit.Pixels => "px",
            Unit.Percentage => "%",
            Unit.Millimeters => "mm",
            Unit.Centimeters => "cm",
            Unit.Inches => "in",
            _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, "Unsupported unit value.")
        };
    }

    public static string ToString(Alignment alignment)
    {
        return alignment switch
        {
            Alignment.TopLeft => "TopLeft",
            Alignment.TopCenter => "TopCenter",
            Alignment.TopRight => "TopRight",
            Alignment.MiddleLeft => "MiddleLeft",
            Alignment.MiddleCenter => "MiddleCenter",
            Alignment.MiddleRight => "MiddleRight",
            Alignment.BottomLeft => "BottomLeft",
            Alignment.BottomCenter => "BottomCenter",
            Alignment.BottomRight => "BottomRight",
            _ => throw new ArgumentOutOfRangeException(nameof(alignment), alignment, "Unsupported alignment value.")
        };
    }
}
