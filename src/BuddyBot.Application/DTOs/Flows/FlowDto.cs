using BuddyBot.Domain.Enums;

namespace BuddyBot.Application.DTOs.Flows;

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
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание потока
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Категория потока
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Теги потока
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Статус потока
    /// </summary>
    public FlowStatus Status { get; set; }

    /// <summary>
    /// Версия потока
    /// </summary>
    public int Version { get; set; }

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
    /// Приблизительное время прохождения в минутах
    /// </summary>
    public int EstimatedDurationMinutes { get; set; }

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
/// DTO для настроек потока
/// </summary>
public class FlowSettingsDto
{
    /// <summary>
    /// Разрешить пропуск шагов
    /// </summary>
    public bool AllowSkipping { get; set; }

    /// <summary>
    /// Требовать последовательное прохождение
    /// </summary>
    public bool RequireSequentialCompletion { get; set; }

    /// <summary>
    /// Максимальное количество попыток
    /// </summary>
    public int? MaxAttempts { get; set; }

    /// <summary>
    /// Время на выполнение в рабочих днях
    /// </summary>
    public int? TimeToCompleteWorkingDays { get; set; }

    /// <summary>
    /// Показывать прогресс
    /// </summary>
    public bool ShowProgress { get; set; }

    /// <summary>
    /// Разрешить повторное прохождение
    /// </summary>
    public bool AllowRetry { get; set; }

    /// <summary>
    /// Отправлять напоминания
    /// </summary>
    public bool SendReminders { get; set; }

    /// <summary>
    /// Дополнительные настройки
    /// </summary>
    public Dictionary<string, object> AdditionalSettings { get; set; } = new();
}