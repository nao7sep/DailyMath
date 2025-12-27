using System;
using System.Globalization;

namespace DailyMathCore.Layout;

/// <summary>
/// Represents a size in pixels (Width and Height).
/// Excludes Margin, includes Padding.
/// </summary>
public readonly struct PxSize : IEquatable<PxSize>
{
    /// <summary>
    /// A shared instance representing a zero size.
    /// </summary>
    public static readonly PxSize Zero = new PxSize(0, 0);

    public double Width { get; }
    public double Height { get; }

    public PxSize(double width, double height)
    {
        Width = width;
        Height = height;
    }

    public static PxSize operator +(PxSize size) => size;
    public static PxSize operator -(PxSize size) => new PxSize(-size.Width, -size.Height);
    public static PxSize operator +(PxSize left, PxSize right) => new PxSize(left.Width + right.Width, left.Height + right.Height);
    public static PxSize operator -(PxSize left, PxSize right) => new PxSize(left.Width - right.Width, left.Height - right.Height);

    public bool Equals(PxSize other) => Width.Equals(other.Width) && Height.Equals(other.Height);
    public override bool Equals(object? obj) => obj is PxSize other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Width, Height);

    public static bool operator ==(PxSize left, PxSize right) => left.Equals(right);
    public static bool operator !=(PxSize left, PxSize right) => !left.Equals(right);

    public override string ToString() => ToString(null);

    public string ToString(string? format)
    {
        string fmt = format ?? LayoutConstants.DefaultNumericFormat;
        var culture = CultureInfo.InvariantCulture;
        return $"{{W:{Width.ToString(fmt, culture)}, H:{Height.ToString(fmt, culture)}}}";
    }
}
