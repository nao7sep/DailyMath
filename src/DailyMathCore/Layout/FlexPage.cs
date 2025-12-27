namespace DailyMathCore.Layout;

public class FlexPage : FlexElement
{
    private double? _dpi;

    /// <summary>
    /// Gets or sets the DPI context for this page and its entire sub-tree.
    /// If null, any attempt to resolve DPI-dependent lengths in this hierarchy will throw.
    /// </summary>
    public new double? Dpi
    {
        get => _dpi;
        set
        {
            if (value.HasValue && value.Value <= 0)
                throw new ArgumentException("DPI must be a positive non-zero value.", nameof(value));
            _dpi = value;
            InvalidateLayout();
        }
    }

    public FlexPage()
    {
        Parent = null;
    }

    public override string ToString() => ToString(null);

    public new string ToString(string? format)
    {
        string dpiStr = Dpi.HasValue
            ? $"{Dpi.Value.ToString(format ?? LayoutConstants.DefaultNumericFormat, System.Globalization.CultureInfo.InvariantCulture)}dpi"
            : $"D:{LayoutConstants.NullValueLiteral}";

        return $"{dpiStr}, {base.ToString(format)}";
    }
}
