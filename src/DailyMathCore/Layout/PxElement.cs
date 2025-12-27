using System;

namespace DailyMathCore.Layout;

/// <summary>
/// Represents the resolved, pixel-based values of an Element, including its definitive position and size.
/// </summary>
public readonly struct PxElement : IEquatable<PxElement>
{
    public PxInset Margin { get; }
    public PxPoint Position { get; }
    public PxSize Size { get; }
    public PxInset Padding { get; }

    public PxElement(PxInset margin, PxPoint position, PxSize size, PxInset padding)
    {
        Margin = margin;
        Position = position;
        Size = size;
        Padding = padding;
    }

    public bool Equals(PxElement other)
    {
        return Margin.Equals(other.Margin) &&
               Position.Equals(other.Position) &&
               Size.Equals(other.Size) &&
               Padding.Equals(other.Padding);
    }

    public override bool Equals(object? obj) => obj is PxElement other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Margin, Position, Size, Padding);

    public static bool operator ==(PxElement left, PxElement right) => left.Equals(right);
    public static bool operator !=(PxElement left, PxElement right) => !left.Equals(right);

    public override string ToString() => ToString(null);

    public string ToString(string? format)
    {
        string fmt = format ?? LayoutConstants.DefaultNumericFormat;
        var culture = System.Globalization.CultureInfo.InvariantCulture;
        return $"ML:{Margin.Left.ToString(fmt, culture)}, MT:{Margin.Top.ToString(fmt, culture)}, MR:{Margin.Right.ToString(fmt, culture)}, MB:{Margin.Bottom.ToString(fmt, culture)}, " +
               $"X:{Position.X.ToString(fmt, culture)}, Y:{Position.Y.ToString(fmt, culture)}, " +
               $"W:{Size.Width.ToString(fmt, culture)}, H:{Size.Height.ToString(fmt, culture)}, " +
               $"PL:{Padding.Left.ToString(fmt, culture)}, PT:{Padding.Top.ToString(fmt, culture)}, PR:{Padding.Right.ToString(fmt, culture)}, PB:{Padding.Bottom.ToString(fmt, culture)}";
    }
}
