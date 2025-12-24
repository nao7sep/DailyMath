using System.Drawing;

namespace DailyMath.Core.Layout;

/// <summary>
/// Contains debug visualization settings for an element.
/// Used to help visualize element boundaries and labels during development.
/// </summary>
public class DebugInfo
{
    /// <summary>
    /// Gets or sets a label to display with the debug visualization.
    /// Null if no label is set.
    /// </summary>
    public string? Label { get; set; }

    /// <summary>
    /// Gets or sets when the debug visualization (border and label) should be visible.
    /// </summary>
    public DebugVisibility Visibility { get; set; } = DebugVisibility.IfRequested;

    /// <summary>
    /// Gets or sets the color of the debug border.
    /// Null if no color is set.
    /// </summary>
    public Color? BorderColor { get; set; }
}
