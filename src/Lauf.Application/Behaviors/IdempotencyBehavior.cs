using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Lauf.Application.Behaviors;

/// <summary>
/// Behavior для обеспечения идемпотентности команд
/// </summary>
/// <typeparam name="TRequest">Тип запроса</typeparam>
/// <typeparam name="TResponse">Тип ответа</typeparam>
public class IdempotencyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<IdempotencyBehavior<TRequest, TResponse>> _logger;

    public IdempotencyBehavior(IMemoryCache cache, ILogger<IdempotencyBehavior<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Проверяем только команды создания
        if (!request.GetType().Name.Contains("Create"))
        {
            return await next();
        }

        // Генерируем ключ для кэша на основе типа команды и её содержимого
        var cacheKey = GenerateCacheKey(request);
        
        // Проверяем, выполняется ли уже такая команда
        if (_cache.TryGetValue(cacheKey, out var cachedResult))
        {
            _logger.LogInformation("Команда {CommandType} с ключом {CacheKey} уже выполняется, возвращаем кэшированный результат", 
                typeof(TRequest).Name, cacheKey);
            return (TResponse)cachedResult!;
        }

        // Помещаем в кэш временную запись о выполнении
        var executionTask = ExecuteCommand(request, next, cancellationToken);
        _cache.Set(cacheKey, executionTask, TimeSpan.FromMinutes(5));

        var result = await executionTask;
        
        // Обновляем кэш с результатом
        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(1));
        
        return result;
    }

    private async Task<TResponse> ExecuteCommand(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при выполнении команды {CommandType}", typeof(TRequest).Name);
            throw;
        }
    }

    private string GenerateCacheKey(TRequest request)
    {
        var requestType = request.GetType().Name;
        var requestJson = System.Text.Json.JsonSerializer.Serialize(request);
        var hash = requestJson.GetHashCode();
        return $"{requestType}_{hash}";
    }
}