@echo off
setlocal
echo [BetterSearchBar] Publishing single-file executable for Windows (x64)...

dotnet publish "%~dp0src\BetterSearchBar\BetterSearchBar.csproj" -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true -o "%~dp0dist"

if %ERRORLEVEL% equ 0 (
    echo.
    echo Build succeeded! Executable is at: dist\BetterSearchBar.exe
) else (
    echo.
    echo Build failed. Please verify that .NET 10 SDK is installed.
)

endlocal
