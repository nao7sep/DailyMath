using DailyMath.Core.Rendering;
using Xunit;

namespace DailyMath.Core.Tests.Rendering;

public class ColorTests
{
    [Fact]
    public void Constructor_WithRgb_CreatesOpaqueColor()
    {
        var color = new Color(100, 150, 200);

        Assert.Equal(100, color.R);
        Assert.Equal(150, color.G);
        Assert.Equal(200, color.B);
        Assert.Equal(255, color.A);
    }

    [Fact]
    public void Constructor_WithRgba_CreatesTransparentColor()
    {
        var color = new Color(100, 150, 200, 128);

        Assert.Equal(100, color.R);
        Assert.Equal(150, color.G);
        Assert.Equal(200, color.B);
        Assert.Equal(128, color.A);
    }

    [Fact]
    public void Constructor_WithZeroAlpha_CreatesFullyTransparentColor()
    {
        var color = new Color(100, 150, 200, 0);

        Assert.Equal(100, color.R);
        Assert.Equal(150, color.G);
        Assert.Equal(200, color.B);
        Assert.Equal(0, color.A);
    }

    [Fact]
    public void Constructor_WithMinValues_HandlesEdgeCase()
    {
        var color = new Color(0, 0, 0, 0);

        Assert.Equal(0, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(0, color.A);
    }

    [Fact]
    public void Constructor_WithMaxValues_HandlesEdgeCase()
    {
        var color = new Color(255, 255, 255, 255);

        Assert.Equal(255, color.R);
        Assert.Equal(255, color.G);
        Assert.Equal(255, color.B);
        Assert.Equal(255, color.A);
    }

    [Fact]
    public void Black_ReturnsOpaqueBlack()
    {
        var black = Color.Black;

        Assert.Equal(0, black.R);
        Assert.Equal(0, black.G);
        Assert.Equal(0, black.B);
        Assert.Equal(255, black.A);
    }

    [Fact]
    public void White_ReturnsOpaqueWhite()
    {
        var white = Color.White;

        Assert.Equal(255, white.R);
        Assert.Equal(255, white.G);
        Assert.Equal(255, white.B);
        Assert.Equal(255, white.A);
    }

    [Fact]
    public void Transparent_ReturnsFullyTransparentBlack()
    {
        var transparent = Color.Transparent;

        Assert.Equal(0, transparent.R);
        Assert.Equal(0, transparent.G);
        Assert.Equal(0, transparent.B);
        Assert.Equal(0, transparent.A);
    }

    [Fact]
    public void Red_ReturnsOpaqueRed()
    {
        var red = Color.Red;

        Assert.Equal(255, red.R);
        Assert.Equal(0, red.G);
        Assert.Equal(0, red.B);
        Assert.Equal(255, red.A);
    }

    [Fact]
    public void Green_ReturnsOpaqueGreen()
    {
        var green = Color.Green;

        Assert.Equal(0, green.R);
        Assert.Equal(255, green.G);
        Assert.Equal(0, green.B);
        Assert.Equal(255, green.A);
    }

    [Fact]
    public void Blue_ReturnsOpaqueBlue()
    {
        var blue = Color.Blue;

        Assert.Equal(0, blue.R);
        Assert.Equal(0, blue.G);
        Assert.Equal(255, blue.B);
        Assert.Equal(255, blue.A);
    }

    [Fact]
    public void Yellow_ReturnsOpaqueYellow()
    {
        var yellow = Color.Yellow;

        Assert.Equal(255, yellow.R);
        Assert.Equal(255, yellow.G);
        Assert.Equal(0, yellow.B);
        Assert.Equal(255, yellow.A);
    }

    [Fact]
    public void Cyan_ReturnsOpaqueCyan()
    {
        var cyan = Color.Cyan;

        Assert.Equal(0, cyan.R);
        Assert.Equal(255, cyan.G);
        Assert.Equal(255, cyan.B);
        Assert.Equal(255, cyan.A);
    }

    [Fact]
    public void Magenta_ReturnsOpaqueMagenta()
    {
        var magenta = Color.Magenta;

        Assert.Equal(255, magenta.R);
        Assert.Equal(0, magenta.G);
        Assert.Equal(255, magenta.B);
        Assert.Equal(255, magenta.A);
    }

    [Fact]
    public void Orange_ReturnsOpaqueOrange()
    {
        var orange = Color.Orange;

        Assert.Equal(255, orange.R);
        Assert.Equal(165, orange.G);
        Assert.Equal(0, orange.B);
        Assert.Equal(255, orange.A);
    }

    [Fact]
    public void Purple_ReturnsOpaquePurple()
    {
        var purple = Color.Purple;

        Assert.Equal(128, purple.R);
        Assert.Equal(0, purple.G);
        Assert.Equal(128, purple.B);
        Assert.Equal(255, purple.A);
    }

    [Fact]
    public void ToString_OpaqueColor_ReturnsHexWithoutAlpha()
    {
        var color = new Color(255, 128, 0);

        Assert.Equal("#FF8000", color.ToString());
    }

    [Fact]
    public void ToString_TransparentColor_ReturnsHexWithAlpha()
    {
        var color = new Color(255, 128, 0, 128);

        Assert.Equal("#FF800080", color.ToString());
    }

    [Fact]
    public void ToString_FullyTransparent_ReturnsHexWithZeroAlpha()
    {
        var color = new Color(100, 150, 200, 0);

        Assert.Equal("#6496C800", color.ToString());
    }

    [Fact]
    public void ToString_Black_ReturnsBlackHex()
    {
        var color = Color.Black;

        Assert.Equal("#000000", color.ToString());
    }

    [Fact]
    public void ToString_White_ReturnsWhiteHex()
    {
        var color = Color.White;

        Assert.Equal("#FFFFFF", color.ToString());
    }

    [Theory]
    [InlineData(0, "#000000")]
    [InlineData(1, "#010000")]
    [InlineData(15, "#0F0000")]
    [InlineData(16, "#100000")]
    [InlineData(255, "#FF0000")]
    public void ToString_RedChannelVariations_FormatsCorrectly(byte red, string expected)
    {
        var color = new Color(red, 0, 0);

        Assert.Equal(expected, color.ToString());
    }

    [Theory]
    [InlineData(0, "#00000000")]
    [InlineData(1, "#00000001")]
    [InlineData(15, "#0000000F")]
    [InlineData(16, "#00000010")]
    [InlineData(128, "#00000080")]
    [InlineData(254, "#000000FE")]
    [InlineData(255, "#000000")]  // Opaque - no alpha suffix
    public void ToString_AlphaChannelVariations_FormatsCorrectly(byte alpha, string expected)
    {
        var color = new Color(0, 0, 0, alpha);

        Assert.Equal(expected, color.ToString());
    }

    [Fact]
    public void Equals_SameValues_ReturnsTrue()
    {
        var color1 = new Color(10, 20, 30, 40);
        var color2 = new Color(10, 20, 30, 40);

        Assert.Equal(color1, color2);
        Assert.True(color1.Equals(color2));
    }

    [Fact]
    public void Equals_DifferentValues_ReturnsFalse()
    {
        var color1 = new Color(10, 20, 30, 40);
        var color2 = new Color(10, 20, 30, 41);

        Assert.NotEqual(color1, color2);
        Assert.False(color1.Equals(color2));
    }

    [Fact]
    public void GetHashCode_SameValues_ReturnsSameHashCode()
    {
        var color1 = new Color(10, 20, 30, 40);
        var color2 = new Color(10, 20, 30, 40);

        Assert.Equal(color1.GetHashCode(), color2.GetHashCode());
    }
}
