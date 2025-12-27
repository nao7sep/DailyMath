using System;

namespace DailyMathCore.Layout;

public readonly struct PxInset : IEquatable<PxInset>
{
    public static readonly PxInset Zero = new PxInset(0);

    public double Left { get; }
    public double Top { get; }
    public double Right { get; }
    public double Bottom { get; }

    public PxInset(double uniform)
    {
        Left = Top = Right = Bottom = uniform;
    }

    public PxInset(double horizontal, double vertical)
    {
        Left = Right = horizontal;
        Top = Bottom = vertical;
    }

    public PxInset(double left, double top, double right, double bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    public static PxInset operator +(PxInset inset) => inset;
    public static PxInset operator -(PxInset inset) => new PxInset(-inset.Left, -inset.Top, -inset.Right, -inset.Bottom);
    public static PxInset operator +(PxInset left, PxInset right) => new PxInset(left.Left + right.Left, left.Top + right.Top, left.Right + right.Right, left.Bottom + right.Bottom);
    public static PxInset operator -(PxInset left, PxInset right) => new PxInset(left.Left - right.Left, left.Top - right.Top, left.Right - right.Right, left.Bottom - right.Bottom);

    public bool Equals(PxInset other) => Left.Equals(other.Left) && Top.Equals(other.Top) && Right.Equals(other.Right) && Bottom.Equals(other.Bottom);
    public override bool Equals(object? obj) => obj is PxInset other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Left, Top, Right, Bottom);

    public static bool operator ==(PxInset left, PxInset right) => left.Equals(right);
    public static bool operator !=(PxInset left, PxInset right) => !left.Equals(right);

    public override string ToString() => ToString(null);

    public string ToString(string? format)
    {
        string fmt = format ?? LayoutConstants.DefaultNumericFormat;
        var culture = System.Globalization.CultureInfo.InvariantCulture;
        return $"{{L:{Left.ToString(fmt, culture)}, T:{Top.ToString(fmt, culture)}, R:{Right.ToString(fmt, culture)}, B:{Bottom.ToString(fmt, culture)}}}";
    }
}
