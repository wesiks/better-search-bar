using System.Runtime.Versioning;
using BetterSearchBar.Core;
using BetterSearchBar.UI;
using Spectre.Console;

namespace BetterSearchBar;

[SupportedOSPlatform("windows")]
public static class Program
{
    public static int Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        if (!OperatingSystem.IsWindows())
        {
            AnsiConsole.MarkupLine("[bold red]Ошибка:[/] Данная утилита предназначена только для операционных систем Windows (Windows 10 / 11).");
            return 1;
        }

        if (args.Length > 0)
        {
            return HandleCliArguments(args);
        }

        ConsoleUI.Run();
        return 0;
    }

    private static int HandleCliArguments(string[] args)
    {
        var arg = args[0].ToLowerInvariant();

        switch (arg)
        {
            case "--clean-all" or "-c":
                AnsiConsole.MarkupLine("[bold cyan]Применение всех твиков очистки поиска...[/]");
                TweaksRegistry.ApplyAll();
                int killed = ProcessHelper.RestartSearchHost();
                AnsiConsole.MarkupLine($"[bold green]✓ Все твики применены. Процесс поиска перезапущен ({killed} инстансов).[/]");
                return 0;

            case "--restore" or "-r":
                AnsiConsole.MarkupLine("[bold yellow]Восстановление стандартных настроек Windows...[/]");
                TweaksRegistry.RevertAll();
                ProcessHelper.RestartSearchHost();
                AnsiConsole.MarkupLine("[bold green]✓ Стандартные настройки восстановлены. Процесс поиска перезапущен.[/]");
                return 0;

            case "--status" or "-s":
                ConsoleUI.PrintHeadlessStatus();
                return 0;

            case "--restart-search":
                int k = ProcessHelper.RestartSearchHost();
                AnsiConsole.MarkupLine($"[bold green]✓ Процесс поиска Windows перезапущен ({k} инстансов).[/]");
                return 0;

            case "--help" or "-h" or "/?":
                PrintHelp();
                return 0;

            default:
                AnsiConsole.MarkupLine($"[bold red]Неизвестный аргумент:[/] {Markup.Escape(args[0])}\n");
                PrintHelp();
                return 1;
        }
    }

    private static void PrintHelp()
    {
        AnsiConsole.MarkupLine("[bold dodgerblue1]BetterSearchBar — CLI утилита для очистки поиска Windows[/]\n");
        AnsiConsole.MarkupLine("[bold]Использование:[/] [white]BetterSearchBar [[опции]][/]\n");
        AnsiConsole.MarkupLine("[bold]Опции без интерактивного режима:[/] ");
        AnsiConsole.MarkupLine("  [white]-c, --clean-all[/]        Отключить всю рекламу, промо-игры, тренды Bing и облачный поиск");
        AnsiConsole.MarkupLine("  [white]-r, --restore[/]          Восстановить стандартные настройки Windows");
        AnsiConsole.MarkupLine("  [white]-s, --status[/]           Показать текущий статус всех компонентов поиска");
        AnsiConsole.MarkupLine("  [white]--restart-search[/]       Перезапустить процесс SearchHost для применения настроек");
        AnsiConsole.MarkupLine("  [white]-h, --help[/]             Показать эту справку\n");
        AnsiConsole.MarkupLine("[grey]При запуске без аргументов открывается интерактивное псевдографическое меню.[/]");
    }
}
