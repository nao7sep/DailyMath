namespace DailyMath.Tests.Layout;

using DailyMath.Core.Layout;

public class LengthTests
{
    // Length Creation Tests (Extension Methods)

    [Fact]
    public void AsPixels_WithDouble_CreatesPixelLength()
    {
        var length = 100.5.AsPixels();

        Assert.Equal(100.5, length.Value);
        Assert.Equal(Unit.Pixels, length.Unit);
    }

    [Fact]
    public void AsPixels_WithInt_CreatesPixelLength()
    {
        var length = 100.AsPixels();

        Assert.Equal(100, length.Value);
        Assert.Equal(Unit.Pixels, length.Unit);
    }

    [Fact]
    public void AsPixels_WithZero_CreatesZeroPixelLength()
    {
        var length = 0.AsPixels();

        Assert.Equal(0, length.Value);
        Assert.Equal(Unit.Pixels, length.Unit);
    }

    [Fact]
    public void AsPixels_WithNegative_CreatesNegativePixelLength()
    {
        var length = (-50).AsPixels();

        Assert.Equal(-50, length.Value);
        Assert.Equal(Unit.Pixels, length.Unit);
    }

    [Fact]
    public void AsInches_WithDouble_CreatesInchLength()
    {
        var length = 8.5.AsInches();

        Assert.Equal(8.5, length.Value);
        Assert.Equal(Unit.Inches, length.Unit);
    }

    [Fact]
    public void AsInches_WithInt_CreatesInchLength()
    {
        var length = 11.AsInches();

        Assert.Equal(11, length.Value);
        Assert.Equal(Unit.Inches, length.Unit);
    }

    [Fact]
    public void AsMillimeters_WithDouble_CreatesMillimeterLength()
    {
        var length = 210.5.AsMillimeters();

        Assert.Equal(210.5, length.Value);
        Assert.Equal(Unit.Millimeters, length.Unit);
    }

    [Fact]
    public void AsMillimeters_WithInt_CreatesMillimeterLength()
    {
        var length = 297.AsMillimeters();

        Assert.Equal(297, length.Value);
        Assert.Equal(Unit.Millimeters, length.Unit);
    }

    [Fact]
    public void AsPercent_WithDouble_CreatesPercentLength()
    {
        var length = 50.5.AsPercent();

        Assert.Equal(50.5, length.Value);
        Assert.Equal(Unit.Percent, length.Unit);
    }

    [Fact]
    public void AsPercent_WithInt_CreatesPercentLength()
    {
        var length = 100.AsPercent();

        Assert.Equal(100, length.Value);
        Assert.Equal(Unit.Percent, length.Unit);
    }

    [Fact]
    public void AsPercent_WithZero_CreatesZeroPercentLength()
    {
        var length = 0.AsPercent();

        Assert.Equal(0, length.Value);
        Assert.Equal(Unit.Percent, length.Unit);
    }

    [Fact]
    public void ExtensionMethods_UsedInMeasureConstructor_WorkCorrectly()
    {
        var measure = new Measure(100.AsPixels(), 200.AsPixels());

        Assert.Equal(100, measure.Width.Value);
        Assert.Equal(200, measure.Height.Value);
        Assert.Equal(Unit.Pixels, measure.Width.Unit);
        Assert.Equal(Unit.Pixels, measure.Height.Unit);
    }

    [Fact]
    public void ExtensionMethods_UsedInInsetConstructor_WorkCorrectly()
    {
        var inset = new Inset(10.AsPixels(), 20.AsPixels(), 30.AsPixels(), 40.AsPixels());

        Assert.Equal(10, inset.Left.Value);
        Assert.Equal(20, inset.Top.Value);
        Assert.Equal(30, inset.Right.Value);
        Assert.Equal(40, inset.Bottom.Value);
    }

    // Unit Conversion Tests

    [Fact]
    public void ToPixels_WithPixels_ReturnsValue()
    {
        var length = new Length(100, Unit.Pixels);
        Assert.Equal(100, length.ToPixels());
    }

    [Fact]
    public void ToPixels_WithInches_RequiresDpi()
    {
        var length = new Length(1, Unit.Inches);
        Assert.Equal(96, length.ToPixels(dpi: 96));
        Assert.Equal(300, length.ToPixels(dpi: 300));
    }

    [Fact]
    public void ToPixels_WithMillimeters_RequiresDpi()
    {
        var length = new Length(25.4, Unit.Millimeters);
        Assert.Equal(96, length.ToPixels(dpi: 96), precision: 10);
    }

    [Fact]
    public void ToPixels_WithPercent_RequiresParentContentSize()
    {
        var length = new Length(50, Unit.Percent);
        Assert.Equal(400, length.ToPixels(parentContentSize: 800));
    }

    [Fact]
    public void ToPixels_WithInches_ThrowsWhenDpiNull()
    {
        var length = new Length(1, Unit.Inches);
        Assert.Throws<ArgumentNullException>(() => length.ToPixels());
    }

    [Fact]
    public void ToPixels_WithPercent_ThrowsWhenParentContentSizeNull()
    {
        var length = new Length(50, Unit.Percent);
        Assert.Throws<ArgumentNullException>(() => length.ToPixels());
    }

    [Fact]
    public void ToInches_WithInches_ReturnsValue()
    {
        var length = new Length(2.5, Unit.Inches);
        Assert.Equal(2.5, length.ToInches());
    }

    [Fact]
    public void ToInches_WithMillimeters_ConvertsCorrectly()
    {
        var length = new Length(25.4, Unit.Millimeters);
        Assert.Equal(1.0, length.ToInches(), precision: 10);
    }

    [Fact]
    public void ToInches_WithPixels_RequiresDpi()
    {
        var length = new Length(96, Unit.Pixels);
        Assert.Equal(1.0, length.ToInches(dpi: 96), precision: 10);
        Assert.Equal(0.32, length.ToInches(dpi: 300), precision: 10);
    }

    [Fact]
    public void ToInches_WithPixels_ThrowsWhenDpiNull()
    {
        var length = new Length(96, Unit.Pixels);
        Assert.Throws<ArgumentNullException>(() => length.ToInches());
    }

    [Fact]
    public void ToMillimeters_WithMillimeters_ReturnsValue()
    {
        var length = new Length(100, Unit.Millimeters);
        Assert.Equal(100, length.ToMillimeters());
    }

    [Fact]
    public void ToMillimeters_WithInches_ConvertsCorrectly()
    {
        var length = new Length(1, Unit.Inches);
        Assert.Equal(25.4, length.ToMillimeters(), precision: 10);
    }

    [Fact]
    public void ToMillimeters_WithPixels_RequiresDpi()
    {
        var length = new Length(96, Unit.Pixels);
        Assert.Equal(25.4, length.ToMillimeters(dpi: 96), precision: 10);
    }

    [Fact]
    public void ToMillimeters_WithPixels_ThrowsWhenDpiNull()
    {
        var length = new Length(96, Unit.Pixels);
        Assert.Throws<ArgumentNullException>(() => length.ToMillimeters());
    }

    [Fact]
    public void ToPercent_WithPercent_ReturnsValue()
    {
        var length = new Length(75, Unit.Percent);
        Assert.Equal(75, length.ToPercent());
    }

    [Fact]
    public void ToPercent_WithInches_ConvertsCorrectly()
    {
        var length = new Length(2, Unit.Inches);
        Assert.Equal(50, length.ToPercent(parentContentSize: 4), precision: 10);
    }

    [Fact]
    public void ToPercent_ThrowsWhenParentContentSizeIsZero()
    {
        var length = new Length(1, Unit.Inches);
        Assert.Throws<ArgumentException>(() => length.ToPercent(parentContentSize: 0));
    }

    // Arithmetic Operator Tests

    [Fact]
    public void Add_TwoInches_ReturnsSum()
    {
        var a = new Length(2, Unit.Inches);
        var b = new Length(3, Unit.Inches);
        var result = a + b;

        Assert.Equal(5, result.Value);
        Assert.Equal(Unit.Inches, result.Unit);
    }

    [Fact]
    public void Add_InchesAndMillimeters_ConvertsAndAdds()
    {
        var a = new Length(1, Unit.Inches); // 25.4mm
        var b = new Length(25.4, Unit.Millimeters); // 1 inch
        var result = a + b;

        Assert.Equal(2, result.Value, precision: 10);
        Assert.Equal(Unit.Inches, result.Unit); // Left operand's unit
    }

    [Fact]
    public void Add_WithPercent_Throws()
    {
        var a = new Length(1, Unit.Inches);
        var b = new Length(50, Unit.Percent);

        Assert.Throws<InvalidOperationException>(() => a + b);
    }

    [Fact]
    public void Add_WithPixels_Throws()
    {
        var a = new Length(100, Unit.Pixels);
        var b = new Length(50, Unit.Pixels);

        Assert.Throws<InvalidOperationException>(() => a + b);
    }

    [Fact]
    public void Subtract_TwoInches_ReturnsDifference()
    {
        var a = new Length(5, Unit.Inches);
        var b = new Length(2, Unit.Inches);
        var result = a - b;

        Assert.Equal(3, result.Value);
        Assert.Equal(Unit.Inches, result.Unit);
    }

    [Fact]
    public void Subtract_InchesAndMillimeters_ConvertsAndSubtracts()
    {
        var a = new Length(2, Unit.Inches); // 50.8mm
        var b = new Length(25.4, Unit.Millimeters); // 1 inch
        var result = a - b;

        Assert.Equal(1, result.Value, precision: 10);
        Assert.Equal(Unit.Inches, result.Unit);
    }

    [Fact]
    public void Subtract_WithPercent_Throws()
    {
        var a = new Length(1, Unit.Inches);
        var b = new Length(50, Unit.Percent);

        Assert.Throws<InvalidOperationException>(() => a - b);
    }

    [Fact]
    public void Subtract_WithPixels_Throws()
    {
        var a = new Length(100, Unit.Pixels);
        var b = new Length(50, Unit.Pixels);

        Assert.Throws<InvalidOperationException>(() => a - b);
    }

    // Use Case: Calculate remaining space
    [Fact]
    public void UseCase_CalculateBodyHeight()
    {
        Length paperHeight = new Length(297, Unit.Millimeters); // A4
        Length headerHeight = new Length(1, Unit.Inches);
        Length footerHeight = new Length(0.5, Unit.Inches);

        Length bodyHeight = paperHeight - headerHeight - footerHeight;

        // 297mm - 25.4mm - 12.7mm = 258.9mm
        Assert.Equal(258.9, bodyHeight.Value, precision: 1);
        Assert.Equal(Unit.Millimeters, bodyHeight.Unit);
    }

    // ToString Tests

    [Fact]
    public void ToString_DefaultFormat_FormatsCorrectly()
    {
        Assert.Equal("100px", new Length(100, Unit.Pixels).ToString());
        Assert.Equal("2.5in", new Length(2.5, Unit.Inches).ToString());
        Assert.Equal("25.4mm", new Length(25.4, Unit.Millimeters).ToString());
        Assert.Equal("50%", new Length(50, Unit.Percent).ToString());
    }

    [Fact]
    public void ToString_CustomFormat_FormatsCorrectly()
    {
        var length = new Length(2.5555, Unit.Inches);
        Assert.Equal("2.56in", length.ToString("0.##"));
        Assert.Equal("2.5555in", length.ToString("0.####"));
        Assert.Equal("2.56in", length.ToString("F2"));
    }
}
