namespace DailyMath.Core.Rendering;

/// <summary>
/// Represents a color with red, green, blue, and alpha components.
/// Platform-agnostic color representation for cross-platform rendering.
/// </summary>
public readonly struct Color : IEquatable<Color>
{
    /// <summary>
    /// Default alpha used when parsing strings in RGBA format if alpha is not present.
    /// Fully opaque (255).
    /// </summary>
    public const byte DefaultAlphaForRgba = 255;

    /// <summary>
    /// Default alpha used when parsing strings in ARGB format if alpha is not present.
    /// Fully opaque (255).
    /// </summary>
    public const byte DefaultAlphaForArgb = 255;

    // --- Special ---
    public static Color Transparent => new(0, 0, 0, 0);

    // --- Grayscale ---
    public static Color Black => new(0, 0, 0);
    public static Color Gray => new(128, 128, 128);
    public static Color White => new(255, 255, 255);

    // --- Additive Primaries (RGB) ---
    public static Color Red => new(255, 0, 0);
    public static Color Green => new(0, 255, 0);
    public static Color Blue => new(0, 0, 255);

    // --- Subtractive Primaries (CMY) ---
    public static Color Cyan => new(0, 255, 255);
    public static Color Magenta => new(255, 0, 255);
    public static Color Yellow => new(255, 255, 0);

    // --- Common Mixed ---
    public static Color Orange => new(255, 165, 0);
    public static Color Purple => new(128, 0, 128);

    /// <summary>
    /// Red channel (0-255).
    /// </summary>
    public byte R { get; }

    /// <summary>
    /// Green channel (0-255).
    /// </summary>
    public byte G { get; }

    /// <summary>
    /// Blue channel (0-255).
    /// </summary>
    public byte B { get; }

    /// <summary>
    /// Alpha (opacity) channel (0-255).
    /// 0 is fully transparent; 255 is fully opaque. Higher values mean less transparency.
    /// </summary>
    public byte A { get; }

    public Color(byte r, byte g, byte b, byte a = 255)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    /// <summary>
    /// Converts this color to a hex string using the specified component order.
    /// Alpha inclusion is controlled by <paramref name="alphaInclusion"/>.
    /// </summary>
    public string ToString(ColorStringFormat format, ColorAlphaInclusion alphaInclusion = ColorAlphaInclusion.WhenNotOpaque)
    {
        bool emitAlpha = alphaInclusion switch
        {
            ColorAlphaInclusion.Always => true,
            ColorAlphaInclusion.Never => false,
            ColorAlphaInclusion.WhenNotOpaque => A != 255,
            _ => throw new ArgumentException($"Unsupported alpha inclusion option: {alphaInclusion}")
        };

        return format switch
        {
            ColorStringFormat.RGBA => emitAlpha
                ? $"#{R:X2}{G:X2}{B:X2}{A:X2}"
                : $"#{R:X2}{G:X2}{B:X2}",
            ColorStringFormat.ARGB => emitAlpha
                ? $"#{A:X2}{R:X2}{G:X2}{B:X2}"
                : $"#{R:X2}{G:X2}{B:X2}",
            _ => throw new ArgumentException($"Unsupported color format: {format}")
        };
    }

    /// <summary>
    /// Converts this color to a hex string in RGBA order, including alpha when the color is not fully opaque.
    /// </summary>
    public override string ToString() => ToString(ColorStringFormat.RGBA, ColorAlphaInclusion.WhenNotOpaque);

    /// <summary>
    /// Parses a color from a hex string in the specified format.
    /// Supports "#RRGGBB", "#RRGGBBAA" (RGBA) and "#AARRGGBB" (ARGB).
    /// When alpha is missing (e.g., "#RRGGBB"), uses <paramref name="defaultAlpha"/> when provided;
    /// otherwise falls back to format-specific defaults (<see cref="DefaultAlphaForRgba"/> or <see cref="DefaultAlphaForArgb"/>).
    /// </summary>
    /// <param name="hexString">Hex string beginning with '#'. Case-insensitive.</param>
    /// <param name="format">Component order to expect when parsing.</param>
    /// <param name="defaultAlpha">Optional default alpha (0-255) to use when not present in the string.</param>
    /// <exception cref="ArgumentException">Thrown when the string is invalid or not supported.</exception>
    public static Color Parse(string hexString, ColorStringFormat format = ColorStringFormat.RGBA, byte? defaultAlpha = null)
    {
        if (string.IsNullOrWhiteSpace(hexString))
            throw new ArgumentException("Color string cannot be null or whitespace.", nameof(hexString));

        return Parse(hexString.AsSpan(), format, defaultAlpha);
    }

    /// <summary>
    /// Parses a color from a hex span in the specified format.
    /// Supports "#RRGGBB", "#RRGGBBAA" (RGBA) and "#AARRGGBB" (ARGB).
    /// Also supports shorthand "#RGB" and "#RGBA" (or "#ARGB").
    /// When alpha is missing (e.g., "#RRGGBB"), uses <paramref name="defaultAlpha"/> when provided;
    /// otherwise falls back to format-specific defaults (<see cref="DefaultAlphaForRgba"/> or <see cref="DefaultAlphaForArgb"/>).
    /// No allocation; directly parses hex digits.
    /// </summary>
    /// <param name="hexString">Hex span beginning with '#'. Case-insensitive.</param>
    /// <param name="format">Component order to expect when parsing.</param>
    /// <param name="defaultAlpha">Optional default alpha (0-255) to use when not present in the span.</param>
    /// <exception cref="ArgumentException">Thrown when the span is invalid or not supported.</exception>
    public static Color Parse(ReadOnlySpan<char> hexString, ColorStringFormat format = ColorStringFormat.RGBA, byte? defaultAlpha = null)
    {
        // Remove leading '#' if present
        if (hexString.Length > 0 && hexString[0] == '#')
            hexString = hexString[1..];

        // Expand shorthand #RGB -> #RRGGBB or #RGBA -> #RRGGBBAA
        // Since we are operating on ReadOnlySpan, we can't mutate. 
        // We will handle shorthand logic by mapping indices virtually or parsing digits directly.
        // Or simpler: handle expansion logic inside the byte parsing flow if length is 3 or 4.
        
        bool isShorthand = hexString.Length == 3 || hexString.Length == 4;
        bool hasAlpha = hexString.Length == 4 || hexString.Length == 8;

        if (hexString.Length != 3 && hexString.Length != 4 && hexString.Length != 6 && hexString.Length != 8)
            throw new ArgumentException($"Unsupported hex length ({hexString.Length}). Use 3, 4, 6, or 8 characters.");

        byte ParseComponent(ReadOnlySpan<char> hex, int index)
        {
            if (isShorthand)
            {
                // Shorthand: Single digit becomes double (e.g., 'A' -> "AA" -> 0xAA)
                int digit = ParseHexDigit(hex[index]);
                if (digit < 0) throw new ArgumentException("Invalid hex digit.");
                return (byte)((digit << 4) | digit);
            }
            else
            {
                // Full: Two digits
                int high = ParseHexDigit(hex[index * 2]);
                int low = ParseHexDigit(hex[index * 2 + 1]);
                if (high < 0 || low < 0) throw new ArgumentException("Invalid hex digit.");
                return (byte)((high << 4) | low);
            }
        }

        byte r, g, b, a;

        switch (format)
        {
            case ColorStringFormat.RGBA:
                r = ParseComponent(hexString, 0);
                g = ParseComponent(hexString, 1);
                b = ParseComponent(hexString, 2);
                
                if (hasAlpha)
                    a = ParseComponent(hexString, 3);
                else
                    a = defaultAlpha ?? DefaultAlphaForRgba;
                break;

            case ColorStringFormat.ARGB:
                if (hasAlpha)
                {
                    a = ParseComponent(hexString, 0);
                    r = ParseComponent(hexString, 1);
                    g = ParseComponent(hexString, 2);
                    b = ParseComponent(hexString, 3);
                }
                else
                {
                    r = ParseComponent(hexString, 0);
                    g = ParseComponent(hexString, 1);
                    b = ParseComponent(hexString, 2);
                    a = defaultAlpha ?? DefaultAlphaForArgb;
                }
                break;

            default:
                throw new ArgumentException($"Unsupported color format: {format}");
        }

        return new Color(r, g, b, a);
    }

    /// <summary>
    /// Parses a single hexadecimal digit (0-9, A-F, a-f).
    /// Returns -1 if the character is not a valid hex digit.
    /// </summary>
    private static int ParseHexDigit(char c)
    {
        if (c >= '0' && c <= '9')
            return c - '0';
        if (c >= 'A' && c <= 'F')
            return c - 'A' + 10;
        if (c >= 'a' && c <= 'f')
            return c - 'a' + 10;
        return -1;
    }

    /// <summary>
    /// Attempts to parse a color from a hex string in the specified format.
    /// Supports "#RRGGBB", "#RRGGBBAA" (RGBA) and "#AARRGGBB" (ARGB).
    /// Also supports shorthand "#RGB" and "#RGBA" (or "#ARGB").
    /// When alpha is missing (e.g., "#RRGGBB"), uses <paramref name="defaultAlpha"/> when provided;
    /// otherwise falls back to format-specific defaults (<see cref="DefaultAlphaForRgba"/> or <see cref="DefaultAlphaForArgb"/>).
    /// Returns false if the string is invalid or parsing fails; does not throw.
    /// </summary>
    /// <param name="hexString">Hex string beginning with '#'. Case-insensitive.</param>
    /// <param name="color">The parsed color if successful; otherwise <see cref="Color.Black"/>.</param>
    /// <param name="format">Component order to expect when parsing.</param>
    /// <param name="defaultAlpha">Optional default alpha (0-255) to use when not present in the string.</param>
    /// <returns>True if parsing succeeded; false if the string is invalid or unsupported.</returns>
    public static bool TryParse(string hexString, out Color color, ColorStringFormat format = ColorStringFormat.RGBA, byte? defaultAlpha = null)
    {
        try
        {
            color = Parse(hexString, format, defaultAlpha);
            return true;
        }
        catch
        {
            color = default;
            return false;
        }
    }

    /// <summary>
    /// Attempts to parse a color from a hex span in the specified format.
    /// Supports "#RRGGBB", "#RRGGBBAA" (RGBA) and "#AARRGGBB" (ARGB).
    /// Also supports shorthand "#RGB" and "#RGBA" (or "#ARGB").
    /// When alpha is missing (e.g., "#RRGGBB"), uses <paramref name="defaultAlpha"/> when provided;
    /// otherwise falls back to format-specific defaults (<see cref="DefaultAlphaForRgba"/> or <see cref="DefaultAlphaForArgb"/>).
    /// Returns false if the span is invalid or parsing fails; does not throw. No allocation.
    /// </summary>
    /// <param name="hexString">Hex span beginning with '#'. Case-insensitive.</param>
    /// <param name="color">The parsed color if successful; otherwise <see cref="Color.Black"/>.</param>
    /// <param name="format">Component order to expect when parsing.</param>
    /// <param name="defaultAlpha">Optional default alpha (0-255) to use when not present in the span.</param>
    /// <returns>True if parsing succeeded; false if the span is invalid or unsupported.</returns>
    public static bool TryParse(ReadOnlySpan<char> hexString, out Color color, ColorStringFormat format = ColorStringFormat.RGBA, byte? defaultAlpha = null)
    {
        try
        {
            color = Parse(hexString, format, defaultAlpha);
            return true;
        }
        catch
        {
            color = default;
            return false;
        }
    }

    public bool Equals(Color other)
    {
        return R == other.R &&
               G == other.G &&
               B == other.B &&
               A == other.A;
    }

    /// <summary>
    /// Determines whether the specified object is equal to this color.
    /// </summary>
    public override bool Equals(object? obj)
    {
        return obj is Color color && Equals(color);
    }

    /// <summary>
    /// Returns a hash code for this color.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(R, G, B, A);
    }

    /// <summary>
    /// Determines whether two colors are equal (component-wise equality).
    /// </summary>
    public static bool operator ==(Color left, Color right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two colors are not equal.
    /// </summary>
    public static bool operator !=(Color left, Color right)
    {
        return !(left == right);
    }
}
