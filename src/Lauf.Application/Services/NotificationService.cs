using Microsoft.Extensions.Logging;
using Lauf.Domain.Entities.Notifications;
using Lauf.Domain.Enums;
using Lauf.Domain.Interfaces;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Domain.Interfaces.Services;

namespace Lauf.Application.Services;

/// <summary>
/// Сервис для работы с уведомлениями
/// </summary>
public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    // Убираем циклическую зависимость
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork,
        ILogger<NotificationService> logger)
    {
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Создать и отправить уведомление о назначении потока
    /// </summary>
    public async Task NotifyFlowAssignedAsync(
        Guid userId, 
        string flowTitle, 
        DateTime deadline,
        Guid flowAssignmentId,
        CancellationToken cancellationToken = default)
    {
        var notification = Notification.CreateFlowAssigned(userId, flowTitle, deadline, flowAssignmentId);
        
        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Создано уведомление о назначении потока {FlowTitle} пользователю {UserId}",
            flowTitle, userId);
    }

    /// <summary>
    /// Создать и отправить напоминание о дедлайне
    /// </summary>
    public async Task NotifyDeadlineReminderAsync(
        Guid userId,
        string flowTitle,
        DateTime deadline,
        int daysLeft,
        Guid flowAssignmentId,
        CancellationToken cancellationToken = default)
    {
        var notification = Notification.CreateDeadlineReminder(
            userId, flowTitle, deadline, daysLeft, flowAssignmentId);
        
        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Создано напоминание о дедлайне для потока {FlowTitle}, осталось {DaysLeft} дн.",
            flowTitle, daysLeft);
    }

    /// <summary>
    /// Создать и отправить уведомление о получении достижения
    /// </summary>
    public async Task NotifyAchievementUnlockedAsync(
        Guid userId,
        string achievementTitle,
        string achievementDescription,
        Guid achievementId,
        CancellationToken cancellationToken = default)
    {
        var notification = Notification.CreateAchievementUnlocked(
            userId, achievementTitle, achievementDescription, achievementId);
        
        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Создано уведомление о достижении {AchievementTitle} для пользователя {UserId}",
            achievementTitle, userId);
    }

    /// <summary>
    /// Создать и отправить уведомление о завершении компонента
    /// </summary>
    public async Task NotifyComponentCompletedAsync(
        Guid userId,
        string componentTitle,
        string flowTitle,
        Guid componentId,
        CancellationToken cancellationToken = default)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = NotificationType.ComponentCompleted,
            Channel = NotificationChannel.Telegram,
            Priority = NotificationPriority.Medium,
            Title = "✅ Компонент завершен!",
            Content = $"Отлично! Вы завершили компонент \"{componentTitle}\" в потоке \"{flowTitle}\"",
            RelatedEntityId = componentId,
            RelatedEntityType = "FlowStepComponent"
        };

        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Создано уведомление о завершении компонента {ComponentTitle} для пользователя {UserId}",
            componentTitle, userId);
    }

    /// <summary>
    /// Создать и отправить уведомление о разблокировке шага
    /// </summary>
    public async Task NotifyStepUnlockedAsync(
        Guid userId,
        string stepTitle,
        string flowTitle,
        Guid stepId,
        CancellationToken cancellationToken = default)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = NotificationType.StepUnlocked,
            Channel = NotificationChannel.Telegram,
            Priority = NotificationPriority.Medium,
            Title = "🔓 Новый шаг доступен!",
            Content = $"Разблокирован новый шаг \"{stepTitle}\" в потоке \"{flowTitle}\"",
            RelatedEntityId = stepId,
            RelatedEntityType = "FlowStep"
        };

        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Создано уведомление о разблокировке шага {StepTitle} для пользователя {UserId}",
            stepTitle, userId);
    }

    /// <summary>
    /// Создать кастомное уведомление
    /// </summary>
    public async Task CreateCustomNotificationAsync(
        Guid userId,
        NotificationType type,
        string title,
        string content,
        NotificationPriority priority = NotificationPriority.Medium,
        DateTime? scheduledAt = null,
        string? metadata = null,
        CancellationToken cancellationToken = default)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = type,
            Channel = NotificationChannel.Telegram,
            Priority = priority,
            Title = title,
            Content = content,
            ScheduledAt = scheduledAt ?? DateTime.UtcNow,
            Metadata = metadata
        };

        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Создано кастомное уведомление {Type} для пользователя {UserId}",
            type, userId);
    }

    /// <summary>
    /// Обработать очередь уведомлений для отправки
    /// </summary>
    public async Task<int> ProcessPendingNotificationsAsync(
        int batchSize = 50,
        CancellationToken cancellationToken = default)
    {
        var pendingNotifications = await _notificationRepository.GetPendingNotificationsAsync(
            batchSize, cancellationToken);

        var processedCount = 0;

        foreach (var notification in pendingNotifications)
        {
            try
            {
                // Временно закомментируем отправку через внешний сервис
                // var result = await _externalNotificationService.SendNotificationAsync(notification, cancellationToken);
                var result = new { IsSuccess = true, ErrorMessage = (string?)null };
                
                if (result.IsSuccess)
                {
                    notification.MarkAsSent();
                    processedCount++;
                }
                else
                {
                    notification.MarkAsFailed(result.ErrorMessage ?? "Неизвестная ошибка");
                }

                await _notificationRepository.UpdateAsync(notification, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Ошибка обработки уведомления {NotificationId}",
                    notification.Id);

                notification.MarkAsFailed(ex.Message);
                await _notificationRepository.UpdateAsync(notification, cancellationToken);
            }
        }

        if (processedCount > 0)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation(
                "Обработано {ProcessedCount} из {TotalCount} уведомлений",
                processedCount, pendingNotifications.Count);
        }

        return processedCount;
    }

    /// <summary>
    /// Отметить уведомление как прочитанное
    /// </summary>
    public async Task MarkAsReadAsync(Guid notificationId, CancellationToken cancellationToken = default)
    {
        await _notificationRepository.MarkAsReadAsync(notificationId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Отметить все уведомления пользователя как прочитанные
    /// </summary>
    public async Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await _notificationRepository.MarkAllAsReadAsync(userId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Получить уведомления пользователя
    /// </summary>
    public async Task<IReadOnlyList<Notification>> GetUserNotificationsAsync(
        Guid userId,
        bool includeRead = false,
        int limit = 50,
        CancellationToken cancellationToken = default)
    {
        return await _notificationRepository.GetUserNotificationsAsync(
            userId, includeRead, limit, cancellationToken);
    }

    /// <summary>
    /// Получить количество непрочитанных уведомлений
    /// </summary>
    public async Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _notificationRepository.GetUnreadCountAsync(userId, cancellationToken);
    }

    // Дополнительные методы для специфичных уведомлений

    public async Task NotifyBuddyAssignedAsync(
        Guid buddyId,
        Guid menteeId, 
        string flowTitle,
        Guid assignmentId,
        CancellationToken cancellationToken = default)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = buddyId,
            Type = NotificationType.BuddyAssigned,
            Channel = NotificationChannel.Telegram,
            Priority = NotificationPriority.High,
            Title = "👥 Назначен новый подопечный",
            Content = $"Вам назначен новый подопечный для прохождения потока \"{flowTitle}\"",
            RelatedEntityId = assignmentId,
            RelatedEntityType = "FlowAssignment"
        };

        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task NotifyReminderNotStartedAsync(
        Guid userId,
        string flowTitle,
        DateTime deadline,
        Guid assignmentId,
        CancellationToken cancellationToken = default)
    {
        var daysLeft = (deadline - DateTime.UtcNow).Days;
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = NotificationType.DeadlineReminder,
            Channel = NotificationChannel.Telegram,
            Priority = NotificationPriority.Medium,
            Title = "⏰ Напоминание о начале обучения",
            Content = $"Не забудьте начать поток \"{flowTitle}\". До дедлайна осталось {daysLeft} дней",
            RelatedEntityId = assignmentId,
            RelatedEntityType = "FlowAssignment"
        };

        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task NotifyReminderInProgressAsync(
        Guid userId,
        string flowTitle,
        int progressPercent,
        DateTime deadline,
        Guid assignmentId,
        CancellationToken cancellationToken = default)
    {
        var daysLeft = (deadline - DateTime.UtcNow).Days;
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = NotificationType.ProgressReminder,
            Channel = NotificationChannel.Telegram,
            Priority = NotificationPriority.Medium,
            Title = "📈 Напоминание о прогрессе",
            Content = $"Поток \"{flowTitle}\" выполнен на {progressPercent}%. До дедлайна {daysLeft} дней",
            RelatedEntityId = assignmentId,
            RelatedEntityType = "FlowAssignment"
        };

        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task NotifyApproachingDeadlineAsync(
        Guid userId,
        string flowTitle,
        DateTime deadline,
        int daysLeft,
        Guid assignmentId,
        CancellationToken cancellationToken = default)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = NotificationType.DeadlineReminder,
            Channel = NotificationChannel.Telegram,
            Priority = NotificationPriority.High,
            Title = "🚨 Приближается дедлайн",
            Content = $"До завершения потока \"{flowTitle}\" осталось {daysLeft} дней!",
            RelatedEntityId = assignmentId,
            RelatedEntityType = "FlowAssignment"
        };

        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task NotifyUrgentDeadlineAsync(
        Guid userId,
        string flowTitle,
        DateTime deadline,
        Guid assignmentId,
        CancellationToken cancellationToken = default)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = NotificationType.UrgentDeadline,
            Channel = NotificationChannel.Telegram,
            Priority = NotificationPriority.Critical,
            Title = "🔥 СРОЧНО: Истекает дедлайн!",
            Content = $"Дедлайн по потоку \"{flowTitle}\" истекает сегодня!",
            RelatedEntityId = assignmentId,
            RelatedEntityType = "FlowAssignment"
        };

        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task NotifyAchievementEarnedAsync(
        Guid userId,
        string achievementTitle,
        string achievementDescription,
        Guid achievementId,
        CancellationToken cancellationToken = default)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = NotificationType.AchievementUnlocked,
            Channel = NotificationChannel.Telegram,
            Priority = NotificationPriority.High,
            Title = "🏆 Достижение получено!",
            Content = $"Поздравляем! Вы получили достижение \"{achievementTitle}\": {achievementDescription}",
            RelatedEntityId = achievementId,
            RelatedEntityType = "Achievement"
        };

        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}