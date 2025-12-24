namespace DailyMath.Tests.Layout;

using DailyMath.Core.Layout;

public class TableTests
{
    [Fact]
    public void Constructor_StoresRowAndColumnCount()
    {
        var cells = new Element[3, 4];
        for (int r = 0; r < 3; r++)
            for (int c = 0; c < 4; c++)
                cells[r, c] = new Element();

        var table = new Table(cells);

        Assert.Equal(3, table.RowCount);
        Assert.Equal(4, table.ColumnCount);
    }

    [Fact]
    public void Indexer_ReturnsCorrectCell()
    {
        var cells = new Element[2, 2];
        var cell00 = new Element();
        var cell11 = new Element();
        cells[0, 0] = cell00;
        cells[1, 1] = cell11;
        cells[0, 1] = new Element();
        cells[1, 0] = new Element();

        var table = new Table(cells);

        Assert.Same(cell00, table[0, 0]);
        Assert.Same(cell11, table[1, 1]);
    }

    [Fact]
    public void Indexer_ThrowsForNegativeRow()
    {
        var table = CreateSimpleTable(2, 2);
        Assert.Throws<ArgumentOutOfRangeException>(() => table[-1, 0]);
    }

    [Fact]
    public void Indexer_ThrowsForNegativeColumn()
    {
        var table = CreateSimpleTable(2, 2);
        Assert.Throws<ArgumentOutOfRangeException>(() => table[0, -1]);
    }

    [Fact]
    public void Indexer_ThrowsForRowOutOfBounds()
    {
        var table = CreateSimpleTable(2, 2);
        Assert.Throws<ArgumentOutOfRangeException>(() => table[2, 0]);
    }

    [Fact]
    public void Indexer_ThrowsForColumnOutOfBounds()
    {
        var table = CreateSimpleTable(2, 2);
        Assert.Throws<ArgumentOutOfRangeException>(() => table[0, 2]);
    }

    [Fact]
    public void GetSpannedRegion_SingleCell_ReturnsCellRegion()
    {
        var container = new Element
        {
            Size = new Measure(new Length(400, Unit.Pixels), new Length(400, Unit.Pixels))
        };

        var table = TableBuilder.Create(container, 2, 2);
        var spannedRegion = table.GetSpannedRegion(0, 0, 1, 1);
        var cellRegion = table[0, 0].GetAbsoluteRegion();

        Assert.Equal(cellRegion.Left, spannedRegion.Left);
        Assert.Equal(cellRegion.Top, spannedRegion.Top);
        Assert.Equal(cellRegion.Right, spannedRegion.Right);
        Assert.Equal(cellRegion.Bottom, spannedRegion.Bottom);
    }

    [Fact]
    public void GetSpannedRegion_TwoHorizontalCells_SpansCorrectly()
    {
        var container = new Element
        {
            Size = new Measure(new Length(400, Unit.Pixels), new Length(400, Unit.Pixels))
        };

        var table = TableBuilder.Create(container, 2, 2);
        var spannedRegion = table.GetSpannedRegion(0, 0, 1, 2);

        var leftCell = table[0, 0].GetAbsoluteRegion();
        var rightCell = table[0, 1].GetAbsoluteRegion();

        Assert.Equal(leftCell.Left, spannedRegion.Left);
        Assert.Equal(leftCell.Top, spannedRegion.Top);
        Assert.Equal(rightCell.Right, spannedRegion.Right);
        Assert.Equal(leftCell.Bottom, spannedRegion.Bottom);
    }

    [Fact]
    public void GetSpannedRegion_TwoVerticalCells_SpansCorrectly()
    {
        var container = new Element
        {
            Size = new Measure(new Length(400, Unit.Pixels), new Length(400, Unit.Pixels))
        };

        var table = TableBuilder.Create(container, 2, 2);
        var spannedRegion = table.GetSpannedRegion(0, 0, 2, 1);

        var topCell = table[0, 0].GetAbsoluteRegion();
        var bottomCell = table[1, 0].GetAbsoluteRegion();

        Assert.Equal(topCell.Left, spannedRegion.Left);
        Assert.Equal(topCell.Top, spannedRegion.Top);
        Assert.Equal(topCell.Right, spannedRegion.Right);
        Assert.Equal(bottomCell.Bottom, spannedRegion.Bottom);
    }

    [Fact]
    public void GetSpannedRegion_EntireTable_SpansCorrectly()
    {
        var container = new Element
        {
            Size = new Measure(new Length(400, Unit.Pixels), new Length(400, Unit.Pixels))
        };

        var table = TableBuilder.Create(container, 3, 3);
        var spannedRegion = table.GetSpannedRegion(0, 0, 3, 3);

        var topLeft = table[0, 0].GetAbsoluteRegion();
        var bottomRight = table[2, 2].GetAbsoluteRegion();

        Assert.Equal(topLeft.Left, spannedRegion.Left);
        Assert.Equal(topLeft.Top, spannedRegion.Top);
        Assert.Equal(bottomRight.Right, spannedRegion.Right);
        Assert.Equal(bottomRight.Bottom, spannedRegion.Bottom);
    }

    [Fact]
    public void GetSpannedRegion_ThrowsForNegativeStartRow()
    {
        var table = CreateSimpleTable(2, 2);
        Assert.Throws<ArgumentOutOfRangeException>(() => table.GetSpannedRegion(-1, 0, 1, 1));
    }

    [Fact]
    public void GetSpannedRegion_ThrowsForNegativeStartColumn()
    {
        var table = CreateSimpleTable(2, 2);
        Assert.Throws<ArgumentOutOfRangeException>(() => table.GetSpannedRegion(0, -1, 1, 1));
    }

    [Fact]
    public void GetSpannedRegion_ThrowsForRowCountExceedingBounds()
    {
        var table = CreateSimpleTable(2, 2);
        Assert.Throws<ArgumentOutOfRangeException>(() => table.GetSpannedRegion(1, 0, 2, 1));
    }

    [Fact]
    public void GetSpannedRegion_ThrowsForColumnCountExceedingBounds()
    {
        var table = CreateSimpleTable(2, 2);
        Assert.Throws<ArgumentOutOfRangeException>(() => table.GetSpannedRegion(0, 1, 1, 2));
    }

    // TableBuilder Tests

    [Fact]
    public void TableBuilder_Create_ThrowsForZeroRows()
    {
        var container = new Element();
        Assert.Throws<ArgumentOutOfRangeException>(() => TableBuilder.Create(container, 0, 2));
    }

    [Fact]
    public void TableBuilder_Create_ThrowsForZeroColumns()
    {
        var container = new Element();
        Assert.Throws<ArgumentOutOfRangeException>(() => TableBuilder.Create(container, 2, 0));
    }

    [Fact]
    public void TableBuilder_Create_ThrowsForNegativeRows()
    {
        var container = new Element();
        Assert.Throws<ArgumentOutOfRangeException>(() => TableBuilder.Create(container, -1, 2));
    }

    [Fact]
    public void TableBuilder_Create_ThrowsForNegativeColumns()
    {
        var container = new Element();
        Assert.Throws<ArgumentOutOfRangeException>(() => TableBuilder.Create(container, 2, -1));
    }

    [Fact]
    public void TableBuilder_Create_EvenlyDistributesCells()
    {
        var container = new Element
        {
            Size = new Measure(new Length(300, Unit.Pixels), new Length(300, Unit.Pixels))
        };

        var table = TableBuilder.Create(container, 3, 3);

        // Each cell should be 100x100
        for (int r = 0; r < 3; r++)
        {
            for (int c = 0; c < 3; c++)
            {
                var region = table[r, c].GetAbsoluteRegion();
                Assert.Equal(100, region.Width, precision: 10);
                Assert.Equal(100, region.Height, precision: 10);
            }
        }
    }

    [Fact]
    public void TableBuilder_Create_BordersCollapsePerfectly()
    {
        var container = new Element
        {
            Size = new Measure(new Length(200, Unit.Pixels), new Length(200, Unit.Pixels))
        };

        var table = TableBuilder.Create(container, 2, 2);

        var topLeft = table[0, 0].GetAbsoluteRegion();
        var topRight = table[0, 1].GetAbsoluteRegion();
        var bottomLeft = table[1, 0].GetAbsoluteRegion();
        var bottomRight = table[1, 1].GetAbsoluteRegion();

        // Horizontal borders collapse
        Assert.True(topLeft.Right == topRight.Left);
        Assert.True(bottomLeft.Right == bottomRight.Left);

        // Vertical borders collapse
        Assert.True(topLeft.Bottom == bottomLeft.Top);
        Assert.True(topRight.Bottom == bottomRight.Top);
    }

    [Fact]
    public void TableBuilder_Create_SingleCellTable()
    {
        var container = new Element
        {
            Size = new Measure(new Length(100, Unit.Pixels), new Length(100, Unit.Pixels))
        };

        var table = TableBuilder.Create(container, 1, 1);
        var region = table[0, 0].GetAbsoluteRegion();

        Assert.Equal(container.GetAbsoluteRegion().Width, region.Width);
        Assert.Equal(container.GetAbsoluteRegion().Height, region.Height);
    }

    // Nested Structure Tests

    [Fact]
    public void NestedStructure_TableWithinElement_CalculatesCorrectly()
    {
        var page = new Element
        {
            Size = new Measure(new Length(400, Unit.Pixels), new Length(400, Unit.Pixels)),
            Padding = new Inset(new Length(20, Unit.Pixels), new Length(20, Unit.Pixels), new Length(20, Unit.Pixels), new Length(20, Unit.Pixels))
        };

        var tableContainer = new Element
        {
            Size = new Measure(new Length(100, Unit.Percent), new Length(100, Unit.Percent)),
            Align = Alignment.Fill
        };
        page.AddChild(tableContainer);

        var table = TableBuilder.Create(tableContainer, 2, 2);

        // Table cells should be positioned within page's content area
        var cell00 = table[0, 0].GetAbsoluteRegion();
        var cell11 = table[1, 1].GetAbsoluteRegion();

        // Page content area: 20 to 380 (360 wide/tall)
        // Each cell: 180x180
        Assert.Equal(20, cell00.Left);
        Assert.Equal(20, cell00.Top);
        Assert.Equal(180, cell00.Width, precision: 10);

        Assert.Equal(200, cell11.Left, precision: 10);
        Assert.Equal(200, cell11.Top, precision: 10);
        Assert.Equal(180, cell11.Width, precision: 10);
    }

    [Fact]
    public void NestedStructure_ElementsWithinTableCells()
    {
        var container = new Element
        {
            Size = new Measure(new Length(200, Unit.Pixels), new Length(200, Unit.Pixels))
        };

        var table = TableBuilder.Create(container, 2, 2);

        // Add a child element to a cell
        var cellChild = new Element
        {
            Size = new Measure(new Length(50, Unit.Percent), new Length(50, Unit.Percent)),
            Margin = new Inset(new Length(10, Unit.Percent), new Length(10, Unit.Percent), new Length(0, Unit.Pixels), new Length(0, Unit.Pixels)),
            Align = Alignment.TopLeft
        };
        table[0, 0].AddChild(cellChild);

        var cellRegion = table[0, 0].GetAbsoluteRegion();
        var childRegion = cellChild.GetAbsoluteRegion();

        // Cell is 100x100 at (0,0)
        // Child: 50x50 with 10% margin (10px) → positioned at (10,10) relative to cell
        Assert.Equal(10, childRegion.Left);
        Assert.Equal(10, childRegion.Top);
        Assert.Equal(50, childRegion.Width);
        Assert.Equal(50, childRegion.Height);
    }

    // Helper Methods

    private Table CreateSimpleTable(int rows, int columns)
    {
        var container = new Element
        {
            Size = new Measure(new Length(400, Unit.Pixels), new Length(400, Unit.Pixels))
        };

        return TableBuilder.Create(container, rows, columns);
    }
}
