using System;

namespace DailyMathCore.Layout;

/// <summary>
/// Represents a point with flexible coordinates.
/// </summary>
public readonly struct FlexPoint : IEquatable<FlexPoint>
{
    public static readonly FlexPoint Zero = new FlexPoint(FlexLength.Zero, FlexLength.Zero);

    public FlexLength X { get; }
    public FlexLength Y { get; }

    public FlexPoint(FlexLength x, FlexLength y)
    {
        X = x;
        Y = y;
    }

    #region Conversion Methods

    /// <summary>
    /// Resolves the flexible point into a pixel-based point.
    /// </summary>
    /// <param name="dpi">DPI context.</param>
    /// <param name="baseWidth">The width used as the basis for resolving relative coordinates.</param>
    /// <param name="baseHeight">The height used as the basis for resolving relative coordinates.</param>
    public PxPoint ToPxPoint(double? dpi, double? baseWidth, double? baseHeight)
    {
        return new PxPoint(X.ToPixels(dpi, baseWidth), Y.ToPixels(dpi, baseHeight));
    }

    public (double X, double Y) ToPixels(double? dpi, double? baseWidth, double? baseHeight)
    {
        var px = ToPxPoint(dpi, baseWidth, baseHeight);
        return (px.X, px.Y);
    }

    #endregion

    #region Operators

    public static FlexPoint operator +(FlexPoint point) => point;
    public static FlexPoint operator -(FlexPoint point) => new FlexPoint(-point.X, -point.Y);
    public static FlexPoint operator +(FlexPoint left, FlexPoint right) => new FlexPoint(left.X + right.X, left.Y + right.Y);
    public static FlexPoint operator -(FlexPoint left, FlexPoint right) => new FlexPoint(left.X - right.X, left.Y - right.Y);

    public static bool operator ==(FlexPoint left, FlexPoint right) => left.Equals(right);
    public static bool operator !=(FlexPoint left, FlexPoint right) => !left.Equals(right);

    #endregion

    #region Equality and Overrides

    public bool Equals(FlexPoint other) => X.Equals(other.X) && Y.Equals(other.Y);
    public override bool Equals(object? obj) => obj is FlexPoint other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(X, Y);

    public override string ToString() => ToString(null);

    public string ToString(string? format)
    {
        return $"{{X:{X.ToString(format)}, Y:{Y.ToString(format)}}}";
    }

    #endregion
}