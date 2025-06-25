namespace Lauf.Application.Services.Interfaces;

/// <summary>
/// Сервис для работы с кэшем
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Получить значение из кэша
    /// </summary>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Получить значение из кэша или выполнить функцию для получения значения
    /// </summary>
    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> getItem, TimeSpan expiration, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Установить значение в кэш
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Установить значение в кэш с абсолютным временем истечения
    /// </summary>
    Task SetAsync<T>(string key, T value, DateTimeOffset absoluteExpiration, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Удалить значение из кэша
    /// </summary>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить все значения по паттерну
    /// </summary>
    Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверить существование ключа в кэше
    /// </summary>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить время истечения ключа
    /// </summary>
    Task RefreshAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить время истечения ключа
    /// </summary>
    Task<TimeSpan?> GetExpirationAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Атомарно увеличить значение счетчика
    /// </summary>
    Task<long> IncrementAsync(string key, long value = 1, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Атомарно уменьшить значение счетчика
    /// </summary>
    Task<long> DecrementAsync(string key, long value = 1, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить информацию о кэше
    /// </summary>
    Task<CacheInfo> GetCacheInfoAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Информация о состоянии кэша
/// </summary>
public class CacheInfo
{
    /// <summary>
    /// Количество ключей в кэше
    /// </summary>
    public long KeyCount { get; set; }

    /// <summary>
    /// Использованная память (в байтах)
    /// </summary>
    public long UsedMemory { get; set; }

    /// <summary>
    /// Доступная память (в байтах)
    /// </summary>
    public long AvailableMemory { get; set; }

    /// <summary>
    /// Количество попаданий в кэш
    /// </summary>
    public long HitCount { get; set; }

    /// <summary>
    /// Количество промахов кэша
    /// </summary>
    public long MissCount { get; set; }

    /// <summary>
    /// Процент попаданий в кэш
    /// </summary>
    public double HitRatio => HitCount + MissCount > 0 ? (double)HitCount / (HitCount + MissCount) * 100 : 0;
}