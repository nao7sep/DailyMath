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
    /// <param name="rows">Number of rows.</param>
    /// <param name="columns">Number of columns.</param>
    /// <returns>A Table providing indexed access to the created cells.</returns>
    public static Table Create(Element container, int rows, int columns)
    {
        if (rows <= 0)
            throw new ArgumentOutOfRangeException(nameof(rows), "Rows must be positive");
        if (columns <= 0)
            throw new ArgumentOutOfRangeException(nameof(columns), "Columns must be positive");

        var cells = new Element[rows, columns];

        // Use precise doubles for even distribution
        double columnPercent = 100.0 / columns;
        double rowPercent = 100.0 / rows;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                var cell = new Element
                {
                    Align = Alignment.TopLeft,

                    // Size as percentage of container's content area
                    Size = new Measure(columnPercent.AsPercent(), rowPercent.AsPercent()),

                    // Position via percentage-based margins
                    // Left margin = column index * column percentage
                    // Top margin = row index * row percentage
                    Margin = new Inset(
                        (c * columnPercent).AsPercent(), // Left
                        (r * rowPercent).AsPercent(),    // Top
                        0.AsPixels(),                     // Right (unused for TopLeft)
                        0.AsPixels()                      // Bottom (unused for TopLeft)
                    )
                };

                container.AddChild(cell);
                cells[r, c] = cell;
            }
        }

        return new Table(cells);
    }
}
