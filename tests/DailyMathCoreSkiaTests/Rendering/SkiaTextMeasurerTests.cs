namespace DailyMath.Core.Skia.Tests.Rendering;

using DailyMath.Core.Rendering;
using DailyMath.Core.Skia.Rendering;

public class SkiaTextMeasurerTests
{
    private readonly SkiaTextMeasurer _measurer = new();

    [Fact]
    public void MeasureText_ReturnsNonZeroForNormalText()
    {
        var spec = new FontSpec("Arial", 12);
        var size = _measurer.MeasureText("Hello", spec);

        Assert.True(size.Width > 0);
        Assert.True(size.Height > 0);
    }

    [Fact]
    public void MeasureText_EmptyStringReturnsZeroWidth()
    {
        var spec = new FontSpec("Arial", 12);
        var size = _measurer.MeasureText("", spec);

        Assert.Equal(0, size.Width);
    }

    [Fact]
    public void MeasureText_EmptyStringHasNonZeroHeight()
    {
        var spec = new FontSpec("Arial", 12);
        var size = _measurer.MeasureText("", spec);

        // Even empty text should have height (font's ascent + descent)
        Assert.True(size.Height > 0);
    }

    [Fact]
    public void MeasureText_LargerFontProducesLargerSize()
    {
        var spec12 = new FontSpec("Arial", 12);
        var spec24 = new FontSpec("Arial", 24);

        var size12 = _measurer.MeasureText("Test", spec12);
        var size24 = _measurer.MeasureText("Test", spec24);

        Assert.True(size24.Width > size12.Width);
        Assert.True(size24.Height > size12.Height);
    }

    [Fact]
    public void MeasureText_BoldTextIsWiderThanNormal()
    {
        var normal = new FontSpec("Arial", 12);
        var bold = normal.AsBold();

        var normalSize = _measurer.MeasureText("Test", normal);
        var boldSize = _measurer.MeasureText("Test", bold);

        Assert.True(boldSize.Width > normalSize.Width);
    }

    [Fact]
    public void MeasureText_VerySmallFont()
    {
        var spec = new FontSpec("Arial", 0.1);
        var size = _measurer.MeasureText("Text", spec);

        // Should still produce valid measurements, just very small
        Assert.True(size.Width > 0);
        Assert.True(size.Height > 0);
        Assert.True(size.Height < 1); // Should be less than 1 point
    }

    [Fact]
    public void MeasureText_VeryLargeFont()
    {
        var spec = new FontSpec("Arial", 500);
        var size = _measurer.MeasureText("X", spec);

        // Should produce large measurements
        Assert.True(size.Width > 100);
        Assert.True(size.Height > 100);
    }

    [Fact]
    public void MeasureText_SingleCharacter()
    {
        var spec = new FontSpec("Arial", 12);
        var size = _measurer.MeasureText("A", spec);

        Assert.True(size.Width > 0);
        Assert.True(size.Height > 0);
    }

    [Fact]
    public void MeasureText_LongerTextIsWider()
    {
        var spec = new FontSpec("Arial", 12);
        var shortSize = _measurer.MeasureText("Hi", spec);
        var longSize = _measurer.MeasureText("Hello World", spec);

        Assert.True(longSize.Width > shortSize.Width);
        Assert.Equal(shortSize.Height, longSize.Height, precision: 1); // Height should be similar
    }

    [Fact]
    public void GetMaxFontSize_ReturnsLargestSizeThatFits()
    {
        var spec = new FontSpec("Arial", 12);
        var maxSize = _measurer.GetMaxFontSize("Test", spec, 100, 50, 6, 72);

        Assert.True(maxSize >= 6);
        Assert.True(maxSize <= 72);

        // Verify it actually fits
        var fitted = spec.WithSize(maxSize);
        var size = _measurer.MeasureText("Test", fitted);
        Assert.True(size.Width <= 100);
        Assert.True(size.Height <= 50);
    }

    [Fact]
    public void GetMaxFontSize_ReturnsMinWhenEvenMinDoesntFit()
    {
        var spec = new FontSpec("Arial", 12);
        // Very small dimensions that even minSize won't fit
        var maxSize = _measurer.GetMaxFontSize("Very Long Text String", spec, 5, 5, 6, 72);

        Assert.Equal(6, maxSize); // Should return minSize
    }

    [Fact]
    public void GetMaxFontSize_ReturnsMaxWhenEvenMaxFits()
    {
        var spec = new FontSpec("Arial", 12);
        // Very large dimensions where maxSize easily fits
        var maxSize = _measurer.GetMaxFontSize("X", spec, 10000, 10000, 6, 72);

        Assert.Equal(72, maxSize); // Should return maxSize
    }

    [Fact]
    public void GetMaxFontSize_EmptyTextReturnsMax()
    {
        var spec = new FontSpec("Arial", 12);
        var maxSize = _measurer.GetMaxFontSize("", spec, 100, 50, 6, 72);

        // Empty text has zero width, so max size should fit
        Assert.Equal(72, maxSize);
    }

    [Fact]
    public void GetMaxFontSize_SingleCharacterFitsLarger()
    {
        var spec = new FontSpec("Arial", 12);
        var singleChar = _measurer.GetMaxFontSize("X", spec, 100, 100, 6, 72);
        var longText = _measurer.GetMaxFontSize("XXXXXXXXXXXX", spec, 100, 100, 6, 72);

        // Single character should fit at larger size
        Assert.True(singleChar > longText);
    }

    [Fact]
    public void GetMaxFontSize_SquareDimensionsWork()
    {
        var spec = new FontSpec("Arial", 12);
        var maxSize = _measurer.GetMaxFontSize("Test", spec, 100, 100, 6, 72);

        Assert.True(maxSize > 6);
        Assert.True(maxSize <= 72);
    }

    [Fact]
    public void GetMaxFontSize_WideRectangleAllowsLargerSize()
    {
        var spec = new FontSpec("Arial", 12);
        var narrow = _measurer.GetMaxFontSize("Test", spec, 50, 100, 6, 72);
        var wide = _measurer.GetMaxFontSize("Test", spec, 200, 100, 6, 72);

        Assert.True(wide > narrow);
    }

    [Fact]
    public void GetMaxFontSize_TallRectangleAllowsLargerSize()
    {
        var spec = new FontSpec("Arial", 12);
        var short = _measurer.GetMaxFontSize("Test", spec, 100, 30, 6, 72);
        var tall = _measurer.GetMaxFontSize("Test", spec, 100, 100, 6, 72);

        Assert.True(tall > short);
    }

    [Fact]
    public void GetMaxFontSize_PrecisionWithinTolerance()
    {
        var spec = new FontSpec("Arial", 12);
        var maxSize = _measurer.GetMaxFontSize("Test", spec, 100, 50, 6, 72);

        // Check that size just above maxSize doesn't fit
        var tooLarge = spec.WithSize(maxSize + 0.02);
        var tooLargeSize = _measurer.MeasureText("Test", tooLarge);
        
        // At least one dimension should exceed bounds (with small tolerance for rounding)
        bool exceedsBounds = tooLargeSize.Width > 100.01 || tooLargeSize.Height > 50.01;
        Assert.True(exceedsBounds);
    }

    [Fact]
    public void GetMaxFontSize_BinarySearchConverges()
    {
        var spec = new FontSpec("Arial", 12);
        // Use a case that will exercise binary search
        var maxSize = _measurer.GetMaxFontSize("Medium", spec, 75, 40, 6, 72);

        // Should be somewhere in the middle range
        Assert.True(maxSize > 10);
        Assert.True(maxSize < 60);
    }

    [Fact]
    public void MeasureText_NullFamilyUsesDefaultFont()
    {
        var spec = new FontSpec(null!, 12);
        var size = _measurer.MeasureText("Test", spec);

        // Should still produce valid measurements
        Assert.True(size.Width > 0);
        Assert.True(size.Height > 0);
    }

    [Fact]
    public void MeasureText_NonExistentFamilyFallsBackGracefully()
    {
        var spec = new FontSpec("NonExistentFontFamily12345", 12);
        var size = _measurer.MeasureText("Test", spec);

        // Should fall back to some default font
        Assert.True(size.Width > 0);
        Assert.True(size.Height > 0);
    }

    [Theory]
    [InlineData(FontWeight.Thin)]
    [InlineData(FontWeight.Light)]
    [InlineData(FontWeight.Normal)]
    [InlineData(FontWeight.Bold)]
    [InlineData(FontWeight.Black)]
    public void MeasureText_AllFontWeightsWork(FontWeight weight)
    {
        var spec = new FontSpec("Arial", 12, weight);
        var size = _measurer.MeasureText("Test", spec);

        Assert.True(size.Width > 0);
        Assert.True(size.Height > 0);
    }

    [Fact]
    public void MeasureText_ItalicProducesDifferentWidth()
    {
        var normal = new FontSpec("Arial", 12);
        var italic = normal.AsItalic();

        var normalSize = _measurer.MeasureText("Test", normal);
        var italicSize = _measurer.MeasureText("Test", italic);

        // Italic might be slightly narrower or wider depending on font
        Assert.NotEqual(normalSize.Width, italicSize.Width);
    }

    [Fact]
    public void GetMaxFontSize_MinEqualsMax_ReturnsThatSize()
    {
        var spec = new FontSpec("Arial", 12);
        var maxSize = _measurer.GetMaxFontSize("Test", spec, 1000, 1000, 24, 24);

        Assert.Equal(24, maxSize);
    }

    [Fact]
    public void GetMaxFontSize_MinGreaterThanMax_ReturnsMin()
    {
        var spec = new FontSpec("Arial", 12);
        // Intentionally reversed min/max
        var maxSize = _measurer.GetMaxFontSize("Test", spec, 1000, 1000, 50, 10);

        // Should handle gracefully and return min
        Assert.Equal(50, maxSize);
    }
}
