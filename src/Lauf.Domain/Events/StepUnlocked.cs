namespace Lauf.Domain.Events;

/// <summary>
/// Событие разблокировки нового шага
/// </summary>
public record StepUnlocked : IDomainEvent
{
    /// <summary>
    /// Уникальный идентификатор события
    /// </summary>
    public Guid EventId { get; } = Guid.NewGuid();

    /// <summary>
    /// Время возникновения события
    /// </summary>
    public DateTime OccurredAt { get; } = DateTime.UtcNow;

    /// <summary>
    /// Версия события
    /// </summary>
    public int Version { get; } = 1;

    /// <summary>
    /// ID пользователя
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// ID назначения потока
    /// </summary>
    public Guid AssignmentId { get; init; }

    /// <summary>
    /// ID прогресса шага
    /// </summary>
    public Guid StepProgressId { get; init; }

    /// <summary>
    /// ID снапшота шага
    /// </summary>
    public Guid StepSnapshotId { get; init; }

    /// <summary>
    /// Название шага
    /// </summary>
    public string StepTitle { get; init; } = string.Empty;

    /// <summary>
    /// Порядковый номер шага
    /// </summary>
    public int StepOrder { get; init; }

    /// <summary>
    /// Количество компонентов в шаге
    /// </summary>
    public int ComponentsCount { get; init; }

    /// <summary>
    /// Расчетное время выполнения шага в минутах
    /// </summary>
    public int EstimatedMinutes { get; init; }

    /// <summary>
    /// ID потока
    /// </summary>
    public Guid FlowId { get; init; }

    /// <summary>
    /// Название потока
    /// </summary>
    public string FlowTitle { get; init; } = string.Empty;

    /// <summary>
    /// ID предыдущего шага (который был завершен)
    /// </summary>
    public Guid? PreviousStepId { get; init; }

    /// <summary>
    /// Название предыдущего шага
    /// </summary>
    public string? PreviousStepTitle { get; init; }

    /// <summary>
    /// Общий прогресс по потоку после разблокировки
    /// </summary>
    public decimal FlowProgressPercentage { get; init; }

    /// <summary>
    /// Является ли шаг последним в потоке
    /// </summary>
    public bool IsLastStep { get; init; }
}