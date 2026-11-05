using System.Runtime.Versioning;
using BetterSearchBar.Core;
using Microsoft.Win32;

namespace BetterSearchBar.Tweaks;

[SupportedOSPlatform("windows")]
public class QuickSearchesTweak : ITweak
{
    private const string UserExplorerPolicyKey = @"Software\Policies\Microsoft\Windows\Explorer";
    private const string MachineExplorerPolicyKey = @"SOFTWARE\Policies\Microsoft\Windows\Explorer";
    private const string MachineSearchPolicyKey = @"SOFTWARE\Policies\Microsoft\Windows\Windows Search";

    public string Id => "quicksearches";
    public string Title => "Блок «Запросы для быстрого поиска» (предложения)";
    public string Description => "Убирает список поисковых подсказок быстрых настроек и предложений на стартовом экране поиска.";
    public bool RequiresElevation => false;

    public bool IsApplied()
    {
        var userSuggestions = RegistryHelper.GetDword(RegistryHive.CurrentUser, UserExplorerPolicyKey, "DisableSearchBoxSuggestions");
        var machineSuggestions = RegistryHelper.GetDword(RegistryHive.LocalMachine, MachineExplorerPolicyKey, "DisableSearchBoxSuggestions");

        return userSuggestions == 1 || machineSuggestions == 1;
    }

    public void Apply()
    {
        RegistryHelper.SetDword(RegistryHive.CurrentUser, UserExplorerPolicyKey, "DisableSearchBoxSuggestions", 1);

        try
        {
            RegistryHelper.SetDword(RegistryHive.LocalMachine, MachineExplorerPolicyKey, "DisableSearchBoxSuggestions", 1);
            RegistryHelper.SetDword(RegistryHive.LocalMachine, MachineSearchPolicyKey, "DoNotUseWebResults", 1);
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

    public void Revert()
    {
        RegistryHelper.DeleteValue(RegistryHive.CurrentUser, UserExplorerPolicyKey, "DisableSearchBoxSuggestions");

        try
        {
            RegistryHelper.DeleteValue(RegistryHive.LocalMachine, MachineExplorerPolicyKey, "DisableSearchBoxSuggestions");
            RegistryHelper.DeleteValue(RegistryHive.LocalMachine, MachineSearchPolicyKey, "DoNotUseWebResults");
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
