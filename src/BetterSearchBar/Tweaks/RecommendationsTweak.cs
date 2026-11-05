using System.Runtime.Versioning;
using BetterSearchBar.Core;
using Microsoft.Win32;

namespace BetterSearchBar.Tweaks;

[SupportedOSPlatform("windows")]
public class RecommendationsTweak : ITweak
{
    private const string UserExplorerPolicyKey = @"Software\Policies\Microsoft\Windows\Explorer";
    private const string MachineExplorerPolicyKey = @"SOFTWARE\Policies\Microsoft\Windows\Explorer";
    private const string ExplorerAdvancedKey = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
    private const string ContentDeliveryKey = @"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager";

    public string Id => "recommendations";
    public string Title => "Блок «Рекомендации» (советы, ярлыки, настройки)";
    public string Description => "Отключает блок «Рекомендации» в меню Пуск и поиске (советы, ярлыки, настройки и рекомендуемые приложения).";
    public bool RequiresElevation => false;

    public bool IsApplied()
    {
        var hideRec = RegistryHelper.GetDword(RegistryHive.CurrentUser, UserExplorerPolicyKey, "HideRecommendedSection");
        var iris = RegistryHelper.GetDword(RegistryHive.CurrentUser, ExplorerAdvancedKey, "Start_IrisRecommendations");
        var sub353698 = RegistryHelper.GetDword(RegistryHive.CurrentUser, ContentDeliveryKey, "SubscribedContent-353698Enabled");

        return hideRec == 1 && iris == 0 && sub353698 == 0;
    }

    public void Apply()
    {
        RegistryHelper.SetDword(RegistryHive.CurrentUser, UserExplorerPolicyKey, "HideRecommendedSection", 1);
        RegistryHelper.SetDword(RegistryHive.CurrentUser, UserExplorerPolicyKey, "NoRecentDocsHistory", 1);

        RegistryHelper.SetDword(RegistryHive.CurrentUser, ExplorerAdvancedKey, "Start_IrisRecommendations", 0);
        RegistryHelper.SetDword(RegistryHive.CurrentUser, ExplorerAdvancedKey, "Start_TrackDocs", 0);
        RegistryHelper.SetDword(RegistryHive.CurrentUser, ExplorerAdvancedKey, "Start_AccountNotifications", 0);
        RegistryHelper.SetDword(RegistryHive.CurrentUser, ExplorerAdvancedKey, "Start_Layout", 1);

        RegistryHelper.SetDword(RegistryHive.CurrentUser, ContentDeliveryKey, "SubscribedContent-353698Enabled", 0);
        RegistryHelper.SetDword(RegistryHive.CurrentUser, ContentDeliveryKey, "SubscribedContent-353694Enabled", 0);
        RegistryHelper.SetDword(RegistryHive.CurrentUser, ContentDeliveryKey, "SubscribedContent-353696Enabled", 0);

        try
        {
            RegistryHelper.SetDword(RegistryHive.LocalMachine, MachineExplorerPolicyKey, "HideRecommendedSection", 1);
            RegistryHelper.SetDword(RegistryHive.LocalMachine, MachineExplorerPolicyKey, "NoRecentDocsHistory", 1);
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

    public void Revert()
    {
        RegistryHelper.DeleteValue(RegistryHive.CurrentUser, UserExplorerPolicyKey, "HideRecommendedSection");
        RegistryHelper.DeleteValue(RegistryHive.CurrentUser, UserExplorerPolicyKey, "NoRecentDocsHistory");

        RegistryHelper.SetDword(RegistryHive.CurrentUser, ExplorerAdvancedKey, "Start_IrisRecommendations", 1);
        RegistryHelper.SetDword(RegistryHive.CurrentUser, ExplorerAdvancedKey, "Start_TrackDocs", 1);
        RegistryHelper.SetDword(RegistryHive.CurrentUser, ExplorerAdvancedKey, "Start_AccountNotifications", 1);
        RegistryHelper.DeleteValue(RegistryHive.CurrentUser, ExplorerAdvancedKey, "Start_Layout");

        RegistryHelper.SetDword(RegistryHive.CurrentUser, ContentDeliveryKey, "SubscribedContent-353698Enabled", 1);
        RegistryHelper.SetDword(RegistryHive.CurrentUser, ContentDeliveryKey, "SubscribedContent-353694Enabled", 1);
        RegistryHelper.SetDword(RegistryHive.CurrentUser, ContentDeliveryKey, "SubscribedContent-353696Enabled", 1);

        try
        {
            RegistryHelper.DeleteValue(RegistryHive.LocalMachine, MachineExplorerPolicyKey, "HideRecommendedSection");
            RegistryHelper.DeleteValue(RegistryHive.LocalMachine, MachineExplorerPolicyKey, "NoRecentDocsHistory");
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}

