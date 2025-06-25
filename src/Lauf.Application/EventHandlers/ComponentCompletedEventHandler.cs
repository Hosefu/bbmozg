using MediatR;
using Microsoft.Extensions.Logging;
using Lauf.Domain.Events;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Application.EventHandlers.Events;

namespace Lauf.Application.EventHandlers;

/// <summary>
/// Обработчик события завершения компонента
/// </summary>
public class ComponentCompletedEventHandler : INotificationHandler<ComponentCompletedNotification>
{
    private readonly ILogger<ComponentCompletedEventHandler> _logger;
    private readonly IUserProgressRepository _progressRepository;
    private readonly IFlowAssignmentRepository _assignmentRepository;
    private readonly IMediator _mediator;

    public ComponentCompletedEventHandler(
        ILogger<ComponentCompletedEventHandler> logger,
        IUserProgressRepository progressRepository,
        IFlowAssignmentRepository assignmentRepository,
        IMediator mediator)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _progressRepository = progressRepository ?? throw new ArgumentNullException(nameof(progressRepository));
        _assignmentRepository = assignmentRepository ?? throw new ArgumentNullException(nameof(assignmentRepository));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Обработка события завершения компонента
    /// </summary>
    public async Task Handle(ComponentCompletedNotification notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        _logger.LogInformation(
            "Обработка события завершения компонента. ComponentSnapshotId: {ComponentSnapshotId}, UserId: {UserId}",
            domainEvent.ComponentSnapshotId,
            domainEvent.UserId);

        try
        {
            // Обновляем прогресс пользователя
            await UpdateUserProgressAsync(domainEvent, cancellationToken);

            // Проверяем, завершен ли шаг целиком
            var isStepCompleted = await CheckStepCompletionAsync(domainEvent, cancellationToken);
            if (isStepCompleted)
            {
                _logger.LogInformation(
                    "Шаг завершен. StepSnapshotId: {StepSnapshotId}, UserId: {UserId}",
                    domainEvent.StepSnapshotId,
                    domainEvent.UserId);

                // Публикуем событие завершения шага
                // StepCompletedEvent будет создан в следующих итерациях
                _logger.LogDebug("Шаг завершен, событие StepCompleted будет добавлено позже");

                // Проверяем, разблокирован ли следующий шаг
                await CheckStepUnlockAsync(domainEvent, cancellationToken);
            }

            // Проверяем достижения
            await CheckAchievementsAsync(domainEvent, cancellationToken);

            _logger.LogInformation(
                "Событие завершения компонента успешно обработано. ComponentSnapshotId: {ComponentSnapshotId}",
                domainEvent.ComponentSnapshotId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Ошибка при обработке события завершения компонента. ComponentSnapshotId: {ComponentSnapshotId}",
                domainEvent.ComponentSnapshotId);
            throw;
        }
    }

    /// <summary>
    /// Обновление прогресса пользователя
    /// </summary>
    private async Task UpdateUserProgressAsync(ComponentCompleted @event, CancellationToken cancellationToken)
    {
        // Обновление прогресса будет реализовано через UserProgress сущность
        _logger.LogInformation(
            "Обновление прогресса пользователя. UserId: {UserId}, ComponentSnapshotId: {ComponentSnapshotId}",
            @event.UserId,
            @event.ComponentSnapshotId);

        await Task.CompletedTask;
    }

    /// <summary>
    /// Проверка завершения шага
    /// </summary>
    private async Task<bool> CheckStepCompletionAsync(ComponentCompleted @event, CancellationToken cancellationToken)
    {
        // Проверка завершения шага будет реализована с детальным анализом компонентов
        _logger.LogInformation(
            "Проверка завершения шага. StepSnapshotId: {StepSnapshotId}, UserId: {UserId}",
            @event.StepSnapshotId,
            @event.UserId);

        await Task.CompletedTask;
        return false; // Базовая реализация
    }

    /// <summary>
    /// Проверка разблокировки следующего шага
    /// </summary>
    private async Task CheckStepUnlockAsync(ComponentCompleted @event, CancellationToken cancellationToken)
    {
        // Логика разблокировки следующего шага будет реализована с учетом настроек потока
        _logger.LogInformation(
            "Проверка разблокировки следующего шага. UserId: {UserId}",
            @event.UserId);

        await Task.CompletedTask;
    }

    /// <summary>
    /// Проверка достижений
    /// </summary>
    private async Task CheckAchievementsAsync(ComponentCompleted @event, CancellationToken cancellationToken)
    {
        // Проверка достижений будет реализована в системе достижений
        _logger.LogInformation(
            "Проверка достижений пользователя. UserId: {UserId}",
            @event.UserId);

        await Task.CompletedTask; // Заглушка
    }
}