using System.Runtime.Versioning;
using BetterSearchBar.Core;
using Microsoft.Win32;

namespace BetterSearchBar.Tweaks;

[SupportedOSPlatform("windows")]
public class CloudSearchTweak : ITweak
{
    private const string MachineSearchPolicyKey = @"SOFTWARE\Policies\Microsoft\Windows\Windows Search";
    private const string CurrentUserSearchSettingsKey = @"Software\Microsoft\Windows\CurrentVersion\SearchSettings";

    public string Id => "cloud";
    public string Title => "Облачный поиск (Microsoft Account & AAD)";
    public string Description => "Отключает синхронизацию облачной истории поиска и интеграцию учетной записи Microsoft/Work в результатах поиска.";
    public bool RequiresElevation => false;

    public bool IsApplied()
    {
        var aad = RegistryHelper.GetDword(RegistryHive.CurrentUser, CurrentUserSearchSettingsKey, "IsAADCloudSearchEnabled");
        var msa = RegistryHelper.GetDword(RegistryHive.CurrentUser, CurrentUserSearchSettingsKey, "IsMSACloudSearchEnabled");
        var allowCloud = RegistryHelper.GetDword(RegistryHive.LocalMachine, MachineSearchPolicyKey, "AllowCloudSearch");

        return (aad == 0 && msa == 0) || allowCloud == 0;
    }

    public void Apply()
    {
        RegistryHelper.SetDword(RegistryHive.CurrentUser, CurrentUserSearchSettingsKey, "IsAADCloudSearchEnabled", 0);
        RegistryHelper.SetDword(RegistryHive.CurrentUser, CurrentUserSearchSettingsKey, "IsMSACloudSearchEnabled", 0);

        try
        {
            RegistryHelper.SetDword(RegistryHive.LocalMachine, MachineSearchPolicyKey, "AllowCloudSearch", 0);
        }
        catch (UnauthorizedAccessException)
        {
            // Allowed for non-elevated user
        }
    }

    public void Revert()
    {
        RegistryHelper.SetDword(RegistryHive.CurrentUser, CurrentUserSearchSettingsKey, "IsAADCloudSearchEnabled", 1);
        RegistryHelper.SetDword(RegistryHive.CurrentUser, CurrentUserSearchSettingsKey, "IsMSACloudSearchEnabled", 1);

        try
        {
            RegistryHelper.DeleteValue(RegistryHive.LocalMachine, MachineSearchPolicyKey, "AllowCloudSearch");
        }
        catch (UnauthorizedAccessException)
        {
            // Allowed for non-elevated user
        }
    }
}
