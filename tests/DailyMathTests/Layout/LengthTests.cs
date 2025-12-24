namespace DailyMath.Tests.Layout;

using DailyMath.Core.Layout;

public class LengthTests
{
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
