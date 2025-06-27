using Lauf.Domain.Enums;

namespace Lauf.Application.DTOs.Flows;

/// <summary>
/// DTO для потока обучения
/// </summary>
public class FlowDto
{
    /// <summary>
    /// Идентификатор потока
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название потока
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Описание потока
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Теги потока
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Статус потока
    /// </summary>
    public FlowStatus Status { get; set; }

    /// <summary>
    /// Приоритет потока
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Обязательный ли поток
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Автор потока
    /// </summary>
    public Guid CreatedById { get; set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Дата публикации
    /// </summary>
    public DateTime? PublishedAt { get; set; }

    /// <summary>
    /// Общее количество шагов
    /// </summary>
    public int TotalSteps { get; set; }

    /// <summary>
    /// Настройки потока
    /// </summary>
    public FlowSettingsDto? Settings { get; set; }

    /// <summary>
    /// Шаги потока (только для детального просмотра)
    /// </summary>
    public List<FlowStepDto>? Steps { get; set; }
}

/// <summary>
/// DTO для настроек потока (упрощенный)
/// </summary>
public class FlowSettingsDto
{
    /// <summary>
    /// Дней на шаг
    /// </summary>
    public int DaysPerStep { get; set; } = 7;

    /// <summary>
    /// Требовать последовательное прохождение компонентов
    /// </summary>
    public bool RequireSequentialCompletionComponents { get; set; } = false;

    /// <summary>
    /// Разрешить самостоятельный перезапуск
    /// </summary>
    public bool AllowSelfRestart { get; set; } = false;

    /// <summary>
    /// Разрешить самостоятельную паузу
    /// </summary>
    public bool AllowSelfPause { get; set; } = true;
}