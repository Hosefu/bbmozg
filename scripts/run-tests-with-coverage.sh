#!/bin/bash

# Скрипт для запуска тестов с покрытием кода и генерации HTML отчётов

set -e

echo "🧪 Запуск тестов с покрытием кода для проекта Lauf..."

# Переходим в корневую директорию
cd "$(dirname "$0")/.."

# Создаем директории для отчётов
mkdir -p TestResults/Coverage
mkdir -p TestResults/Reports

echo "📦 Установка reportgenerator (если не установлен)..."
if ! command -v reportgenerator &> /dev/null; then
    dotnet tool install --global dotnet-reportgenerator-globaltool
fi

echo "🔧 Очистка предыдущих результатов..."
rm -rf TestResults/*

echo "🏗️  Сборка проекта..."
dotnet build --configuration Release

echo "🧪 Запуск тестов с покрытием..."

# Запускаем тесты для каждого проекта с покрытием
dotnet test tests/Lauf.Shared.Tests/Lauf.Shared.Tests.csproj \
  --configuration Release \
  --no-build \
  --logger "trx;LogFileName=shared-tests.trx" \
  --collect:"XPlat Code Coverage" \
  --results-directory TestResults/

dotnet test tests/Lauf.Domain.Tests/Lauf.Domain.Tests.csproj \
  --configuration Release \
  --no-build \
  --logger "trx;LogFileName=domain-tests.trx" \
  --collect:"XPlat Code Coverage" \
  --results-directory TestResults/

dotnet test tests/Lauf.Application.Tests/Lauf.Application.Tests.csproj \
  --configuration Release \
  --no-build \
  --logger "trx;LogFileName=application-tests.trx" \
  --collect:"XPlat Code Coverage" \
  --results-directory TestResults/

dotnet test tests/Lauf.Infrastructure.Tests/Lauf.Infrastructure.Tests.csproj \
  --configuration Release \
  --no-build \
  --logger "trx;LogFileName=infrastructure-tests.trx" \
  --collect:"XPlat Code Coverage" \
  --results-directory TestResults/

dotnet test tests/Lauf.Api.Tests/Lauf.Api.Tests.csproj \
  --configuration Release \
  --no-build \
  --logger "trx;LogFileName=api-tests.trx" \
  --collect:"XPlat Code Coverage" \
  --results-directory TestResults/

echo "📊 Генерация HTML отчёта покрытия..."

# Генерируем объединенный HTML отчёт
reportgenerator \
  "-reports:TestResults/**/coverage.cobertura.xml" \
  "-targetdir:TestResults/Reports" \
  "-reporttypes:Html;HtmlSummary;Badges;TextSummary" \
  "-title:Lauf - Code Coverage Report" \
  "-tag:$(date +%Y%m%d_%H%M%S)" \
  "-historydir:TestResults/History"

echo "📈 Генерация отчёта по тестам..."

# Генерируем отчёт по тестам
reportgenerator \
  "-reports:TestResults/**/*.trx" \
  "-targetdir:TestResults/Reports/TestResults" \
  "-reporttypes:Html" \
  "-title:Lauf - Test Results Report"

echo "✅ Готово!"
echo ""
echo "📋 Результаты:"
echo "   📊 HTML отчёт покрытия: TestResults/Reports/index.html"
echo "   🧪 Отчёт тестов: TestResults/Reports/TestResults/index.html"
echo "   📁 Все файлы: TestResults/"
echo ""
echo "💡 Для просмотра в браузере:"
echo "   open TestResults/Reports/index.html"
echo ""

# Показываем краткую сводку покрытия
if [ -f "TestResults/Reports/Summary.txt" ]; then
    echo "📊 Краткая сводка покрытия:"
    cat TestResults/Reports/Summary.txt
fi