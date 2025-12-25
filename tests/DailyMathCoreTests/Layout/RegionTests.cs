namespace DailyMath.Tests.Layout;

using DailyMath.Core.Layout;

public class RegionTests
{
    [Fact]
    public void Constructor_StoresEdgesAsSourceOfTruth()
    {
        var region = new Region(10, 20, 110, 120);

        Assert.Equal(10, region.Left);
        Assert.Equal(20, region.Top);
        Assert.Equal(110, region.Right);
        Assert.Equal(120, region.Bottom);
    }

    [Fact]
    public void Width_CalculatedFromEdges()
    {
        var region = new Region(10, 20, 110, 120);
        Assert.Equal(100, region.Width); // Right - Left
    }

    [Fact]
    public void Height_CalculatedFromEdges()
    {
        var region = new Region(10, 20, 110, 120);
        Assert.Equal(100, region.Height); // Bottom - Top
    }

    [Fact]
    public void BorderCollapse_AdjacentCellsShareEdges()
    {
        // Simulate two adjacent cells in a grid
        var leftCell = new Region(0, 0, 100, 100);
        var rightCell = new Region(100, 0, 200, 100);

        // The critical guarantee: shared edges are bit-wise identical
        Assert.Equal(leftCell.Right, rightCell.Left);
        Assert.True(leftCell.Right == rightCell.Left); // Exact floating-point equality
    }

    [Fact]
    public void BorderCollapse_VerticallyAdjacentCellsShareEdges()
    {
        var topCell = new Region(0, 0, 100, 100);
        var bottomCell = new Region(0, 100, 100, 200);

        // The critical guarantee: shared edges are bit-wise identical
        Assert.Equal(topCell.Bottom, bottomCell.Top);
        Assert.True(topCell.Bottom == bottomCell.Top); // Exact floating-point equality
    }

    [Fact]
    public void BorderCollapse_FourCellGridCornerMeeting()
    {
        // 2x2 grid
        var topLeft = new Region(0, 0, 100, 100);
        var topRight = new Region(100, 0, 200, 100);
        var bottomLeft = new Region(0, 100, 100, 200);
        var bottomRight = new Region(100, 100, 200, 200);

        // Horizontal edges match
        Assert.True(topLeft.Right == topRight.Left);
        Assert.True(bottomLeft.Right == bottomRight.Left);

        // Vertical edges match
        Assert.True(topLeft.Bottom == bottomLeft.Top);
        Assert.True(topRight.Bottom == bottomRight.Top);

        // Corner point is identical across all four cells
        Assert.True(topLeft.Right == bottomLeft.Right);
        Assert.True(topLeft.Right == topRight.Left);
        Assert.True(topLeft.Bottom == topRight.Bottom);
        Assert.True(topLeft.Bottom == bottomLeft.Top);
    }

    [Fact]
    public void ZeroSizeRegion_IsValid()
    {
        var region = new Region(50, 50, 50, 50);

        Assert.Equal(0, region.Width);
        Assert.Equal(0, region.Height);
    }

    [Fact]
    public void NegativeDimensionRegion_IsValid()
    {
        // Edges can be specified in any order (useful for some calculations)
        var region = new Region(100, 100, 50, 50);

        Assert.Equal(-50, region.Width);
        Assert.Equal(-50, region.Height);
    }
}
