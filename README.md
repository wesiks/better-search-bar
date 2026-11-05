# BetterSearchBar

CLI-утилита на C# (.NET 10) для очистки поиска и меню «Пуск» Windows 10/11 от рекламы, веб-поиска Bing, хайлайтов и рекомендаций.

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![Windows](https://img.shields.io/badge/Platform-Windows%2010%20%2F%2011-0078D6)](https://microsoft.com/windows)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Author](https://img.shields.io/badge/Author-wesiks-blue)](https://github.com/wesiks)
[![Telegram](https://img.shields.io/badge/Telegram-%40wesiks-2CA5E0)](https://t.me/wesiks)

## Возможности

- Отключение рекламы и промо-блоков («Игры для вас», карточки)
- Отключение Search Highlights (дудлы, картинка дня Bing)
- Отключение веб-поиска Bing (только локальный поиск файлов и программ)
- Отключение синхронизации облачного поиска Microsoft
- Отключение и очистка блока «Главные приложения»
- Отключение подсказок «Запросы для быстрого поиска»
- Отключение блока «Рекомендации» в меню Пуск
- Восстановление и запуск службы Windows Search (устранение предупреждения об отключенном индексировании)
- Мгновенное применение настроек без перезагрузки Windows
- Интерактивное меню с выбором компонентов и поддержка тихих CLI-флагов

## Сборка и запуск

Запуск из исходников:
```bash
dotnet run --project src/BetterSearchBar
```

Сборка автономного исполняемого файла:
```bash
dotnet publish src/BetterSearchBar -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true -o ./dist
```

## Использование CLI

| Аргумент | Описание |
|---|---|
| `-c`, `--clean-all` | Применить все твики очистки |
| `-r`, `--restore` | Восстановить стандартные настройки Windows |
| `-s`, `--status` | Показать текущий статус компонентов |
| `--restart-search` | Перезапустить процессы поиска и меню Пуск |
| `-h`, `--help` | Показать справку |

## Контакты

- Автор: wesiks
- Telegram: https://t.me/wesiks
- Лицензия: MIT
