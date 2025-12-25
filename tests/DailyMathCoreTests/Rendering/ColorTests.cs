namespace DailyMath.Core.Tests.Rendering;

using DailyMath.Core.Rendering;

public class ColorTests
{
    [Fact]
    public void Constructor_CreatesColorWithCorrectComponents()
    {
        var color = new Color(100, 150, 200, 255);

        Assert.Equal(100, color.R);
        Assert.Equal(150, color.G);
        Assert.Equal(200, color.B);
        Assert.Equal(255, color.A);
    }

    [Fact]
    public void Constructor_DefaultsAlphaTo255()
    {
        var color = new Color(100, 150, 200);

        Assert.Equal(255, color.A);
    }

    [Fact]
    public void Constructor_HandlesMinValues()
    {
        var color = new Color(0, 0, 0, 0);

        Assert.Equal(0, color.R);
        Assert.Equal(0, color.G);
        Assert.Equal(0, color.B);
        Assert.Equal(0, color.A);
    }

    [Fact]
    public void Constructor_HandlesMaxValues()
    {
        var color = new Color(255, 255, 255, 255);

        Assert.Equal(255, color.R);
        Assert.Equal(255, color.G);
        Assert.Equal(255, color.B);
        Assert.Equal(255, color.A);
    }

    [Fact]
    public void ToString_ReturnsHexFormat()
    {
        var color = new Color(100, 150, 200, 255);
        var result = color.ToString();

        Assert.Equal("#FF6496C8", result);
    }

    [Fact]
    public void ToString_HandlesTransparentColor()
    {
        var color = new Color(100, 150, 200, 0);
        var result = color.ToString();

        Assert.Equal("#006496C8", result);
    }

    [Fact]
    public void ToString_HandlesSingleDigitHex()
    {
        var color = new Color(1, 2, 3, 4);
        var result = color.ToString();

        Assert.Equal("#04010203", result);
    }

    [Theory]
    [InlineData(0, 0, 0, "#FF000000")]
    [InlineData(255, 255, 255, "#FFFFFFFF")]
    [InlineData(255, 0, 0, "#FFFF0000")]
    [InlineData(0, 255, 0, "#FF00FF00")]
    [InlineData(0, 0, 255, "#FF0000FF")]
    public void PredefinedColors_HaveCorrectValues(byte r, byte g, byte b, string expectedToString)
    {
        var color = new Color(r, g, b);
        Assert.Equal(expectedToString, color.ToString());
    }

    [Fact]
    public void Equality_TwoColorsWithSameValues_AreEqual()
    {
        var color1 = new Color(100, 150, 200, 128);
        var color2 = new Color(100, 150, 200, 128);

        Assert.Equal(color1.R, color2.R);
        Assert.Equal(color1.G, color2.G);
        Assert.Equal(color1.B, color2.B);
        Assert.Equal(color1.A, color2.A);
    }

    [Fact]
    public void StaticColors_AreDistinct()
    {
        // Verify all 8 debug colors are actually different
        var colors = new[]
        {
            Color.Red, Color.Green, Color.Blue, Color.Yellow,
            Color.Cyan, Color.Magenta, Color.Orange, Color.Purple
        };

        // Check each pair is different
        for (int i = 0; i < colors.Length; i++)
        {
            for (int j = i + 1; j < colors.Length; j++)
            {
                bool isDifferent = colors[i].R != colors[j].R ||
                                 colors[i].G != colors[j].G ||
                                 colors[i].B != colors[j].B ||
                                 colors[i].A != colors[j].A;
                Assert.True(isDifferent, $"Color {i} and {j} should be different");
            }
        }
    }
}
