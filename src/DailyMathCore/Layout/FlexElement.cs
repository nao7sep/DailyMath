using System;
using System.Collections.Generic;
using System.Globalization;

namespace DailyMathCore.Layout;

/// <summary>
/// The base class for all layout objects. Elements are layered, meaning children 
/// occupy the parent's container without pushing each other.
/// 
/// Terminology:
/// Size: The dimensions of the element, including Padding but excluding Margin.
/// Container: The parent element's Size minus its Padding. This defines the available area for the element's Margin and Size.
/// </summary>
public class FlexElement
{
    private PxElement? _pxCache;
    private Alignment _alignment = Alignment.TopLeft;
    private FlexInset _margin = FlexInset.Zero;
    private FlexLength? _width = null;
    private FlexLength? _height = null;
    private FlexInset _padding = FlexInset.Zero;

    public FlexElement? Parent { get; internal set; }
    public List<FlexElement> Children { get; } = new();

    public Alignment Alignment
    {
        get => _alignment;
        set { _alignment = value; InvalidateLayout(); }
    }

    public FlexInset Margin
    {
        get => _margin;
        set { _margin = value; InvalidateLayout(); }
    }

    public FlexLength? Width
    {
        get => _width;
        set { _width = value; InvalidateLayout(); }
    }

    public FlexLength? Height
    {
        get => _height;
        set { _height = value; InvalidateLayout(); }
    }

    public FlexInset Padding
    {
        get => _padding;
        set { _padding = value; InvalidateLayout(); }
    }

    public double Dpi
    {
        get
        {
            for (FlexElement? current = this; current != null; current = current.Parent)
            {
                if (current is FlexPage page && page.Dpi.HasValue)
                    return page.Dpi.Value;
            }

            throw new InvalidOperationException("DPI context not found. Ensure the root element has a DPI value set before resolving lengths.");
        }
    }

    public void InvalidateLayout()
    {
        _pxCache = null;
        foreach (var child in Children)
        {
            child.InvalidateLayout();
        }
    }

    public void AddChild(FlexElement child)
    {
        if (child == this)
            throw new InvalidOperationException("An element cannot be its own child.");
        
        if (child.Parent != null && child.Parent != this)
            throw new InvalidOperationException("Element already belongs to another parent.");

        // Circularity check
        for (FlexElement? current = this; current != null; current = current.Parent)
        {
            if (current == child)
                throw new InvalidOperationException("Cannot add child: Circular hierarchy detected.");
        }
        
        child.Parent = this;
        if (!Children.Contains(child))
        {
            Children.Add(child);
        }
        child.InvalidateLayout();
    }

    public void RemoveChild(FlexElement child)
    {
        if (Children.Remove(child))
        {
            child.Parent = null;
            child.InvalidateLayout();
        }
    }

    public FlexAnchor GetAnchor(Alignment alignment) => new FlexAnchor(this, alignment, isContainerRelative: false);

    public FlexAnchor GetContainerAnchor(Alignment alignment) => new FlexAnchor(this, alignment, isContainerRelative: true);

    public PxElement ToPxElement()
    {
        if (_pxCache.HasValue) return _pxCache.Value;

        double dpi = Dpi;

        if (Parent == null)
        {
            if (Margin != FlexInset.Zero)
                throw new InvalidOperationException("Root element cannot have a Margin.");

            if (Width == null || Height == null)
                throw new InvalidOperationException("Root element must have explicit Width and Height specified.");

            double w = Width.Value.ToPixels(dpi, null);
            double h = Height.Value.ToPixels(dpi, null);

            if (w < 0 || h < 0)
                throw new InvalidOperationException("Root element dimensions cannot be negative.");
            
            _pxCache = new PxElement(
                PxInset.Zero,
                PxPoint.Zero,
                new PxSize(w, h),
                Padding.ToPxInset(dpi, w, h));
        }
        else
        {
            var pPx = Parent.ToPxElement();
            double containerAbsX = pPx.Position.X + pPx.Padding.Left;
            double containerAbsY = pPx.Position.Y + pPx.Padding.Top;
            double cw = pPx.Size.Width - pPx.Padding.Left - pPx.Padding.Right;
            double ch = pPx.Size.Height - pPx.Padding.Top - pPx.Padding.Bottom;

            if (cw < -LayoutConstants.Epsilon || ch < -LayoutConstants.Epsilon)
            {
                throw new InvalidOperationException($"Container size cannot be negative. Parent Size ({FormatPx(pPx.Size.Width)}x{FormatPx(pPx.Size.Height)}) is smaller than its Padding ({FormatPx(pPx.Padding.Left + pPx.Padding.Right)}x{FormatPx(pPx.Padding.Top + pPx.Padding.Bottom)}).");
            }

            (double offsetX, double w, double resML, double resMR) = ResolveAxis(
                Width, Margin.Left, Margin.Right, cw, dpi,
                Alignment is Alignment.TopLeft or Alignment.MiddleLeft or Alignment.BottomLeft,
                Alignment is Alignment.TopCenter or Alignment.MiddleCenter or Alignment.BottomCenter,
                Alignment is Alignment.TopRight or Alignment.MiddleRight or Alignment.BottomRight);

            (double offsetY, double h, double resMT, double resMB) = ResolveAxis(
                Height, Margin.Top, Margin.Bottom, ch, dpi,
                Alignment is Alignment.TopLeft or Alignment.TopCenter or Alignment.TopRight,
                Alignment is Alignment.MiddleLeft or Alignment.MiddleCenter or Alignment.MiddleRight,
                Alignment is Alignment.BottomLeft or Alignment.BottomCenter or Alignment.BottomRight);

            _pxCache = new PxElement(
                new PxInset(resML, resMT, resMR, resMB),
                new PxPoint(containerAbsX + offsetX, containerAbsY + offsetY),
                new PxSize(w, h),
                Padding.ToPxInset(dpi, w, h));
        }

        return _pxCache.Value;
    }

    private static (double offset, double size, double resolvedMarginStart, double resolvedMarginEnd) ResolveAxis(
        FlexLength? requestedSize,
        FlexLength requestedMarginStart,
        FlexLength requestedMarginEnd,
        double containerSize,
        double dpi,
        bool isStartAligned,
        bool isCenterAligned,
        bool isEndAligned)
    {
        double ms = requestedMarginStart.ToPixels(dpi, containerSize);
        double me = requestedMarginEnd.ToPixels(dpi, containerSize);

        if (ms < -LayoutConstants.Epsilon || me < -LayoutConstants.Epsilon)
            throw new InvalidOperationException("Negative margins are not supported.");

        if (requestedSize == null) // "Fill" logic
        {
            double s = containerSize - ms - me;
            if (s < -LayoutConstants.Epsilon)
            {
                throw new InvalidOperationException($"Layout overflow: Margins ({FormatPx(ms + me)}px) exceed Container Size ({FormatPx(containerSize)}px) for filling element.");
            }
            return (ms, s, ms, me);
        }

        double size = requestedSize.Value.ToPixels(dpi, containerSize);
        if (size < -LayoutConstants.Epsilon)
            throw new InvalidOperationException("Element dimensions cannot be negative.");

        // Boundary check
        if (ms + size + me > containerSize + LayoutConstants.Epsilon)
        {
            throw new InvalidOperationException($"Layout overflow: Total size ({FormatPx(ms + size + me)}px) exceeds Container Size ({FormatPx(containerSize)}px).");
        }

        if (isStartAligned)
        {
            return (ms, size, ms, containerSize - ms - size);
        }
        
        if (isEndAligned)
        {
            double actualMS = containerSize - me - size;
            return (actualMS, size, actualMS, me);
        }

        // Center Aligned: Margins are completely ignored.
        double centeredMargin = (containerSize - size) / 2;
        return (centeredMargin, size, centeredMargin, centeredMargin);
    }

    private static string FormatPx(double value) => value.ToString(LayoutConstants.DefaultNumericFormat, CultureInfo.InvariantCulture);

    public (double MarginLeft, double MarginTop, double MarginRight, double MarginBottom,
            double X, double Y,
            double Width, double Height,
            double PaddingLeft, double PaddingTop, double PaddingRight, double PaddingBottom) 
        ToPixels()
    {
        var px = ToPxElement();
        return (px.Margin.Left, px.Margin.Top, px.Margin.Right, px.Margin.Bottom,
                px.Position.X, px.Position.Y,
                px.Size.Width, px.Size.Height,
                px.Padding.Left, px.Padding.Top, px.Padding.Right, px.Padding.Bottom);
    }

    public override string ToString() => ToString(null);

    public string ToString(string? format)
    {
        string wStr = Width?.ToString(format) ?? LayoutConstants.NullValueLiteral;
        string hStr = Height?.ToString(format) ?? LayoutConstants.NullValueLiteral;

        return $"{LayoutEnumConverter.ToString(Alignment)}, " +
               $"ML:{Margin.Left.ToString(format)}, MT:{Margin.Top.ToString(format)}, MR:{Margin.Right.ToString(format)}, MB:{Margin.Bottom.ToString(format)}, " +
               $"W:{wStr}, H:{hStr}, " +
               $"PL:{Padding.Left.ToString(format)}, PT:{Padding.Top.ToString(format)}, PR:{Padding.Right.ToString(format)}, PB:{Padding.Bottom.ToString(format)}";
    }
}
