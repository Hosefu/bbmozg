# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**ФЛОФ (FLOF)** — корпоративная система онбординга сотрудников через Telegram-бота и веб-приложение.

**Ключевые архитектурные концепции:**
- **Потоки (Flows)**: Шаблоны обучающих программ
- **Назначения (Assignments)**: Привязка пользователя к потоку с дедлайнами
- **Бадди (Buddy)**: Наставник, курирующий прохождение
- **Снапшоты (Snapshots)**: Неизменяемые копии потоков на момент назначения (ключевая особенность архитектуры)
- **Компоненты (Components)**: Атомарные единицы контента (статьи, тесты, задания)

## Development Commands

### Build and Run
```bash
# Restore packages
dotnet restore

# Build solution
dotnet build

# Run API server
cd src/BuddyBot.Api
dotnet run --urls="http://localhost:5000"

# Clean build artifacts
dotnet clean
```

### Testing
```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/BuddyBot.Domain.Tests/

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Database Operations
```bash
# Add migration
cd src/BuddyBot.Infrastructure
dotnet ef migrations add <MigrationName> --startup-project ../BuddyBot.Api

# Update database
dotnet ef database update --startup-project ../BuddyBot.Api

# Generate SQL script
dotnet ef migrations script --startup-project ../BuddyBot.Api
```

## Architecture

### Technology Stack (.NET 9.0)
- **ASP.NET Core 9.0** + **C# 12** — основная платформа
- **GraphQL (HotChocolate 13.7)** — API с автогенерацией схемы
- **PostgreSQL + Entity Framework Core 9.0** — база данных с Code-First подходом
- **SignalR** — real-time коммуникация с WebSocket/long-polling fallback
- **Redis** — кэширование, pub/sub, сессии
- **Hangfire** — фоновые задачи и планировщик
- **MediatR** — CQRS implementation с командами и запросами
- **Telegram.Bot SDK** — интеграция с Telegram Bot API

### Clean Architecture Structure
```
src/
├── BuddyBot.Api/          # API слой (GraphQL, SignalR, Controllers)
├── BuddyBot.Application/  # Логика приложения (CQRS, MediatR, DTOs)
├── BuddyBot.Infrastructure/ # Внешние зависимости (БД, Telegram, файлы)
├── BuddyBot.Domain/       # Доменная логика и сущности
└── BuddyBot.Shared/       # Общие утилиты и константы
```

**Направление зависимостей:** API → Application → Domain ← Infrastructure
- Domain — независимый центральный слой
- Application зависит только от Domain
- Infrastructure реализует интерфейсы из Domain
- API использует Application и Infrastructure

### Key Domain Patterns
- **Snapshot Pattern**: Полное копирование потоков при назначении для обеспечения неизменности
- **Domain Events**: Событийно-ориентированная архитектура для уведомлений и прогресса
- **Value Objects**: Типизированные значения (TelegramUserId, ProgressPercentage)
- **Repository Pattern**: Абстракция доступа к данным через интерфейсы

### Configuration Structure
Приложение использует стандартную конфигурацию ASP.NET Core:
- **Development**: appsettings.Development.json (локальные настройки, увеличенное логирование)
- **Production**: appsettings.json (продакшн настройки)

Основные конфигурационные секции:
- `ConnectionStrings` — PostgreSQL подключения
- `Redis` — настройки кэширования и pub/sub
- `TelegramBot` — токен и webhook URL
- `JWT` — аутентификация и авторизация
- `FileStorage` — локальное хранилище или AWS S3
- `BackgroundJobs` — Hangfire dashboard и воркеры

## Development Guidelines

### Staged Development Process
Проект разрабатывается по 10-этапному плану (`docs/План.md`). Текущий этап: **1 (завершен)**

**Важно:** Каждый этап должен завершаться тестированием работоспособности. Не переносить ошибки между этапами.

### Code Standards
- **Язык комментариев**: Русский
- **Документирование**: Обязательные XML-комментарии для всех публичных членов
- **Комментарии поясняют "почему", а не "что"**
- **Соблюдение спецификации**: Всегда сверяться с `docs/Спецификация.html`
- **Архитектурные принципы**: Следовать документации в `docs/Архитектура.md`

### Key Development Practices
- Использование CQRS через MediatR для разделения команд и запросов
- Pipeline behaviors для кросс-функциональности (логирование, валидация, кэширование)
- FluentValidation для валидации входных данных
- AutoMapper для маппинга между слоями
- Structured logging через Serilog
- Domain Events для межсервисного взаимодействия

### Testing Strategy
- Unit тесты для доменной логики
- Integration тесты для API endpoints
- Repository тесты с in-memory database
- GraphQL schema validation tests

## Important Notes

### Snapshots Implementation
При назначении потока пользователю система создает полную копию (snapshot) всего содержимого. Это гарантирует, что изменения в оригинальном потоке не влияют на пользователей, уже проходящих обучение.

### Real-time Features
SignalR используется для:
- Уведомления о прогрессе
- Buddy-dashboard updates  
- Системные уведомления
- Взаимодействие с наставниками

### Background Processing
Hangfire обрабатывает:
- Отложенные уведомления
- Проверки дедлайнов
- Генерация отчетов
- Синхронизация прогресса