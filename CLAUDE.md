# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Лауф (Lauf)** — корпоративная система онбординга сотрудников через Telegram-бота и веб-приложение.

**Ключевые архитектурные концепции:**
- **Потоки (Flows)**: Шаблоны обучающих программ
- **Назначения (Assignments)**: Привязка пользователя к потоку с дедлайнами
- **Наставники (Mentors)**: Кураторы, курирующие прохождение
- **Версионирование (Versioning)**: Полная система версий для потоков, этапов и компонентов с физическим хранением в БД (заменила snapshot архитектуру)
- **Компоненты (Components)**: Атомарные единицы контента (статьи, тесты, задания)

## Development Commands

### Build and Run
```bash
# Restore packages
dotnet restore

# Build solution
dotnet build

# Run API server (GraphQL Playground available at /playground)
cd src/Lauf.Api
dotnet run --urls="http://localhost:5000"

# Clean build artifacts
dotnet clean
```

### Testing
```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/Lauf.Domain.Tests/

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run comprehensive tests with HTML reports
./scripts/run-tests-with-coverage.sh

# Frontend commands
cd frontend

# Install dependencies
npm install

# Run development server
npm run dev

# Build for production
npm run build

# Type checking
npm run type-check

# Linting
npm run lint          # ESLint
npm run lint:scss     # Stylelint for SCSS
npm run lint:all      # All linting tools + type check

# Formatting
npm run format        # Format code with Prettier
npm run format:check  # Check formatting

# Fix issues
npm run fix:all       # Fix all linting and formatting issues

# Complete check
npm run check:all     # Run all checks (type, lint, format)

# Run single test method
dotnet test --filter "FullyQualifiedName~TestMethodName"
```

### Database Operations
```bash
# Add migration (use proper PATH for EF tools)
export PATH="$PATH:$HOME/.dotnet/tools" && dotnet ef migrations add <MigrationName> --project src/Lauf.Infrastructure --startup-project src/Lauf.Api

# Update database
export PATH="$PATH:$HOME/.dotnet/tools" && dotnet ef database update --project src/Lauf.Infrastructure --startup-project src/Lauf.Api

# Generate SQL script
export PATH="$PATH:$HOME/.dotnet/tools" && dotnet ef migrations script --project src/Lauf.Infrastructure --startup-project src/Lauf.Api
```

## Architecture

### Technology Stack (.NET 9.0)
- **ASP.NET Core 9.0** + **C# 12** — основная платформа
- **GraphQL (HotChocolate 13.7)** — API с автогенерацией схемы, Playground UI
- **SQLite + Entity Framework Core 9.0** — база данных с Code-First подходом (локальная разработка)
- **SignalR** — real-time коммуникация (NotificationHub, ProgressHub)
- **MediatR** — CQRS с командами/запросами и pipeline behaviors
- **In-Memory Cache/File Storage** — кэширование и файловое хранилище (production-ready заглушки)
- **Background Jobs** — планировщик заданий (memory implementation)
- **xUnit + FluentAssertions** — тестирование с автогенерацией отчётов

### Clean Architecture Structure
```
src/
├── Lauf.Api/          # API слой (GraphQL, SignalR, Controllers)
├── Lauf.Application/  # Логика приложения (CQRS, MediatR, DTOs)
├── Lauf.Infrastructure/ # Внешние зависимости (БД, Telegram, файлы)
├── Lauf.Domain/       # Доменная логика и сущности
└── Lauf.Shared/       # Общие утилиты и константы
```

**Направление зависимостей:** API → Application → Domain ← Infrastructure
- Domain — независимый центральный слой
- Application зависит только от Domain
- Infrastructure реализует интерфейсы из Domain
- API использует Application и Infrastructure

### Key Domain Patterns
- **Versioning Pattern**: Полное физическое версионирование сущностей в БД (FlowVersion, ComponentVersion) с каскадным созданием версий
- **Domain Events**: Событийно-ориентированная архитектура для уведомлений и прогресса
- **Value Objects**: Типизированные значения (TelegramUserId, ProgressPercentage)
- **Repository Pattern**: Абстракция доступа к данным через интерфейсы
- **CQRS**: Разделение команд и запросов через MediatR

### Configuration Structure
Приложение использует стандартную конфигурацию ASP.NET Core:
- **Development**: appsettings.Development.json (локальные настройки, увеличенное логирование)
- **Production**: appsettings.json (продакшн настройки)

Основные конфигурационные секции:
- `ConnectionStrings` — SQLite подключения (локальная разработка)
- `Redis` — настройки кэширования и pub/sub
- `TelegramBot` — токен и webhook URL
- `JWT` — аутентификация и авторизация
- `FileStorage` — локальное хранилище или AWS S3
- `BackgroundJobs` — Hangfire dashboard и воркеры

## Development Guidelines

### Development Status
Проект завершен согласно 10-этапному плану (`docs/План.md`). **Все этапы 1-10 реализованы.**

**Текущее состояние:**
- ✅ Архитектура версионирования полностью реализована (заменила snapshot систему)
- ✅ Полная функциональность без ошибок сборки
- ✅ Новая миграция InitialVersioningArchitecture создана с нуля
- ✅ Версионирование прозрачно для API (скрыто от фронтенда)

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
- AutoMapper для маппинга между слоями с прозрачным версионированием
- Structured logging через Serilog
- Domain Events для межсервисного взаимодействия

### Versioning System Structure
```
Domain.Entities.Versions/
├── FlowVersion.cs          # Версия потока с метаданными
├── FlowStepVersion.cs      # Версия этапа потока
├── ComponentVersion.cs     # Базовая версия компонента
├── ArticleComponentVersion.cs  # Версия статьи
├── QuizComponentVersion.cs     # Версия теста
└── TaskComponentVersion.cs     # Версия задания

Domain.Services/
└── IVersioningService.cs   # Сервис управления версиями

Application.Mappings/
└── VersioningMappingProfile.cs  # Прозрачный маппинг версий в DTO
```

### Testing Strategy
- Unit тесты для доменной логики
- Integration тесты для API endpoints
- Repository тесты с in-memory database
- GraphQL schema validation tests

## Important Notes

### Versioning Architecture
Система использует полное физическое версионирование вместо snapshot подхода:
- **FlowVersion, FlowStepVersion, ComponentVersion** — отдельные таблицы для каждой версии сущности
- **Каскадное версионирование**: при создании новой версии потока создаются версии всех этапов и компонентов
- **Transparent API**: версионирование полностью скрыто от фронтенда через AutoMapper профили
- **IVersioningService**: центральный сервис для управления жизненным циклом версий
- **Immutable Assignments**: FlowAssignment ссылается на конкретную FlowVersionId для неизменности контента

### Real-time Features
SignalR используется для:
- Уведомления о прогрессе
- Dashboard обновления для наставников  
- Системные уведомления
- Взаимодействие с наставниками

### Background Processing
Background Job Service обрабатывает:
- Отложенные уведомления
- Проверки дедлайнов
- Генерация отчетов
- Синхронизация прогресса

### API Endpoints
- **REST API:** `/api/users`, `/api/health` 
- **GraphQL:** `/graphql` (с Playground UI)
- **SignalR Hubs:** `/hubs/notifications`, `/hubs/progress`
- **Documentation:** `/docs`, `/playground`, `/voyager`

### Critical Domain Concepts
- **Versioning Pattern:** FlowAssignment ссылается на FlowVersionId для получения неизменяемого контента
- **Progressive Disclosure:** Шаги разблокируются по мере прохождения (если RequireSequentialCompletion=true)
- **Component-Based Content:** Статьи, тесты, задания как атомарные компоненты с TPT наследованием
- **Version Transparency:** API всегда возвращает активные версии, скрывая детали версионирования от клиентов
- **Deadline Calculation:** Рабочие дни с учетом праздников и рабочих часов