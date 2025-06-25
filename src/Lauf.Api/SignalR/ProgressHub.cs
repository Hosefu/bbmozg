using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Lauf.Application.Services.Interfaces;
using System.Security.Claims;

namespace Lauf.Api.SignalR;

/// <summary>
/// SignalR хаб для отслеживания прогресса
/// </summary>
[Authorize]
public class ProgressHub : Hub
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ProgressHub> _logger;

    public ProgressHub(ICurrentUserService currentUserService, ILogger<ProgressHub> logger)
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
            _logger.LogInformation("Пользователь {UserId} подключился к ProgressHub", userId);
        }
        else
        {
            _logger.LogWarning("Попытка подключения к ProgressHub без идентификации пользователя");
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
            _logger.LogInformation("Пользователь {UserId} отключился от ProgressHub", userId);
        }

        if (exception != null)
        {
            _logger.LogError(exception, "Ошибка при отключении от ProgressHub");
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Подписаться на обновления прогресса конкретного потока
    /// </summary>
    /// <param name="flowAssignmentId">ID назначения потока</param>
    public async Task SubscribeToFlowProgress(Guid flowAssignmentId)
    {
        var userId = GetCurrentUserId();
        if (userId.HasValue)
        {
            var groupName = $"FlowProgress_{flowAssignmentId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            
            await Clients.Caller.SendAsync("FlowProgressSubscriptionConfirmed", flowAssignmentId);
            _logger.LogInformation("Пользователь {UserId} подписался на прогресс потока {FlowAssignmentId}", userId, flowAssignmentId);
        }
    }

    /// <summary>
    /// Отписаться от обновлений прогресса потока
    /// </summary>
    /// <param name="flowAssignmentId">ID назначения потока</param>
    public async Task UnsubscribeFromFlowProgress(Guid flowAssignmentId)
    {
        var userId = GetCurrentUserId();
        if (userId.HasValue)
        {
            var groupName = $"FlowProgress_{flowAssignmentId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            
            await Clients.Caller.SendAsync("FlowProgressUnsubscriptionConfirmed", flowAssignmentId);
            _logger.LogInformation("Пользователь {UserId} отписался от прогресса потока {FlowAssignmentId}", userId, flowAssignmentId);
        }
    }

    /// <summary>
    /// Подписаться на обновления прогресса для наставника (все подопечные)
    /// </summary>
    public async Task SubscribeToMenteeProgress()
    {
        var userId = GetCurrentUserId();
        if (userId.HasValue)
        {
            var groupName = $"Mentor_{userId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            
            await Clients.Caller.SendAsync("MenteeProgressSubscriptionConfirmed");
            _logger.LogInformation("Наставник {UserId} подписался на прогресс своих подопечных", userId);
        }
    }

    /// <summary>
    /// Отписаться от обновлений прогресса подопечных
    /// </summary>
    public async Task UnsubscribeFromMenteeProgress()
    {
        var userId = GetCurrentUserId();
        if (userId.HasValue)
        {
            var groupName = $"Mentor_{userId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            
            await Clients.Caller.SendAsync("MenteeProgressUnsubscriptionConfirmed");
            _logger.LogInformation("Наставник {UserId} отписался от прогресса своих подопечных", userId);
        }
    }

    /// <summary>
    /// Получить текущий прогресс потока
    /// </summary>
    /// <param name="flowAssignmentId">ID назначения потока</param>
    public async Task GetCurrentFlowProgress(Guid flowAssignmentId)
    {
        var userId = GetCurrentUserId();
        if (userId.HasValue)
        {
            try
            {
                // Здесь должна быть логика получения прогресса через MediatR
                var progress = new
                {
                    FlowAssignmentId = flowAssignmentId,
                    CompletedSteps = 0,
                    TotalSteps = 0,
                    CompletionPercentage = 0.0,
                    LastActivity = DateTime.UtcNow
                };
                
                await Clients.Caller.SendAsync("CurrentFlowProgress", progress);
                _logger.LogInformation("Отправлен текущий прогресс потока {FlowAssignmentId} пользователю {UserId}", flowAssignmentId, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении прогресса потока {FlowAssignmentId}", flowAssignmentId);
                await Clients.Caller.SendAsync("Error", "Не удалось получить прогресс потока");
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