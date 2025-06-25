using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace Lauf.Api.Hubs;

/// <summary>
/// SignalR хаб для уведомлений
/// </summary>
[Authorize]
public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Подключение к хабу
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.Identity?.Name;
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            _logger.LogInformation("Пользователь {UserId} подключился к NotificationHub", userId);
        }

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Отключение от хаба
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.Identity?.Name;
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
            _logger.LogInformation("Пользователь {UserId} отключился от NotificationHub", userId);
        }

        if (exception != null)
        {
            _logger.LogError(exception, "Ошибка при отключении от NotificationHub");
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Присоединиться к группе администраторов
    /// </summary>
    public async Task JoinAdminGroup()
    {
        var isAdmin = Context.User?.IsInRole("Admin") == true;
        if (isAdmin)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "admins");
            _logger.LogInformation("Администратор подключился к группе admins");
        }
    }

    /// <summary>
    /// Покинуть группу администраторов
    /// </summary>
    public async Task LeaveAdminGroup()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "admins");
        _logger.LogInformation("Пользователь покинул группу admins");
    }

    /// <summary>
    /// Присоединиться к группе менторов
    /// </summary>
    public async Task JoinMentorGroup()
    {
        var isMentor = Context.User?.IsInRole("Mentor") == true || Context.User?.IsInRole("Buddy") == true;
        if (isMentor)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "mentors");
            _logger.LogInformation("Ментор подключился к группе mentors");
        }
    }

    /// <summary>
    /// Покинуть группу менторов
    /// </summary>
    public async Task LeaveMentorGroup()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "mentors");
        _logger.LogInformation("Пользователь покинул группу mentors");
    }

    /// <summary>
    /// Отправить личное сообщение пользователю
    /// </summary>
    public async Task SendPrivateMessage(string targetUserId, string message)
    {
        var senderId = Context.User?.Identity?.Name;
        if (!string.IsNullOrEmpty(senderId))
        {
            await Clients.Group($"user_{targetUserId}").SendAsync("ReceivePrivateMessage", senderId, message);
            _logger.LogInformation("Сообщение отправлено от {SenderId} к {TargetUserId}", senderId, targetUserId);
        }
    }

    /// <summary>
    /// Отметить уведомление как прочитанное
    /// </summary>
    public async Task MarkNotificationAsRead(Guid notificationId)
    {
        var userId = Context.User?.Identity?.Name;
        if (!string.IsNullOrEmpty(userId))
        {
            await Clients.Group($"user_{userId}").SendAsync("NotificationMarkedAsRead", notificationId);
            _logger.LogInformation("Уведомление {NotificationId} отмечено как прочитанное пользователем {UserId}", 
                notificationId, userId);
        }
    }
}