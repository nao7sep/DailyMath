namespace DailyMath.Core.Layout;

using System.Globalization;

using System;

/// <summary>
/// Represents a 2D coordinate in absolute rendering space (pixels).
/// Immutable struct used for final drawing operations.
///
/// Design Note on Units:
/// While layout definition types (like <see cref="Length"/> and <see cref="Measure"/>) are dynamic
/// and support multiple units (Inches, Percent, etc.), <see cref="Point"/> and <see cref="Region"/>
/// represent the result of layout calculations. Therefore, they strictly use <see cref="double"/>
/// representing raw pixels.
/// </summary>
public readonly struct Point : IEquatable<Point>
{
    /// <summary>
    /// The X coordinate (horizontal position) in pixels.
    /// </summary>
    public double X { get; }

    /// <summary>
    /// The Y coordinate (vertical position) in pixels.
    /// </summary>
    public double Y { get; }

    /// <summary>
    /// Gets a Point at (0, 0).
    /// </summary>
    public static Point Zero { get; } = new Point(0, 0);

    /// <summary>
    /// Initializes a new instance of the <see cref="Point"/> struct.
    /// </summary>
    /// <param name="x">The X coordinate in pixels.</param>
    /// <param name="y">The Y coordinate in pixels.</param>
    public Point(double x, double y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Returns a string representation of the coordinates with the specified format.
    /// </summary>
    /// <param name="format">The numeric format string (e.g., "F2", "0.##").</param>
    /// <param name="includeTypeName">If true, prefixes the result with "Point: ".</param>
    /// <returns>A string representation of the coordinates.</returns>
    public string ToString(string format, bool includeTypeName = false)
    {
        string coords = $"{X.ToString(format, CultureInfo.InvariantCulture)}px, {Y.ToString(format, CultureInfo.InvariantCulture)}px";
        return includeTypeName ? $"Point: {coords}" : coords;
    }

    /// <summary>
    /// Returns a string representation of the coordinates (e.g., "10.5px, 20px").
    /// Does not include parentheses to match other content-only ToString implementations.
    /// </summary>
    public override string ToString() => ToString("0.##", false);

    /// <summary>
    /// Determines whether the specified point is equal to the current point.
    /// </summary>
    public bool Equals(Point other)
    {
        // Exact equality is appropriate here as these are absolute coordinates
        // derived from the same layout calculation.
        return X.Equals(other.X) && Y.Equals(other.Y);
    }

    /// <summary>
    /// Determines whether the specified object is a Point and is equal to the current point.
    /// </summary>
    public override bool Equals(object? obj) => obj is Point other && Equals(other);

    /// <summary>
    /// Returns a hash code for this point.
    /// </summary>
    public override int GetHashCode() => HashCode.Combine(X, Y);

    /// <summary>
    /// Compares two points for equality.
    /// </summary>
    public static bool operator ==(Point left, Point right) => left.Equals(right);

    /// <summary>
    /// Compares two points for inequality.
    /// </summary>
    public static bool operator !=(Point left, Point right) => !left.Equals(right);
}
