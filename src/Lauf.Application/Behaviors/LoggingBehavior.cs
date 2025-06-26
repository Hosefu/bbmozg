using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;
using System.Reflection;

namespace Lauf.Application.Behaviors;

/// <summary>
/// Behavior для детального логирования выполнения команд и запросов
/// </summary>
/// <typeparam name="TRequest">Тип запроса</typeparam>
/// <typeparam name="TResponse">Тип ответа</typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestId = Guid.NewGuid().ToString("N")[..8];
        var stopwatch = Stopwatch.StartNew();

        // Определяем тип операции и иконку
        var operationType = GetOperationType(requestName);
        var icon = GetOperationIcon(operationType);

        // Логируем начало с деталями
        _logger.LogInformation(
            "{Icon} [MED-{RequestId}] START {OperationType}: {RequestName} | Handler: {HandlerName}",
            icon, requestId, operationType, requestName, GetHandlerName(requestName));

        // Логируем параметры (если не слишком большие)
        LogRequestParameters(request, requestId);

        try
        {
            var response = await next();
            
            stopwatch.Stop();
            
            // Логируем успешное завершение
            _logger.LogInformation(
                "✅ [MED-{RequestId}] SUCCESS {OperationType}: {RequestName} за {ElapsedMs}ms",
                requestId, operationType, requestName, stopwatch.ElapsedMilliseconds);

            // Логируем медленные операции
            if (stopwatch.ElapsedMilliseconds > 1000)
            {
                _logger.LogWarning(
                    "🐌 [MED-{RequestId}] МЕДЛЕННАЯ ОПЕРАЦИЯ: {RequestName} выполнялась {ElapsedMs}ms",
                    requestId, requestName, stopwatch.ElapsedMilliseconds);
            }

            // Логируем результат (если это не большой объект)
            LogResponseResult(response, requestId, operationType);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, 
                "🚨 [MED-{RequestId}] ERROR {OperationType}: {RequestName} за {ElapsedMs}ms | Ошибка: {ErrorMessage}", 
                requestId, operationType, requestName, stopwatch.ElapsedMilliseconds, ex.Message);
            throw;
        }
    }

    private void LogRequestParameters(TRequest request, string requestId)
    {
        try
        {
            // Получаем параметры через рефлексию
            var requestType = typeof(TRequest);
            var properties = requestType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            if (properties.Length > 0 && properties.Length <= 10) // Не логируем слишком сложные объекты
            {
                var parameters = new Dictionary<string, object?>();
                
                foreach (var prop in properties)
                {
                    try
                    {
                        var value = prop.GetValue(request);
                        if (value != null && !IsComplexType(prop.PropertyType))
                        {
                            parameters[prop.Name] = value;
                        }
                        else if (value != null)
                        {
                            parameters[prop.Name] = $"[{prop.PropertyType.Name}]";
                        }
                    }
                    catch
                    {
                        parameters[prop.Name] = "[Error reading value]";
                    }
                }

                if (parameters.Any())
                {
                    var parametersJson = JsonSerializer.Serialize(parameters, new JsonSerializerOptions 
                    { 
                        WriteIndented = false,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                    
                    _logger.LogDebug("[MED-{RequestId}] Parameters: {Parameters}", requestId, parametersJson);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "[MED-{RequestId}] Не удалось залогировать параметры", requestId);
        }
    }

    private void LogResponseResult(TResponse response, string requestId, string operationType)
    {
        try
        {
            if (response == null)
            {
                _logger.LogDebug("[MED-{RequestId}] Result: null", requestId);
                return;
            }

            var responseType = typeof(TResponse);
            
            // Для простых типов логируем значение
            if (!IsComplexType(responseType))
            {
                _logger.LogDebug("[MED-{RequestId}] Result: {Result}", requestId, response);
            }
            // Для коллекций логируем количество элементов
            else if (response is System.Collections.ICollection collection)
            {
                _logger.LogDebug("[MED-{RequestId}] Result: Collection with {Count} items", requestId, collection.Count);
            }
            // Для объектов логируем тип
            else
            {
                _logger.LogDebug("[MED-{RequestId}] Result: {ResultType}", requestId, responseType.Name);
            }
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "[MED-{RequestId}] Не удалось залогировать результат", requestId);
        }
    }

    private bool IsComplexType(Type type)
    {
        return !type.IsPrimitive 
            && type != typeof(string) 
            && type != typeof(DateTime) 
            && type != typeof(DateTimeOffset)
            && type != typeof(TimeSpan)
            && type != typeof(Guid)
            && !type.IsEnum
            && Nullable.GetUnderlyingType(type) == null;
    }

    private string GetOperationType(string requestName)
    {
        if (requestName.EndsWith("Command")) return "COMMAND";
        if (requestName.EndsWith("Query")) return "QUERY";
        if (requestName.Contains("Notification")) return "EVENT";
        return "REQUEST";
    }

    private string GetOperationIcon(string operationType)
    {
        return operationType switch
        {
            "COMMAND" => "⚡",
            "QUERY" => "🔍",
            "EVENT" => "📢",
            _ => "🔄"
        };
    }

    private string GetHandlerName(string requestName)
    {
        // Пытаемся угадать имя хэндлера по имени запроса
        if (requestName.EndsWith("Command"))
        {
            return requestName.Replace("Command", "CommandHandler");
        }
        if (requestName.EndsWith("Query"))
        {
            return requestName.Replace("Query", "QueryHandler");
        }
        return requestName + "Handler";
    }
}