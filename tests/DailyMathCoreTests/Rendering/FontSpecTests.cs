namespace DailyMath.Core.Tests.Rendering;

using DailyMath.Core.Rendering;

public class FontSpecTests
{
    [Fact]
    public void Constructor_CreatesWithBasicProperties()
    {
        var spec = new FontSpec("Arial", 12);

        Assert.Equal("Arial", spec.Family);
        Assert.Equal(12, spec.SizeInPoints);
        Assert.Equal(FontWeight.Normal, spec.Weight);
        Assert.Equal(FontStyle.None, spec.Style);
    }

    [Fact]
    public void Constructor_CreatesWithAllProperties()
    {
        var spec = new FontSpec("Times", 14, FontWeight.Bold, FontStyle.Italic);

        Assert.Equal("Times", spec.Family);
        Assert.Equal(14, spec.SizeInPoints);
        Assert.Equal(FontWeight.Bold, spec.Weight);
        Assert.Equal(FontStyle.Italic, spec.Style);
    }

    [Fact]
    public void Constructor_ThrowsOnZeroSize()
    {
        Assert.Throws<ArgumentException>(() => new FontSpec("Arial", 0));
    }

    [Fact]
    public void Constructor_ThrowsOnNegativeSize()
    {
        Assert.Throws<ArgumentException>(() => new FontSpec("Arial", -1));
    }

    [Fact]
    public void Constructor_AcceptsVerySmallSize()
    {
        var spec = new FontSpec("Arial", 0.1);
        Assert.Equal(0.1, spec.SizeInPoints);
    }

    [Fact]
    public void Constructor_AcceptsVeryLargeSize()
    {
        var spec = new FontSpec("Arial", 1000);
        Assert.Equal(1000, spec.SizeInPoints);
    }

    [Fact]
    public void Constructor_AllowsNullFamily()
    {
        var spec = new FontSpec(null!, 12);
        Assert.Null(spec.Family);
    }

    [Fact]
    public void WithSize_ReturnsNewInstanceWithUpdatedSize()
    {
        var original = new FontSpec("Arial", 12);
        var updated = original.WithSize(16);

        Assert.Equal(12, original.SizeInPoints); // Original unchanged
        Assert.Equal(16, updated.SizeInPoints);
        Assert.Equal(original.Family, updated.Family);
        Assert.Equal(original.Weight, updated.Weight);
        Assert.Equal(original.Style, updated.Style);
    }

    [Fact]
    public void WithSize_ThrowsOnZero()
    {
        var spec = new FontSpec("Arial", 12);
        Assert.Throws<ArgumentException>(() => spec.WithSize(0));
    }

    [Fact]
    public void WithSize_ThrowsOnNegative()
    {
        var spec = new FontSpec("Arial", 12);
        Assert.Throws<ArgumentException>(() => spec.WithSize(-5));
    }

    [Fact]
    public void WithWeight_ReturnsNewInstanceWithUpdatedWeight()
    {
        var original = new FontSpec("Arial", 12);
        var updated = original.WithWeight(FontWeight.Bold);

        Assert.Equal(FontWeight.Normal, original.Weight); // Original unchanged
        Assert.Equal(FontWeight.Bold, updated.Weight);
        Assert.Equal(original.Family, updated.Family);
        Assert.Equal(original.SizeInPoints, updated.SizeInPoints);
        Assert.Equal(original.Style, updated.Style);
    }

    [Fact]
    public void WithStyle_ReplacesEntireStyle()
    {
        var original = new FontSpec("Arial", 12, FontWeight.Normal, FontStyle.Italic);
        var updated = original.WithStyle(FontStyle.Underline);

        Assert.Equal(FontStyle.Italic, original.Style); // Original unchanged
        Assert.Equal(FontStyle.Underline, updated.Style); // Replaced, not combined
    }

    [Fact]
    public void WithStyle_CanCombineFlags()
    {
        var original = new FontSpec("Arial", 12);
        var updated = original.WithStyle(FontStyle.Italic | FontStyle.Underline);

        Assert.True(updated.Style.HasFlag(FontStyle.Italic));
        Assert.True(updated.Style.HasFlag(FontStyle.Underline));
    }

    [Fact]
    public void AsBold_SetsBoldWeight()
    {
        var original = new FontSpec("Arial", 12);
        var bold = original.AsBold();

        Assert.Equal(FontWeight.Normal, original.Weight);
        Assert.Equal(FontWeight.Bold, bold.Weight);
    }

    [Fact]
    public void AsBold_PreservesOtherProperties()
    {
        var original = new FontSpec("Times", 14, FontWeight.Light, FontStyle.Italic);
        var bold = original.AsBold();

        Assert.Equal(original.Family, bold.Family);
        Assert.Equal(original.SizeInPoints, bold.SizeInPoints);
        Assert.Equal(original.Style, bold.Style);
        Assert.Equal(FontWeight.Bold, bold.Weight);
    }

    [Fact]
    public void AsItalic_AddsItalicStyle()
    {
        var original = new FontSpec("Arial", 12);
        var italic = original.AsItalic();

        Assert.Equal(FontStyle.None, original.Style);
        Assert.True(italic.Style.HasFlag(FontStyle.Italic));
    }

    [Fact]
    public void AsItalic_PreservesExistingStyles()
    {
        var original = new FontSpec("Arial", 12, FontWeight.Normal, FontStyle.Underline);
        var italic = original.AsItalic();

        Assert.True(italic.Style.HasFlag(FontStyle.Italic));
        Assert.True(italic.Style.HasFlag(FontStyle.Underline));
    }

    [Fact]
    public void AsItalic_IsIdempotent()
    {
        var original = new FontSpec("Arial", 12);
        var italic1 = original.AsItalic();
        var italic2 = italic1.AsItalic();

        Assert.Equal(italic1.Style, italic2.Style);
    }

    [Fact]
    public void ChainedBuilders_AllModificationsApply()
    {
        var spec = new FontSpec("Arial", 10)
            .WithSize(12)
            .WithWeight(FontWeight.Bold)
            .AsItalic()
            .WithSize(14);

        Assert.Equal(14, spec.SizeInPoints);
        Assert.Equal(FontWeight.Bold, spec.Weight);
        Assert.True(spec.Style.HasFlag(FontStyle.Italic));
    }

    [Fact]
    public void PointsPerInch_Is72()
    {
        Assert.Equal(72.0, FontSpec.PointsPerInch);
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(0.5)]
    [InlineData(1.0)]
    [InlineData(6.0)]
    [InlineData(12.0)]
    [InlineData(72.0)]
    [InlineData(144.0)]
    [InlineData(999.99)]
    public void WithSize_AcceptsVariousSizes(double size)
    {
        var spec = new FontSpec("Arial", 12).WithSize(size);
        Assert.Equal(size, spec.SizeInPoints);
    }

    [Fact]
    public void MultipleWithWeight_LastOneWins()
    {
        var spec = new FontSpec("Arial", 12)
            .WithWeight(FontWeight.Bold)
            .WithWeight(FontWeight.Light);

        Assert.Equal(FontWeight.Light, spec.Weight);
    }

    [Fact]
    public void WithStyle_ReplacesNotAdds()
    {
        var spec = new FontSpec("Arial", 12)
            .WithStyle(FontStyle.Italic)
            .WithStyle(FontStyle.Underline);

        Assert.False(spec.Style.HasFlag(FontStyle.Italic));
        Assert.True(spec.Style.HasFlag(FontStyle.Underline));
    }
}
