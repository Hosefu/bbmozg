using MediatR;
using Lauf.Domain.Events;

namespace Lauf.Application.EventHandlers.Events;

/// <summary>
/// MediatR уведомление для события назначения потока
/// </summary>
public record FlowAssignedNotification : INotification
{
    /// <summary>
    /// Доменное событие
    /// </summary>
    public FlowAssigned DomainEvent { get; init; }

    public FlowAssignedNotification(FlowAssigned domainEvent)
    {
        DomainEvent = domainEvent;
    }
}