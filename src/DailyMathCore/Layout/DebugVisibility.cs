namespace DailyMath.Core.Layout;

/// <summary>
/// Specifies when debug visualization (borders and labels) should be rendered for an element.
/// </summary>
/// <remarks>
/// <para><b>Typical Development Workflow:</b></para>
/// <para>
/// 1. Start with <see cref="IfRequested"/> (default) - set a border color or label and enable
///    debug rendering globally to verify the element is positioned and sized correctly.
/// </para>
/// <para>
/// 2. Once verified, change to <see cref="Never"/> if you're confident the element is correct
///    and won't need further debugging. This keeps debug rendering fast even when enabled globally.
/// </para>
/// <para>
/// 3. Use <see cref="Always"/> for elements that are still being adjusted or have uncertain
///    positioning/sizing. These will show even if global debug rendering is off, making them
///    easy to track while iterating on the layout.
/// </para>
/// <para>
/// 4. Final cleanup: Comment out the debug rendering method call entirely. This is a single
///    line change and removes all debug overhead.
/// </para>
/// </remarks>
public enum DebugVisibility
{
    /// <summary>
    /// Show debug visualization only when debug rendering is explicitly requested.
    /// This is the recommended default for most elements during development.
    /// </summary>
    IfRequested,

    /// <summary>
    /// Never show debug visualization, even when debug rendering is requested.
    /// Use this for elements that have been tested and verified correct,
    /// so they don't clutter the debug view while testing other elements.
    /// </summary>
    Never,

    /// <summary>
    /// Always show debug visualization, regardless of debug rendering settings.
    /// Use this for elements with uncertain positioning or sizing that need
    /// continuous visibility while iterating on the layout.
    /// </summary>
    Always
}
