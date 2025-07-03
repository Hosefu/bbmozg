namespace Lauf.Application.DTOs.Components;

/// <summary>
/// DTO для компонента задания (упрощенная версия)
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
    /// Содержимое задания
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Тип компонента
    /// </summary>
    public string Type { get; set; } = "TASK";

    /// <summary>
    /// Учитывать ли регистр
    /// </summary>
    public bool IsCaseSensitive { get; set; }
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