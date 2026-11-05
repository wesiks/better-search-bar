using System.Runtime.Versioning;
using BetterSearchBar.Core;
using Microsoft.Win32;

namespace BetterSearchBar.Tweaks;

[SupportedOSPlatform("windows")]
public class AdsTweak : ITweak
{
    private const string ContentDeliveryKey = @"Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager";
    private const string CloudContentPolicyKey = @"SOFTWARE\Policies\Microsoft\Windows\CloudContent";

    public string Id => "ads";
    public string Title => "Реклама и промо-игры («Игры для вас», карточки)";
    public string Description => "Отключает предложения спонсируемых игр, рекламные карточки и автоустановку мусорных приложений в меню поиска и системы.";
    public bool RequiresElevation => false;

    public bool IsApplied()
    {
        var suggestions = RegistryHelper.GetDword(RegistryHive.CurrentUser, ContentDeliveryKey, "SystemPaneSuggestionsEnabled");
        var sub338388 = RegistryHelper.GetDword(RegistryHive.CurrentUser, ContentDeliveryKey, "SubscribedContent-338388Enabled");
        var sub338389 = RegistryHelper.GetDword(RegistryHive.CurrentUser, ContentDeliveryKey, "SubscribedContent-338389Enabled");

        return suggestions == 0 && (sub338388 == 0 || sub338389 == 0);
    }

    public void Apply()
    {
        RegistryHelper.SetDword(RegistryHive.CurrentUser, ContentDeliveryKey, "SystemPaneSuggestionsEnabled", 0);
        RegistryHelper.SetDword(RegistryHive.CurrentUser, ContentDeliveryKey, "SubscribedContent-338388Enabled", 0);
        RegistryHelper.SetDword(RegistryHive.CurrentUser, ContentDeliveryKey, "SubscribedContent-338389Enabled", 0);
        RegistryHelper.SetDword(RegistryHive.CurrentUser, ContentDeliveryKey, "SilentInstalledAppsEnabled", 0);

        try
        {
            RegistryHelper.SetDword(RegistryHive.LocalMachine, CloudContentPolicyKey, "DisableWindowsConsumerFeatures", 1);
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

    public void Revert()
    {
        RegistryHelper.SetDword(RegistryHive.CurrentUser, ContentDeliveryKey, "SystemPaneSuggestionsEnabled", 1);
        RegistryHelper.SetDword(RegistryHive.CurrentUser, ContentDeliveryKey, "SubscribedContent-338388Enabled", 1);
        RegistryHelper.SetDword(RegistryHive.CurrentUser, ContentDeliveryKey, "SubscribedContent-338389Enabled", 1);
        RegistryHelper.SetDword(RegistryHive.CurrentUser, ContentDeliveryKey, "SilentInstalledAppsEnabled", 1);

        try
        {
            RegistryHelper.DeleteValue(RegistryHive.LocalMachine, CloudContentPolicyKey, "DisableWindowsConsumerFeatures");
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
