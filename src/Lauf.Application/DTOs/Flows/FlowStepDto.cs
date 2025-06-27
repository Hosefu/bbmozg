using Lauf.Domain.Enums;

namespace Lauf.Application.DTOs.Flows;

/// <summary>
/// DTO для шага потока
/// </summary>
public class FlowStepDto
{
    /// <summary>
    /// Идентификатор шага
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор потока
    /// </summary>
    public Guid FlowId { get; set; }

    /// <summary>
    /// Название шага
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание шага
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Порядковый номер шага (для фронтенда, начиная с 0)
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Обязательный ли шаг
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Приблизительное время выполнения в минутах
    /// </summary>
    public int EstimatedDurationMinutes { get; set; }

    /// <summary>
    /// Статус шага
    /// </summary>
    public StepStatus Status { get; set; }

    /// <summary>
    /// Инструкции для прохождения шага
    /// </summary>
    public string Instructions { get; set; } = string.Empty;

    /// <summary>
    /// Дополнительные заметки
    /// </summary>
    public string Notes { get; set; } = string.Empty;

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Общее количество компонентов
    /// </summary>
    public int TotalComponents { get; set; }

    /// <summary>
    /// Количество обязательных компонентов
    /// </summary>
    public int RequiredComponents { get; set; }

    /// <summary>
    /// Компоненты шага (только для детального просмотра)
    /// </summary>
    public List<FlowStepComponentDto>? Components { get; set; }
}

/// <summary>
/// DTO для компонента шага
/// </summary>
public class FlowStepComponentDto
{
    /// <summary>
    /// Идентификатор компонента
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор шага
    /// </summary>
    public Guid FlowStepId { get; set; }

    /// <summary>
    /// Идентификатор связанного компонента контента
    /// </summary>
    public Guid ComponentId { get; set; }

    /// <summary>
    /// Тип компонента
    /// </summary>
    public ComponentType ComponentType { get; set; }

    /// <summary>
    /// Название компонента
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание компонента
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Порядковый номер компонента (для фронтенда, начиная с 0)
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Обязательный ли компонент
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Статус компонента
    /// </summary>
    public ComponentStatus Status { get; set; }

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
    /// Настройки компонента
    /// </summary>
    public Dictionary<string, object> Settings { get; set; } = new();

    /// <summary>
    /// Инструкции для компонента
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

    /// <summary>
    /// Данные компонента (статья, квиз или задание)
    /// </summary>
    public object? Component { get; set; }
}