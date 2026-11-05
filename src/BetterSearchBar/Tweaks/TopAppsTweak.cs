using System.Runtime.Versioning;
using BetterSearchBar.Core;
using Microsoft.Win32;

namespace BetterSearchBar.Tweaks;

[SupportedOSPlatform("windows")]
public class TopAppsTweak : ITweak
{
    private const string SearchSettingsKey = @"Software\Microsoft\Windows\CurrentVersion\SearchSettings";
    private const string ExplorerAdvancedKey = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
    private const string UserExplorerPolicyKey = @"Software\Policies\Microsoft\Windows\Explorer";
    private const string MachineExplorerPolicyKey = @"SOFTWARE\Policies\Microsoft\Windows\Explorer";
    private const string JumplistDataKey = @"Software\Microsoft\Windows\CurrentVersion\Search\JumplistData";

    public string Id => "topapps";
    public string Title => "Блок «Главные приложения» и история запусков";
    public string Description => "Убирает список часто используемых и недавних программ из меню поиска и отключает отслеживание их запуска.";
    public bool RequiresElevation => false;

    public bool IsApplied()
    {
        var deviceHistory = RegistryHelper.GetDword(RegistryHive.CurrentUser, SearchSettingsKey, "IsDeviceSearchHistoryEnabled");
        var trackProgs = RegistryHelper.GetDword(RegistryHive.CurrentUser, ExplorerAdvancedKey, "Start_TrackProgs");
        var noInstrumentation = RegistryHelper.GetDword(RegistryHive.CurrentUser, UserExplorerPolicyKey, "NoInstrumentation");

        return deviceHistory == 0 && (trackProgs == 0 || noInstrumentation == 1);
    }

    public void Apply()
    {
        RegistryHelper.SetDword(RegistryHive.CurrentUser, SearchSettingsKey, "IsDeviceSearchHistoryEnabled", 0);
        RegistryHelper.SetDword(RegistryHive.CurrentUser, ExplorerAdvancedKey, "Start_TrackProgs", 0);
        RegistryHelper.SetDword(RegistryHive.CurrentUser, UserExplorerPolicyKey, "NoInstrumentation", 1);
        RegistryHelper.SetDword(RegistryHive.CurrentUser, UserExplorerPolicyKey, "ShowMostUsedApps", 0);

        try
        {
            RegistryHelper.SetDword(RegistryHive.LocalMachine, MachineExplorerPolicyKey, "NoInstrumentation", 1);
            RegistryHelper.SetDword(RegistryHive.LocalMachine, MachineExplorerPolicyKey, "ShowMostUsedApps", 0);
        }
        catch (UnauthorizedAccessException)
        {
        }

        RegistryHelper.ClearSubKeyValues(RegistryHive.CurrentUser, JumplistDataKey);
    }

    public void Revert()
    {
        RegistryHelper.SetDword(RegistryHive.CurrentUser, SearchSettingsKey, "IsDeviceSearchHistoryEnabled", 1);
        RegistryHelper.SetDword(RegistryHive.CurrentUser, ExplorerAdvancedKey, "Start_TrackProgs", 1);
        RegistryHelper.DeleteValue(RegistryHive.CurrentUser, UserExplorerPolicyKey, "NoInstrumentation");
        RegistryHelper.DeleteValue(RegistryHive.CurrentUser, UserExplorerPolicyKey, "ShowMostUsedApps");

        try
        {
            RegistryHelper.DeleteValue(RegistryHive.LocalMachine, MachineExplorerPolicyKey, "NoInstrumentation");
            RegistryHelper.DeleteValue(RegistryHive.LocalMachine, MachineExplorerPolicyKey, "ShowMostUsedApps");
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
