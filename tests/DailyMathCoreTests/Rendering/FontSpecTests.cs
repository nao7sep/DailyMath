using System;
using DailyMath.Core.Rendering;
using Xunit;

namespace DailyMath.Core.Tests.Rendering;

public class FontSpecTests
{
    [Fact]
    public void Constructor_WithValidParameters_CreatesFont()
    {
        var font = new FontSpec("Arial", 12);

        Assert.Equal("Arial", font.Family);
        Assert.Equal(12, font.SizeInPoints);
        Assert.Equal(FontWeight.Normal, font.Weight);
        Assert.Equal(FontStyle.None, font.Style);
    }

    [Fact]
    public void Constructor_WithAllParameters_CreatesFont()
    {
        var font = new FontSpec("Times New Roman", 14, FontWeight.Bold, FontStyle.Italic);

        Assert.Equal("Times New Roman", font.Family);
        Assert.Equal(14, font.SizeInPoints);
        Assert.Equal(FontWeight.Bold, font.Weight);
        Assert.Equal(FontStyle.Italic, font.Style);
    }

    [Fact]
    public void Constructor_WithZeroSize_ThrowsArgumentOutOfRangeException()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new FontSpec("Arial", 0));

        Assert.Equal("sizeInPoints", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNegativeSize_ThrowsArgumentOutOfRangeException()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new FontSpec("Arial", -5));

        Assert.Equal("sizeInPoints", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithVerySmallPositiveSize_DoesNotThrow()
    {
        var font = new FontSpec("Arial", 0.0001);

        Assert.Equal(0.0001, font.SizeInPoints);
    }

    [Fact]
    public void Constructor_WithVeryLargeSize_DoesNotThrow()
    {
        var font = new FontSpec("Arial", 10000);

        Assert.Equal(10000, font.SizeInPoints);
    }

    [Fact]
    public void PointsPerInch_IsStandardTypographicConstant()
    {
        Assert.Equal(72.0, FontSpec.PointsPerInch);
    }

    [Fact]
    public void WithSize_CreatesNewFontWithDifferentSize()
    {
        var original = new FontSpec("Arial", 12, FontWeight.Bold, FontStyle.Italic);
        var modified = original.WithSize(18);

        Assert.Equal("Arial", modified.Family);
        Assert.Equal(18, modified.SizeInPoints);
        Assert.Equal(FontWeight.Bold, modified.Weight);
        Assert.Equal(FontStyle.Italic, modified.Style);

        // Original unchanged
        Assert.Equal(12, original.SizeInPoints);
    }

    [Fact]
    public void WithSize_InvalidSize_ThrowsArgumentOutOfRangeException()
    {
        var font = new FontSpec("Arial", 12);

        Assert.Throws<ArgumentOutOfRangeException>(() => font.WithSize(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => font.WithSize(-10));
    }

    [Fact]
    public void WithWeight_CreatesNewFontWithDifferentWeight()
    {
        var original = new FontSpec("Arial", 12, FontWeight.Normal, FontStyle.Italic);
        var modified = original.WithWeight(FontWeight.Black);

        Assert.Equal("Arial", modified.Family);
        Assert.Equal(12, modified.SizeInPoints);
        Assert.Equal(FontWeight.Black, modified.Weight);
        Assert.Equal(FontStyle.Italic, modified.Style);

        // Original unchanged
        Assert.Equal(FontWeight.Normal, original.Weight);
    }

    [Fact]
    public void WithStyle_CreatesNewFontWithDifferentStyle()
    {
        var original = new FontSpec("Arial", 12, FontWeight.Bold, FontStyle.None);
        var modified = original.WithStyle(FontStyle.Underline | FontStyle.Strikethrough);

        Assert.Equal("Arial", modified.Family);
        Assert.Equal(12, modified.SizeInPoints);
        Assert.Equal(FontWeight.Bold, modified.Weight);
        Assert.Equal(FontStyle.Underline | FontStyle.Strikethrough, modified.Style);

        // Original unchanged
        Assert.Equal(FontStyle.None, original.Style);
    }

    [Fact]
    public void WithStyle_ReplacesEntireStyle()
    {
        var original = new FontSpec("Arial", 12, FontWeight.Normal, FontStyle.Italic | FontStyle.Underline);
        var modified = original.WithStyle(FontStyle.Strikethrough);

        Assert.Equal(FontStyle.Strikethrough, modified.Style);
        Assert.False(modified.Style.HasFlag(FontStyle.Italic));
        Assert.False(modified.Style.HasFlag(FontStyle.Underline));
    }

    [Fact]
    public void AsBold_WithTrue_SetsBoldWeight()
    {
        var font = new FontSpec("Arial", 12, FontWeight.Light);
        var bold = font.AsBold(true);

        Assert.Equal(FontWeight.Bold, bold.Weight);
        Assert.Equal("Arial", bold.Family);
        Assert.Equal(12, bold.SizeInPoints);
    }

    [Fact]
    public void AsBold_WithFalse_SetsNormalWeight()
    {
        var font = new FontSpec("Arial", 12, FontWeight.Black);
        var normal = font.AsBold(false);

        Assert.Equal(FontWeight.Normal, normal.Weight);
        Assert.Equal("Arial", normal.Family);
        Assert.Equal(12, normal.SizeInPoints);
    }

    [Fact]
    public void AsBold_DefaultParameter_SetsBoldWeight()
    {
        var font = new FontSpec("Arial", 12);
        var bold = font.AsBold();

        Assert.Equal(FontWeight.Bold, bold.Weight);
    }

    [Fact]
    public void AsItalic_WithTrue_AddsItalicStyle()
    {
        var font = new FontSpec("Arial", 12, FontWeight.Normal, FontStyle.None);
        var italic = font.AsItalic(true);

        Assert.True(italic.Style.HasFlag(FontStyle.Italic));
        Assert.Equal("Arial", italic.Family);
        Assert.Equal(12, italic.SizeInPoints);
    }

    [Fact]
    public void AsItalic_WithFalse_RemovesItalicStyle()
    {
        var font = new FontSpec("Arial", 12, FontWeight.Normal, FontStyle.Italic);
        var normal = font.AsItalic(false);

        Assert.False(normal.Style.HasFlag(FontStyle.Italic));
        Assert.Equal("Arial", normal.Family);
        Assert.Equal(12, normal.SizeInPoints);
    }

    [Fact]
    public void AsItalic_DefaultParameter_AddsItalicStyle()
    {
        var font = new FontSpec("Arial", 12);
        var italic = font.AsItalic();

        Assert.True(italic.Style.HasFlag(FontStyle.Italic));
    }

    [Fact]
    public void AsItalic_PreservesOtherStyles_WhenAdding()
    {
        var font = new FontSpec("Arial", 12, FontWeight.Normal, FontStyle.Underline | FontStyle.Strikethrough);
        var italic = font.AsItalic(true);

        Assert.True(italic.Style.HasFlag(FontStyle.Italic));
        Assert.True(italic.Style.HasFlag(FontStyle.Underline));
        Assert.True(italic.Style.HasFlag(FontStyle.Strikethrough));
    }

    [Fact]
    public void AsItalic_PreservesOtherStyles_WhenRemoving()
    {
        var font = new FontSpec("Arial", 12, FontWeight.Normal, FontStyle.Italic | FontStyle.Underline | FontStyle.Strikethrough);
        var normal = font.AsItalic(false);

        Assert.False(normal.Style.HasFlag(FontStyle.Italic));
        Assert.True(normal.Style.HasFlag(FontStyle.Underline));
        Assert.True(normal.Style.HasFlag(FontStyle.Strikethrough));
    }

    [Fact]
    public void AsItalic_WhenAlreadyItalic_RemainsItalic()
    {
        var font = new FontSpec("Arial", 12, FontWeight.Normal, FontStyle.Italic);
        var italic = font.AsItalic(true);

        Assert.Equal(FontStyle.Italic, italic.Style);
    }

    [Fact]
    public void AsItalic_WhenAlreadyNormal_RemainsNormal()
    {
        var font = new FontSpec("Arial", 12, FontWeight.Normal, FontStyle.None);
        var normal = font.AsItalic(false);

        Assert.Equal(FontStyle.None, normal.Style);
    }

    [Fact]
    public void FluentChaining_MultipleModifications()
    {
        var font = new FontSpec("Arial", 10)
            .WithSize(14)
            .WithWeight(FontWeight.SemiBold)
            .AsItalic()
            .WithStyle(FontStyle.Italic | FontStyle.Underline);

        Assert.Equal("Arial", font.Family);
        Assert.Equal(14, font.SizeInPoints);
        Assert.Equal(FontWeight.SemiBold, font.Weight);
        Assert.Equal(FontStyle.Italic | FontStyle.Underline, font.Style);
    }

    [Fact]
    public void ToString_BasicFont_FormatsCorrectly()
    {
        var font = new FontSpec("Arial", 12);

        Assert.Equal("Arial, 12pt", font.ToString());
    }

    [Fact]
    public void ToString_WithBoldWeight_IncludesWeight()
    {
        var font = new FontSpec("Arial", 12, FontWeight.Bold);

        Assert.Equal("Arial, 12pt Bold", font.ToString());
    }

    [Fact]
    public void ToString_WithNormalWeight_DoesNotIncludeWeight()
    {
        var font = new FontSpec("Arial", 12, FontWeight.Normal);

        Assert.Equal("Arial, 12pt", font.ToString());
    }

    [Fact]
    public void ToString_WithStyle_IncludesStyle()
    {
        var font = new FontSpec("Arial", 12, FontWeight.Normal, FontStyle.Italic);

        Assert.Equal("Arial, 12pt Italic", font.ToString());
    }

    [Fact]
    public void ToString_WithWeightAndStyle_IncludesBoth()
    {
        var font = new FontSpec("Arial", 12, FontWeight.Bold, FontStyle.Italic);

        Assert.Equal("Arial, 12pt Bold Italic", font.ToString());
    }

    [Fact]
    public void ToString_WithMultipleStyles_FormatsCorrectly()
    {
        var font = new FontSpec("Arial", 12, FontWeight.Bold, FontStyle.Italic | FontStyle.Underline);

        var result = font.ToString();
        Assert.StartsWith("Arial, 12pt Bold", result);
        // The exact formatting of combined flags may vary, just verify it includes the style info
        Assert.Contains("Italic", result);
    }

    [Fact]
    public void ToString_WithFractionalSize_ShowsDecimal()
    {
        var font = new FontSpec("Arial", 12.5);

        Assert.Equal("Arial, 12.5pt", font.ToString());
    }

    [Fact]
    public void ToString_WithFamilyContainingSpaces_PreservesSpaces()
    {
        var font = new FontSpec("Times New Roman", 14);

        Assert.Equal("Times New Roman, 14pt", font.ToString());
    }

    [Theory]
    [InlineData(FontWeight.Thin)]
    [InlineData(FontWeight.ExtraLight)]
    [InlineData(FontWeight.Light)]
    [InlineData(FontWeight.Medium)]
    [InlineData(FontWeight.SemiBold)]
    [InlineData(FontWeight.ExtraBold)]
    [InlineData(FontWeight.Black)]
    public void ToString_WithVariousWeights_IncludesWeightName(FontWeight weight)
    {
        var font = new FontSpec("Arial", 12, weight);

        var result = font.ToString();
        Assert.Contains(weight.ToString(), result);
    }

    [Fact]
    public void Immutability_ModificationsDoNotAffectOriginal()
    {
        var original = new FontSpec("Arial", 12, FontWeight.Normal, FontStyle.None);

        var modified1 = original.WithSize(18);
        var modified2 = original.WithWeight(FontWeight.Bold);
        var modified3 = original.AsItalic();
        var modified4 = original.AsBold();

        // Verify original is unchanged
        Assert.Equal("Arial", original.Family);
        Assert.Equal(12, original.SizeInPoints);
        Assert.Equal(FontWeight.Normal, original.Weight);
        Assert.Equal(FontStyle.None, original.Style);

        // Verify modifications created new instances
        Assert.NotEqual(original.SizeInPoints, modified1.SizeInPoints);
        Assert.NotEqual(original.Weight, modified2.Weight);
        Assert.NotEqual(original.Style, modified3.Style);
        Assert.NotEqual(original.Weight, modified4.Weight);
    }
}
