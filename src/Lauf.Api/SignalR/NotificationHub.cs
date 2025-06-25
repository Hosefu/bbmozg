using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Lauf.Application.Services.Interfaces;
using System.Security.Claims;

namespace Lauf.Api.SignalR;

/// <summary>
/// SignalR хаб для уведомлений
/// </summary>
[Authorize]
public class NotificationHub : Hub
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ICurrentUserService currentUserService, ILogger<NotificationHub> logger)
    {
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Подключение к хабу
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var userId = GetCurrentUserId();
        if (userId.HasValue)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
            _logger.LogInformation("Пользователь {UserId} подключился к NotificationHub", userId);
        }
        else
        {
            _logger.LogWarning("Попытка подключения к NotificationHub без идентификации пользователя");
        }

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Отключение от хаба
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetCurrentUserId();
        if (userId.HasValue)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
            _logger.LogInformation("Пользователь {UserId} отключился от NotificationHub", userId);
        }

        if (exception != null)
        {
            _logger.LogError(exception, "Ошибка при отключении от NotificationHub");
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Подписаться на уведомления определенного типа
    /// </summary>
    /// <param name="notificationType">Тип уведомлений</param>
    public async Task SubscribeToNotificationType(string notificationType)
    {
        var userId = GetCurrentUserId();
        if (userId.HasValue)
        {
            var groupName = $"NotificationType_{notificationType}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            
            await Clients.Caller.SendAsync("SubscriptionConfirmed", notificationType);
            _logger.LogInformation("Пользователь {UserId} подписался на уведомления типа {NotificationType}", userId, notificationType);
        }
    }

    /// <summary>
    /// Отписаться от уведомлений определенного типа
    /// </summary>
    /// <param name="notificationType">Тип уведомлений</param>
    public async Task UnsubscribeFromNotificationType(string notificationType)
    {
        var userId = GetCurrentUserId();
        if (userId.HasValue)
        {
            var groupName = $"NotificationType_{notificationType}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            
            await Clients.Caller.SendAsync("UnsubscriptionConfirmed", notificationType);
            _logger.LogInformation("Пользователь {UserId} отписался от уведомлений типа {NotificationType}", userId, notificationType);
        }
    }

    /// <summary>
    /// Отметить уведомление как прочитанное
    /// </summary>
    /// <param name="notificationId">ID уведомления</param>
    public async Task MarkNotificationAsRead(Guid notificationId)
    {
        var userId = GetCurrentUserId();
        if (userId.HasValue)
        {
            try
            {
                // Здесь должна быть логика отметки уведомления как прочитанного через MediatR
                await Clients.Caller.SendAsync("NotificationMarkedAsRead", notificationId);
                _logger.LogInformation("Пользователь {UserId} отметил уведомление {NotificationId} как прочитанное", userId, notificationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при отметке уведомления {NotificationId} как прочитанного", notificationId);
                await Clients.Caller.SendAsync("Error", "Не удалось отметить уведомление как прочитанное");
            }
        }
    }

    /// <summary>
    /// Получить количество непрочитанных уведомлений
    /// </summary>
    public async Task GetUnreadNotificationCount()
    {
        var userId = GetCurrentUserId();
        if (userId.HasValue)
        {
            try
            {
                // Здесь должна быть логика получения количества непрочитанных уведомлений через MediatR
                var count = 0; // Заглушка
                await Clients.Caller.SendAsync("UnreadNotificationCount", count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении количества непрочитанных уведомлений для пользователя {UserId}", userId);
                await Clients.Caller.SendAsync("Error", "Не удалось получить количество непрочитанных уведомлений");
            }
        }
    }

    /// <summary>
    /// Получить ID текущего пользователя
    /// </summary>
    /// <returns>ID пользователя или null</returns>
    private Guid? GetCurrentUserId()
    {
        var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? Context.User?.FindFirst("sub")?.Value;
        
        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }

        return null;
    }
}