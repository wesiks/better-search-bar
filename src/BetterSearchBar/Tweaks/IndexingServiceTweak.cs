using System.Diagnostics;
using System.Runtime.Versioning;
using BetterSearchBar.Core;

namespace BetterSearchBar.Tweaks;

[SupportedOSPlatform("windows")]
public class IndexingServiceTweak : ITweak
{
    public string Id => "indexing";
    public string Title => "Уведомление об индексировании (служба Windows Search)";
    public string Description => "Запускает службу поиска Windows, убирая предупреждение «Индексирование поиска отключено» и восстанавливая локальный поиск.";
    public bool RequiresElevation => true;

    public bool IsApplied()
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "sc.exe",
                Arguments = "query wsearch",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process == null) return false;

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return output.Contains("RUNNING", StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    public void Apply()
    {
        RunScCommand("config wsearch start= delayed-auto");
        RunScCommand("start wsearch");
    }

    public void Revert()
    {
        RunScCommand("stop wsearch");
        RunScCommand("config wsearch start= disabled");
    }

    private static void RunScCommand(string arguments)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "sc.exe",
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            process?.WaitForExit();
        }
        catch
        {
        }
    }
}
