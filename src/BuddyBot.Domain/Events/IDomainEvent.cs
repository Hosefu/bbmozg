namespace BuddyBot.Domain.Events;

/// <summary>
/// Интерфейс доменного события
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Уникальный идентификатор события
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// Время возникновения события
    /// </summary>
    DateTime OccurredAt { get; }

    /// <summary>
    /// Версия события (для совместимости)
    /// </summary>
    int Version { get; }
}