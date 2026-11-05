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
                    }
                    finally
                    {
                        p.Dispose();
                    }
                }
            }
            catch
            {
            }
        }

        try
        {
            SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_FLUSH, nint.Zero, nint.Zero);
        }
        catch
        {
        }

        return killed;
    }
}
