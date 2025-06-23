namespace Lauf.Domain.Events;

/// <summary>
/// Событие назначения потока пользователю
/// </summary>
public record FlowAssigned : IDomainEvent
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
    /// ID назначения потока
    /// </summary>
    public Guid AssignmentId { get; init; }

    /// <summary>
    /// ID пользователя
    /// </summary>
    public Guid UserId { get; init; }

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
    /// Дедлайн выполнения
    /// </summary>
    public DateTime DeadlineDate { get; init; }

    /// <summary>
    /// Является ли поток обязательным
    /// </summary>
    public bool IsRequired { get; init; }

    /// <summary>
    /// ID бадди (наставника)
    /// </summary>
    public Guid? BuddyId { get; init; }

    /// <summary>
    /// Приоритет назначения
    /// </summary>
    public int Priority { get; init; }

    /// <summary>
    /// Дополнительные метаданные
    /// </summary>
    public Dictionary<string, object> Metadata { get; init; } = new();
}