using Microsoft.AspNetCore.SignalR;

namespace Lauf.Api.SignalR;

/// <summary>
/// Базовый класс для SignalR хабов с логированием
/// </summary>
public abstract class BaseLoggingHub : Hub
{
    protected readonly ILogger _logger;

    protected BaseLoggingHub(ILogger logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var connectionInfo = GetConnectionInfo();
        
        _logger.LogInformation(
            "⚡ [HUB-{HubName}] CONNECT {ConnectionId} | User: {UserId} | IP: {ClientIP} | UA: {UserAgent}",
            GetHubName(), Context.ConnectionId, GetUserId(), connectionInfo.ClientIP, connectionInfo.UserAgent);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionInfo = GetConnectionInfo();
        
        if (exception != null)
        {
            _logger.LogWarning(exception,
                "⚡ [HUB-{HubName}] DISCONNECT_ERROR {ConnectionId} | User: {UserId} | Error: {ErrorMessage}",
                GetHubName(), Context.ConnectionId, GetUserId(), exception.Message);
        }
        else
        {
            _logger.LogInformation(
                "⚡ [HUB-{HubName}] DISCONNECT {ConnectionId} | User: {UserId} | Duration: {Duration}",
                GetHubName(), Context.ConnectionId, GetUserId(), GetConnectionDuration());
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Логирует вызов метода хаба
    /// </summary>
    protected void LogMethodCall(string methodName, object? parameters = null)
    {
        var paramString = parameters != null ? $" | Params: {System.Text.Json.JsonSerializer.Serialize(parameters)}" : "";
        
        _logger.LogInformation(
            "⚡ [HUB-{HubName}] METHOD {ConnectionId} -> {MethodName}{Parameters}",
            GetHubName(), Context.ConnectionId, methodName, paramString);
    }

    /// <summary>
    /// Логирует отправку сообщения клиенту
    /// </summary>
    protected void LogClientCall(string methodName, string? targetConnection = null, object? data = null)
    {
        var target = targetConnection ?? "ALL";
        var dataString = data != null ? $" | Data: {System.Text.Json.JsonSerializer.Serialize(data)}" : "";
        
        _logger.LogInformation(
            "⚡ [HUB-{HubName}] SEND {Target} <- {MethodName}{Data}",
            GetHubName(), target, methodName, dataString);
    }

    /// <summary>
    /// Логирует присоединение к группе
    /// </summary>
    protected async Task LoggedAddToGroupAsync(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        
        _logger.LogInformation(
            "⚡ [HUB-{HubName}] GROUP_JOIN {ConnectionId} -> {GroupName} | User: {UserId}",
            GetHubName(), Context.ConnectionId, groupName, GetUserId());
    }

    /// <summary>
    /// Логирует выход из группы
    /// </summary>
    protected async Task LoggedRemoveFromGroupAsync(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        
        _logger.LogInformation(
            "⚡ [HUB-{HubName}] GROUP_LEAVE {ConnectionId} <- {GroupName} | User: {UserId}",
            GetHubName(), Context.ConnectionId, groupName, GetUserId());
    }

    private ConnectionInfo GetConnectionInfo()
    {
        var httpContext = Context.GetHttpContext();
        if (httpContext == null)
        {
            return new ConnectionInfo { ClientIP = "unknown", UserAgent = "unknown" };
        }

        var clientIP = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var userAgent = httpContext.Request.Headers.UserAgent.ToString();

        return new ConnectionInfo { ClientIP = clientIP, UserAgent = userAgent };
    }

    private string GetUserId()
    {
        return Context.User?.Identity?.Name ?? Context.UserIdentifier ?? "anonymous";
    }

    private string GetHubName()
    {
        return GetType().Name.Replace("Hub", "");
    }

    private string GetConnectionDuration()
    {
        // Простая заглушка для длительности соединения
        return "unknown";
    }

    private record ConnectionInfo
    {
        public string ClientIP { get; init; } = "";
        public string UserAgent { get; init; } = "";
    }
} 