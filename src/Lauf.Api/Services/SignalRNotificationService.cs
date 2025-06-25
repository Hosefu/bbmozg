using Microsoft.AspNetCore.SignalR;
using Lauf.Api.Hubs;

namespace Lauf.Api.Services;

/// <summary>
/// Сервис для отправки уведомлений через SignalR
/// </summary>
public class SignalRNotificationService
{
    private readonly IHubContext<NotificationHub> _notificationHub;
    private readonly IHubContext<ProgressHub> _progressHub;
    private readonly ILogger<SignalRNotificationService> _logger;

    public SignalRNotificationService(
        IHubContext<NotificationHub> notificationHub,
        IHubContext<ProgressHub> progressHub,
        ILogger<SignalRNotificationService> logger)
    {
        _notificationHub = notificationHub;
        _progressHub = progressHub;
        _logger = logger;
    }

    /// <summary>
    /// Отправить уведомление пользователю
    /// </summary>
    public async Task SendNotificationToUserAsync(
        Guid userId, 
        string title, 
        string message, 
        string type = "info",
        CancellationToken cancellationToken = default)
    {
        try
        {
            var notification = new
            {
                Id = Guid.NewGuid(),
                Title = title,
                Message = message,
                Type = type,
                Timestamp = DateTime.UtcNow,
                IsRead = false
            };

            await _notificationHub.Clients.Group($"user_{userId}")
                .SendAsync("ReceiveNotification", notification, cancellationToken);

            _logger.LogInformation("Уведомление отправлено пользователю {UserId}: {Title}", userId, title);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отправке уведомления пользователю {UserId}", userId);
        }
    }

    /// <summary>
    /// Отправить уведомление всем администраторам
    /// </summary>
    public async Task SendNotificationToAdminsAsync(
        string title, 
        string message, 
        string type = "info",
        CancellationToken cancellationToken = default)
    {
        try
        {
            var notification = new
            {
                Id = Guid.NewGuid(),
                Title = title,
                Message = message,
                Type = type,
                Timestamp = DateTime.UtcNow,
                IsRead = false
            };

            await _notificationHub.Clients.Group("admins")
                .SendAsync("ReceiveNotification", notification, cancellationToken);

            _logger.LogInformation("Уведомление отправлено всем администраторам: {Title}", title);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отправке уведомления администраторам");
        }
    }

    /// <summary>
    /// Отправить уведомление всем менторам
    /// </summary>
    public async Task SendNotificationToMentorsAsync(
        string title, 
        string message, 
        string type = "info",
        CancellationToken cancellationToken = default)
    {
        try
        {
            var notification = new
            {
                Id = Guid.NewGuid(),
                Title = title,
                Message = message,
                Type = type,
                Timestamp = DateTime.UtcNow,
                IsRead = false
            };

            await _notificationHub.Clients.Group("mentors")
                .SendAsync("ReceiveNotification", notification, cancellationToken);

            _logger.LogInformation("Уведомление отправлено всем менторам: {Title}", title);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отправке уведомления менторам");
        }
    }

    /// <summary>
    /// Обновить прогресс назначения
    /// </summary>
    public async Task UpdateAssignmentProgressAsync(
        Guid assignmentId, 
        Guid userId, 
        int progressPercentage,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var progressUpdate = new
            {
                AssignmentId = assignmentId,
                UserId = userId,
                ProgressPercentage = progressPercentage,
                Timestamp = DateTime.UtcNow
            };

            // Отправляем обновление пользователю
            await _progressHub.Clients.Group($"user_{userId}")
                .SendAsync("ProgressUpdated", progressUpdate, cancellationToken);

            // Отправляем обновление в группу назначения
            await _progressHub.Clients.Group($"assignment_{assignmentId}")
                .SendAsync("AssignmentProgressUpdated", progressUpdate, cancellationToken);

            // Отправляем обновление менторам для мониторинга
            await _progressHub.Clients.Group("mentors_monitoring")
                .SendAsync("StudentProgressUpdated", progressUpdate, cancellationToken);

            _logger.LogInformation("Прогресс назначения {AssignmentId} обновлен до {Progress}%", 
                assignmentId, progressPercentage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении прогресса назначения {AssignmentId}", assignmentId);
        }
    }

    /// <summary>
    /// Уведомить о начале прохождения потока
    /// </summary>
    public async Task NotifyFlowStartedAsync(
        Guid assignmentId, 
        Guid userId, 
        string flowTitle,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await SendNotificationToUserAsync(
                userId,
                "Поток обучения начат",
                $"Вы начали прохождение потока: {flowTitle}",
                "success",
                cancellationToken);

            await _progressHub.Clients.Group("mentors_monitoring")
                .SendAsync("FlowStarted", new
                {
                    AssignmentId = assignmentId,
                    UserId = userId,
                    FlowTitle = flowTitle,
                    StartedAt = DateTime.UtcNow
                }, cancellationToken);

            _logger.LogInformation("Отправлено уведомление о начале потока {FlowTitle} пользователем {UserId}", 
                flowTitle, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отправке уведомления о начале потока");
        }
    }

    /// <summary>
    /// Уведомить о завершении потока
    /// </summary>
    public async Task NotifyFlowCompletedAsync(
        Guid assignmentId, 
        Guid userId, 
        string flowTitle,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await SendNotificationToUserAsync(
                userId,
                "Поток обучения завершен!",
                $"Поздравляем! Вы успешно завершили поток: {flowTitle}",
                "success",
                cancellationToken);

            await _progressHub.Clients.Group("mentors_monitoring")
                .SendAsync("FlowCompleted", new
                {
                    AssignmentId = assignmentId,
                    UserId = userId,
                    FlowTitle = flowTitle,
                    CompletedAt = DateTime.UtcNow
                }, cancellationToken);

            _logger.LogInformation("Отправлено уведомление о завершении потока {FlowTitle} пользователем {UserId}", 
                flowTitle, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отправке уведомления о завершении потока");
        }
    }

    /// <summary>
    /// Уведомить о приближающемся дедлайне
    /// </summary>
    public async Task NotifyUpcomingDeadlineAsync(
        Guid userId, 
        string flowTitle, 
        DateTime dueDate,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var daysLeft = (dueDate - DateTime.UtcNow).Days;
            var message = daysLeft switch
            {
                1 => $"Завтра истекает срок выполнения потока: {flowTitle}",
                > 1 => $"Через {daysLeft} дней истекает срок выполнения потока: {flowTitle}",
                _ => $"Сегодня истекает срок выполнения потока: {flowTitle}"
            };

            await SendNotificationToUserAsync(
                userId,
                "Приближается дедлайн",
                message,
                "warning",
                cancellationToken);

            _logger.LogInformation("Отправлено уведомление о приближающемся дедлайне пользователю {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отправке уведомления о дедлайне");
        }
    }

    /// <summary>
    /// Уведомить о просроченном назначении
    /// </summary>
    public async Task NotifyOverdueAssignmentAsync(
        Guid userId, 
        string flowTitle, 
        DateTime dueDate,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var daysOverdue = (DateTime.UtcNow - dueDate).Days;
            var message = $"Просрочено на {daysOverdue} дней: {flowTitle}";

            await SendNotificationToUserAsync(
                userId,
                "Просроченное задание",
                message,
                "error",
                cancellationToken);

            await SendNotificationToMentorsAsync(
                "Просроченное назначение",
                $"Пользователь просрочил выполнение потока: {flowTitle}",
                "warning",
                cancellationToken);

            _logger.LogInformation("Отправлено уведомление о просроченном назначении пользователю {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отправке уведомления о просроченном назначении");
        }
    }
}