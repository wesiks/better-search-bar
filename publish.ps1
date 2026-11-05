Write-Host "[BetterSearchBar] Публикация автономного single-file exe для Windows (x64)..." -ForegroundColor Cyan

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$projPath = Join-Path $scriptDir "src\BetterSearchBar\BetterSearchBar.csproj"
$outDir = Join-Path $scriptDir "dist"

dotnet publish $projPath -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true -o $outDir

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n[✓] Сборка завершена успешно!" -ForegroundColor Green
    Write-Host "Файл: $outDir\BetterSearchBar.exe`n" -ForegroundColor White
} else {
    Write-Host "`n[✗] Ошибка при сборке. Проверьте наличие .NET 10 SDK." -ForegroundColor Red
}

