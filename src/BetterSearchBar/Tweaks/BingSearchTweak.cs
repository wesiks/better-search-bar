using System.Runtime.Versioning;
using BetterSearchBar.Core;
using Microsoft.Win32;

namespace BetterSearchBar.Tweaks;

[SupportedOSPlatform("windows")]
public class BingSearchTweak : ITweak
{
    private const string CurrentUserSearchKey = @"Software\Microsoft\Windows\CurrentVersion\Search";
    private const string CurrentUserExplorerPolicyKey = @"Software\Policies\Microsoft\Windows\Explorer";
    private const string MachineSearchPolicyKey = @"SOFTWARE\Policies\Microsoft\Windows\Windows Search";

    public string Id => "bing";
    public string Title => "Веб-поиск Bing в строке поиска";
    public string Description => "Отключает результаты поиска из интернета и Bing в меню поиска Windows. Поиск будет работать мгновенно и только по локальным файлам и программам.";
    public bool RequiresElevation => false;

    public bool IsApplied()
    {
        var bingEnabled = RegistryHelper.GetDword(RegistryHive.CurrentUser, CurrentUserSearchKey, "BingSearchEnabled");
        var disableSuggestions = RegistryHelper.GetDword(RegistryHive.CurrentUser, CurrentUserExplorerPolicyKey, "DisableSearchBoxSuggestions");
        var disableWebSearch = RegistryHelper.GetDword(RegistryHive.LocalMachine, MachineSearchPolicyKey, "DisableWebSearch");

        return bingEnabled == 0 || disableSuggestions == 1 || disableWebSearch == 1;
    }

    public void Apply()
    {
        RegistryHelper.SetDword(RegistryHive.CurrentUser, CurrentUserSearchKey, "BingSearchEnabled", 0);
        RegistryHelper.SetDword(RegistryHive.CurrentUser, CurrentUserSearchKey, "CortanaConsent", 0);
        RegistryHelper.SetDword(RegistryHive.CurrentUser, CurrentUserExplorerPolicyKey, "DisableSearchBoxSuggestions", 1);

        try
        {
            RegistryHelper.SetDword(RegistryHive.LocalMachine, MachineSearchPolicyKey, "ConnectedSearchUseWeb", 0);
            RegistryHelper.SetDword(RegistryHive.LocalMachine, MachineSearchPolicyKey, "DisableWebSearch", 1);
        }
        catch (UnauthorizedAccessException)
        {
            // Allowed for non-elevated user
        }
    }

    public void Revert()
    {
        RegistryHelper.SetDword(RegistryHive.CurrentUser, CurrentUserSearchKey, "BingSearchEnabled", 1);
        RegistryHelper.SetDword(RegistryHive.CurrentUser, CurrentUserSearchKey, "CortanaConsent", 1);
        RegistryHelper.DeleteValue(RegistryHive.CurrentUser, CurrentUserExplorerPolicyKey, "DisableSearchBoxSuggestions");

        try
        {
            RegistryHelper.DeleteValue(RegistryHive.LocalMachine, MachineSearchPolicyKey, "ConnectedSearchUseWeb");
            RegistryHelper.DeleteValue(RegistryHive.LocalMachine, MachineSearchPolicyKey, "DisableWebSearch");
        }
        catch (UnauthorizedAccessException)
        {
            // Allowed for non-elevated user
        }
    }
}
