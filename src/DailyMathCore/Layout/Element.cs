namespace DailyMath.Core.Layout;

/// <summary>
/// Represents a general-purpose layout element that can contain children.
/// Supports flexible positioning via alignment, sizing in various units (including percentages),
/// and a proper box model (margin outside, padding inside).
///
/// Design Philosophy:
/// - Lazy calculation: GetAbsoluteRegion() calculates positions on-demand (not during construction).
/// - Suitable for deterministic print layouts where paper dimensions are known upfront.
/// - Supports adaptive layout selection (CSS @media-style): build different structures based on paper size,
///   e.g., BuildWorksheet(paperWidth, paperHeight) can choose 2 vs 3 columns based on dimensions.
/// - Calculations are cheap enough to rebuild entire hierarchies for different paper sizes.
/// </summary>
public class Element
{
    // Hierarchy

    /// <summary>
    /// Gets the parent element, or null if this is a root element.
    /// </summary>
    public Element? Parent { get; private set; }

    private readonly List<Element> _children = new();

    /// <summary>
    /// Gets the read-only list of child elements.
    /// </summary>
    public IReadOnlyList<Element> Children => _children;

    // Layout Definitions

    /// <summary>
    /// Gets or sets how this element is aligned within its parent's content area.
    /// </summary>
    public Alignment Alignment { get; set; } = Alignment.TopLeft;

    /// <summary>
    /// Gets or sets the margin (space outside this element).
    /// Which margins are used depends on the Alignment setting.
    /// </summary>
    public Inset Margin { get; set; } = Inset.Zero;

    /// <summary>
    /// Gets or sets the desired size of this element.
    /// Ignored if Alignment is set to Fill.
    /// </summary>
    public Measure Size { get; set; }

    /// <summary>
    /// Gets or sets the padding (space inside this element).
    /// Padding defines the content area for child elements.
    /// </summary>
    public Inset Padding { get; set; } = Inset.Zero;

    // Context

    /// <summary>
    /// Gets or sets the DPI override for this element.
    /// If null, uses the parent's effective DPI.
    /// </summary>
    public double? Dpi { get; set; }

    /// <summary>
    /// Gets the default DPI used when no element in the hierarchy specifies a DPI.
    /// Set to 96.0, which is the standard Windows screen DPI where 1 CSS pixel = 1 device pixel,
    /// and 1 point = 1.333... pixels (72 points per inch). This makes screen preview rendering
    /// work in the most intuitive way before final print rendering at higher DPI.
    /// </summary>
    public static double DefaultDpi { get; } = 96.0;

    // Debug

    /// <summary>
    /// Gets the debug visualization settings for this element.
    /// </summary>
    public DebugInfo Debug { get; } = new();

    // Hierarchy Management

    /// <summary>
    /// Adds a child element to this element.
    /// Automatically removes the child from its previous parent if it had one.
    /// </summary>
    public void AddChild(Element child)
    {
        if (child.Parent != null)
            child.Parent.RemoveChild(child);

        child.Parent = this;
        _children.Add(child);
    }

    /// <summary>
    /// Removes a child element from this element.
    /// </summary>
    public void RemoveChild(Element child)
    {
        if (_children.Contains(child))
        {
            child.Parent = null;
            _children.Remove(child);
        }
    }

    // Core Calculation

    /// <summary>
    /// Gets the effective DPI by walking up the parent chain until a non-null DPI is found.
    /// Returns DefaultDpi if no element in the hierarchy specifies a DPI.
    /// </summary>
    public double GetEffectiveDpi()
    {
        if (Dpi.HasValue)
            return Dpi.Value;

        return Parent?.GetEffectiveDpi() ?? DefaultDpi;
    }

    /// <summary>
    /// Calculates and returns the absolute position and size of this element in pixels.
    /// Uses recursive calculation based on parent's region and this element's layout properties.
    /// Ensures pixel-perfect positioning with no rounding gaps between parent and child.
    /// </summary>
    public Region GetAbsoluteRegion()
    {
        double dpi = GetEffectiveDpi();

        // Root case: No parent, positioned at origin with strict size
        if (Parent == null)
        {
            double width = Size.Width.ToPixels(dpi);
            double height = Size.Height.ToPixels(dpi);
            return new Region(0, 0, width, height);
        }

        // Get parent's absolute region
        Region parentRegion = Parent.GetAbsoluteRegion();

        // Calculate parent's content box (total area minus padding)
        // This is the box model: padding defines the content area for children
        double parentPadLeft = Parent.Padding.Left.ToPixels(dpi, parentRegion.Width);
        double parentPadRight = Parent.Padding.Right.ToPixels(dpi, parentRegion.Width);
        double parentPadTop = Parent.Padding.Top.ToPixels(dpi, parentRegion.Height);
        double parentPadBottom = Parent.Padding.Bottom.ToPixels(dpi, parentRegion.Height);

        double parentContentWidth = parentRegion.Width - parentPadLeft - parentPadRight;
        double parentContentHeight = parentRegion.Height - parentPadTop - parentPadBottom;

        // Absolute position of parent's content area
        double contentStartX = parentRegion.Left + parentPadLeft;
        double contentStartY = parentRegion.Top + parentPadTop;

        // Calculate margins relative to parent's content box
        double marginLeft = Margin.Left.ToPixels(dpi, parentContentWidth);
        double marginRight = Margin.Right.ToPixels(dpi, parentContentWidth);
        double marginTop = Margin.Top.ToPixels(dpi, parentContentHeight);
        double marginBottom = Margin.Bottom.ToPixels(dpi, parentContentHeight);

        // Calculate this element's edges directly
        double left;
        double top;
        double right;
        double bottom;

        if (Alignment == Alignment.Fill)
        {
            // Fill mode: Ignore Size, stretch to fill parent's content area
            // but respect this element's own margins
            left = contentStartX + marginLeft;
            top = contentStartY + marginTop;
            right = contentStartX + parentContentWidth - marginRight;
            bottom = contentStartY + parentContentHeight - marginBottom;
        }
        else
        {
            // Normal sizing: Use Size property to calculate dimensions
            double elementWidth = Size.Width.ToPixels(dpi, parentContentWidth);
            double elementHeight = Size.Height.ToPixels(dpi, parentContentHeight);

            // Horizontal positioning based on alignment
            switch (Alignment)
            {
                case Alignment.TopLeft:
                case Alignment.BottomLeft:
                    left = contentStartX + marginLeft;
                    right = left + elementWidth;
                    break;

                case Alignment.TopRight:
                case Alignment.BottomRight:
                    // Anchor to right: Parent's right edge minus margin
                    right = contentStartX + parentContentWidth - marginRight;
                    left = right - elementWidth;
                    break;

                default:
                    left = contentStartX + marginLeft;
                    right = left + elementWidth;
                    break;
            }

            // Vertical positioning based on alignment
            switch (Alignment)
            {
                case Alignment.TopLeft:
                case Alignment.TopRight:
                    top = contentStartY + marginTop;
                    bottom = top + elementHeight;
                    break;

                case Alignment.BottomLeft:
                case Alignment.BottomRight:
                    // Anchor to bottom: Parent's bottom edge minus margin
                    bottom = contentStartY + parentContentHeight - marginBottom;
                    top = bottom - elementHeight;
                    break;

                default:
                    top = contentStartY + marginTop;
                    bottom = top + elementHeight;
                    break;
            }
        }

        return new Region(left, top, right, bottom);
    }

    /// <summary>
    /// Returns a string representation of the element's key properties.
    /// </summary>
    /// <param name="format">The numeric format string for sizes (e.g., "0.##", "F1").</param>
    /// <param name="includeTypeName">If true, prefixes the result with "Element: ".</param>
    /// <returns>A string representation of the element.</returns>
    public string ToString(string format, bool includeTypeName = false)
    {
        string alignment = AlignmentToString();
        string margin = Margin.ToString(format);
        string size = Size.ToString(format);
        string padding = Padding.ToString(format);
        return $"{(includeTypeName ? "Element: " : "")}{alignment}, {margin} ({size}) {padding}";
    }

    /// <summary>
    /// Returns a string representation of the element's key properties.
    /// </summary>
    public override string ToString() => ToString("0.##", false);

    /// <summary>
    /// Converts the alignment to a string, validating it is a defined enum value.
    /// </summary>
    private string AlignmentToString()
    {
        if (!Enum.IsDefined(typeof(Alignment), Alignment))
            throw new InvalidOperationException($"Invalid Alignment value: {Alignment}");
        return Alignment.ToString();
    }
}
