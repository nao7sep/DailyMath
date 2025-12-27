using System;

namespace DailyMathCore.Layout;

public readonly struct PxPoint : IEquatable<PxPoint>
{
    public static readonly PxPoint Zero = new PxPoint(0, 0);

    public double X { get; }
    public double Y { get; }

    public PxPoint(double x, double y)
    {
        X = x;
        Y = y;
    }

    public static PxPoint operator +(PxPoint point) => point;
    public static PxPoint operator -(PxPoint point) => new PxPoint(-point.X, -point.Y);
    public static PxPoint operator +(PxPoint left, PxPoint right) => new PxPoint(left.X + right.X, left.Y + right.Y);
    public static PxPoint operator -(PxPoint left, PxPoint right) => new PxPoint(left.X - right.X, left.Y - right.Y);

    public bool Equals(PxPoint other) => X.Equals(other.X) && Y.Equals(other.Y);
    public override bool Equals(object? obj) => obj is PxPoint other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(X, Y);

    public static bool operator ==(PxPoint left, PxPoint right) => left.Equals(right);
    public static bool operator !=(PxPoint left, PxPoint right) => !left.Equals(right);

    public override string ToString() => ToString(null);

    public string ToString(string? format)
    {
        string fmt = format ?? LayoutConstants.DefaultNumericFormat;
        var culture = System.Globalization.CultureInfo.InvariantCulture;
        return $"{{X:{X.ToString(fmt, culture)}, Y:{Y.ToString(fmt, culture)}}}";
    }
}
