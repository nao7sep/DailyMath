namespace DailyMathCore.Layout;

public static class LayoutConstants
{
    public const string DefaultNumericFormat = "0.##";
    public const string NullValueLiteral = "(null)";
    
    /// <summary>
    /// Small value used for floating-point comparisons in layout calculations.
    /// </summary>
    public const double Epsilon = 0.000001;
}