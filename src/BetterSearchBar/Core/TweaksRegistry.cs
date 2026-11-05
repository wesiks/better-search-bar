using System.Runtime.Versioning;
using BetterSearchBar.Tweaks;

namespace BetterSearchBar.Core;

[SupportedOSPlatform("windows")]
public static class TweaksRegistry
{
    private static readonly List<ITweak> _tweaks =
    [
        new AdsTweak(),
        new SearchHighlightsTweak(),
        new BingSearchTweak(),
        new CloudSearchTweak()
    ];

    public static IReadOnlyList<ITweak> All => _tweaks;

    public static ITweak? GetById(string id) =>
        _tweaks.FirstOrDefault(t => string.Equals(t.Id, id, StringComparison.OrdinalIgnoreCase));

    public static void ApplyAll()
    {
        foreach (var tweak in _tweaks)
        {
            tweak.Apply();
        }
    }

    public static void RevertAll()
    {
        foreach (var tweak in _tweaks)
        {
            tweak.Revert();
        }
    }

    public static void ApplySelected(IEnumerable<ITweak> selected)
    {
        var selectedList = selected.ToList();
        var selectedIds = new HashSet<string>(selectedList.Select(t => t.Id), StringComparer.OrdinalIgnoreCase);

        foreach (var tweak in _tweaks)
        {
            if (selectedIds.Contains(tweak.Id))
            {
                tweak.Apply();
            }
            else
            {
                tweak.Revert();
            }
        }
    }
}

