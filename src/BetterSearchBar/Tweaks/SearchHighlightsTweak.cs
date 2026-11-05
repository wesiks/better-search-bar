using System.Runtime.Versioning;
using BetterSearchBar.Core;
using Microsoft.Win32;

namespace BetterSearchBar.Tweaks;

[SupportedOSPlatform("windows")]
public class SearchHighlightsTweak : ITweak
{
    private const string SearchSettingsKey = @"Software\Microsoft\Windows\CurrentVersion\SearchSettings";
    private const string WindowsSearchPolicyKey = @"SOFTWARE\Policies\Microsoft\Windows\Windows Search";

    public string Id => "highlights";
    public string Title => "Search Highlights («Изображение дня», дудлы Bing)";
    public string Description => "Убирает ежедневные картинки Bing, исторические факты, тренды и анимированные дудлы из строки и окна поиска.";
    public bool RequiresElevation => false;

    public bool IsApplied()
    {
        var dynamicBox = RegistryHelper.GetDword(RegistryHive.CurrentUser, SearchSettingsKey, "IsDynamicSearchBoxEnabled");
        var policyDynamic = RegistryHelper.GetDword(RegistryHive.LocalMachine, WindowsSearchPolicyKey, "EnableDynamicContentInWSB");

        return dynamicBox == 0 || policyDynamic == 0;
    }

    public void Apply()
    {
        RegistryHelper.SetDword(RegistryHive.CurrentUser, SearchSettingsKey, "IsDynamicSearchBoxEnabled", 0);

        try
        {
            RegistryHelper.SetDword(RegistryHive.LocalMachine, WindowsSearchPolicyKey, "EnableDynamicContentInWSB", 0);
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

    public void Revert()
    {
        RegistryHelper.SetDword(RegistryHive.CurrentUser, SearchSettingsKey, "IsDynamicSearchBoxEnabled", 1);

        try
        {
            RegistryHelper.DeleteValue(RegistryHive.LocalMachine, WindowsSearchPolicyKey, "EnableDynamicContentInWSB");
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
