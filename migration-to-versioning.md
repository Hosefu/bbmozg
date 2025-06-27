# Миграция на архитектуру полного версионирования

## Концепция новой архитектуры

### Принципы:
- **Все версии хранятся физически** в структурированных таблицах
- **Один активный is_active = true** на каждую сущность
- **Снапшоты = ссылки на конкретные версии** компонентов
- **Никакого JSON** - только типизированные поля
- **Каскадное версионирование** - изменение компонента создает новую версию всей иерархии

Договор о версиях — все версии всегда и везде — целое число. Отсчёт начинается с 1. Любое изменение сущности приводит к копии с версией +1


## План миграции

### Этап 1: Проектирование новой схемы БД
- [ ] **1.1** Спроектировать новые таблицы с версионированием
  - `FlowVersions (id, original_flow_id, version, is_active, title, description, ...)`
  - `FlowStepVersions (id, original_step_id, version, is_active, flow_version_id, ...)`
  - `ComponentVersions (id, original_component_id, version, is_active, step_version_id, ...)`
- [ ] **1.2** Спроектировать специализированные таблицы контента
  - `ArticleComponentVersions (component_version_id, content, reading_time_minutes)`
  - `QuizComponentVersions (component_version_id, passing_score, time_limit_minutes)`
  - `QuizOptionVersions (id, quiz_version_id, text, is_correct, points, order)`
  - `TaskComponentVersions (component_version_id, instructions, submission_type, max_file_size)`
- [ ] **1.3** Обновить FlowAssignments для ссылок на версии
  - `FlowAssignments.flow_version_id` вместо FlowSnapshotId
- [ ] **1.4** Создать индексы для производительности
  - Составные индексы (original_id, version)
  - Индексы на is_active поля

### Этап 2: Создание новых доменных сущностей
- [ ] **2.1** Создать базовые версионные entity
  - `FlowVersion`, `FlowStepVersion`, `ComponentVersion`
  - Базовый интерфейс `IVersionedEntity<T>`
- [ ] **2.2** Создать специализированные компонент entity
  - `ArticleComponentVersion`, `QuizComponentVersion`, `TaskComponentVersion`
  - Связи через ComponentVersion
- [ ] **2.3** Обновить агрегаты и правила версионирования
  - Логика создания новых версий при изменениях
  - Правила каскадного версионирования
- [ ] **2.4** Создать Value Objects для версий
  - `Version`, `VersionedId<T>` для типобезопасности

### Этап 3: Обновление слоя данных
- [ ] **3.1** Создать EF Core конфигурации для новых сущностей
  - TPT настройки для ComponentVersion наследования
  - Правильные FK и индексы
- [ ] **3.2** Создать новые репозитории
  - `IFlowVersionRepository`, `IComponentVersionRepository`
  - Методы GetActiveVersion(), GetVersion(int), CreateNewVersion()
- [ ] **3.3** Создать сервис версионирования
  - `VersioningService` для создания новых версий
  - Логика каскадного обновления версий
- [ ] **3.4** Обновить ApplicationDbContext
  - Добавить новые DbSet
  - Обновить OnModelCreating

### Этап 4: Обновление Application слоя
- [ ] **4.1** Создать новые Commands/Queries
  - `CreateFlowVersionCommand`, `UpdateComponentVersionCommand`
  - `GetFlowVersionQuery`, `GetActiveFlowQuery`
- [ ] **4.2** Обновить существующие handlers
  - CreateFlowCommandHandler - создавать первую версию
  - UpdateFlowCommandHandler - создавать новую версию
  - AssignFlowCommandHandler - ссылаться на версии
- [ ] **4.3** Обновить DTOs и AutoMapper профили
  - FlowVersionDto, ComponentVersionDto
  - Маппинг между версиями и DTO
- [ ] **4.4** Обновить валидаторы
  - Проверки версий, активных статусов

### Этап 5: Обновление API слоя
- [ ] **5.1** Обновить GraphQL типы и резолверы
  - FlowVersionType, ComponentVersionType
  - Запросы GetFlow возвращают активные версии
- [ ] **5.2** Обновить мутации
  - CreateFlow создает v1, UpdateFlow создает новую версию
  - Параметры версий в запросах (опционально)
- [ ] **5.3** Обновить SignalR хабы
  - Уведомления о новых версиях
  - Прогресс относительно зафиксированных версий

### Этап 6: Миграция данных не нужна. Поддержка старых данных не требуется.

### Этап 7: Обновление бизнес-логики
- [ ] **7.1** Обновить FlowAssignment логику
  - Создание assignment с версиями вместо снапшотов
  - Получение контента по зафиксированным версиям
- [ ] **7.2** Обновить прогресс трекинг
  - FlowProgress работает с версиями компонентов
  - UserProgress учитывает версии
- [ ] **7.3** Обновить уведомления
  - Отправка уведомлений относительно версий
  - История версий в уведомлениях

### Этап 8: Обновление тестов
- [ ] **8.1** Создать unit тесты для версионирования
  - Тесты создания версий
  - Тесты каскадного версионирования
- [ ] **8.2** Обновить integration тесты
  - Тесты API с версиями
  - Тесты миграции данных
- [ ] **8.3** Обновить E2E тесты
  - Тесты полного жизненного цикла с версиями
  - Тесты assignment с версиями

### Этап 9: Производительность и оптимизация
- [ ] **9.1** Создать индексы для быстрых запросов
  - Индексы на (original_id, is_active)
  - Индексы на (original_id, version)
- [ ] **9.2** Оптимизировать запросы
  - Batch loading для версий
  - Кэширование активных версий
- [ ] **9.3** Создать архивацию старых версий
  - Политики хранения версий
  - Скрипты очистки неиспользуемых версий

### Этап 10: Финализация
- [ ] **10.1** Удалить старый snapshot код
  - Удалить FlowSnapshot, FlowStepSnapshot, ComponentSnapshot entities
  - Удалить FlowSnapshotService и репозитории
  - Очистить неиспользуемые миграции
- [ ] **10.3** Финальное тестирование
  - Полный регресс всех функций
  - Нагрузочное тестирование
  - Проверка миграции на staging

## Критические точки

### Обратная совместимость
- Graceful degradation при проблемах с версиями

### Производительность  
- Версионные таблицы будут расти быстро
- Нужна стратегия архивации и индексирования

### Целостность данных
- Каскадное версионирование может создать много версий
- Нужны правила когда создавать новые версии

