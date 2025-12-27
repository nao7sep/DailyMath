using System;

namespace DailyMathCore.Layout;

/// <summary>
/// Represents a set of insets (Margin or Padding) with flexible lengths.
/// </summary>
public readonly struct FlexInset : IEquatable<FlexInset>
{
    public static readonly FlexInset Zero = new FlexInset(FlexLength.Zero);

    public FlexLength Left { get; }
    public FlexLength Top { get; }
    public FlexLength Right { get; }
    public FlexLength Bottom { get; }

    public FlexInset(FlexLength uniformLength)
    {
        Left = Top = Right = Bottom = uniformLength;
    }

    public FlexInset(FlexLength horizontal, FlexLength vertical)
    {
        Left = Right = horizontal;
        Top = Bottom = vertical;
    }

    public FlexInset(FlexLength left, FlexLength top, FlexLength right, FlexLength bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    #region Conversion Methods

    /// <summary>
    /// Resolves the flexible insets into pixel-based insets.
    /// </summary>
    /// <param name="dpi">DPI context.</param>
    /// <param name="baseWidth">The width used as the basis for resolving relative insets.</param>
    /// <param name="baseHeight">The height used as the basis for resolving relative insets.</param>
    public PxInset ToPxInset(double? dpi, double? baseWidth, double? baseHeight)
    {
        return new PxInset(
            Left.ToPixels(dpi, baseWidth),
            Top.ToPixels(dpi, baseHeight),
            Right.ToPixels(dpi, baseWidth),
            Bottom.ToPixels(dpi, baseHeight)
        );
    }

    public (double Left, double Top, double Right, double Bottom) ToPixels(double? dpi, double? baseWidth, double? baseHeight)
    {
        var px = ToPxInset(dpi, baseWidth, baseHeight);
        return (px.Left, px.Top, px.Right, px.Bottom);
    }

    #endregion

    #region Operators

    public static FlexInset operator +(FlexInset inset) => inset;
    public static FlexInset operator -(FlexInset inset) => new FlexInset(-inset.Left, -inset.Top, -inset.Right, -inset.Bottom);

    public static FlexInset operator +(FlexInset left, FlexInset right)
    {
        return new FlexInset(
            left.Left + right.Left,
            left.Top + right.Top,
            left.Right + right.Right,
            left.Bottom + right.Bottom
        );
    }

    public static FlexInset operator -(FlexInset left, FlexInset right)
    {
        return new FlexInset(
            left.Left - right.Left,
            left.Top - right.Top,
            left.Right - right.Right,
            left.Bottom - right.Bottom
        );
    }

    public static bool operator ==(FlexInset left, FlexInset right) => left.Equals(right);
    public static bool operator !=(FlexInset left, FlexInset right) => !left.Equals(right);

    #endregion

    #region Equality and Overrides

    public bool Equals(FlexInset other)
    {
        return Left.Equals(other.Left) &&
               Top.Equals(other.Top) &&
               Right.Equals(other.Right) &&
               Bottom.Equals(other.Bottom);
    }

    public override bool Equals(object? obj) => obj is FlexInset other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Left, Top, Right, Bottom);

    public override string ToString() => ToString(null);

    public string ToString(string? format)
    {
        return $"{{L:{Left.ToString(format)}, T:{Top.ToString(format)}, R:{Right.ToString(format)}, B:{Bottom.ToString(format)}}}";
    }

    #endregion
}