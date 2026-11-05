using System.Runtime.Versioning;
using Microsoft.Win32;

namespace BetterSearchBar.Core;

[SupportedOSPlatform("windows")]
public static class RegistryHelper
{
    public static int? GetDword(RegistryHive hive, string subKeyPath, string valueName)
    {
        try
        {
            using var baseKey = RegistryKey.OpenBaseKey(hive, RegistryView.Default);
            using var subKey = baseKey.OpenSubKey(subKeyPath, writable: false);
            if (subKey == null) return null;

            var val = subKey.GetValue(valueName);
            if (val is int intVal) return intVal;
            if (val != null && int.TryParse(val.ToString(), out int parsed)) return parsed;
            return null;
        }
        catch
        {
            return null;
        }
    }

    public static bool SetDword(RegistryHive hive, string subKeyPath, string valueName, int value)
    {
        try
        {
            using var baseKey = RegistryKey.OpenBaseKey(hive, RegistryView.Default);
            using var subKey = baseKey.CreateSubKey(subKeyPath, writable: true);
            if (subKey == null) return false;

            subKey.SetValue(valueName, value, RegistryValueKind.DWord);
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            throw;
        }
        catch
        {
            return false;
        }
    }

    public static bool DeleteValue(RegistryHive hive, string subKeyPath, string valueName)
    {
        try
        {
            using var baseKey = RegistryKey.OpenBaseKey(hive, RegistryView.Default);
            using var subKey = baseKey.OpenSubKey(subKeyPath, writable: true);
            if (subKey == null) return true;

            if (subKey.GetValue(valueName) != null)
            {
                subKey.DeleteValue(valueName, throwOnMissingValue: false);
            }
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            throw;
        }
        catch
        {
            return false;
        }
    }

    public static bool ClearSubKeyValues(RegistryHive hive, string subKeyPath)
    {
        try
        {
            using var baseKey = RegistryKey.OpenBaseKey(hive, RegistryView.Default);
            using var subKey = baseKey.OpenSubKey(subKeyPath, writable: true);
            if (subKey == null) return true;

            foreach (var valName in subKey.GetValueNames())
            {
                subKey.DeleteValue(valName, throwOnMissingValue: false);
            }
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            throw;
        }
        catch
        {
            return false;
        }
    }
}
