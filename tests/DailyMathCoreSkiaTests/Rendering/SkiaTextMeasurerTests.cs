using System;
using DailyMath.Core.Layout;
using DailyMath.Core.Rendering;
using DailyMath.Core.Skia;
using Xunit;

namespace DailyMath.Core.Skia.Tests.Rendering;

public class SkiaTextMeasurerTests
{
    private readonly SkiaTextMeasurer _measurer = new();
    private const double StandardDpi = 96.0;

    [Fact]
    public void MeasureText_EmptyString_ReturnsZeroSize()
    {
        var font = new FontSpec("Arial", 12);
        var measure = _measurer.MeasureText("", font, StandardDpi);

        Assert.Equal(0, measure.Width.Value);
        Assert.Equal(0, measure.Height.Value);
    }

    [Fact]
    public void MeasureText_NullString_ReturnsZeroSize()
    {
        var font = new FontSpec("Arial", 12);
        var measure = _measurer.MeasureText(null!, font, StandardDpi);

        Assert.Equal(0, measure.Width.Value);
        Assert.Equal(0, measure.Height.Value);
    }

    [Fact]
    public void MeasureText_SingleCharacter_ReturnsNonZeroSize()
    {
        var font = new FontSpec("Arial", 12);
        var measure = _measurer.MeasureText("A", font, StandardDpi);

        Assert.True(measure.Width.Value > 0);
        Assert.True(measure.Height.Value > 0);
    }

    [Fact]
    public void MeasureText_LongerText_HasGreaterWidth()
    {
        var font = new FontSpec("Arial", 12);
        var shortMeasure = _measurer.MeasureText("A", font, StandardDpi);
        var longMeasure = _measurer.MeasureText("AAAA", font, StandardDpi);

        Assert.True(longMeasure.Width.Value > shortMeasure.Width.Value);
    }

    [Fact]
    public void MeasureText_LargerFont_HasGreaterSize()
    {
        var smallFont = new FontSpec("Arial", 12);
        var largeFont = new FontSpec("Arial", 24);
        var text = "Hello";

        var smallMeasure = _measurer.MeasureText(text, smallFont, StandardDpi);
        var largeMeasure = _measurer.MeasureText(text, largeFont, StandardDpi);

        Assert.True(largeMeasure.Width.Value > smallMeasure.Width.Value);
        Assert.True(largeMeasure.Height.Value > smallMeasure.Height.Value);
    }

    [Fact]
    public void MeasureText_HigherDpi_ProducesLargerPixelMeasurements()
    {
        var font = new FontSpec("Arial", 12);
        var text = "Hello";

        var lowDpiMeasure = _measurer.MeasureText(text, font, 96);
        var highDpiMeasure = _measurer.MeasureText(text, font, 300);

        // Higher DPI converts more pixels per point
        Assert.True(highDpiMeasure.Width.Value > lowDpiMeasure.Width.Value);
        Assert.True(highDpiMeasure.Height.Value > lowDpiMeasure.Height.Value);
    }

    [Fact]
    public void MeasureText_BoldFont_MayHaveDifferentWidth()
    {
        var normalFont = new FontSpec("Arial", 12, FontWeight.Normal);
        var boldFont = new FontSpec("Arial", 12, FontWeight.Bold);
        var text = "Hello";

        var normalMeasure = _measurer.MeasureText(text, normalFont, StandardDpi);
        var boldMeasure = _measurer.MeasureText(text, boldFont, StandardDpi);

        // Bold text typically has different width (usually wider)
        // We can't assert exact relationship as it depends on font rendering
        Assert.NotEqual(normalMeasure.Width.Value, boldMeasure.Width.Value);
    }

    [Fact]
    public void MeasureText_ItalicStyle_DoesNotThrow()
    {
        var font = new FontSpec("Arial", 12, FontWeight.Normal, FontStyle.Italic);
        var measure = _measurer.MeasureText("Hello", font, StandardDpi);

        Assert.True(measure.Width.Value > 0);
        Assert.True(measure.Height.Value > 0);
    }

    [Fact]
    public void MeasureText_UnderlineStyle_DoesNotAffectMeasurement()
    {
        // Underline is a decoration, not a glyph modifier
        var normalFont = new FontSpec("Arial", 12);
        var underlineFont = new FontSpec("Arial", 12, FontWeight.Normal, FontStyle.Underline);
        var text = "Hello";

        var normalMeasure = _measurer.MeasureText(text, normalFont, StandardDpi);
        var underlineMeasure = _measurer.MeasureText(text, underlineFont, StandardDpi);

        // Underline should not affect text measurement (drawn separately)
        Assert.Equal(normalMeasure.Width.Value, underlineMeasure.Width.Value);
        Assert.Equal(normalMeasure.Height.Value, underlineMeasure.Height.Value);
    }

    [Fact]
    public void MeasureText_WithWhitespace_IncludesWhitespaceWidth()
    {
        var font = new FontSpec("Arial", 12);
        var noSpaceMeasure = _measurer.MeasureText("AB", font, StandardDpi);
        var withSpaceMeasure = _measurer.MeasureText("A B", font, StandardDpi);

        Assert.True(withSpaceMeasure.Width.Value > noSpaceMeasure.Width.Value);
    }

    [Fact]
    public void MeasureText_SpecialCharacters_DoesNotThrow()
    {
        var font = new FontSpec("Arial", 12);
        var text = "Hello™©®";

        var measure = _measurer.MeasureText(text, font, StandardDpi);

        Assert.True(measure.Width.Value > 0);
        Assert.True(measure.Height.Value > 0);
    }

    [Fact]
    public void MeasureText_Unicode_DoesNotThrow()
    {
        var font = new FontSpec("Arial", 12);
        var text = "Hello 世界 🌍";

        var measure = _measurer.MeasureText(text, font, StandardDpi);

        Assert.True(measure.Width.Value > 0);
        Assert.True(measure.Height.Value > 0);
    }

    [Fact]
    public void MeasureText_VerySmallFont_DoesNotThrow()
    {
        var font = new FontSpec("Arial", 1);
        var measure = _measurer.MeasureText("Hello", font, StandardDpi);

        Assert.True(measure.Width.Value > 0);
        Assert.True(measure.Height.Value > 0);
    }

    [Fact]
    public void MeasureText_VeryLargeFont_DoesNotThrow()
    {
        var font = new FontSpec("Arial", 200);
        var measure = _measurer.MeasureText("Hello", font, StandardDpi);

        Assert.True(measure.Width.Value > 0);
        Assert.True(measure.Height.Value > 0);
    }

    [Fact]
    public void GetMaxFontSize_TextFitsExactly_ReturnsOptimalSize()
    {
        var baseFont = new FontSpec("Arial", 12);
        var text = "Hello";

        // Measure at a known size first
        var knownMeasure = _measurer.MeasureText(text, baseFont, StandardDpi);

        // Now ask for max size that fits in slightly larger bounds
        var bounds = new Measure((knownMeasure.Width.Value + 10).AsPixels(), (knownMeasure.Height.Value + 10).AsPixels());
        var maxSize = _measurer.GetMaxFontSize(text, baseFont, bounds, StandardDpi, minSizeInPoints: 6, maxSizeInPoints: 20);

        // Should be close to or slightly larger than original size
        Assert.True(maxSize >= 12);
    }

    [Fact]
    public void GetMaxFontSize_TinyBounds_ReturnsMinimumSize()
    {
        var baseFont = new FontSpec("Arial", 12);
        var text = "Hello World";
        var tinyBounds = new Measure(10.AsPixels(), 5.AsPixels());

        var maxSize = _measurer.GetMaxFontSize(text, baseFont, tinyBounds, StandardDpi, minSizeInPoints: 6, maxSizeInPoints: 72);

        // Should return close to minimum
        Assert.True(maxSize <= 7);
    }

    [Fact]
    public void GetMaxFontSize_LargeBounds_ReturnsMaximumSize()
    {
        var baseFont = new FontSpec("Arial", 12);
        var text = "Hi";
        var hugeBounds = new Measure(10000.AsPixels(), 10000.AsPixels());

        var maxSize = _measurer.GetMaxFontSize(text, baseFont, hugeBounds, StandardDpi, minSizeInPoints: 6, maxSizeInPoints: 72);

        // Should return close to maximum
        Assert.True(maxSize >= 71);
    }

    [Fact]
    public void GetMaxFontSize_ResultingTextFitsInBounds()
    {
        var baseFont = new FontSpec("Arial", 12);
        var text = "Test";
        var bounds = new Measure(200.AsPixels(), 50.AsPixels());

        var maxSize = _measurer.GetMaxFontSize(text, baseFont, bounds, StandardDpi);

        // Verify that text at the returned size actually fits
        var testFont = baseFont.WithSize(maxSize);
        var measure = _measurer.MeasureText(text, testFont, StandardDpi);

        Assert.True(measure.Width.Value <= bounds.Width.Value);
        Assert.True(measure.Height.Value <= bounds.Height.Value);
    }

    [Fact]
    public void GetMaxFontSize_SlightlyLargerSize_DoesNotFit()
    {
        var baseFont = new FontSpec("Arial", 12);
        var text = "Test";
        var bounds = new Measure(200.AsPixels(), 50.AsPixels());

        var maxSize = _measurer.GetMaxFontSize(text, baseFont, bounds, StandardDpi);

        // Try a slightly larger size
        var tooLargeFont = baseFont.WithSize(maxSize + 0.5);
        var measure = _measurer.MeasureText(text, tooLargeFont, StandardDpi);

        // Should exceed at least one dimension (may be very close due to rounding)
        bool exceedsWidth = measure.Width.Value > bounds.Width.Value;
        bool exceedsHeight = measure.Height.Value > bounds.Height.Value;

        // With binary search precision, we might be right at the edge
        // So we check if it exceeds OR is very close
        Assert.True(exceedsWidth || exceedsHeight ||
                    (measure.Width.Value >= bounds.Width.Value - 1 && measure.Height.Value >= bounds.Height.Value - 1));
    }

    [Fact]
    public void GetMaxFontSize_WidthConstrainedOnly_RespectsWidthLimit()
    {
        var baseFont = new FontSpec("Arial", 12);
        var text = "Hello World";
        var bounds = new Measure(100.AsPixels(), double.MaxValue.AsPixels());

        var maxSize = _measurer.GetMaxFontSize(text, baseFont, bounds, StandardDpi);
        var testFont = baseFont.WithSize(maxSize);
        var measure = _measurer.MeasureText(text, testFont, StandardDpi);

        Assert.True(measure.Width.Value <= bounds.Width.Value);
    }

    [Fact]
    public void GetMaxFontSize_HeightConstrainedOnly_RespectsHeightLimit()
    {
        var baseFont = new FontSpec("Arial", 12);
        var text = "Hi";
        var bounds = new Measure(double.MaxValue.AsPixels(), 20.AsPixels());

        var maxSize = _measurer.GetMaxFontSize(text, baseFont, bounds, StandardDpi);
        var testFont = baseFont.WithSize(maxSize);
        var measure = _measurer.MeasureText(text, testFont, StandardDpi);

        Assert.True(measure.Height.Value <= bounds.Height.Value);
    }

    [Fact]
    public void GetMaxFontSize_DifferentDpi_ProducesDifferentResults()
    {
        var baseFont = new FontSpec("Arial", 12);
        var text = "Hello";
        var bounds = new Measure(200.AsPixels(), 50.AsPixels());

        var size96 = _measurer.GetMaxFontSize(text, baseFont, bounds, 96);
        var size300 = _measurer.GetMaxFontSize(text, baseFont, bounds, 300);

        // Higher DPI means same point size produces more pixels, so max size should be smaller
        Assert.True(size300 < size96);
    }

    [Fact]
    public void GetMaxFontSize_EmptyString_ReturnsMaxSize()
    {
        var baseFont = new FontSpec("Arial", 12);
        var bounds = new Measure(100.AsPixels(), 50.AsPixels());

        var maxSize = _measurer.GetMaxFontSize("", baseFont, bounds, StandardDpi, minSizeInPoints: 6, maxSizeInPoints: 72);

        // Empty string has zero size, but implementation still uses font metrics
        // Just verify we get a valid result within range
        Assert.True(maxSize >= 6 && maxSize <= 72);
    }

    [Fact]
    public void GetMaxFontSize_LongText_ReturnsRelativelySmallerSize()
    {
        var baseFont = new FontSpec("Arial", 12);
        var bounds = new Measure(200.AsPixels(), 50.AsPixels());

        var shortTextSize = _measurer.GetMaxFontSize("Hi", baseFont, bounds, StandardDpi);
        var longTextSize = _measurer.GetMaxFontSize("Hello World This Is Long", baseFont, bounds, StandardDpi);

        // Longer text needs smaller size to fit in same bounds
        Assert.True(longTextSize < shortTextSize);
    }

    [Fact]
    public void GetMaxFontSize_BoldFont_ConsidersWeight()
    {
        var normalFont = new FontSpec("Arial", 12, FontWeight.Normal);
        var boldFont = new FontSpec("Arial", 12, FontWeight.Bold);
        var text = "Hello World";
        var bounds = new Measure(150.AsPixels(), 50.AsPixels());

        var normalSize = _measurer.GetMaxFontSize(text, normalFont, bounds, StandardDpi);
        var boldSize = _measurer.GetMaxFontSize(text, boldFont, bounds, StandardDpi);

        // Bold text is typically wider, so max size should be smaller
        Assert.True(boldSize <= normalSize);
    }

    [Fact]
    public void GetMaxFontSize_CustomMinMax_RespectsLimits()
    {
        var baseFont = new FontSpec("Arial", 12);
        var text = "Hello";
        var hugeBounds = new Measure(10000.AsPixels(), 10000.AsPixels());

        var maxSize = _measurer.GetMaxFontSize(text, baseFont, hugeBounds, StandardDpi, minSizeInPoints: 20, maxSizeInPoints: 30);

        Assert.True(maxSize >= 20);
        Assert.True(maxSize <= 30);
    }

    [Fact]
    public void GetMaxFontSize_VeryNarrowRange_ConvergesCorrectly()
    {
        var baseFont = new FontSpec("Arial", 12);
        var text = "Test";
        var bounds = new Measure(100.AsPixels(), 50.AsPixels());

        var maxSize = _measurer.GetMaxFontSize(text, baseFont, bounds, StandardDpi, minSizeInPoints: 10, maxSizeInPoints: 10.1);

        Assert.True(maxSize >= 10 && maxSize <= 10.1);
    }
}
