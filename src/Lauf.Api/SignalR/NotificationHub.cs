using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Lauf.Application.Services.Interfaces;
using System.Security.Claims;

namespace Lauf.Api.SignalR;

/// <summary>
/// SignalR хаб для уведомлений
/// </summary>
[Authorize]
public class NotificationHub : BaseLoggingHub
{
    private readonly ICurrentUserService _currentUserService;

    public NotificationHub(ICurrentUserService currentUserService, ILogger<NotificationHub> logger) : base(logger)
    {
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
    }

    /// <summary>
    /// Подключение к хабу
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var userId = GetCurrentUserId();
        if (userId.HasValue)
        {
            await LoggedAddToGroupAsync($"User_{userId}");
        }
        else
        {
            _logger.LogWarning("⚡ [HUB-Notification] CONNECT_FAILED {ConnectionId} - no user identification", Context.ConnectionId);
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
            await LoggedRemoveFromGroupAsync($"User_{userId}");
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Подписаться на уведомления определенного типа
    /// </summary>
    /// <param name="notificationType">Тип уведомлений</param>
    public async Task SubscribeToNotificationType(string notificationType)
    {
        LogMethodCall(nameof(SubscribeToNotificationType), new { notificationType });
        
        var userId = GetCurrentUserId();
        if (userId.HasValue)
        {
            var groupName = $"NotificationType_{notificationType}";
            await LoggedAddToGroupAsync(groupName);
            
            LogClientCall("SubscriptionConfirmed", Context.ConnectionId, notificationType);
            await Clients.Caller.SendAsync("SubscriptionConfirmed", notificationType);
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