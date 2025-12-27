namespace DailyMath.Core.Layout;

/// <summary>
/// Provides factory methods for creating tables with evenly distributed cells.
/// </summary>
public static class TableBuilder
{
    /// <summary>
    /// Creates a table with evenly distributed rows and columns within a container element.
    /// Each cell is positioned using percentage-based margins and sizing to ensure
    /// pixel-perfect alignment (no gaps from rounding).
    /// </summary>
    /// <param name="container">The parent element that will contain the table cells.</param>
    /// <param name="rowCount">Number of rows.</param>
    /// <param name="columnCount">Number of columns.</param>
    /// <returns>A Table providing indexed access to the created cells.</returns>
    public static Table Create(Element container, int rowCount, int columnCount)
    {
        if (rowCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(rowCount), "Rows must be positive");
        if (columnCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(columnCount), "Columns must be positive");

        var cells = new Element[rowCount, columnCount];

        // Use precise doubles for even distribution
        double columnPercent = 100.0 / columnCount;
        double rowPercent = 100.0 / rowCount;

        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
            {
                var cell = new Element
                {
                    Alignment = Alignment.TopLeft,

                    // Size as percentage of container's content area
                    Size = new Measure(columnPercent.AsPercent(), rowPercent.AsPercent()),

                    // Position via percentage-based margins
                    // Left margin = column index * column percentage
                    // Top margin = row index * row percentage
                    Margin = new Inset(
                        (columnIndex * columnPercent).AsPercent(), // Left
                        (rowIndex * rowPercent).AsPercent(),       // Top
                        0.AsPixels(),                              // Right (unused for TopLeft)
                        0.AsPixels()                               // Bottom (unused for TopLeft)
                    )
                };

                container.AddChild(cell);
                cells[rowIndex, columnIndex] = cell;
            }
        }

        return new Table(cells);
    }
}
