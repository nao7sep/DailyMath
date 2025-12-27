namespace DailyMath.Core.Layout;

using System.Globalization;

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
    public Element this[int rowIndex, int columnIndex]
    {
        get
        {
            if (rowIndex < 0 || rowIndex >= RowCount)
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            if (columnIndex < 0 || columnIndex >= ColumnCount)
                throw new ArgumentOutOfRangeException(nameof(columnIndex));
            return _cells[rowIndex, columnIndex];
        }
    }

    /// <summary>
    /// Calculates the region spanning multiple cells.
    /// Useful for drawing content across merged cells without creating a spanning mechanism.
    /// </summary>
    /// <param name="rowIndex">Starting row index (inclusive).</param>
    /// <param name="columnIndex">Starting column index (inclusive).</param>
    /// <param name="rowCount">Number of rows to span.</param>
    /// <param name="columnCount">Number of columns to span.</param>
    /// <returns>A region covering the specified cell range.</returns>
    public Region GetSpannedRegion(int rowIndex, int columnIndex, int rowCount = 1, int columnCount = 1)
    {
        if (rowIndex < 0 || rowIndex >= RowCount)
            throw new ArgumentOutOfRangeException(nameof(rowIndex));
        if (columnIndex < 0 || columnIndex >= ColumnCount)
            throw new ArgumentOutOfRangeException(nameof(columnIndex));
        if (rowIndex + rowCount > RowCount)
            throw new ArgumentOutOfRangeException(nameof(rowCount));
        if (columnIndex + columnCount > ColumnCount)
            throw new ArgumentOutOfRangeException(nameof(columnCount));

        // Get regions of corner cells
        Region topLeft = _cells[rowIndex, columnIndex].GetAbsoluteRegion();
        Region bottomRight = _cells[rowIndex + rowCount - 1, columnIndex + columnCount - 1].GetAbsoluteRegion();

        return new Region(
            topLeft.Left,
            topLeft.Top,
            bottomRight.Right,
            bottomRight.Bottom
        );
    }

    /// <summary>
    /// Returns a string representation of the table's dimensions.
    /// </summary>
    /// <param name="includeTypeName">If true, prefixes the result with "Table: ".</param>
    /// <returns>A string representation of the table.</returns>
    public string ToString(bool includeTypeName = false)
    {
        string dimensions = $"{RowCount.ToString(CultureInfo.InvariantCulture)}x{ColumnCount.ToString(CultureInfo.InvariantCulture)}";
        return includeTypeName ? $"Table: {dimensions}" : dimensions;
    }

    /// <summary>
    /// Returns a string representation of the table's dimensions.
    /// </summary>
    public override string ToString() => ToString(false);
}
