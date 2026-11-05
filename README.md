# BetterSearchBar 🔍

> **BetterSearchBar** — современная легковесная C# (.NET 10) CLI-утилита для полной очистки поиска Windows 10 и 11 от рекламы, спонсорских карточек, трендов Bing и навязчивых рекомендаций.

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Windows](https://img.shields.io/badge/Platform-Windows%2010%20%2F%2011-0078D6?logo=windows)](https://microsoft.com/windows)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![GitHub wesiks](https://img.shields.io/badge/Author-wesiks-blue?logo=github)](https://github.com/wesiks)

---

## 🎯 Что решает BetterSearchBar

Штатный поиск Windows часто перегружен лишними веб-запросами и рекламой:
- **Реклама и промо-игры:** блоки «Игры для вас», спонсируемые приложения из Microsoft Store;
- **Search Highlights:** ежедневные фоны Bing, дудлы, новости и факты дня, замедляющие открытие поиска;
- **Веб-поиск Bing:** отправка любого локального запроса в интернет вместо быстрого нахождения локальных файлов и программ;
- **Облачный поиск:** синхронизация истории запросов аккаунта Microsoft;
- **Главные приложения:** список недавних/часто запускаемых программ и отслеживание действий;
- **Запросы для быстрого поиска:** навязчивые подсказки настроек на стартовом экране;
- **Уведомления службы поиска:** предупреждения вида «Индексирование поиска отключено».

**BetterSearchBar** возвращает поиску его первозданную функцию — **мгновенный и чистый локальный поиск без задержек и мусора**.

---

## ✨ Возможности

- **Интерактивное TUI-меню:** красивый терминальный интерфейс на базе [Spectre.Console](https://spectreconsole.net/) с чекбоксами для выбора конкретных компонентов.
- **Быстрая очистка в 1 клик:** отключение всего лишнего за секунду.
- **Мгновенное применение:** утилита корректно перезапускает процесс `SearchHost.exe` (`SearchApp.exe`), поэтому изменения видны **сразу без перезагрузки Windows**.
- **100% обратимость:** в любой момент можно восстановить заводские настройки Windows.
- **Поддержка тихой автоматизации:** CLI-флаги для скриптов и пакетной настройки систем.
- **Безопасность:** меняются только документированные системные ключи реестра (`HKCU` и `HKLM Policies`).

---

## 🖥 Скриншот интерфейса

```text
─────────────────── BetterSearchBar — Очистка поиска Windows ───────────────────
Режим прав:  Администратор 
Система: Microsoft Windows NT 10.0.26200.0

                      Текущее состояние компонентов поиска                      
╭───┬────────────────────────────┬─────────────────────┬───────────────────────────╮
│ # │ Компонент                  │       Статус        │ Описание                  │
├───┼────────────────────────────┼─────────────────────┼───────────────────────────┤
│ 1 │ Реклама и промо-игры       │ ✓ Чисто (отключено) │ Отключает предложения     │
│   │ («Игры для вас», карточки) │                     │ спонсируемых игр и промо  │
│ 2 │ Search Highlights          │ ✓ Чисто (отключено) │ Убирает дудлы и картинки  │
│   │ («Изображение дня»)        │                     │ дня Bing из строки поиска │
│ 3 │ Веб-поиск Bing             │ ✓ Чисто (отключено) │ Мгновенный поиск только   │
│   │                            │                     │ по локальным файлам       │
│ 4 │ Облачный поиск             │ ✓ Чисто (отключено) │ Отключает синхронизацию   │
│   │ (Microsoft Account & AAD)  │                     │ облачной истории поиска   │
│ 5 │ Блок «Главные приложения»  │ ✓ Чисто (отключено) │ Убирает список недавних   │
│   │                            │                     │ программ и отслеживание   │
│ 6 │ «Запросы для быстрого      │ ✓ Чисто (отключено) │ Убирает поисковые         │
│   │ поиска» (предложения)      │                     │ подсказки настроек        │
│ 7 │ Служба Windows Search      │ ✓ Чисто (отключено) │ Восстанавливает службу и  │
│   │ (индексирование)           │                     │ убирает предупреждения    │
╰───┴────────────────────────────┴─────────────────────┴───────────────────────────╯
```

---

## 🚀 Быстрый старт

### Требования
- **ОС:** Windows 10 (1809+) или Windows 11
- **Среда выполнения:** .NET 10 Runtime (или сборка в self-contained exe)

### Запуск из исходников
```bash
git clone https://github.com/wesiks/better-search-bar.git
cd better-search-bar
dotnet run --project src/BetterSearchBar
```

### Сборка в один автономный `.exe` файл (Self-contained)
Не требует установленного .NET на целевом компьютере:
```bash
dotnet publish src/BetterSearchBar -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true -o ./publish
```
Готовый исполняемый файл будет находиться в папке `publish/BetterSearchBar.exe`.

---

## ⚙️ Использование в командной строке (CLI-аргументы)

Утилиту можно запускать без интерактивного меню в автоматических скриптах:

| Аргумент | Описание |
|---|---|
| `-c`, `--clean-all` | Отключить всю рекламу, промо-игры, тренды Bing и облачный поиск |
| `-r`, `--restore` | Восстановить стандартные настройки Windows |
| `-s`, `--status` | Вывести текущий статус компонентов поиска |
| `--restart-search` | Перезапустить процессы `SearchHost` / `SearchApp` |
| `-h`, `--help` | Показать справку по командам |

Пример:
```powershell
# Полная очистка в один шаг
BetterSearchBar.exe --clean-all

# Проверка статуса
BetterSearchBar.exe --status
```

---

## 🛠 Технические детали

Изменения вносятся в следующие разделы реестра:

1. **Реклама и промо-игры:**
   - `HKCU\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager`:
     - `SystemPaneSuggestionsEnabled = 0`
     - `SubscribedContent-338388Enabled = 0`
     - `SubscribedContent-338389Enabled = 0`
     - `SilentInstalledAppsEnabled = 0`
   - `HKLM\SOFTWARE\Policies\Microsoft\Windows\CloudContent`:
     - `DisableWindowsConsumerFeatures = 1`

2. **Search Highlights:**
   - `HKCU\Software\Microsoft\Windows\CurrentVersion\SearchSettings`:
     - `IsDynamicSearchBoxEnabled = 0`
   - `HKLM\SOFTWARE\Policies\Microsoft\Windows\Windows Search`:
     - `EnableDynamicContentInWSB = 0`

3. **Веб-поиск Bing:**
   - `HKCU\Software\Microsoft\Windows\CurrentVersion\Search`:
     - `BingSearchEnabled = 0`
     - `CortanaConsent = 0`
   - `HKCU\Software\Policies\Microsoft\Windows\Explorer`:
     - `DisableSearchBoxSuggestions = 1`
   - `HKLM\SOFTWARE\Policies\Microsoft\Windows\Windows Search`:
     - `ConnectedSearchUseWeb = 0`
     - `DisableWebSearch = 1`

4. **Облачный поиск:**
   - `HKCU\Software\Microsoft\Windows\CurrentVersion\SearchSettings`:
     - `IsAADCloudSearchEnabled = 0`
     - `IsMSACloudSearchEnabled = 0`
   - `HKLM\SOFTWARE\Policies\Microsoft\Windows\Windows Search`:
     - `AllowCloudSearch = 0`

---

## 📄 Лицензия

Распространяется под лицензией MIT. Подробнее см. в файле [LICENSE](LICENSE).

