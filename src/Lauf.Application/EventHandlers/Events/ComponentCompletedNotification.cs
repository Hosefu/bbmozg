using MediatR;
using Lauf.Domain.Events;

namespace Lauf.Application.EventHandlers.Events;

/// <summary>
/// MediatR уведомление для события завершения компонента
/// </summary>
public record ComponentCompletedNotification : INotification
{
    /// <summary>
    /// Доменное событие
    /// </summary>
    public ComponentCompleted DomainEvent { get; init; }

    public ComponentCompletedNotification(ComponentCompleted domainEvent)
    {
        DomainEvent = domainEvent;
    }
}