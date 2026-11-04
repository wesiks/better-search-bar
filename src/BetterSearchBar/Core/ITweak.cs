namespace BetterSearchBar.Core;

/// <summary>
/// Defines the contract for an individual Windows Search tweak.
/// </summary>
public interface ITweak
{
    /// <summary>
    /// Unique identifier for the tweak.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Human-readable title of the tweak.
    /// </summary>
    string Title { get; }

    /// <summary>
    /// Detailed description of what the tweak removes or configures.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Indicates whether applying this tweak requires Administrator privileges.
    /// </summary>
    bool RequiresElevation { get; }

    /// <summary>
    /// Checks whether the tweak is currently applied (i.e., clutter is disabled).
    /// </summary>
    bool IsApplied();

    /// <summary>
    /// Applies the tweak (disables clutter/ads/bing/highlights).
    /// </summary>
    void Apply();

    /// <summary>
    /// Reverts the tweak to standard Windows defaults.
    /// </summary>
    void Revert();
}
