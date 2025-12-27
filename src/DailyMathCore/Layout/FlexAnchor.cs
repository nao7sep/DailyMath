using System;

namespace DailyMathCore.Layout;

/// <summary>
/// Represents a pointer to a specific location relative to an element's anchor point.
/// 
/// Terminology:
/// Reference Size: For a FlexAnchor, the reference size used to resolve its Offset percentages 
/// is either the element's Size or its Container, depending on how the anchor was created.
/// </summary>
public readonly struct FlexAnchor
{
    public FlexElement Element { get; }
    public Alignment Alignment { get; }
    public FlexPoint Offset { get; }
    public bool IsContainerRelative { get; }

    public FlexAnchor(FlexElement element, Alignment alignment, bool isContainerRelative = false) 
        : this(element, alignment, FlexPoint.Zero, isContainerRelative) { }

    public FlexAnchor(FlexElement element, Alignment alignment, FlexPoint offset, bool isContainerRelative)
    {
        Element = element;
        Alignment = alignment;
        Offset = offset;
        IsContainerRelative = isContainerRelative;
    }

    #region Conversion Methods

    /// <summary>
    /// Resolves the anchor into an absolute pixel-based position.
    /// </summary>
    public PxPoint ToPxPoint()
    {
        var px = Element.ToPxElement();
        double x = px.Position.X;
        double y = px.Position.Y;
        double w = px.Size.Width;
        double h = px.Size.Height;

        if (IsContainerRelative)
        {
            x += px.Padding.Left;
            y += px.Padding.Top;
            w -= px.Padding.Left + px.Padding.Right;
            h -= px.Padding.Top + px.Padding.Bottom;
        }

        switch (Alignment)
        {
            case Alignment.TopCenter:
            case Alignment.MiddleCenter:
            case Alignment.BottomCenter:
                x += w / 2;
                break;
            case Alignment.TopRight:
            case Alignment.MiddleRight:
            case Alignment.BottomRight:
                x += w;
                break;
        }

        switch (Alignment)
        {
            case Alignment.MiddleLeft:
            case Alignment.MiddleCenter:
            case Alignment.MiddleRight:
                y += h / 2;
                break;
            case Alignment.BottomLeft:
            case Alignment.BottomCenter:
            case Alignment.BottomRight:
                y += h;
                break;
        }

        var pxOffset = Offset.ToPxPoint(Element.Dpi, w, h);
        
        return new PxPoint(x + pxOffset.X, y + pxOffset.Y);
    }

    #endregion

    #region Operators

    public static FlexAnchor operator +(FlexAnchor anchor, FlexPoint offset)
    {
        return new FlexAnchor(anchor.Element, anchor.Alignment, anchor.Offset + offset, anchor.IsContainerRelative);
    }

    public static FlexAnchor operator -(FlexAnchor anchor, FlexPoint offset)
    {
        return new FlexAnchor(anchor.Element, anchor.Alignment, anchor.Offset - offset, anchor.IsContainerRelative);
    }

    #endregion

    #region Overrides

    public override string ToString() => ToString(null);

    public string ToString(string? format)
    {
        string fmt = format ?? LayoutConstants.DefaultNumericFormat;
        var culture = System.Globalization.CultureInfo.InvariantCulture;

        string ex, ey, ew, eh, epl, ept, epr, epb;
        
        // ToString should not throw to ensure it remains reliable for debuggers and logging
        // even if the layout context (DPI, Root Size, etc.) is not yet valid.
        try
        {
            var px = Element.ToPxElement();
            ex = px.Position.X.ToString(fmt, culture);
            ey = px.Position.Y.ToString(fmt, culture);
            ew = px.Size.Width.ToString(fmt, culture);
            eh = px.Size.Height.ToString(fmt, culture);
            epl = px.Padding.Left.ToString(fmt, culture);
            ept = px.Padding.Top.ToString(fmt, culture);
            epr = px.Padding.Right.ToString(fmt, culture);
            epb = px.Padding.Bottom.ToString(fmt, culture);
        }
        catch
        {
            ex = ey = ew = eh = epl = ept = epr = epb = "?";
        }

        string alignmentStr = LayoutEnumConverter.ToString(Alignment);
        string ox = Offset.X.ToString(format);
        string oy = Offset.Y.ToString(format);
        string cr = IsContainerRelative ? "True" : "False";

        return $"ex:{ex}, ey:{ey}, ew:{ew}, eh:{eh}, epl:{epl}, ept:{ept}, epr:{epr}, epb:{epb}, {alignmentStr}, x:{ox}, y:{oy}, cr:{cr}";
    }

    #endregion
}