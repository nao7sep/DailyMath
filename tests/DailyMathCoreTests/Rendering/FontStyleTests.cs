namespace DailyMath.Core.Tests.Rendering;

using DailyMath.Core.Rendering;

public class FontStyleTests
{
    [Fact]
    public void None_HasValueZero()
    {
        Assert.Equal(0, (int)FontStyle.None);
    }

    [Fact]
    public void Italic_HasValueOne()
    {
        Assert.Equal(1, (int)FontStyle.Italic);
    }

    [Fact]
    public void CombinedFlags_AddBits()
    {
        var combined = FontStyle.Italic | FontStyle.Underline;
        Assert.Equal(3, (int)combined);
    }

    [Fact]
    public void AllFlags_CombineCorrectly()
    {
        var all = FontStyle.Italic | FontStyle.Underline | FontStyle.Strikethrough | FontStyle.Overline;
        Assert.Equal(15, (int)all); // 1 + 2 + 4 + 8
    }

    [Fact]
    public void HasFlag_DetectsItalicInCombination()
    {
        var combined = FontStyle.Italic | FontStyle.Underline;
        Assert.True(combined.HasFlag(FontStyle.Italic));
    }

    [Fact]
    public void HasFlag_DetectsUnderlineInCombination()
    {
        var combined = FontStyle.Italic | FontStyle.Underline;
        Assert.True(combined.HasFlag(FontStyle.Underline));
    }

    [Fact]
    public void HasFlag_ReturnsFalseForMissingFlag()
    {
        var style = FontStyle.Italic;
        Assert.False(style.HasFlag(FontStyle.Underline));
    }

    [Fact]
    public void HasFlag_NoneDoesNotHaveOtherFlags()
    {
        Assert.False(FontStyle.None.HasFlag(FontStyle.Italic));
        Assert.False(FontStyle.None.HasFlag(FontStyle.Underline));
        Assert.False(FontStyle.None.HasFlag(FontStyle.Strikethrough));
        Assert.False(FontStyle.None.HasFlag(FontStyle.Overline));
    }

    [Fact]
    public void BitwiseAnd_ExtractsSpecificFlag()
    {
        var combined = FontStyle.Italic | FontStyle.Underline | FontStyle.Strikethrough;
        var onlyUnderline = combined & FontStyle.Underline;
        Assert.Equal(FontStyle.Underline, onlyUnderline);
    }

    [Fact]
    public void BitwiseXor_TogglesFlags()
    {
        var style = FontStyle.Italic;
        var toggled = style ^ FontStyle.Italic; // Remove italic
        Assert.Equal(FontStyle.None, toggled);
    }

    [Fact]
    public void BitwiseXor_AddsFlag()
    {
        var style = FontStyle.Italic;
        var added = style ^ FontStyle.Underline; // Add underline
        Assert.Equal(FontStyle.Italic | FontStyle.Underline, added);
    }

    [Fact]
    public void BitwiseNot_InvertsAllFlags()
    {
        var style = FontStyle.None;
        var inverted = ~style;
        // ~0 should have all bits set (as int would be -1)
        Assert.NotEqual(FontStyle.None, inverted);
    }

    [Theory]
    [InlineData(FontStyle.Italic | FontStyle.Underline, FontStyle.Italic, true)]
    [InlineData(FontStyle.Italic | FontStyle.Underline, FontStyle.Strikethrough, false)]
    [InlineData(FontStyle.None, FontStyle.Italic, false)]
    [InlineData(FontStyle.Italic | FontStyle.Underline | FontStyle.Strikethrough, FontStyle.Underline, true)]
    public void HasFlag_VariousCombinations(FontStyle style, FontStyle flag, bool expected)
    {
        Assert.Equal(expected, style.HasFlag(flag));
    }

    [Fact]
    public void MultipleFlags_CanBeRemovedWithBitwiseAnd()
    {
        var all = FontStyle.Italic | FontStyle.Underline | FontStyle.Strikethrough | FontStyle.Overline;
        var withoutUnderline = all & ~FontStyle.Underline;
        
        Assert.True(withoutUnderline.HasFlag(FontStyle.Italic));
        Assert.False(withoutUnderline.HasFlag(FontStyle.Underline));
        Assert.True(withoutUnderline.HasFlag(FontStyle.Strikethrough));
        Assert.True(withoutUnderline.HasFlag(FontStyle.Overline));
    }
}
