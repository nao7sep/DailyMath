namespace DailyMath.Core.Layout;

/// <summary>
/// Represents a grid of elements arranged in rows and columns.
/// Provides indexed access to cells but does not manage grid lines -
/// grid lines emerge naturally when rendering cell borders.
/// </summary>
public class Table
{
    private readonly Element[,] _cells;

    /// <summary>
    /// Gets the number of rows in this table.
    /// </summary>
    public int RowCount { get; }

    /// <summary>
    /// Gets the number of columns in this table.
    /// </summary>
    public int ColumnCount { get; }

    /// <summary>
    /// Creates a new table with the specified cells.
    /// </summary>
    /// <param name="cells">A 2D array of elements [row, column].</param>
    public Table(Element[,] cells)
    {
        _cells = cells;
        RowCount = cells.GetLength(0);
        ColumnCount = cells.GetLength(1);
    }

    /// <summary>
    /// Gets the element at the specified row and column.
    /// </summary>
    public Element this[int row, int column]
    {
        get
        {
            if (row < 0 || row >= RowCount)
                throw new ArgumentOutOfRangeException(nameof(row));
            if (column < 0 || column >= ColumnCount)
                throw new ArgumentOutOfRangeException(nameof(column));
            return _cells[row, column];
        }
    }

    /// <summary>
    /// Calculates the region spanning multiple cells.
    /// Useful for drawing content across merged cells without creating a spanning mechanism.
    /// </summary>
    /// <param name="startRow">Starting row index (inclusive).</param>
    /// <param name="startColumn">Starting column index (inclusive).</param>
    /// <param name="rowCount">Number of rows to span.</param>
    /// <param name="columnCount">Number of columns to span.</param>
    /// <returns>A region covering the specified cell range.</returns>
    public Region GetSpannedRegion(int startRow, int startColumn, int rowCount = 1, int columnCount = 1)
    {
        if (startRow < 0 || startRow >= RowCount)
            throw new ArgumentOutOfRangeException(nameof(startRow));
        if (startColumn < 0 || startColumn >= ColumnCount)
            throw new ArgumentOutOfRangeException(nameof(startColumn));
        if (startRow + rowCount > RowCount)
            throw new ArgumentOutOfRangeException(nameof(rowCount));
        if (startColumn + columnCount > ColumnCount)
            throw new ArgumentOutOfRangeException(nameof(columnCount));

        // Get regions of corner cells
        Region topLeft = _cells[startRow, startColumn].GetAbsoluteRegion();
        Region bottomRight = _cells[startRow + rowCount - 1, startColumn + columnCount - 1].GetAbsoluteRegion();

        return new Region(
            topLeft.Left,
            topLeft.Top,
            bottomRight.Right,
            bottomRight.Bottom
        );
    }
}
