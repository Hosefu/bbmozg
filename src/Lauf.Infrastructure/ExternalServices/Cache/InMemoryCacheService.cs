using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Lauf.Application.Services.Interfaces;
using System.Text.Json;

namespace Lauf.Infrastructure.ExternalServices.Cache;

/// <summary>
/// Сервис для работы с in-memory кэшем
/// </summary>
public class InMemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<InMemoryCacheService> _logger;

    public InMemoryCacheService(IMemoryCache cache, ILogger<InMemoryCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Получить значение из кэша
    /// </summary>
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var cached = _cache.Get<T>(key);
            if (cached != null)
            {
                _logger.LogDebug("Кэш попадание для ключа {Key}", key);
            }
            else
            {
                _logger.LogDebug("Кэш промах для ключа {Key}", key);
            }
            
            return Task.FromResult(cached);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении из кэша ключа {Key}", key);
            return Task.FromResult<T?>(null);
        }
    }

    /// <summary>
    /// Установить значение в кэш
    /// </summary>
    public Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration,
                Priority = CacheItemPriority.Normal
            };

            _cache.Set(key, value, options);
            _logger.LogDebug("Значение установлено в кэш для ключа {Key} с временем жизни {Expiration}", key, expiration);
            
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при установке в кэш ключа {Key}", key);
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Получить или установить значение в кэш
    /// </summary>
    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> getItem, TimeSpan expiration, CancellationToken cancellationToken = default) where T : class
    {
        var cached = await GetAsync<T>(key, cancellationToken);
        if (cached != null)
        {
            return cached;
        }

        var value = await getItem();
        if (value != null)
        {
            await SetAsync(key, value, expiration, cancellationToken);
        }

        return value!;
    }

    /// <summary>
    /// Установить значение в кэш с абсолютным временем истечения
    /// </summary>
    public Task SetAsync<T>(string key, T value, DateTimeOffset absoluteExpiration, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = absoluteExpiration,
                Priority = CacheItemPriority.Normal
            };

            _cache.Set(key, value, options);
            _logger.LogDebug("Значение установлено в кэш для ключа {Key} с абсолютным временем истечения {Expiration}", key, absoluteExpiration);
            
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при установке в кэш ключа {Key}", key);
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Удалить значение из кэша
    /// </summary>
    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            _cache.Remove(key);
            _logger.LogDebug("Значение удалено из кэша для ключа {Key}", key);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении из кэша ключа {Key}", key);
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Проверить существование ключа в кэше
    /// </summary>
    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var exists = _cache.TryGetValue(key, out _);
            return Task.FromResult(exists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при проверке существования ключа {Key}", key);
            return Task.FromResult(false);
        }
    }

    /// <summary>
    /// Инкремент числового значения
    /// </summary>
    public Task<long> IncrementAsync(string key, long value = 1, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentValue = _cache.Get<long?>(key) ?? 0;
            var newValue = currentValue + value;
            
            var options = new MemoryCacheEntryOptions
            {
                Priority = CacheItemPriority.Normal
            };

            if (expiration.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = expiration.Value;
            }

            _cache.Set(key, newValue, options);
            
            _logger.LogDebug("Инкремент ключа {Key} на {Value}, новое значение: {NewValue}", key, value, newValue);
            return Task.FromResult(newValue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при инкременте ключа {Key}", key);
            return Task.FromResult(0L);
        }
    }

    /// <summary>
    /// Декремент числового значения
    /// </summary>
    public async Task<long> DecrementAsync(string key, long value = 1, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        return await IncrementAsync(key, -value, expiration, cancellationToken);
    }

    /// <summary>
    /// Удалить несколько ключей по паттерну
    /// </summary>
    public Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        // IMemoryCache не поддерживает удаление по паттерну
        _logger.LogWarning("Удаление по паттерну не поддерживается в InMemoryCache");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Установить время жизни для существующего ключа
    /// </summary>
    public Task<bool> ExpireAsync(string key, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        try
        {
            if (_cache.TryGetValue(key, out var value))
            {
                var options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration,
                    Priority = CacheItemPriority.Normal
                };

                _cache.Set(key, value, options);
                _logger.LogDebug("Установлено время жизни {Expiration} для ключа {Key}", expiration, key);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при установке времени жизни для ключа {Key}", key);
            return Task.FromResult(false);
        }
    }

    /// <summary>
    /// Обновить время истечения ключа
    /// </summary>
    public Task RefreshAsync(string key, CancellationToken cancellationToken = default)
    {
        // IMemoryCache не поддерживает обновление времени истечения без пересоздания
        _logger.LogWarning("Обновление времени истечения не поддерживается в InMemoryCache");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Получить время истечения ключа
    /// </summary>
    public Task<TimeSpan?> GetExpirationAsync(string key, CancellationToken cancellationToken = default)
    {
        // IMemoryCache не предоставляет информацию о времени истечения
        _logger.LogWarning("Получение времени истечения не поддерживается в InMemoryCache");
        return Task.FromResult<TimeSpan?>(null);
    }

    /// <summary>
    /// Получить информацию о кэше
    /// </summary>
    public Task<CacheInfo> GetCacheInfoAsync(CancellationToken cancellationToken = default)
    {
        // IMemoryCache не предоставляет подробную статистику
        var cacheInfo = new CacheInfo
        {
            KeyCount = 0, // Не можем получить количество ключей
            UsedMemory = 0, // Не можем получить используемую память
            AvailableMemory = GC.GetTotalMemory(false),
            HitCount = 0, // Не можем получить статистику попаданий
            MissCount = 0
        };
        
        return Task.FromResult(cacheInfo);
    }
}