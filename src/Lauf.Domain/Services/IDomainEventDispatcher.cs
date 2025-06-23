using Lauf.Domain.Events;

namespace Lauf.Domain.Services;

/// <summary>
/// Интерфейс диспетчера доменных событий
/// </summary>
public interface IDomainEventDispatcher
{
    /// <summary>
    /// Опубликовать доменное событие
    /// </summary>
    /// <param name="domainEvent">Доменное событие</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Опубликовать несколько доменных событий
    /// </summary>
    /// <param name="domainEvents">Список доменных событий</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task PublishManyAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);

    /// <summary>
    /// Подписаться на доменное событие
    /// </summary>
    /// <typeparam name="TEvent">Тип события</typeparam>
    /// <param name="handler">Обработчик события</param>
    void Subscribe<TEvent>(Func<TEvent, CancellationToken, Task> handler) where TEvent : IDomainEvent;

    /// <summary>
    /// Отписаться от доменного события
    /// </summary>
    /// <typeparam name="TEvent">Тип события</typeparam>
    /// <param name="handler">Обработчик события</param>
    void Unsubscribe<TEvent>(Func<TEvent, CancellationToken, Task> handler) where TEvent : IDomainEvent;

    /// <summary>
    /// Получить все события для аудита
    /// </summary>
    /// <param name="fromDate">Дата начала</param>
    /// <param name="toDate">Дата окончания</param>
    /// <param name="eventTypes">Типы событий для фильтрации</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список событий</returns>
    Task<List<IDomainEvent>> GetEventsForAuditAsync(
        DateTime fromDate,
        DateTime toDate,
        string[]? eventTypes = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить события для конкретного пользователя
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="fromDate">Дата начала</param>
    /// <param name="toDate">Дата окончания</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список событий пользователя</returns>
    Task<List<IDomainEvent>> GetUserEventsAsync(
        Guid userId,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken = default);
}