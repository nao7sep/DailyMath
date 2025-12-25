using System;

namespace DailyMath.Core.Rendering;

/// <summary>
/// Defines the complete specification for a font configuration.
/// Immutable struct designed for thread-safety and predictability in layout engines.
/// </summary>
public readonly struct FontSpec : IEquatable<FontSpec>
{
    /// <summary>
    /// The number of Points per inch. This is a universal constant in digital typography.
    /// </summary>
    public const double PointsPerInch = 72.0;

    /// <summary>
    /// Gets the font family name (e.g., "Arial", "Consolas").
    /// </summary>
    public string Family { get; }

    /// <summary>
    /// Gets the size of the font in Points (1/72 inch).
    /// This is the standard physical unit for typography.
    /// </summary>
    public double SizeInPoints { get; }

    /// <summary>
    /// Gets the weight (thickness) of the strokes.
    /// </summary>
    public FontWeight Weight { get; }

    /// <summary>
    /// Gets the style flags (decorations and slant).
    /// </summary>
    public FontStyle Style { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FontSpec"/> struct.
    /// </summary>
    /// <param name="family">The font family name.</param>
    /// <param name="sizeInPoints">The size in points.</param>
    /// <param name="weight">The font weight (default is Normal).</param>
    /// <param name="style">The font style flags (default is None).</param>
    public FontSpec(string family, double sizeInPoints, FontWeight weight = FontWeight.Normal, FontStyle style = FontStyle.None)
    {
        if (string.IsNullOrWhiteSpace(family))
            throw new ArgumentException("Font family name cannot be null or whitespace.", nameof(family));

        if (sizeInPoints <= 0 || double.IsNaN(sizeInPoints) || double.IsInfinity(sizeInPoints))
            throw new ArgumentOutOfRangeException(nameof(sizeInPoints), "Font size must be a positive finite number.");

        Family = family;
        SizeInPoints = sizeInPoints;
        Weight = weight;
        Style = style;
    }

    // --- Fluent Builder Methods (Immutable Modifiers) ---

    /// <summary>
    /// Creates a copy of this font with a different size.
    /// </summary>
    public FontSpec WithSize(double sizeInPoints)
        => new(Family, sizeInPoints, Weight, Style);

    /// <summary>
    /// Creates a copy of this font with a different weight.
    /// </summary>
    public FontSpec WithWeight(FontWeight weight)
        => new(Family, SizeInPoints, weight, Style);

    /// <summary>
    /// Creates a copy of this font with a different style.
    /// Replaces the entire style set.
    /// </summary>
    public FontSpec WithStyle(FontStyle style)
        => new(Family, SizeInPoints, Weight, style);

    // --- Convenience Helpers ---

    /// <summary>
    /// Creates a copy of this font with the Bold weight (700) or Normal (400).
    /// </summary>
    public FontSpec AsBold(bool bold = true)
        => new(Family, SizeInPoints, bold ? FontWeight.Bold : FontWeight.Normal, Style);

    /// <summary>
    /// Creates a copy of this font with the Italic flag added or removed.
    /// </summary>
    public FontSpec AsItalic(bool italic = true)
    {
        var newStyle = italic ? (Style | FontStyle.Italic) : (Style & ~FontStyle.Italic);
        return new(Family, SizeInPoints, Weight, newStyle);
    }

    /// <summary>
    /// Returns a string describing the font (e.g., "Arial 12pt Bold").
    /// </summary>
    public override string ToString()
    {
        var desc = $"{Family}, {SizeInPoints}pt";
        if (Weight != FontWeight.Normal) desc += $" {Weight}";
        if (Style != FontStyle.None) desc += $" {Style}";
        return desc;
    }

    // --- Equality ---

    public bool Equals(FontSpec other)
    {
        // Note: Using exact equality for doubles since SizeInPoints comes from user input
        // and should match exactly for fonts to be considered equal
        return Family == other.Family &&
               SizeInPoints == other.SizeInPoints &&
               Weight == other.Weight &&
               Style == other.Style;
    }

    public override bool Equals(object? obj)
    {
        return obj is FontSpec spec && Equals(spec);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Family, SizeInPoints, Weight, Style);
    }

    public static bool operator ==(FontSpec left, FontSpec right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(FontSpec left, FontSpec right)
    {
        return !(left == right);
    }
}
