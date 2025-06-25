using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace Lauf.Api.Hubs;

/// <summary>
/// SignalR хаб для отслеживания прогресса обучения
/// </summary>
[Authorize]
public class ProgressHub : Hub
{
    private readonly ILogger<ProgressHub> _logger;

    public ProgressHub(ILogger<ProgressHub> logger)
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
            _logger.LogInformation("Пользователь {UserId} подключился к ProgressHub", userId);
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
            _logger.LogInformation("Пользователь {UserId} отключился от ProgressHub", userId);
        }

        if (exception != null)
        {
            _logger.LogError(exception, "Ошибка при отключении от ProgressHub");
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Присоединиться к группе отслеживания потока
    /// </summary>
    public async Task JoinFlowGroup(Guid flowId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"flow_{flowId}");
        _logger.LogInformation("Пользователь присоединился к группе потока {FlowId}", flowId);
    }

    /// <summary>
    /// Покинуть группу отслеживания потока
    /// </summary>
    public async Task LeaveFlowGroup(Guid flowId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"flow_{flowId}");
        _logger.LogInformation("Пользователь покинул группу потока {FlowId}", flowId);
    }

    /// <summary>
    /// Присоединиться к группе отслеживания назначения
    /// </summary>
    public async Task JoinAssignmentGroup(Guid assignmentId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"assignment_{assignmentId}");
        _logger.LogInformation("Пользователь присоединился к группе назначения {AssignmentId}", assignmentId);
    }

    /// <summary>
    /// Покинуть группу отслеживания назначения
    /// </summary>
    public async Task LeaveAssignmentGroup(Guid assignmentId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"assignment_{assignmentId}");
        _logger.LogInformation("Пользователь покинул группу назначения {AssignmentId}", assignmentId);
    }

    /// <summary>
    /// Обновить прогресс прохождения компонента
    /// </summary>
    public async Task UpdateComponentProgress(Guid assignmentId, Guid componentId, int progressPercentage)
    {
        var userId = Context.User?.Identity?.Name;
        if (!string.IsNullOrEmpty(userId))
        {
            // Отправляем обновление прогресса всем в группе назначения
            await Clients.Group($"assignment_{assignmentId}")
                .SendAsync("ComponentProgressUpdated", assignmentId, componentId, progressPercentage);
            
            // Отправляем обновление пользователю
            await Clients.Group($"user_{userId}")
                .SendAsync("ProgressUpdated", assignmentId, progressPercentage);

            _logger.LogInformation("Прогресс компонента {ComponentId} обновлен до {Progress}% пользователем {UserId}", 
                componentId, progressPercentage, userId);
        }
    }

    /// <summary>
    /// Отметить начало прохождения компонента
    /// </summary>
    public async Task StartComponent(Guid assignmentId, Guid componentId)
    {
        var userId = Context.User?.Identity?.Name;
        if (!string.IsNullOrEmpty(userId))
        {
            await Clients.Group($"assignment_{assignmentId}")
                .SendAsync("ComponentStarted", assignmentId, componentId, DateTime.UtcNow);

            _logger.LogInformation("Пользователь {UserId} начал прохождение компонента {ComponentId}", 
                userId, componentId);
        }
    }

    /// <summary>
    /// Отметить завершение компонента
    /// </summary>
    public async Task CompleteComponent(Guid assignmentId, Guid componentId)
    {
        var userId = Context.User?.Identity?.Name;
        if (!string.IsNullOrEmpty(userId))
        {
            await Clients.Group($"assignment_{assignmentId}")
                .SendAsync("ComponentCompleted", assignmentId, componentId, DateTime.UtcNow);

            _logger.LogInformation("Пользователь {UserId} завершил прохождение компонента {ComponentId}", 
                userId, componentId);
        }
    }

    /// <summary>
    /// Присоединиться к группе наставников для мониторинга
    /// </summary>
    public async Task JoinMentorMonitoringGroup()
    {
        var isMentor = Context.User?.IsInRole("Mentor") == true || 
                      Context.User?.IsInRole("Buddy") == true || 
                      Context.User?.IsInRole("Admin") == true;
        
        if (isMentor)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "mentors_monitoring");
            _logger.LogInformation("Ментор присоединился к группе мониторинга");
        }
    }

    /// <summary>
    /// Покинуть группу наставников для мониторинга
    /// </summary>
    public async Task LeaveMentorMonitoringGroup()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "mentors_monitoring");
        _logger.LogInformation("Пользователь покинул группу мониторинга менторов");
    }

    /// <summary>
    /// Запросить текущий прогресс назначения
    /// </summary>
    public async Task RequestCurrentProgress(Guid assignmentId)
    {
        var userId = Context.User?.Identity?.Name;
        if (!string.IsNullOrEmpty(userId))
        {
            // Здесь можно добавить логику получения текущего прогресса из базы данных
            // и отправку его обратно клиенту
            await Clients.Caller.SendAsync("CurrentProgressRequested", assignmentId);
            
            _logger.LogInformation("Запрошен текущий прогресс назначения {AssignmentId} пользователем {UserId}", 
                assignmentId, userId);
        }
    }
}