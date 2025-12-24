namespace DailyMath.Tests.Layout;

using DailyMath.Core.Layout;

public class ElementTests
{
    // Hierarchy Tests

    [Fact]
    public void Parent_DefaultsToNull()
    {
        var element = new Element();
        Assert.Null(element.Parent);
    }

    [Fact]
    public void AddChild_SetsParentCorrectly()
    {
        var parent = new Element();
        var child = new Element();

        parent.AddChild(child);

        Assert.Equal(parent, child.Parent);
        Assert.Contains(child, parent.Children);
    }

    [Fact]
    public void AddChild_RemovesFromPreviousParent()
    {
        var parent1 = new Element();
        var parent2 = new Element();
        var child = new Element();

        parent1.AddChild(child);
        parent2.AddChild(child);

        Assert.Equal(parent2, child.Parent);
        Assert.DoesNotContain(child, parent1.Children);
        Assert.Contains(child, parent2.Children);
    }

    [Fact]
    public void RemoveChild_RemovesChildCorrectly()
    {
        var parent = new Element();
        var child = new Element();

        parent.AddChild(child);
        parent.RemoveChild(child);

        Assert.Null(child.Parent);
        Assert.DoesNotContain(child, parent.Children);
    }

    // DPI Inheritance Tests

    [Fact]
    public void GetEffectiveDpi_WithNoDpi_ReturnsDefaultDpi()
    {
        var element = new Element();
        Assert.Equal(Element.DefaultDpi, element.GetEffectiveDpi());
    }

    [Fact]
    public void GetEffectiveDpi_WithDpiSet_ReturnsSetValue()
    {
        var element = new Element { Dpi = 300 };
        Assert.Equal(300, element.GetEffectiveDpi());
    }

    [Fact]
    public void GetEffectiveDpi_InheritsFromParent()
    {
        var parent = new Element { Dpi = 300 };
        var child = new Element();
        parent.AddChild(child);

        Assert.Equal(300, child.GetEffectiveDpi());
    }

    [Fact]
    public void GetEffectiveDpi_ChildOverridesParent()
    {
        var parent = new Element { Dpi = 300 };
        var child = new Element { Dpi = 150 };
        parent.AddChild(child);

        Assert.Equal(150, child.GetEffectiveDpi());
    }

    // Box Model: Root Element Tests

    [Fact]
    public void GetAbsoluteRegion_RootElement_PositionedAtOrigin()
    {
        var root = new Element
        {
            Size = new Measure(
                new Length(8.5, Unit.Inches),
                new Length(11, Unit.Inches)
            ),
            Dpi = 96
        };

        var region = root.GetAbsoluteRegion();

        Assert.Equal(0, region.Left);
        Assert.Equal(0, region.Top);
        Assert.Equal(8.5 * 96, region.Right);
        Assert.Equal(11 * 96, region.Bottom);
    }

    // Box Model: Padding Tests

    [Fact]
    public void GetAbsoluteRegion_ChildRespectsPadding()
    {
        var parent = new Element
        {
            Size = new Measure(new Length(200, Unit.Pixels), new Length(200, Unit.Pixels)),
            Padding = new Inset(
                new Length(10, Unit.Pixels), // Left
                new Length(10, Unit.Pixels), // Top
                new Length(10, Unit.Pixels), // Right
                new Length(10, Unit.Pixels)  // Bottom
            )
        };

        var child = new Element
        {
            Size = new Measure(new Length(100, Unit.Percent), new Length(100, Unit.Percent)),
            Align = Alignment.Fill
        };

        parent.AddChild(child);

        var childRegion = child.GetAbsoluteRegion();

        // Child should fill parent's content area (200 - 10*2 = 180)
        Assert.Equal(10, childRegion.Left);
        Assert.Equal(10, childRegion.Top);
        Assert.Equal(190, childRegion.Right);
        Assert.Equal(190, childRegion.Bottom);
        Assert.Equal(180, childRegion.Width);
        Assert.Equal(180, childRegion.Height);
    }

    // Box Model: Margin Tests

    [Fact]
    public void GetAbsoluteRegion_ChildRespectsMargin()
    {
        var parent = new Element
        {
            Size = new Measure(new Length(200, Unit.Pixels), new Length(200, Unit.Pixels))
        };

        var child = new Element
        {
            Size = new Measure(new Length(100, Unit.Pixels), new Length(100, Unit.Pixels)),
            Margin = new Inset(
                new Length(20, Unit.Pixels), // Left
                new Length(20, Unit.Pixels), // Top
                new Length(0, Unit.Pixels),  // Right
                new Length(0, Unit.Pixels)   // Bottom
            ),
            Align = Alignment.TopLeft
        };

        parent.AddChild(child);

        var childRegion = child.GetAbsoluteRegion();

        Assert.Equal(20, childRegion.Left);
        Assert.Equal(20, childRegion.Top);
        Assert.Equal(120, childRegion.Right);
        Assert.Equal(120, childRegion.Bottom);
    }

    // Alignment Tests

    [Fact]
    public void GetAbsoluteRegion_TopLeft_PositionsCorrectly()
    {
        var parent = new Element
        {
            Size = new Measure(new Length(200, Unit.Pixels), new Length(200, Unit.Pixels))
        };

        var child = new Element
        {
            Size = new Measure(new Length(50, Unit.Pixels), new Length(50, Unit.Pixels)),
            Margin = new Inset(new Length(10, Unit.Pixels), new Length(10, Unit.Pixels), new Length(0, Unit.Pixels), new Length(0, Unit.Pixels)),
            Align = Alignment.TopLeft
        };

        parent.AddChild(child);
        var region = child.GetAbsoluteRegion();

        Assert.Equal(10, region.Left);
        Assert.Equal(10, region.Top);
        Assert.Equal(60, region.Right);
        Assert.Equal(60, region.Bottom);
    }

    [Fact]
    public void GetAbsoluteRegion_TopRight_PositionsCorrectly()
    {
        var parent = new Element
        {
            Size = new Measure(new Length(200, Unit.Pixels), new Length(200, Unit.Pixels))
        };

        var child = new Element
        {
            Size = new Measure(new Length(50, Unit.Pixels), new Length(50, Unit.Pixels)),
            Margin = new Inset(new Length(0, Unit.Pixels), new Length(10, Unit.Pixels), new Length(10, Unit.Pixels), new Length(0, Unit.Pixels)),
            Align = Alignment.TopRight
        };

        parent.AddChild(child);
        var region = child.GetAbsoluteRegion();

        // Right edge: 200 - 10 = 190, Left: 190 - 50 = 140
        Assert.Equal(140, region.Left);
        Assert.Equal(10, region.Top);
        Assert.Equal(190, region.Right);
        Assert.Equal(60, region.Bottom);
    }

    [Fact]
    public void GetAbsoluteRegion_BottomLeft_PositionsCorrectly()
    {
        var parent = new Element
        {
            Size = new Measure(new Length(200, Unit.Pixels), new Length(200, Unit.Pixels))
        };

        var child = new Element
        {
            Size = new Measure(new Length(50, Unit.Pixels), new Length(50, Unit.Pixels)),
            Margin = new Inset(new Length(10, Unit.Pixels), new Length(0, Unit.Pixels), new Length(0, Unit.Pixels), new Length(10, Unit.Pixels)),
            Align = Alignment.BottomLeft
        };

        parent.AddChild(child);
        var region = child.GetAbsoluteRegion();

        // Bottom edge: 200 - 10 = 190, Top: 190 - 50 = 140
        Assert.Equal(10, region.Left);
        Assert.Equal(140, region.Top);
        Assert.Equal(60, region.Right);
        Assert.Equal(190, region.Bottom);
    }

    [Fact]
    public void GetAbsoluteRegion_BottomRight_PositionsCorrectly()
    {
        var parent = new Element
        {
            Size = new Measure(new Length(200, Unit.Pixels), new Length(200, Unit.Pixels))
        };

        var child = new Element
        {
            Size = new Measure(new Length(50, Unit.Pixels), new Length(50, Unit.Pixels)),
            Margin = new Inset(new Length(0, Unit.Pixels), new Length(0, Unit.Pixels), new Length(10, Unit.Pixels), new Length(10, Unit.Pixels)),
            Align = Alignment.BottomRight
        };

        parent.AddChild(child);
        var region = child.GetAbsoluteRegion();

        Assert.Equal(140, region.Left);
        Assert.Equal(140, region.Top);
        Assert.Equal(190, region.Right);
        Assert.Equal(190, region.Bottom);
    }

    [Fact]
    public void GetAbsoluteRegion_Fill_StretchesToParentContentArea()
    {
        var parent = new Element
        {
            Size = new Measure(new Length(200, Unit.Pixels), new Length(200, Unit.Pixels)),
            Padding = new Inset(new Length(10, Unit.Pixels), new Length(10, Unit.Pixels), new Length(10, Unit.Pixels), new Length(10, Unit.Pixels))
        };

        var child = new Element
        {
            Margin = new Inset(new Length(5, Unit.Pixels), new Length(5, Unit.Pixels), new Length(5, Unit.Pixels), new Length(5, Unit.Pixels)),
            Align = Alignment.Fill
        };

        parent.AddChild(child);
        var region = child.GetAbsoluteRegion();

        // Parent content: 10 to 190 (180 wide/tall)
        // Child with margins: 15 to 185 (170 wide/tall)
        Assert.Equal(15, region.Left);
        Assert.Equal(15, region.Top);
        Assert.Equal(185, region.Right);
        Assert.Equal(185, region.Bottom);
    }

    // Percentage Tests

    [Fact]
    public void GetAbsoluteRegion_PercentageSize_CalculatesCorrectly()
    {
        var parent = new Element
        {
            Size = new Measure(new Length(400, Unit.Pixels), new Length(400, Unit.Pixels))
        };

        var child = new Element
        {
            Size = new Measure(new Length(50, Unit.Percent), new Length(25, Unit.Percent)),
            Align = Alignment.TopLeft
        };

        parent.AddChild(child);
        var region = child.GetAbsoluteRegion();

        Assert.Equal(200, region.Width); // 50% of 400
        Assert.Equal(100, region.Height); // 25% of 400
    }

    [Fact]
    public void GetAbsoluteRegion_PercentageMargin_CalculatesCorrectly()
    {
        var parent = new Element
        {
            Size = new Measure(new Length(400, Unit.Pixels), new Length(400, Unit.Pixels))
        };

        var child = new Element
        {
            Size = new Measure(new Length(100, Unit.Pixels), new Length(100, Unit.Pixels)),
            Margin = new Inset(new Length(25, Unit.Percent), new Length(25, Unit.Percent), new Length(0, Unit.Pixels), new Length(0, Unit.Pixels)),
            Align = Alignment.TopLeft
        };

        parent.AddChild(child);
        var region = child.GetAbsoluteRegion();

        // 25% of 400 = 100
        Assert.Equal(100, region.Left);
        Assert.Equal(100, region.Top);
    }

    // Nested Structure Tests

    [Fact]
    public void NestedStructure_ThreeLevels_CalculatesCorrectly()
    {
        var root = new Element
        {
            Size = new Measure(new Length(400, Unit.Pixels), new Length(400, Unit.Pixels)),
            Padding = new Inset(new Length(10, Unit.Pixels), new Length(10, Unit.Pixels), new Length(10, Unit.Pixels), new Length(10, Unit.Pixels))
        };

        var level1 = new Element
        {
            Size = new Measure(new Length(100, Unit.Percent), new Length(100, Unit.Percent)),
            Align = Alignment.Fill,
            Padding = new Inset(new Length(5, Unit.Pixels), new Length(5, Unit.Pixels), new Length(5, Unit.Pixels), new Length(5, Unit.Pixels))
        };
        root.AddChild(level1);

        var level2 = new Element
        {
            Size = new Measure(new Length(100, Unit.Pixels), new Length(100, Unit.Pixels)),
            Margin = new Inset(new Length(10, Unit.Pixels), new Length(10, Unit.Pixels), new Length(0, Unit.Pixels), new Length(0, Unit.Pixels)),
            Align = Alignment.TopLeft
        };
        level1.AddChild(level2);

        var rootRegion = root.GetAbsoluteRegion();
        var level1Region = level1.GetAbsoluteRegion();
        var level2Region = level2.GetAbsoluteRegion();

        // Root: 0-400
        Assert.Equal(400, rootRegion.Width);

        // Level1: fills root content (10-390 = 380 wide)
        Assert.Equal(380, level1Region.Width);
        Assert.Equal(10, level1Region.Left);

        // Level2: positioned in level1's content with margin
        // Level1 content starts at 15 (10+5), margin adds 10 = 25
        Assert.Equal(25, level2Region.Left);
        Assert.Equal(100, level2Region.Width);
    }

    [Fact]
    public void DpiInheritance_DeepHierarchy_InheritsFromRoot()
    {
        var root = new Element { Dpi = 300 };
        var level1 = new Element();
        var level2 = new Element();
        var level3 = new Element();

        root.AddChild(level1);
        level1.AddChild(level2);
        level2.AddChild(level3);

        Assert.Equal(300, level3.GetEffectiveDpi());
    }

    [Fact]
    public void DpiInheritance_MiddleLevelOverride_DescendantsInheritOverride()
    {
        var root = new Element { Dpi = 96 };
        var level1 = new Element { Dpi = 300 };
        var level2 = new Element();

        root.AddChild(level1);
        level1.AddChild(level2);

        Assert.Equal(96, root.GetEffectiveDpi());
        Assert.Equal(300, level1.GetEffectiveDpi());
        Assert.Equal(300, level2.GetEffectiveDpi());
    }

    [Fact]
    public void DpiInheritance_AffectsPhysicalUnitCalculations()
    {
        var root96 = new Element
        {
            Size = new Measure(new Length(1, Unit.Inches), new Length(1, Unit.Inches)),
            Dpi = 96
        };

        var root300 = new Element
        {
            Size = new Measure(new Length(1, Unit.Inches), new Length(1, Unit.Inches)),
            Dpi = 300
        };

        var region96 = root96.GetAbsoluteRegion();
        var region300 = root300.GetAbsoluteRegion();

        Assert.Equal(96, region96.Width);
        Assert.Equal(300, region300.Width);
    }

    [Fact]
    public void ComplexLayout_HeaderBodyFooter_WithMixedUnits()
    {
        var page = new Element
        {
            Size = new Measure(new Length(210, Unit.Millimeters), new Length(297, Unit.Millimeters)),
            Dpi = 96
        };

        var headerHeight = new Length(25.4, Unit.Millimeters); // 1 inch
        var footerHeight = new Length(12.7, Unit.Millimeters); // 0.5 inch

        var header = new Element
        {
            Size = new Measure(new Length(100, Unit.Percent), headerHeight),
            Align = Alignment.TopLeft
        };

        var footer = new Element
        {
            Size = new Measure(new Length(100, Unit.Percent), footerHeight),
            Align = Alignment.BottomLeft
        };

        page.AddChild(header);
        page.AddChild(footer);

        var pageRegion = page.GetAbsoluteRegion();
        var headerRegion = header.GetAbsoluteRegion();
        var footerRegion = footer.GetAbsoluteRegion();

        // Header at top
        Assert.Equal(0, headerRegion.Top);
        Assert.Equal(pageRegion.Width, headerRegion.Width);

        // Footer at bottom
        Assert.Equal(pageRegion.Bottom, footerRegion.Bottom);
        Assert.Equal(pageRegion.Width, footerRegion.Width);

        // They don't overlap
        Assert.True(headerRegion.Bottom < footerRegion.Top);
    }

    [Fact]
    public void ComplexLayout_ArithmeticForBodyHeight()
    {
        var pageHeight = new Length(297, Unit.Millimeters);
        var headerHeight = new Length(1, Unit.Inches);
        var footerHeight = new Length(0.5, Unit.Inches);
        var bodyHeight = pageHeight - headerHeight - footerHeight;

        var page = new Element
        {
            Size = new Measure(new Length(210, Unit.Millimeters), pageHeight),
            Dpi = 96
        };

        var body = new Element
        {
            Size = new Measure(new Length(100, Unit.Percent), bodyHeight),
            Margin = new Inset(new Length(0, Unit.Pixels), headerHeight, new Length(0, Unit.Pixels), new Length(0, Unit.Pixels)),
            Align = Alignment.TopLeft
        };

        page.AddChild(body);

        var pageRegion = page.GetAbsoluteRegion();
        var bodyRegion = body.GetAbsoluteRegion();

        // Body should start after header
        Assert.Equal(headerHeight.ToPixels(96), bodyRegion.Top, precision: 10);

        // Body height should be calculated correctly
        double expectedBodyHeight = bodyHeight.ToPixels(96);
        Assert.Equal(expectedBodyHeight, bodyRegion.Height, precision: 10);
    }

    [Fact]
    public void ComplexLayout_UnevenColumnsWithPercentages()
    {
        // Create a 3-column layout where columns are 25%, 50%, 25%
        var container = new Element
        {
            Size = new Measure(new Length(400, Unit.Pixels), new Length(400, Unit.Pixels))
        };

        var col1 = new Element
        {
            Size = new Measure(new Length(25, Unit.Percent), new Length(100, Unit.Percent)),
            Margin = new Inset(new Length(0, Unit.Percent), new Length(0, Unit.Pixels), new Length(0, Unit.Pixels), new Length(0, Unit.Pixels)),
            Align = Alignment.TopLeft
        };

        var col2 = new Element
        {
            Size = new Measure(new Length(50, Unit.Percent), new Length(100, Unit.Percent)),
            Margin = new Inset(new Length(25, Unit.Percent), new Length(0, Unit.Pixels), new Length(0, Unit.Pixels), new Length(0, Unit.Pixels)),
            Align = Alignment.TopLeft
        };

        var col3 = new Element
        {
            Size = new Measure(new Length(25, Unit.Percent), new Length(100, Unit.Percent)),
            Margin = new Inset(new Length(75, Unit.Percent), new Length(0, Unit.Pixels), new Length(0, Unit.Pixels), new Length(0, Unit.Pixels)),
            Align = Alignment.TopLeft
        };

        container.AddChild(col1);
        container.AddChild(col2);
        container.AddChild(col3);

        var col1Region = col1.GetAbsoluteRegion();
        var col2Region = col2.GetAbsoluteRegion();
        var col3Region = col3.GetAbsoluteRegion();

        // Verify widths
        Assert.Equal(100, col1Region.Width); // 25% of 400
        Assert.Equal(200, col2Region.Width); // 50% of 400
        Assert.Equal(100, col3Region.Width); // 25% of 400

        // Verify positioning
        Assert.Equal(0, col1Region.Left);
        Assert.Equal(100, col2Region.Left); // 25% of 400
        Assert.Equal(300, col3Region.Left); // 75% of 400

        // Verify borders collapse
        Assert.True(col1Region.Right == col2Region.Left);
        Assert.True(col2Region.Right == col3Region.Left);
    }
}
