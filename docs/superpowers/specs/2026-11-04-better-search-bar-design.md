# BetterSearchBar — Design Specification

## Overview
**BetterSearchBar** is a modern C# (.NET 10) command-line interface (CLI) utility designed to clean up and declutter the native Windows 10/11 Search interface (`SearchHost.exe` / `SearchApp.exe`).

It eliminates:
- Search Highlights (daily Bing pictures, trivia, "On this day" historical facts);
- Promotional game tiles and sponsored store app suggestions ("Games for you" / "Игры для вас");
- Web search / Bing suggestions embedded into local file and application search;
- Cloud search / Microsoft Account query synchronization.

The utility features an interactive terminal interface built with **Spectre.Console**, offering granular control (checkbox multiselect), status introspection, full restoration capabilities, and instant application without requiring a system reboot.

---

## Architecture & Project Structure

### Solution Layout
```
better search bar/
├── .gitignore
├── README.md
├── BetterSearchBar.sln
├── src/
│   └── BetterSearchBar/
│       ├── BetterSearchBar.csproj
│       ├── Program.cs             // Entry point, CLI argument parsing, elevation check
│       ├── UI/
│       │   └── ConsoleUI.cs       // Interactive menus, status tables, prompts via Spectre.Console
│       ├── Core/
│       │   ├── ITweak.cs          // Interface defining individual tweak behavior
│       │   ├── TweakCategory.cs   // Categorization and state model
│       │   ├── TweaksRegistry.cs  // Registry of all available tweaks
│       │   ├── RegistryHelper.cs  // Safe registry access wrapper for HKCU and HKLM
│       │   └── ProcessHelper.cs   // Graceful restart of SearchHost/SearchApp & Explorer notification
│       └── Tweaks/
│           ├── AdsTweak.cs              // Disables promotional cards, "Games for you", suggestions
│           ├── SearchHighlightsTweak.cs  // Disables "Image of the day", Bing daily content
│           ├── BingSearchTweak.cs        // Disables Bing web search in local search bar
│           └── CloudSearchTweak.cs       // Disables cloud search and account history sync
```

---

## Core Interfaces and Components

### 1. `ITweak` Interface
```csharp
public interface ITweak
{
    string Id { get; }
    string Title { get; }
    string Description { get; }
    bool RequiresElevation { get; }

    bool IsApplied();
    void Apply();
    void Revert();
}
```

### 2. Tweak Details & Registry Keys

#### A. Ads & Game Promotions (`AdsTweak`)
- **Target:** Removes "Games for you", sponsored app tiles, and Windows consumer suggestions.
- **Registry Keys (HKCU & HKLM):**
  - `HKCU\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager`:
    - `SystemPaneSuggestionsEnabled = 0` (Revert: `1`)
    - `SubscribedContent-338388Enabled = 0` (Revert: `1`)
    - `SubscribedContent-338389Enabled = 0` (Revert: `1`)
    - `SilentInstalledAppsEnabled = 0` (Revert: `1`)
  - `HKLM\SOFTWARE\Policies\Microsoft\Windows\CloudContent`:
    - `DisableWindowsConsumerFeatures = 1` (Revert: delete value or `0`)

#### B. Search Highlights (`SearchHighlightsTweak`)
- **Target:** Removes Bing daily images ("Изображение дня"), "On this day", trending topics, and search doodles.
- **Registry Keys:**
  - `HKCU\Software\Microsoft\Windows\CurrentVersion\SearchSettings`:
    - `IsDynamicSearchBoxEnabled = 0` (Revert: `1`)
  - `HKLM\SOFTWARE\Policies\Microsoft\Windows\Windows Search`:
    - `EnableDynamicContentInWSB = 0` (Revert: delete value or `1`)

#### C. Bing Web Search (`BingSearchTweak`)
- **Target:** Removes web search results from the search box so only local files, settings, and apps are queried.
- **Registry Keys:**
  - `HKCU\Software\Microsoft\Windows\CurrentVersion\Search`:
    - `BingSearchEnabled = 0` (Revert: `1`)
    - `CortanaConsent = 0` (Revert: `1`)
  - `HKCU\Software\Policies\Microsoft\Windows\Explorer`:
    - `DisableSearchBoxSuggestions = 1` (Revert: `0`)
  - `HKLM\SOFTWARE\Policies\Microsoft\Windows\Windows Search`:
    - `ConnectedSearchUseWeb = 0` (Revert: `1`)
    - `DisableWebSearch = 1` (Revert: `0`)

#### D. Cloud Account Search (`CloudSearchTweak`)
- **Target:** Disables synchronization of Microsoft account cloud history in search.
- **Registry Keys:**
  - `HKLM\SOFTWARE\Policies\Microsoft\Windows\Windows Search`:
    - `AllowCloudSearch = 0` (Revert: `1`)

---

## Application & Refresh Mechanics

When registry values are updated, Windows caches search UI inside `SearchHost.exe` (Windows 11) or `SearchApp.exe` (Windows 10).
- `ProcessHelper.RestartSearchHost()` identifies and gracefully terminates any running `SearchHost` / `SearchApp` process instances.
- Windows automatically relaunches the process when the user next presses `Win + S` or clicks the search bar, reflecting the new configuration immediately without requiring a full Windows restart or explorer disruption.

---

## User Interaction Flow (Spectre.Console)

### Main Menu
1. Displays header banner and elevated (Admin) status badge.
2. Shows an interactive status table listing each component and whether it is currently Clean or Cluttered.
3. Menu items:
   - **[1] Настроить очистку (выбор компонентов)** — opens interactive checkbox selection.
   - **[2] Быстрая очистка (отключить весь мусор)** — applies all tweaks in one operation.
   - **[3] Восстановить стандартные настройки Windows** — reverts all tweaks back to Windows default values.
   - **[4] Перезапустить процесс поиска** — triggers SearchHost restart.
   - **[5] Выход**

### Granular Selection Menu (Option 1)
Using `MultiSelectionPrompt`:
```text
Отметьте компоненты для отключения ([Пробел] - выбор, [Enter] - подтвердить):
[X] Реклама и промо-игры («Игры для вас», карточки)
[X] Search Highlights («Изображение дня», дудлы Bing)
[X] Веб-поиск Bing (локальный поиск файлов и программ)
[ ] Облачный поиск (Microsoft Account)
```

### CLI Automation Arguments
The executable supports headless CLI execution:
- `--clean-all`: Applies all cleaning tweaks and restarts SearchHost.
- `--restore`: Reverts all tweaks back to factory defaults.
- `--status`: Outputs the current state of tweaks to stdout and exits with status code 0.

---

## Deployment & Build
- Framework: `net10.0`
- Output: Single-file executable option (`dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true`)
- Git Integration: Repository initialized locally, committed, and synced to GitHub (`wesiks/better-search-bar`).

