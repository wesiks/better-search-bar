using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace BetterSearchBar.Core;

[SupportedOSPlatform("windows")]
public static class ProcessHelper
{
    [DllImport("shell32.dll", SetLastError = true)]
    private static extern void SHChangeNotify(int wEventId, uint uFlags, nint dwItem1, nint dwItem2);

    private const int SHCNE_ASSOCCHANGED = 0x08000000;
    private const uint SHCNF_FLUSH = 0x1000;

    /// <summary>
    /// Gracefully terminates SearchHost / SearchApp / SearchUI processes to force Windows Search
    /// to reload settings immediately when invoked.
    /// </summary>
    public static int RestartSearchHost()
    {
        int killed = 0;
        string[] targetProcessNames = ["SearchHost", "SearchApp", "SearchUI"];

        foreach (var procName in targetProcessNames)
        {
            try
            {
                var processes = Process.GetProcessesByName(procName);
                foreach (var p in processes)
                {
                    try
                    {
                        p.Kill();
                        p.WaitForExit(1500);
                        killed++;
                    }
                    catch
                    {
                        // Process already terminated or lack of permissions
                    }
                    finally
                    {
                        p.Dispose();
                    }
                }
            }
            catch
            {
                // Ignore process enumeration errors
            }
        }

        // Notify shell of association/system environment changes
        try
        {
            SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_FLUSH, nint.Zero, nint.Zero);
        }
        catch
        {
            // Ignore
        }

        return killed;
    }
}
