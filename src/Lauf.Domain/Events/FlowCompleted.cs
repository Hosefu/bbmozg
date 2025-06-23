namespace Lauf.Domain.Events;

/// <summary>
/// Событие завершения потока
/// </summary>
public record FlowCompleted : IDomainEvent
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
    /// ID прогресса потока
    /// </summary>
    public Guid FlowProgressId { get; init; }

    /// <summary>
    /// ID потока
    /// </summary>
    public Guid FlowId { get; init; }

    /// <summary>
    /// Название потока
    /// </summary>
    public string FlowTitle { get; init; } = string.Empty;

    /// <summary>
    /// ID снапшота потока
    /// </summary>
    public Guid FlowSnapshotId { get; init; }

    /// <summary>
    /// Дата начала прохождения
    /// </summary>
    public DateTime StartedAt { get; init; }

    /// <summary>
    /// Дата завершения
    /// </summary>
    public DateTime CompletedAt { get; init; }

    /// <summary>
    /// Дедлайн потока
    /// </summary>
    public DateTime DeadlineDate { get; init; }

    /// <summary>
    /// Был ли поток завершен в срок
    /// </summary>
    public bool CompletedOnTime { get; init; }

    /// <summary>
    /// Количество дней опоздания (если есть)
    /// </summary>
    public int DaysOverdue { get; init; }

    /// <summary>
    /// Общее время обучения в минутах
    /// </summary>
    public int TotalLearningTimeMinutes { get; init; }

    /// <summary>
    /// Количество завершенных шагов
    /// </summary>
    public int CompletedStepsCount { get; init; }

    /// <summary>
    /// Общее количество шагов
    /// </summary>
    public int TotalStepsCount { get; init; }

    /// <summary>
    /// Количество завершенных компонентов
    /// </summary>
    public int CompletedComponentsCount { get; init; }

    /// <summary>
    /// Общее количество компонентов
    /// </summary>
    public int TotalComponentsCount { get; init; }

    /// <summary>
    /// Финальный процент прогресса
    /// </summary>
    public decimal FinalProgressPercentage { get; init; }

    /// <summary>
    /// Был ли поток обязательным
    /// </summary>
    public bool WasRequired { get; init; }

    /// <summary>
    /// ID бадди (наставника)
    /// </summary>
    public Guid? BuddyId { get; init; }

    /// <summary>
    /// Статистика по типам компонентов
    /// </summary>
    public Dictionary<string, int> ComponentTypeStats { get; init; } = new();

    /// <summary>
    /// Дополнительные метаданные
    /// </summary>
    public Dictionary<string, object> Metadata { get; init; } = new();
}