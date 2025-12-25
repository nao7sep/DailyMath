using DailyMath.Core.Rendering;
using Xunit;

namespace DailyMath.Core.Tests.Rendering;

public class FontStyleTests
{
    [Fact]
    public void None_HasZeroValue()
    {
        Assert.Equal(0, (int)FontStyle.None);
    }

    [Fact]
    public void Italic_HasCorrectBitFlag()
    {
        Assert.Equal(1, (int)FontStyle.Italic);
    }

    [Fact]
    public void Underline_HasCorrectBitFlag()
    {
        Assert.Equal(2, (int)FontStyle.Underline);
    }

    [Fact]
    public void Strikethrough_HasCorrectBitFlag()
    {
        Assert.Equal(4, (int)FontStyle.Strikethrough);
    }

    [Fact]
    public void Overline_HasCorrectBitFlag()
    {
        Assert.Equal(8, (int)FontStyle.Overline);
    }

    [Fact]
    public void BitwiseOr_CombinesTwoStyles()
    {
        var combined = FontStyle.Italic | FontStyle.Underline;

        Assert.Equal(3, (int)combined);
        Assert.True(combined.HasFlag(FontStyle.Italic));
        Assert.True(combined.HasFlag(FontStyle.Underline));
    }

    [Fact]
    public void BitwiseOr_CombinesThreeStyles()
    {
        var combined = FontStyle.Italic | FontStyle.Underline | FontStyle.Strikethrough;

        Assert.Equal(7, (int)combined);
        Assert.True(combined.HasFlag(FontStyle.Italic));
        Assert.True(combined.HasFlag(FontStyle.Underline));
        Assert.True(combined.HasFlag(FontStyle.Strikethrough));
    }

    [Fact]
    public void BitwiseOr_CombinesAllStyles()
    {
        var combined = FontStyle.Italic | FontStyle.Underline | FontStyle.Strikethrough | FontStyle.Overline;

        Assert.Equal(15, (int)combined);
        Assert.True(combined.HasFlag(FontStyle.Italic));
        Assert.True(combined.HasFlag(FontStyle.Underline));
        Assert.True(combined.HasFlag(FontStyle.Strikethrough));
        Assert.True(combined.HasFlag(FontStyle.Overline));
    }

    [Fact]
    public void BitwiseAnd_RemovesStyle()
    {
        var combined = FontStyle.Italic | FontStyle.Underline | FontStyle.Strikethrough;
        var removed = combined & ~FontStyle.Underline;

        Assert.Equal(5, (int)removed);
        Assert.True(removed.HasFlag(FontStyle.Italic));
        Assert.False(removed.HasFlag(FontStyle.Underline));
        Assert.True(removed.HasFlag(FontStyle.Strikethrough));
    }

    [Fact]
    public void BitwiseAnd_RemovesMultipleStyles()
    {
        var combined = FontStyle.Italic | FontStyle.Underline | FontStyle.Strikethrough | FontStyle.Overline;
        var removed = combined & ~(FontStyle.Underline | FontStyle.Overline);

        Assert.Equal(5, (int)removed);
        Assert.True(removed.HasFlag(FontStyle.Italic));
        Assert.False(removed.HasFlag(FontStyle.Underline));
        Assert.True(removed.HasFlag(FontStyle.Strikethrough));
        Assert.False(removed.HasFlag(FontStyle.Overline));
    }

    [Fact]
    public void BitwiseXor_TogglesStyle()
    {
        var style = FontStyle.Italic;
        var toggled = style ^ FontStyle.Underline;

        Assert.True(toggled.HasFlag(FontStyle.Italic));
        Assert.True(toggled.HasFlag(FontStyle.Underline));

        var toggledBack = toggled ^ FontStyle.Underline;
        Assert.True(toggledBack.HasFlag(FontStyle.Italic));
        Assert.False(toggledBack.HasFlag(FontStyle.Underline));
    }

    [Fact]
    public void HasFlag_DetectsNone()
    {
        var style = FontStyle.None;

        Assert.True(style.HasFlag(FontStyle.None));
        Assert.False(style.HasFlag(FontStyle.Italic));
    }

    [Fact]
    public void HasFlag_DetectsSingleStyle()
    {
        var style = FontStyle.Italic;

        Assert.True(style.HasFlag(FontStyle.Italic));
        Assert.False(style.HasFlag(FontStyle.Underline));
        Assert.False(style.HasFlag(FontStyle.Strikethrough));
    }

    [Fact]
    public void HasFlag_DetectsStyleInCombination()
    {
        var style = FontStyle.Italic | FontStyle.Strikethrough;

        Assert.True(style.HasFlag(FontStyle.Italic));
        Assert.False(style.HasFlag(FontStyle.Underline));
        Assert.True(style.HasFlag(FontStyle.Strikethrough));
    }

    [Fact]
    public void HasFlag_NoneInCombination_ReturnsTrue()
    {
        var style = FontStyle.Italic | FontStyle.Underline;

        // None (0) is included in any combination - this is standard .NET HasFlag behavior
        Assert.True(style.HasFlag(FontStyle.None));
    }

    [Fact]
    public void BitwiseOr_WithNone_PreservesOriginal()
    {
        var style = FontStyle.Italic | FontStyle.None;

        Assert.Equal(FontStyle.Italic, style);
    }

    [Fact]
    public void BitwiseAnd_WithNone_ReturnsNone()
    {
        var style = FontStyle.Italic & FontStyle.None;

        Assert.Equal(FontStyle.None, style);
    }

    [Fact]
    public void BitwiseOr_SameStyle_IsIdempotent()
    {
        var style = FontStyle.Italic | FontStyle.Italic;

        Assert.Equal(FontStyle.Italic, style);
    }

    [Fact]
    public void BitwiseAnd_RemovalOfNonExistentStyle_NoEffect()
    {
        var style = FontStyle.Italic;
        var result = style & ~FontStyle.Underline;

        Assert.Equal(FontStyle.Italic, result);
    }

    [Fact]
    public void ComplexOperation_AddRemoveToggle()
    {
        // Start with Italic
        var style = FontStyle.Italic;

        // Add Underline and Strikethrough
        style |= FontStyle.Underline | FontStyle.Strikethrough;
        Assert.True(style.HasFlag(FontStyle.Italic));
        Assert.True(style.HasFlag(FontStyle.Underline));
        Assert.True(style.HasFlag(FontStyle.Strikethrough));

        // Remove Underline
        style &= ~FontStyle.Underline;
        Assert.True(style.HasFlag(FontStyle.Italic));
        Assert.False(style.HasFlag(FontStyle.Underline));
        Assert.True(style.HasFlag(FontStyle.Strikethrough));

        // Toggle Overline (add)
        style ^= FontStyle.Overline;
        Assert.True(style.HasFlag(FontStyle.Overline));

        // Toggle Overline again (remove)
        style ^= FontStyle.Overline;
        Assert.False(style.HasFlag(FontStyle.Overline));

        // Final state: Italic | Strikethrough
        Assert.Equal(5, (int)style);
    }

    [Fact]
    public void BitwiseNot_InvertsSingleStyle()
    {
        var style = ~FontStyle.Italic;

        // ~1 should have all bits set except bit 0
        Assert.False(style.HasFlag(FontStyle.Italic));
        Assert.True(style.HasFlag(FontStyle.Underline));
        Assert.True(style.HasFlag(FontStyle.Strikethrough));
        Assert.True(style.HasFlag(FontStyle.Overline));
    }

    [Fact]
    public void BitwiseNot_InvertsNone()
    {
        var style = ~FontStyle.None;

        // ~0 should have all bits set
        Assert.True(style.HasFlag(FontStyle.Italic));
        Assert.True(style.HasFlag(FontStyle.Underline));
        Assert.True(style.HasFlag(FontStyle.Strikethrough));
        Assert.True(style.HasFlag(FontStyle.Overline));
    }
}
