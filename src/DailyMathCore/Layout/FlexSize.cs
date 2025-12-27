using System;
using System.Globalization;

namespace DailyMathCore.Layout;

/// <summary>
/// Represents a size with flexible dimensions.
/// </summary>
public readonly struct FlexSize : IEquatable<FlexSize>
{
    public static readonly FlexSize Zero = new FlexSize(FlexLength.Zero, FlexLength.Zero);

    public FlexLength Width { get; }
    public FlexLength Height { get; }

    public FlexSize(FlexLength width, FlexLength height)
    {
        Width = width;
        Height = height;
    }

    #region Conversion Methods

    /// <summary>
    /// Resolves the flexible size into a pixel-based size.
    /// </summary>
    /// <param name="dpi">DPI context.</param>
    /// <param name="baseWidth">The width used as the basis for resolving relative dimensions.</param>
    /// <param name="baseHeight">The height used as the basis for resolving relative dimensions.</param>
    public PxSize ToPxSize(double? dpi, double? baseWidth, double? baseHeight)
    {
        return new PxSize(Width.ToPixels(dpi, baseWidth), Height.ToPixels(dpi, baseHeight));
    }

    /// <summary>
    /// Provided for API consistency. Returns the resolved dimensions as a tuple.
    /// </summary>
    public (double Width, double Height) ToPixels(double? dpi, double? baseWidth, double? baseHeight)
    {
        var px = ToPxSize(dpi, baseWidth, baseHeight);
        return (px.Width, px.Height);
    }

    #endregion

    #region Operators

    public static FlexSize operator +(FlexSize size) => size;
    public static FlexSize operator -(FlexSize size) => new FlexSize(-size.Width, -size.Height);
    public static FlexSize operator +(FlexSize left, FlexSize right) => new FlexSize(left.Width + right.Width, left.Height + right.Height);
    public static FlexSize operator -(FlexSize left, FlexSize right) => new FlexSize(left.Width - right.Width, left.Height - right.Height);

    public static bool operator ==(FlexSize left, FlexSize right) => left.Equals(right);
    public static bool operator !=(FlexSize left, FlexSize right) => !left.Equals(right);

    #endregion

    #region Equality and Overrides

    public bool Equals(FlexSize other) => Width.Equals(other.Width) && Height.Equals(other.Height);
    public override bool Equals(object? obj) => obj is FlexSize other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Width, Height);

    public override string ToString() => ToString(null);

    public string ToString(string? format)
    {
        return $"{{W:{Width.ToString(format)}, H:{Height.ToString(format)}}}";
    }

    #endregion
}
