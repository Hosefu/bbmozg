namespace Lauf.Application.DTOs.Components;

/// <summary>
/// DTO для компонента задания
/// </summary>
public class TaskComponentDto
{
    /// <summary>
    /// Уникальный идентификатор компонента
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название задания
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание задания
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Описание задания в формате Markdown
    /// </summary>
    public string TaskDescription { get; set; } = string.Empty;

    /// <summary>
    /// Критерии оценки задания
    /// </summary>
    public string AcceptanceCriteria { get; set; } = string.Empty;

    /// <summary>
    /// Подсказки для выполнения задания
    /// </summary>
    public List<TaskHintDto> Hints { get; set; } = new();

    /// <summary>
    /// Файлы-шаблоны для задания
    /// </summary>
    public List<TaskTemplateDto> Templates { get; set; } = new();

    /// <summary>
    /// Тестовые cases для проверки решения
    /// </summary>
    public List<TestCaseDto> TestCases { get; set; } = new();

    /// <summary>
    /// Ограничение по времени в секундах
    /// </summary>
    public int? TimeLimit { get; set; }

    /// <summary>
    /// Разрешить ли частичную оценку
    /// </summary>
    public bool AllowPartialCredit { get; set; }

    /// <summary>
    /// Включена ли валидация кода
    /// </summary>
    public bool CodeValidationEnabled { get; set; }

    /// <summary>
    /// Язык программирования для валидации
    /// </summary>
    public string? CodeLanguage { get; set; }

    /// <summary>
    /// Таймаут валидации кода в секундах
    /// </summary>
    public int CodeValidationTimeout { get; set; }

    /// <summary>
    /// Лимит памяти для валидации кода
    /// </summary>
    public string CodeMemoryLimit { get; set; } = string.Empty;

    /// <summary>
    /// Включены ли подсказки
    /// </summary>
    public bool HintsEnabled { get; set; }

    /// <summary>
    /// Максимальное количество подсказок
    /// </summary>
    public int MaxHints { get; set; }

    /// <summary>
    /// Штраф за каждую подсказку в баллах
    /// </summary>
    public int PenaltyPerHint { get; set; }

    /// <summary>
    /// Разрешить ли загрузку файлов
    /// </summary>
    public bool AllowFileUpload { get; set; }

    /// <summary>
    /// Максимальный размер файла
    /// </summary>
    public string MaxFileSize { get; set; } = string.Empty;

    /// <summary>
    /// Разрешенные расширения файлов
    /// </summary>
    public List<string> AllowedExtensions { get; set; } = new();

    /// <summary>
    /// Обязательна ли загрузка файлов
    /// </summary>
    public bool RequireFiles { get; set; }

    /// <summary>
    /// Включена ли автоматическая оценка
    /// </summary>
    public bool AutoGrade { get; set; }

    /// <summary>
    /// Требуется ли ручная проверка
    /// </summary>
    public bool ManualReview { get; set; }

    /// <summary>
    /// Шаблон обратной связи
    /// </summary>
    public string FeedbackTemplate { get; set; } = string.Empty;

    /// <summary>
    /// Разрешены ли обсуждения
    /// </summary>
    public bool AllowDiscussion { get; set; }

    /// <summary>
    /// Доступна ли помощь наставника
    /// </summary>
    public bool MentorHelp { get; set; }

    /// <summary>
    /// Категории задания
    /// </summary>
    public List<string> Categories { get; set; } = new();

    /// <summary>
    /// Уровень сложности задания
    /// </summary>
    public string Difficulty { get; set; } = string.Empty;

    /// <summary>
    /// Навыки, которые развивает задание
    /// </summary>
    public List<string> Skills { get; set; } = new();

    /// <summary>
    /// Статус компонента
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Приблизительное время выполнения в минутах
    /// </summary>
    public int EstimatedDurationMinutes { get; set; }

    /// <summary>
    /// Максимальное количество попыток
    /// </summary>
    public int? MaxAttempts { get; set; }

    /// <summary>
    /// Минимальный проходной балл
    /// </summary>
    public int? MinPassingScore { get; set; }

    /// <summary>
    /// Дополнительные инструкции
    /// </summary>
    public string Instructions { get; set; } = string.Empty;

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO для подсказки задания
/// </summary>
public class TaskHintDto
{
    /// <summary>
    /// Уникальный идентификатор подсказки
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Текст подсказки
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Порядковый номер подсказки
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Штраф за использование подсказки
    /// </summary>
    public int Penalty { get; set; }

    /// <summary>
    /// Доступна ли подсказка после определенного времени
    /// </summary>
    public int? AvailableAfterMinutes { get; set; }
}

/// <summary>
/// DTO для шаблона файла задания
/// </summary>
public class TaskTemplateDto
{
    /// <summary>
    /// Уникальный идентификатор шаблона
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название шаблона
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Описание шаблона
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Содержимое шаблона
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Тип файла
    /// </summary>
    public string FileType { get; set; } = string.Empty;

    /// <summary>
    /// Язык программирования (если применимо)
    /// </summary>
    public string? Language { get; set; }

    /// <summary>
    /// Обязательный ли шаблон
    /// </summary>
    public bool IsRequired { get; set; }
}

/// <summary>
/// DTO для тестового case задания
/// </summary>
public class TestCaseDto
{
    /// <summary>
    /// Уникальный идентификатор тестового case
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название тестового case
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Описание тестового case
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Входные данные для теста
    /// </summary>
    public string Input { get; set; } = string.Empty;

    /// <summary>
    /// Ожидаемый результат
    /// </summary>
    public string ExpectedOutput { get; set; } = string.Empty;

    /// <summary>
    /// Обязательный ли тестовый case
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Активен ли тестовый case
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Видимый ли тестовый case для студента
    /// </summary>
    public bool IsVisible { get; set; }

    /// <summary>
    /// Вес тестового case в общей оценке
    /// </summary>
    public int Weight { get; set; }

    /// <summary>
    /// Таймаут выполнения теста в секундах
    /// </summary>
    public int? TimeoutSeconds { get; set; }
}