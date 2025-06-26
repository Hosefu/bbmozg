using System.Diagnostics;
using System.Text;

namespace Lauf.Api.Middleware;

/// <summary>
/// Middleware для детального логирования HTTP запросов и ответов
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString("N")[..8];
        
        // Логируем входящий запрос
        await LogIncomingRequest(context, requestId);

        try
        {
            await _next(context);
            stopwatch.Stop();
            
            // Логируем ответ без перехвата потока
            LogResponse(context, requestId, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, 
                "🚨 [REQ-{RequestId}] ОШИБКА {Method} {Path} за {ElapsedMs}ms", 
                requestId, context.Request.Method, context.Request.Path, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }

    private async Task LogIncomingRequest(HttpContext context, string requestId)
    {
        var request = context.Request;
        var userAgent = request.Headers.UserAgent.ToString();
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        
        // Определяем тип запроса
        var requestType = GetRequestType(request);
        var icon = GetRequestIcon(request);

        _logger.LogInformation(
            "{Icon} [REQ-{RequestId}] {RequestType} {Method} {Path}{QueryString} | IP: {ClientIP} | UA: {UserAgent}",
            icon, requestId, requestType, request.Method, request.Path, request.QueryString, 
            clientIp, userAgent);

        // Логируем заголовки авторизации (без токена)
        if (request.Headers.ContainsKey("Authorization"))
        {
            var authHeader = request.Headers["Authorization"].ToString();
            var authType = authHeader.Split(' ').FirstOrDefault() ?? "Unknown";
            _logger.LogDebug("[REQ-{RequestId}] Auth: {AuthType}", requestId, authType);
        }

        // Логируем тело POST/PUT запросов (только для REST API)
        if (IsRestApiRequest(request) && (request.Method == "POST" || request.Method == "PUT") && request.ContentLength > 0)
        {
            await LogRequestBody(request, requestId);
        }
    }

    private async Task LogRequestBody(HttpRequest request, string requestId)
    {
        try
        {
            request.EnableBuffering();
            var body = await new StreamReader(request.Body).ReadToEndAsync();
            request.Body.Position = 0;

            if (!string.IsNullOrEmpty(body) && body.Length < 2000) // Ограничиваем размер лога
            {
                _logger.LogDebug("[REQ-{RequestId}] Body: {RequestBody}", requestId, body);
            }
            else if (body.Length >= 2000)
            {
                _logger.LogDebug("[REQ-{RequestId}] Body: [Large payload - {Size} chars]", requestId, body.Length);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[REQ-{RequestId}] Не удалось прочитать тело запроса", requestId);
        }
    }

    private void LogResponse(HttpContext context, string requestId, long elapsedMs)
    {
        try
        {
            var response = context.Response;
            var responseSize = response.ContentLength ?? 0;
            
            // Определяем уровень логирования по статус коду
            var logLevel = GetLogLevelByStatusCode(response.StatusCode);
            var icon = GetResponseIcon(response.StatusCode);
            var statusCategory = GetStatusCategory(response.StatusCode);

            _logger.Log(logLevel,
                "{Icon} [REQ-{RequestId}] {StatusCategory} {StatusCode} | {ElapsedMs}ms | {ResponseSize} bytes | {ContentType}",
                icon, requestId, statusCategory, response.StatusCode, elapsedMs, responseSize, 
                response.ContentType ?? "unknown");

            // Логируем медленные запросы отдельно
            if (elapsedMs > 1000)
            {
                _logger.LogWarning(
                    "🐌 [REQ-{RequestId}] МЕДЛЕННЫЙ ЗАПРОС: {Method} {Path} выполнялся {ElapsedMs}ms",
                    requestId, context.Request.Method, context.Request.Path, elapsedMs);
            }

            // Логируем GraphQL операции отдельно
            if (IsGraphQLRequest(context.Request))
            {
                LogGraphQLOperation(context, requestId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[REQ-{RequestId}] Ошибка при логировании ответа", requestId);
        }
    }

    private void LogGraphQLOperation(HttpContext context, string requestId)
    {
        try
        {
            // Извлекаем информацию из query string для SDL запросов
            if (context.Request.Query.ContainsKey("sdl"))
            {
                _logger.LogInformation(
                    "🔸 [REQ-{RequestId}] GraphQL Schema Request (SDL)",
                    requestId);
                return;
            }

            // Для других GraphQL запросов просто логируем базовую информацию
            _logger.LogInformation(
                "🔸 [REQ-{RequestId}] GraphQL Operation",
                requestId);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "[REQ-{RequestId}] Не удалось разобрать GraphQL операцию", requestId);
        }
    }



    private string GetRequestType(HttpRequest request)
    {
        if (IsGraphQLRequest(request)) return "GraphQL";
        if (IsSignalRRequest(request)) return "SignalR";
        if (IsRestApiRequest(request)) return "REST";
        if (IsHealthCheckRequest(request)) return "Health";
        if (IsHangfireRequest(request)) return "Hangfire";
        return "Static";
    }

    private string GetRequestIcon(HttpRequest request)
    {
        if (IsGraphQLRequest(request)) return "🔸";
        if (IsSignalRRequest(request)) return "⚡";
        if (IsRestApiRequest(request)) return "📡";
        if (IsHealthCheckRequest(request)) return "💚";
        if (IsHangfireRequest(request)) return "⚙️";
        return "📄";
    }

    private string GetResponseIcon(int statusCode)
    {
        return statusCode switch
        {
            >= 200 and < 300 => "✅",
            >= 300 and < 400 => "↩️",
            >= 400 and < 500 => "⚠️",
            >= 500 => "🚨",
            _ => "❓"
        };
    }

    private string GetStatusCategory(int statusCode)
    {
        return statusCode switch
        {
            >= 200 and < 300 => "SUCCESS",
            >= 300 and < 400 => "REDIRECT",
            >= 400 and < 500 => "CLIENT_ERROR",
            >= 500 => "SERVER_ERROR",
            _ => "UNKNOWN"
        };
    }

    private LogLevel GetLogLevelByStatusCode(int statusCode)
    {
        return statusCode switch
        {
            >= 200 and < 300 => LogLevel.Information,
            >= 300 and < 400 => LogLevel.Information,
            >= 400 and < 500 => LogLevel.Warning,
            >= 500 => LogLevel.Error,
            _ => LogLevel.Information
        };
    }

    private bool IsGraphQLRequest(HttpRequest request) => 
        request.Path.StartsWithSegments("/graphql");

    private bool IsSignalRRequest(HttpRequest request) => 
        request.Path.StartsWithSegments("/hubs");

    private bool IsRestApiRequest(HttpRequest request) => 
        request.Path.StartsWithSegments("/api");

    private bool IsHealthCheckRequest(HttpRequest request) => 
        request.Path.StartsWithSegments("/health");

    private bool IsHangfireRequest(HttpRequest request) => 
        request.Path.StartsWithSegments("/hangfire");
} 