using System;

namespace DailyMath.Core.Rendering;

/// <summary>
/// Defines the complete specification for a font configuration.
/// Immutable struct designed for thread-safety and predictability in layout engines.
/// </summary>
public readonly struct FontSpec
{
    /// <summary>
    /// Gets the font family name (e.g., "Arial", "Consolas").
    /// </summary>
    public string Family { get; }

    /// <summary>
    /// Gets the size of the font in Points (1/72 inch).
    /// This is the standard physical unit for typography.
    /// </summary>
    public double SizePoints { get; }

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
    /// <param name="sizePoints">The size in points.</param>
    /// <param name="weight">The font weight (default is Normal).</param>
    /// <param name="style">The font style flags (default is None).</param>
    public FontSpec(string family, double sizePoints, FontWeight weight = FontWeight.Normal, FontStyle style = FontStyle.None)
    {
        // Guard against invalid sizes (optional, but good practice)
        if (sizePoints <= 0)
            throw new ArgumentOutOfRangeException(nameof(sizePoints), "Font size must be positive.");

        Family = family;
        SizePoints = sizePoints;
        Weight = weight;
        Style = style;
    }

    // --- Fluent Builder Methods (Immutable Modifiers) ---

    /// <summary>
    /// Creates a copy of this font with a different size.
    /// </summary>
    public FontSpec WithSize(double sizePoints)
        => new(Family, sizePoints, Weight, Style);

    /// <summary>
    /// Creates a copy of this font with a different weight.
    /// </summary>
    public FontSpec WithWeight(FontWeight weight)
        => new(Family, SizePoints, weight, Style);

    /// <summary>
    /// Creates a copy of this font with a different style.
    /// Replaces the entire style set.
    /// </summary>
    public FontSpec WithStyle(FontStyle style)
        => new(Family, SizePoints, Weight, style);

    // --- Convenience Helpers ---

    /// <summary>
    /// Creates a copy of this font with the Bold weight (700) or Normal (400).
    /// </summary>
    public FontSpec AsBold(bool bold = true)
        => new(Family, SizePoints, bold ? FontWeight.Bold : FontWeight.Normal, Style);

    /// <summary>
    /// Creates a copy of this font with the Italic flag added or removed.
    /// </summary>
    public FontSpec AsItalic(bool italic = true)
    {
        var newStyle = italic ? (Style | FontStyle.Italic) : (Style & ~FontStyle.Italic);
        return new(Family, SizePoints, Weight, newStyle);
    }

    /// <summary>
    /// Returns a string describing the font (e.g., "Arial 12pt Bold").
    /// </summary>
    public override string ToString()
    {
        var desc = $"{Family}, {SizePoints}pt";
        if (Weight != FontWeight.Normal) desc += $" {Weight}";
        if (Style != FontStyle.None) desc += $" {Style}";
        return desc;
    }
}
