# PowerShell скрипт для запуска тестов с покрытием кода и генерации HTML отчётов

param(
    [switch]$OpenReport = $false
)

Write-Host "🧪 Запуск тестов с покрытием кода для проекта Lauf..." -ForegroundColor Green

# Переходим в корневую директорию
Set-Location (Split-Path $PSScriptRoot -Parent)

# Создаем директории для отчётов
New-Item -ItemType Directory -Force -Path "TestResults/Coverage" | Out-Null
New-Item -ItemType Directory -Force -Path "TestResults/Reports" | Out-Null

Write-Host "📦 Проверка reportgenerator..." -ForegroundColor Yellow
try {
    $null = Get-Command reportgenerator -ErrorAction Stop
} catch {
    Write-Host "Установка reportgenerator..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-reportgenerator-globaltool
}

Write-Host "🔧 Очистка предыдущих результатов..." -ForegroundColor Yellow
if (Test-Path "TestResults") {
    Remove-Item -Recurse -Force "TestResults/*"
}

Write-Host "🏗️  Сборка проекта..." -ForegroundColor Yellow
dotnet build --configuration Release

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Ошибка сборки!" -ForegroundColor Red
    exit 1
}

Write-Host "🧪 Запуск тестов с покрытием..." -ForegroundColor Yellow

$testProjects = @(
    "tests/Lauf.Shared.Tests/Lauf.Shared.Tests.csproj",
    "tests/Lauf.Domain.Tests/Lauf.Domain.Tests.csproj",
    "tests/Lauf.Application.Tests/Lauf.Application.Tests.csproj",
    "tests/Lauf.Infrastructure.Tests/Lauf.Infrastructure.Tests.csproj",
    "tests/Lauf.Api.Tests/Lauf.Api.Tests.csproj"
)

$testNames = @("shared", "domain", "application", "infrastructure", "api")

for ($i = 0; $i -lt $testProjects.Length; $i++) {
    $project = $testProjects[$i]
    $name = $testNames[$i]
    
    if (Test-Path $project) {
        Write-Host "  ▶️  Тестирование $name..." -ForegroundColor Cyan
        
        dotnet test $project `
            --configuration Release `
            --no-build `
            --logger "trx;LogFileName=$name-tests.trx" `
            --collect:"XPlat Code Coverage" `
            --results-directory TestResults/
            
        if ($LASTEXITCODE -ne 0) {
            Write-Host "⚠️  Тесты $name завершились с ошибками" -ForegroundColor Yellow
        }
    } else {
        Write-Host "⚠️  Проект $project не найден" -ForegroundColor Yellow
    }
}

Write-Host "📊 Генерация HTML отчёта покрытия..." -ForegroundColor Yellow

$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"

# Генерируем объединенный HTML отчёт
reportgenerator `
    "-reports:TestResults/**/coverage.cobertura.xml" `
    "-targetdir:TestResults/Reports" `
    "-reporttypes:Html;HtmlSummary;Badges;TextSummary;JsonSummary" `
    "-title:Lauf - Code Coverage Report" `
    "-tag:$timestamp" `
    "-historydir:TestResults/History"

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Отчёты успешно сгенерированы!" -ForegroundColor Green
} else {
    Write-Host "❌ Ошибка генерации отчётов" -ForegroundColor Red
}

Write-Host ""
Write-Host "📋 Результаты:" -ForegroundColor Green
Write-Host "   📊 HTML отчёт покрытия: TestResults/Reports/index.html" -ForegroundColor White
Write-Host "   🧪 Файлы тестов: TestResults/*.trx" -ForegroundColor White
Write-Host "   📁 Все файлы: TestResults/" -ForegroundColor White

# Показываем краткую сводку покрытия
if (Test-Path "TestResults/Reports/Summary.txt") {
    Write-Host ""
    Write-Host "📊 Краткая сводка покрытия:" -ForegroundColor Green
    Get-Content "TestResults/Reports/Summary.txt"
}

# Показываем JSON сводку если есть
if (Test-Path "TestResults/Reports/Summary.json") {
    try {
        $summary = Get-Content "TestResults/Reports/Summary.json" | ConvertFrom-Json
        Write-Host ""
        Write-Host "📈 Метрики покрытия:" -ForegroundColor Green
        Write-Host "   Покрытие линий: $($summary.summary.linecoverage)%" -ForegroundColor White
        Write-Host "   Покрытие веток: $($summary.summary.branchcoverage)%" -ForegroundColor White
        Write-Host "   Покрываемые линии: $($summary.summary.coveredlines) из $($summary.summary.coverablelines)" -ForegroundColor White
    } catch {
        # Игнорируем ошибки парсинга JSON
    }
}

if ($OpenReport) {
    Write-Host ""
    Write-Host "🌐 Открытие отчёта в браузере..." -ForegroundColor Yellow
    if (Test-Path "TestResults/Reports/index.html") {
        Start-Process "TestResults/Reports/index.html"
    }
}