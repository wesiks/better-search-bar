namespace BetterSearchBar.Core;

public interface ITweak
{
    string Id { get; }
    string Title { get; }
    string Description { get; }
    bool RequiresElevation { get; }

    bool IsApplied();
    void Apply();
    void Revert();
}
