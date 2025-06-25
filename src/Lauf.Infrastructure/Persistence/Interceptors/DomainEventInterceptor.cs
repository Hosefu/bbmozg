using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using Lauf.Domain.Events;

namespace Lauf.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Перехватчик для обработки доменных событий
/// </summary>
public class DomainEventInterceptor : SaveChangesInterceptor
{
    private readonly IMediator _mediator;
    private readonly ILogger<DomainEventInterceptor> _logger;

    public DomainEventInterceptor(IMediator mediator, ILogger<DomainEventInterceptor> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Перехват операций сохранения (синхронная версия)
    /// </summary>
    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        if (eventData.Context != null)
        {
            DispatchDomainEventsSync(eventData.Context);
        }

        return base.SavedChanges(eventData, result);
    }

    /// <summary>
    /// Перехват операций сохранения (асинхронная версия)
    /// </summary>
    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData, 
        int result, 
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context != null)
        {
            await DispatchDomainEventsAsync(eventData.Context, cancellationToken);
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// Синхронная отправка доменных событий
    /// </summary>
    private void DispatchDomainEventsSync(DbContext context)
    {
        var domainEvents = ExtractDomainEvents(context);

        if (!domainEvents.Any())
        {
            return;
        }

        _logger.LogInformation("Обработка {Count} доменных событий", domainEvents.Count);

        foreach (var domainEvent in domainEvents)
        {
            try
            {
                _logger.LogDebug("Публикация доменного события: {EventType}", domainEvent.GetType().Name);
                
                // Создаем wrapper для MediatR
                var notification = CreateNotificationWrapper(domainEvent);
                if (notification != null)
                {
                    // Синхронная публикация (не рекомендуется для продакшена)
                    _mediator.Publish(notification).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке доменного события {EventType}", domainEvent.GetType().Name);
                // Не прерываем выполнение, чтобы не откатить транзакцию
            }
        }
    }

    /// <summary>
    /// Асинхронная отправка доменных событий
    /// </summary>
    private async Task DispatchDomainEventsAsync(DbContext context, CancellationToken cancellationToken)
    {
        var domainEvents = ExtractDomainEvents(context);

        if (!domainEvents.Any())
        {
            return;
        }

        _logger.LogInformation("Асинхронная обработка {Count} доменных событий", domainEvents.Count);

        var tasks = domainEvents.Select(async domainEvent =>
        {
            try
            {
                _logger.LogDebug("Публикация доменного события: {EventType}", domainEvent.GetType().Name);
                
                // Создаем wrapper для MediatR
                var notification = CreateNotificationWrapper(domainEvent);
                if (notification != null)
                {
                    await _mediator.Publish(notification, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке доменного события {EventType}", domainEvent.GetType().Name);
                // Не прерываем выполнение, чтобы не откатить транзакцию
            }
        });

        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Извлечение доменных событий из контекста
    /// </summary>
    private List<IDomainEvent> ExtractDomainEvents(DbContext context)
    {
        var domainEvents = new List<IDomainEvent>();

        var entities = context.ChangeTracker.Entries()
            .Where(e => e.Entity is IHasDomainEvents)
            .Select(e => e.Entity as IHasDomainEvents)
            .Where(e => e != null && e.DomainEvents.Any())
            .ToList();

        foreach (var entity in entities)
        {
            if (entity != null)
            {
                domainEvents.AddRange(entity.DomainEvents);
                entity.ClearDomainEvents();
            }
        }

        return domainEvents;
    }

    /// <summary>
    /// Создание wrapper'а для MediatR из доменного события
    /// </summary>
    private INotification? CreateNotificationWrapper(IDomainEvent domainEvent)
    {
        // Здесь нужно создать соответствующий Notification wrapper
        // В зависимости от типа доменного события
        
        return domainEvent.GetType().Name switch
        {
            nameof(Domain.Events.FlowAssigned) => 
                new Application.EventHandlers.Events.FlowAssignedNotification((Domain.Events.FlowAssigned)domainEvent),
            
            nameof(Domain.Events.ComponentCompleted) => 
                new Application.EventHandlers.Events.ComponentCompletedNotification((Domain.Events.ComponentCompleted)domainEvent),
            
            // Добавляем другие события по мере необходимости
            _ => null
        };
    }
}

/// <summary>
/// Интерфейс для сущностей с доменными событиями
/// </summary>
public interface IHasDomainEvents
{
    /// <summary>
    /// Список доменных событий
    /// </summary>
    IReadOnlyList<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// Добавить доменное событие
    /// </summary>
    void AddDomainEvent(IDomainEvent domainEvent);

    /// <summary>
    /// Очистить доменные события
    /// </summary>
    void ClearDomainEvents();
}