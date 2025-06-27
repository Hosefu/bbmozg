using MediatR;
using Microsoft.Extensions.Logging;
using Lauf.Domain.Events;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Domain.Interfaces.Services;
using Lauf.Application.EventHandlers.Events;
using Lauf.Application.Services;

namespace Lauf.Application.EventHandlers;

/// <summary>
/// Обработчик события завершения компонента
/// </summary>
public class ComponentCompletedEventHandler : INotificationHandler<ComponentCompletedNotification>
{
    private readonly ILogger<ComponentCompletedEventHandler> _logger;
    private readonly IUserProgressRepository _progressRepository;
    private readonly IFlowAssignmentRepository _assignmentRepository;
    private readonly INotificationService _notificationService;
    private readonly AchievementCalculationService _achievementCalculationService;
    private readonly IUserAchievementRepository _userAchievementRepository;
    private readonly IMediator _mediator;

    public ComponentCompletedEventHandler(
        ILogger<ComponentCompletedEventHandler> logger,
        IUserProgressRepository progressRepository,
        IFlowAssignmentRepository assignmentRepository,
        INotificationService notificationService,
        AchievementCalculationService achievementCalculationService,
        IUserAchievementRepository userAchievementRepository,
        IMediator mediator)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _progressRepository = progressRepository ?? throw new ArgumentNullException(nameof(progressRepository));
        _assignmentRepository = assignmentRepository ?? throw new ArgumentNullException(nameof(assignmentRepository));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _achievementCalculationService = achievementCalculationService ?? throw new ArgumentNullException(nameof(achievementCalculationService));
        _userAchievementRepository = userAchievementRepository ?? throw new ArgumentNullException(nameof(userAchievementRepository));
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

            // Отправляем уведомление о завершении компонента
            await SendComponentCompletedNotificationAsync(domainEvent, cancellationToken);

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
        try
        {
            _logger.LogInformation(
                "Обновление прогресса пользователя. UserId: {UserId}, ComponentSnapshotId: {ComponentSnapshotId}",
                @event.UserId,
                @event.ComponentSnapshotId);

            // Новая архитектура - упрощенное обновление прогресса
            // TODO: Реализовать обновление FlowAssignmentProgress
            _logger.LogInformation(
                "Компонент завершен. ComponentId: {ComponentId}, UserId: {UserId}, TimeSpent: {TimeSpent} минут",
                @event.ComponentSnapshotId,
                @event.UserId,
                @event.TimeSpentMinutes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Ошибка обновления прогресса пользователя. UserId: {UserId}, ComponentSnapshotId: {ComponentSnapshotId}",
                @event.UserId, @event.ComponentSnapshotId);
            // Не пробрасываем исключение, чтобы не нарушить основной процесс
        }
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
    /// Отправка уведомления о завершении компонента
    /// </summary>
    private async Task SendComponentCompletedNotificationAsync(ComponentCompleted @event, CancellationToken cancellationToken)
    {
        try
        {
            // Получаем информацию о компоненте и потоке
            // Пока используем заглушки для названий
            var componentTitle = $"Компонент {@event.ComponentSnapshotId}";
            var flowTitle = "Поток обучения";

            await _notificationService.NotifyComponentCompletedAsync(
                @event.UserId,
                componentTitle,
                flowTitle,
                @event.ComponentSnapshotId,
                cancellationToken);

            _logger.LogInformation(
                "Уведомление о завершении компонента отправлено пользователю {UserId}",
                @event.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Ошибка отправки уведомления о завершении компонента. UserId: {UserId}",
                @event.UserId);
            // Не пробрасываем исключение
        }
    }

    /// <summary>
    /// Уведомление о завершении шага
    /// </summary>
    private async Task NotifyStepCompletedAsync(Guid userId, Guid stepId, CancellationToken cancellationToken)
    {
        try
        {
            // Заглушка для названия шага - в реальности получаем из снапшота
            var stepTitle = $"Шаг {stepId}";
            var flowTitle = "Поток обучения";

            _logger.LogInformation(
                "Отправка уведомления о завершении шага {StepId} пользователю {UserId}",
                stepId, userId);

            await Task.CompletedTask; // TODO: Реализовать уведомления о завершении шага
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка отправки уведомления о завершении шага");
        }
    }

    /// <summary>
    /// Уведомление о разблокировке следующего шага
    /// </summary>
    private async Task NotifyStepUnlockedAsync(Guid userId, Guid stepId, CancellationToken cancellationToken)
    {
        try
        {
            // Заглушка для названия шага - в реальности получаем из снапшота
            var stepTitle = $"Шаг {stepId}";
            var flowTitle = "Поток обучения";

            await _notificationService.NotifyStepUnlockedAsync(
                userId, stepTitle, flowTitle, stepId, cancellationToken);

            _logger.LogInformation(
                "Отправлено уведомление о разблокировке шага {StepId} пользователю {UserId}",
                stepId, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка отправки уведомления о разблокировке шага");
        }
    }

    /// <summary>
    /// Уведомление о завершении потока
    /// </summary>
    private async Task NotifyFlowCompletedAsync(Guid userId, Guid assignmentId, Guid flowId, CancellationToken cancellationToken)
    {
        try
        {
            // Заглушка для названия потока - в реальности получаем из репозитория
            var flowTitle = "Поток обучения";

            _logger.LogInformation(
                "Отправка уведомления о завершении потока {FlowId} пользователю {UserId}",
                flowId, userId);

            await Task.CompletedTask; // TODO: Реализовать уведомления о завершении потока
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка отправки уведомления о завершении потока");
        }
    }

    /// <summary>
    /// Проверка достижений
    /// </summary>
    private async Task CheckAchievementsAsync(ComponentCompleted @event, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Проверка достижений пользователя. UserId: {UserId}",
                @event.UserId);

            // Получаем новые достижения
            var newAchievements = await _achievementCalculationService.CheckNewAchievementsAsync(@event.UserId, cancellationToken);

            foreach (var achievement in newAchievements)
            {
                // TODO: Временно закомментируем до реализации IUserAchievementRepository
                _logger.LogInformation(
                    "Обнаружено достижение '{AchievementTitle}' для пользователя {UserId}",
                    achievement.Title, @event.UserId);
                
                /*
                // Проверяем, что достижение еще не получено
                var existingAchievement = await _userAchievementRepository.GetByUserAndAchievementAsync(@event.UserId, achievement.Id, cancellationToken);
                if (existingAchievement == null)
                {
                    // Создаем новое достижение пользователя
                    var userAchievement = new Domain.Entities.Users.UserAchievement
                    {
                        Id = Guid.NewGuid(),
                        UserId = @event.UserId,
                        AchievementId = achievement.Id,
                        EarnedAt = DateTime.UtcNow
                    };

                    await _userAchievementRepository.AddAsync(userAchievement, cancellationToken);

                    // Отправляем уведомление о новом достижении
                    await _notificationService.NotifyAchievementEarnedAsync(
                        @event.UserId,
                        achievement.Title,
                        achievement.Description,
                        achievement.Id,
                        cancellationToken);
                */

                    // _logger.LogInformation(
                    //     "Получено новое достижение '{AchievementTitle}' пользователем {UserId}",
                    //     achievement.Title, @event.UserId);
                // }
            }

            _logger.LogInformation(
                "Проверка достижений завершена. Найдено новых достижений: {NewAchievementsCount}",
                newAchievements.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Ошибка проверки достижений пользователя. UserId: {UserId}",
                @event.UserId);
            // Не пробрасываем исключение, чтобы не нарушить основной процесс
        }
    }
}