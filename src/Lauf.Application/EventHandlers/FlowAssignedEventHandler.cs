using MediatR;
using Microsoft.Extensions.Logging;
using Lauf.Domain.Events;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Domain.Interfaces.Services;
using Lauf.Application.EventHandlers.Events;

namespace Lauf.Application.EventHandlers;

/// <summary>
/// Обработчик события назначения потока пользователю
/// </summary>
public class FlowAssignedEventHandler : INotificationHandler<FlowAssignedNotification>
{
    private readonly ILogger<FlowAssignedEventHandler> _logger;
    private readonly IUserProgressRepository _progressRepository;
    private readonly INotificationService _notificationService;
    private readonly IFlowRepository _flowRepository;
    private readonly IFlowAssignmentRepository _assignmentRepository;
    public FlowAssignedEventHandler(
        ILogger<FlowAssignedEventHandler> logger,
        IUserProgressRepository progressRepository,
        INotificationService notificationService,
        IFlowRepository flowRepository,
        IFlowAssignmentRepository assignmentRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _progressRepository = progressRepository ?? throw new ArgumentNullException(nameof(progressRepository));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _flowRepository = flowRepository ?? throw new ArgumentNullException(nameof(flowRepository));
        _assignmentRepository = assignmentRepository ?? throw new ArgumentNullException(nameof(assignmentRepository));
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
        try
        {
            // Получаем назначение потока
            var assignment = await _assignmentRepository.GetByIdAsync(@event.AssignmentId, cancellationToken);
            if (assignment == null)
            {
                _logger.LogWarning("Назначение потока {AssignmentId} не найдено", @event.AssignmentId);
                return;
            }

            // Новая архитектура - FlowAssignmentProgress создается при создании назначения
            _logger.LogInformation(
                "Начальный прогресс для назначения {AssignmentId} будет создан автоматически",
                @event.AssignmentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Ошибка создания начального прогресса для назначения {AssignmentId}",
                @event.AssignmentId);
            throw;
        }
    }

    /// <summary>
    /// Отправка уведомления пользователю
    /// </summary>
    private async Task SendNotificationAsync(FlowAssigned @event, CancellationToken cancellationToken)
    {
        try
        {
            // Получаем информацию о потоке
            var flow = await _flowRepository.GetByIdAsync(@event.FlowId, cancellationToken);
            if (flow == null)
            {
                _logger.LogWarning("Поток {FlowId} не найден для отправки уведомления", @event.FlowId);
                return;
            }

            // Отправляем уведомление о назначении потока
            await _notificationService.NotifyFlowAssignedAsync(
                @event.UserId,
                flow.Name,
                @event.DeadlineDate,
                @event.AssignmentId,
                cancellationToken);

            _logger.LogInformation(
                "Уведомление о назначении потока '{FlowTitle}' отправлено пользователю {UserId}",
                flow.Name, @event.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Ошибка отправки уведомления о назначении потока. UserId: {UserId}, FlowId: {FlowId}",
                @event.UserId, @event.FlowId);
            // Не пробрасываем исключение, чтобы не нарушить основной процесс
        }
    }

    /// <summary>
    /// Уведомление бадди о новом подопечном
    /// </summary>
    private async Task NotifyBuddyAsync(FlowAssigned @event, CancellationToken cancellationToken)
    {
        try
        {
            // Получаем информацию о потоке
            var flow = await _flowRepository.GetByIdAsync(@event.FlowId, cancellationToken);
            if (flow == null)
            {
                _logger.LogWarning("Поток {FlowId} не найден для уведомления бадди", @event.FlowId);
                return;
            }

            // Отправляем уведомление бадди
            await _notificationService.NotifyBuddyAssignedAsync(
                @event.BuddyId!.Value,
                @event.UserId,
                flow.Name,
                @event.AssignmentId,
                cancellationToken);

            _logger.LogInformation(
                "Уведомление бадди {BuddyId} о назначении потока '{FlowTitle}' пользователю {UserId} отправлено",
                @event.BuddyId, flow.Name, @event.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Ошибка отправки уведомления бадди. BuddyId: {BuddyId}, UserId: {UserId}",
                @event.BuddyId, @event.UserId);
            // Не пробрасываем исключение, чтобы не нарушить основной процесс
        }
    }
}