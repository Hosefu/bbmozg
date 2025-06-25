using MediatR;
using Microsoft.Extensions.Logging;
using Lauf.Domain.Events;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Application.EventHandlers.Events;

namespace Lauf.Application.EventHandlers;

/// <summary>
/// Обработчик события назначения потока пользователю
/// </summary>
public class FlowAssignedEventHandler : INotificationHandler<FlowAssignedNotification>
{
    private readonly ILogger<FlowAssignedEventHandler> _logger;
    private readonly IUserProgressRepository _progressRepository;

    public FlowAssignedEventHandler(
        ILogger<FlowAssignedEventHandler> logger,
        IUserProgressRepository progressRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _progressRepository = progressRepository ?? throw new ArgumentNullException(nameof(progressRepository));
    }

    /// <summary>
    /// Обработка события назначения потока
    /// </summary>
    public async Task Handle(FlowAssignedNotification notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        _logger.LogInformation(
            "Обработка события назначения потока. AssignmentId: {AssignmentId}, UserId: {UserId}, FlowId: {FlowId}",
            domainEvent.AssignmentId,
            domainEvent.UserId,
            domainEvent.FlowId);

        try
        {
            // Создаем начальную запись прогресса пользователя
            await CreateInitialProgressAsync(domainEvent, cancellationToken);

            // Отправляем уведомление пользователю
            await SendNotificationAsync(domainEvent, cancellationToken);

            // Уведомляем бадди если назначен
            if (domainEvent.BuddyId.HasValue)
            {
                await NotifyBuddyAsync(domainEvent, cancellationToken);
            }

            _logger.LogInformation(
                "Событие назначения потока успешно обработано. AssignmentId: {AssignmentId}",
                domainEvent.AssignmentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Ошибка при обработке события назначения потока. AssignmentId: {AssignmentId}",
                domainEvent.AssignmentId);
            throw;
        }
    }

    /// <summary>
    /// Создание начальной записи прогресса
    /// </summary>
    private async Task CreateInitialProgressAsync(FlowAssigned @event, CancellationToken cancellationToken)
    {
        // Создание начального прогресса будет реализовано когда добавится сущность UserProgress
        _logger.LogDebug(
            "Создание начального прогресса для назначения {AssignmentId}",
            @event.AssignmentId);
        
        await Task.CompletedTask;
    }

    /// <summary>
    /// Отправка уведомления пользователю
    /// </summary>
    private async Task SendNotificationAsync(FlowAssigned @event, CancellationToken cancellationToken)
    {
        // Отправка уведомлений будет реализована через Notification Service
        _logger.LogInformation(
            "Отправка уведомления пользователю о назначении потока. UserId: {UserId}",
            @event.UserId);
        
        await Task.CompletedTask;
    }

    /// <summary>
    /// Уведомление бадди о новом подопечном
    /// </summary>
    private async Task NotifyBuddyAsync(FlowAssigned @event, CancellationToken cancellationToken)
    {
        // Уведомление бадди будет реализовано через Notification Service
        _logger.LogInformation(
            "Отправка уведомления бадди о новом назначении. BuddyId: {BuddyId}",
            @event.BuddyId);
        
        await Task.CompletedTask;
    }
}