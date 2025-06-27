# Схема БД для архитектуры полного версионирования

## Основные версионные таблицы

### FlowVersions
```sql
CREATE TABLE FlowVersions (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    OriginalFlowId UNIQUEIDENTIFIER NOT NULL,
    Version INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 0,
    
    -- Основные поля Flow
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000) NOT NULL,
    Tags NVARCHAR(MAX) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Draft',
    Priority INT NOT NULL,
    IsRequired BIT NOT NULL,
    CreatedById UNIQUEIDENTIFIER NOT NULL,
    
    -- Аудит
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NOT NULL,
    PublishedAt DATETIME2 NULL,
    
    -- Ограничения
    CONSTRAINT FK_FlowVersions_Users_CreatedById FOREIGN KEY (CreatedById) REFERENCES Users(Id),
    CONSTRAINT UQ_FlowVersions_OriginalId_Version UNIQUE (OriginalFlowId, Version),
    CONSTRAINT UQ_FlowVersions_OriginalId_Active UNIQUE (OriginalFlowId, IsActive) WHERE IsActive = 1
);
```

### FlowStepVersions
```sql
CREATE TABLE FlowStepVersions (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    OriginalStepId UNIQUEIDENTIFIER NOT NULL,
    Version INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 0,
    FlowVersionId UNIQUEIDENTIFIER NOT NULL,
    
    -- Основные поля FlowStep
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000) NOT NULL,
    [Order] NVARCHAR(50) NOT NULL,
    IsRequired BIT NOT NULL,
    EstimatedDurationMinutes INT NOT NULL,
    Status INT NOT NULL,
    Instructions NVARCHAR(2000) NOT NULL,
    Notes NVARCHAR(MAX) NOT NULL,
    
    -- Аудит
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NOT NULL,
    
    -- Ограничения
    CONSTRAINT FK_FlowStepVersions_FlowVersions FOREIGN KEY (FlowVersionId) REFERENCES FlowVersions(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_FlowStepVersions_OriginalId_Version UNIQUE (OriginalStepId, Version),
    CONSTRAINT UQ_FlowStepVersions_OriginalId_Active UNIQUE (OriginalStepId, IsActive) WHERE IsActive = 1
);
```

### ComponentVersions
```sql
CREATE TABLE ComponentVersions (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    OriginalComponentId UNIQUEIDENTIFIER NOT NULL,
    Version INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 0,
    StepVersionId UNIQUEIDENTIFIER NOT NULL,
    
    -- Основные поля Component
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000) NOT NULL,
    ComponentType NVARCHAR(50) NOT NULL, -- 'Article', 'Quiz', 'Task'
    Status NVARCHAR(50) NOT NULL DEFAULT 'Draft',
    [Order] NVARCHAR(50) NOT NULL,
    IsRequired BIT NOT NULL DEFAULT 1,
    EstimatedDurationMinutes INT NOT NULL DEFAULT 15,
    MaxAttempts INT NULL,
    MinPassingScore INT NULL,
    Instructions NVARCHAR(2000) NOT NULL,
    
    -- Аудит
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NOT NULL,
    
    -- Ограничения
    CONSTRAINT FK_ComponentVersions_FlowStepVersions FOREIGN KEY (StepVersionId) REFERENCES FlowStepVersions(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_ComponentVersions_OriginalId_Version UNIQUE (OriginalComponentId, Version),
    CONSTRAINT UQ_ComponentVersions_OriginalId_Active UNIQUE (OriginalComponentId, IsActive) WHERE IsActive = 1
);
```

## Специализированные таблицы контента

### ArticleComponentVersions
```sql
CREATE TABLE ArticleComponentVersions (
    ComponentVersionId UNIQUEIDENTIFIER PRIMARY KEY,
    Content NVARCHAR(MAX) NOT NULL,
    ReadingTimeMinutes INT NOT NULL DEFAULT 15,
    
    CONSTRAINT FK_ArticleComponentVersions_ComponentVersions FOREIGN KEY (ComponentVersionId) REFERENCES ComponentVersions(Id) ON DELETE CASCADE
);
```

### QuizComponentVersions
```sql
CREATE TABLE QuizComponentVersions (
    ComponentVersionId UNIQUEIDENTIFIER PRIMARY KEY,
    PassingScore INT NOT NULL DEFAULT 80,
    TimeLimitMinutes INT NULL,
    AllowMultipleAttempts BIT NOT NULL DEFAULT 1,
    ShowCorrectAnswers BIT NOT NULL DEFAULT 1,
    ShuffleQuestions BIT NOT NULL DEFAULT 0,
    ShuffleAnswers BIT NOT NULL DEFAULT 0,
    
    CONSTRAINT FK_QuizComponentVersions_ComponentVersions FOREIGN KEY (ComponentVersionId) REFERENCES ComponentVersions(Id) ON DELETE CASCADE
);
```

### QuizOptionVersions
```sql
CREATE TABLE QuizOptionVersions (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    QuizVersionId UNIQUEIDENTIFIER NOT NULL,
    Text NVARCHAR(500) NOT NULL,
    IsCorrect BIT NOT NULL DEFAULT 0,
    Points INT NOT NULL DEFAULT 0,
    [Order] INT NOT NULL,
    Explanation NVARCHAR(1000) NULL,
    
    CONSTRAINT FK_QuizOptionVersions_QuizComponentVersions FOREIGN KEY (QuizVersionId) REFERENCES QuizComponentVersions(ComponentVersionId) ON DELETE CASCADE
);
```

### TaskComponentVersions
```sql
CREATE TABLE TaskComponentVersions (
    ComponentVersionId UNIQUEIDENTIFIER PRIMARY KEY,
    Instructions NVARCHAR(MAX) NOT NULL,
    SubmissionType NVARCHAR(50) NOT NULL DEFAULT 'Text', -- 'Text', 'File', 'Link'
    MaxFileSize INT NULL, -- в байтах
    AllowedFileTypes NVARCHAR(200) NULL, -- "pdf,doc,docx"
    RequiresMentorApproval BIT NOT NULL DEFAULT 1,
    AutoApprovalKeywords NVARCHAR(MAX) NULL,
    
    CONSTRAINT FK_TaskComponentVersions_ComponentVersions FOREIGN KEY (ComponentVersionId) REFERENCES ComponentVersions(Id) ON DELETE CASCADE
);
```

## Обновленная таблица FlowAssignments

### FlowAssignments (с версиями)
```sql
CREATE TABLE FlowAssignments (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    OriginalFlowId UNIQUEIDENTIFIER NOT NULL, -- ссылка на оригинальный Flow
    FlowVersionId UNIQUEIDENTIFIER NOT NULL, -- ссылка на конкретную версию
    BuddyId UNIQUEIDENTIFIER NULL,
    AssignedById UNIQUEIDENTIFIER NOT NULL,
    
    -- Статус и прогресс
    Status NVARCHAR(50) NOT NULL DEFAULT 'Assigned',
    Priority INT NOT NULL,
    
    -- Временные метки
    AssignedAt DATETIME2 NOT NULL,
    StartedAt DATETIME2 NULL,
    DueDate DATETIME2 NULL,
    CompletedAt DATETIME2 NULL,
    LastActivityAt DATETIME2 NULL,
    PausedAt DATETIME2 NULL,
    PauseReason NVARCHAR(500) NULL,
    
    -- Прогресс
    ProgressPercent INT NOT NULL DEFAULT 0,
    CompletedSteps INT NOT NULL DEFAULT 0,
    TotalSteps INT NOT NULL DEFAULT 0,
    AttemptCount INT NOT NULL DEFAULT 0,
    FinalScore INT NULL,
    
    -- Метаданные
    AdminNotes NVARCHAR(MAX) NOT NULL DEFAULT '',
    UserFeedback NVARCHAR(MAX) NOT NULL DEFAULT '',
    UserRating INT NULL,
    
    -- Аудит
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NOT NULL,
    
    -- Ограничения
    CONSTRAINT FK_FlowAssignments_Users_UserId FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    CONSTRAINT FK_FlowAssignments_Users_BuddyId FOREIGN KEY (BuddyId) REFERENCES Users(Id) ON DELETE SET NULL,
    CONSTRAINT FK_FlowAssignments_Users_AssignedById FOREIGN KEY (AssignedById) REFERENCES Users(Id) ON DELETE RESTRICT,
    CONSTRAINT FK_FlowAssignments_FlowVersions FOREIGN KEY (FlowVersionId) REFERENCES FlowVersions(Id) ON DELETE RESTRICT
);
```

## Индексы для производительности

```sql
-- Индексы для быстрого поиска активных версий
CREATE INDEX IX_FlowVersions_OriginalId_Active ON FlowVersions(OriginalFlowId, IsActive) WHERE IsActive = 1;
CREATE INDEX IX_FlowStepVersions_OriginalId_Active ON FlowStepVersions(OriginalStepId, IsActive) WHERE IsActive = 1;
CREATE INDEX IX_ComponentVersions_OriginalId_Active ON ComponentVersions(OriginalComponentId, IsActive) WHERE IsActive = 1;

-- Индексы для версионных запросов
CREATE INDEX IX_FlowVersions_OriginalId_Version ON FlowVersions(OriginalFlowId, Version);
CREATE INDEX IX_FlowStepVersions_OriginalId_Version ON FlowStepVersions(OriginalStepId, Version);
CREATE INDEX IX_ComponentVersions_OriginalId_Version ON ComponentVersions(OriginalComponentId, Version);

-- Индексы для иерархических запросов
CREATE INDEX IX_FlowStepVersions_FlowVersionId ON FlowStepVersions(FlowVersionId);
CREATE INDEX IX_ComponentVersions_StepVersionId ON ComponentVersions(StepVersionId);

-- Индексы для назначений
CREATE INDEX IX_FlowAssignments_UserId_Status ON FlowAssignments(UserId, Status);
CREATE INDEX IX_FlowAssignments_FlowVersionId ON FlowAssignments(FlowVersionId);
CREATE INDEX IX_FlowAssignments_AssignedAt ON FlowAssignments(AssignedAt);
```

## Ключевые особенности

### 1. Версионирование
- Каждая сущность имеет `OriginalId` + `Version` + `IsActive`
- Уникальные ограничения на (OriginalId, Version) и (OriginalId, IsActive)
- Каскадное удаление версий при удалении родительских версий

### 2. Специализация контента
- Отдельные таблицы для ArticleComponent, QuizComponent, TaskComponent
- Связь через ComponentVersionId (1:1)
- Типизированные поля вместо JSON

### 3. Назначения
- Ссылаются на конкретные версии FlowVersions
- Сохраняют OriginalFlowId для удобства запросов
- Неизменяемый контент после назначения

### 4. Производительность
- Индексы на активные версии
- Индексы на версионные запросы
- Правильные FK для JOIN'ов