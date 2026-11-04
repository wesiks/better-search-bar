using System.Diagnostics;
using System.Runtime.Versioning;
using System.Security.Principal;
using BetterSearchBar.Core;
using Spectre.Console;

namespace BetterSearchBar.UI;

[SupportedOSPlatform("windows")]
public static class ConsoleUI
{
    public static void Run()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        if (Console.IsInputRedirected)
        {
            PrintHeadlessStatus();
            return;
        }

        while (true)
        {
            AnsiConsole.Clear();
            RenderHeader();
            RenderStatusTable();

            var isElevated = IsAdministrator();
            var choices = new List<string>
            {
                "1. Настроить очистку (выбрать компоненты вручную)",
                "2. Быстрая очистка (отключить весь мусор и рекламу)",
                "3. Восстановить стандартные настройки Windows",
                "4. Перезапустить процесс поиска (SearchHost)",
            };

            if (!isElevated)
            {
                choices.Add("5. Перезапустить утилиту с правами Администратора");
            }

            choices.Add("0. Выход");

            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("\n[bold cyan]Выберите действие:[/]")
                    .PageSize(10)
                    .HighlightStyle(new Style(foreground: Color.Cyan1, decoration: Decoration.Bold))
                    .AddChoices(choices));

            if (selection.StartsWith("0"))
            {
                AnsiConsole.MarkupLine("[grey]Завершение работы BetterSearchBar. До свидания![/]");
                break;
            }

            if (selection.StartsWith("1"))
            {
                HandleCustomSelection();
            }
            else if (selection.StartsWith("2"))
            {
                HandleApplyAll();
            }
            else if (selection.StartsWith("3"))
            {
                HandleRevertAll();
            }
            else if (selection.StartsWith("4"))
            {
                HandleRestartSearch();
            }
            else if (selection.StartsWith("5"))
            {
                RelaunchAsAdmin();
                break;
            }
        }
    }

    public static void RenderHeader()
    {
        var rule = new Rule("[bold dodgerblue1]BetterSearchBar — Очистка поиска Windows[/]")
        {
            Style = Style.Parse("dodgerblue1")
        };
        rule.Centered();
        AnsiConsole.Write(rule);

        var isElevated = IsAdministrator();
        var adminBadge = isElevated
            ? "[bold green] Администратор [/]"
            : "[bold yellow] Обычный пользователь (для полного отключения системных политик рекомендуется запуск от Администратора) [/]";

        AnsiConsole.MarkupLine($"[grey]Режим прав:[/] {adminBadge}");
        AnsiConsole.MarkupLine($"[grey]Система:[/] [white]{Environment.OSVersion}[/]\n");
    }

    public static void RenderStatusTable()
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Grey37)
            .Title("[bold white]Текущее состояние компонентов поиска[/]");

        table.AddColumn(new TableColumn("[grey]#[/]").Centered());
        table.AddColumn(new TableColumn("[bold]Компонент[/]"));
        table.AddColumn(new TableColumn("[bold]Статус[/]").Centered());
        table.AddColumn(new TableColumn("[grey]Описание[/]"));

        int index = 1;
        foreach (var tweak in TweaksRegistry.All)
        {
            bool applied = tweak.IsApplied();
            string status = applied
                ? "[bold green]✓ Чисто (отключено)[/]"
                : "[bold red]✗ Мусор активен[/]";

            table.AddRow(
                $"[grey]{index++}[/]",
                $"[bold]{Markup.Escape(tweak.Title)}[/]",
                status,
                $"[grey]{Markup.Escape(tweak.Description)}[/]");
        }

        AnsiConsole.Write(table);
    }

    private static void HandleCustomSelection()
    {
        AnsiConsole.Clear();
        RenderHeader();

        var prompt = new MultiSelectionPrompt<ITweak>()
            .Title("[bold yellow]Отметьте компоненты, которые нужно ОТКЛЮЧИТЬ (очистить):[/]\n" +
                   "[grey](Используйте [bold white]Пробел[/] для выбора/снятия отметки, [bold white]Enter[/] для подтверждения)[/]\n")
            .Required(false)
            .PageSize(10)
            .UseConverter(t => t.Title)
            .HighlightStyle(new Style(foreground: Color.Cyan1, decoration: Decoration.Bold));

        foreach (var tweak in TweaksRegistry.All)
        {
            var choice = prompt.AddChoice(tweak);
            if (tweak.IsApplied())
            {
                choice.Select();
            }
        }

        var selectedTweaks = AnsiConsole.Prompt(prompt);

        AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .Start("[bold cyan]Применение выбранных настроек...[/]", ctx =>
            {
                TweaksRegistry.ApplySelected(selectedTweaks);
                Thread.Sleep(300);
                ctx.Status("[bold cyan]Перезапуск процесса поиска Windows...[/]");
                ProcessHelper.RestartSearchHost();
                Thread.Sleep(300);
            });

        AnsiConsole.MarkupLine("\n[bold green]✓ Выбранные настройки успешно применены![/]");
        PressAnyKeyToContinue();
    }

    private static void HandleApplyAll()
    {
        AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .Start("[bold cyan]Отключение всех видов рекламы, Bing и мусора в поиске...[/]", ctx =>
            {
                TweaksRegistry.ApplyAll();
                Thread.Sleep(300);
                ctx.Status("[bold cyan]Перезапуск процесса поиска Windows...[/]");
                ProcessHelper.RestartSearchHost();
                Thread.Sleep(300);
            });

        var panel = new Panel("[bold green]Все компоненты успешно очищены![/]\n[grey]Реклама, баннеры, тренды Bing и облачный поиск отключены.\nПоиск теперь работает максимально быстро и локально.[/]")
        {
            Border = BoxBorder.Rounded,
            BorderStyle = new Style(Color.Green)
        };
        AnsiConsole.Write(panel);
        PressAnyKeyToContinue();
    }

    private static void HandleRevertAll()
    {
        bool confirm = AnsiConsole.Confirm("[bold red]Вы уверены, что хотите вернуть стандартные настройки Windows (включить рекламу и Bing)?[/]", defaultValue: false);
        if (!confirm) return;

        AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .Start("[bold cyan]Восстановление стандартных настроек Windows...[/]", ctx =>
            {
                TweaksRegistry.RevertAll();
                Thread.Sleep(300);
                ctx.Status("[bold cyan]Перезапуск процесса поиска Windows...[/]");
                ProcessHelper.RestartSearchHost();
                Thread.Sleep(300);
            });

        AnsiConsole.MarkupLine("[bold yellow]Стандартные настройки Windows восстановлены.[/]");
        PressAnyKeyToContinue();
    }

    private static void HandleRestartSearch()
    {
        int killed = 0;
        AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .Start("[bold cyan]Перезапуск фоновых процессов поиска...[/]", _ =>
            {
                killed = ProcessHelper.RestartSearchHost();
                Thread.Sleep(400);
            });

        AnsiConsole.MarkupLine($"[bold green]✓ Процессы поиска обновлены (перезапущено процессов: {killed}).[/]");
        AnsiConsole.MarkupLine("[grey]Нажмите Win + S или откройте поиск на панели задач, чтобы увидеть изменения.[/]");
        PressAnyKeyToContinue();
    }

    public static void PrintHeadlessStatus()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        RenderHeader();
        RenderStatusTable();
    }

    private static void PressAnyKeyToContinue()
    {
        if (Console.IsInputRedirected) return;
        AnsiConsole.Markup("\n[grey]Нажмите любую клавишу для возврата в меню...[/]");
        try
        {
            Console.ReadKey(intercept: true);
        }
        catch
        {
            // Ignore if key cannot be read
        }
    }

    private static bool IsAdministrator()
    {
        try
        {
            using var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        catch
        {
            return false;
        }
    }

    private static void RelaunchAsAdmin()
    {
        try
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = Environment.ProcessPath ?? Process.GetCurrentProcess().MainModule?.FileName ?? "dotnet",
                UseShellExecute = true,
                Verb = "runas"
            };

            Process.Start(processInfo);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Не удалось перезапустить с повышенными привилегиями: {Markup.Escape(ex.Message)}[/]");
            PressAnyKeyToContinue();
        }
    }
}
