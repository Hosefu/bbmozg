using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Lauf.Domain.Entities.Notifications;
using Lauf.Domain.Enums;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Infrastructure.Persistence;

namespace Lauf.Infrastructure.Persistence.Repositories;

/// <summary>
/// Репозиторий для работы с уведомлениями
/// </summary>
public class NotificationRepository : INotificationRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<NotificationRepository> _logger;

    public NotificationRepository(
        ApplicationDbContext context,
        ILogger<NotificationRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Получить уведомление по идентификатору
    /// </summary>
    public async Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .Include(n => n.User)
            .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
    }

    /// <summary>
    /// Получить уведомления пользователя
    /// </summary>
    public async Task<IReadOnlyList<Notification>> GetUserNotificationsAsync(
        Guid userId, 
        bool includeRead = false, 
        int limit = 50,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Notifications
            .Where(n => n.UserId == userId);

        if (!includeRead)
        {
            query = query.Where(n => n.Status != NotificationStatus.Read);
        }

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Получить уведомления готовые к отправке
    /// </summary>
    public async Task<IReadOnlyList<Notification>> GetPendingNotificationsAsync(
        int batchSize = 100,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        
        return await _context.Notifications
            .Include(n => n.User)
            .Where(n => n.Status == NotificationStatus.Pending)
            .Where(n => n.ScheduledAt <= now)
            .Where(n => n.AttemptCount < n.MaxAttempts)
            .OrderBy(n => n.Priority)
            .ThenBy(n => n.ScheduledAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Получить количество непрочитанных уведомлений пользователя
    /// </summary>
    public async Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .Where(n => n.Status != NotificationStatus.Read)
            .CountAsync(cancellationToken);
    }

    /// <summary>
    /// Добавить уведомление
    /// </summary>
    public async Task AddAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        await _context.Notifications.AddAsync(notification, cancellationToken);
        
        _logger.LogInformation(
            "Добавлено уведомление {NotificationId} для пользователя {UserId} типа {Type}",
            notification.Id, notification.UserId, notification.Type);
    }

    /// <summary>
    /// Добавить несколько уведомлений
    /// </summary>
    public async Task AddRangeAsync(IEnumerable<Notification> notifications, CancellationToken cancellationToken = default)
    {
        var notificationList = notifications.ToList();
        await _context.Notifications.AddRangeAsync(notificationList, cancellationToken);
        
        _logger.LogInformation(
            "Добавлено {Count} уведомлений",
            notificationList.Count);
    }

    /// <summary>
    /// Обновить уведомление
    /// </summary>
    public async Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        _context.Notifications.Update(notification);
        
        _logger.LogDebug(
            "Обновлено уведомление {NotificationId}, статус: {Status}",
            notification.Id, notification.Status);
        
        await Task.CompletedTask;
    }

    /// <summary>
    /// Отметить уведомление как прочитанное
    /// </summary>
    public async Task MarkAsReadAsync(Guid notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await GetByIdAsync(notificationId, cancellationToken);
        if (notification != null)
        {
            notification.MarkAsRead();
            await UpdateAsync(notification, cancellationToken);
        }
    }

    /// <summary>
    /// Отметить все уведомления пользователя как прочитанные
    /// </summary>
    public async Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var unreadNotifications = await _context.Notifications
            .Where(n => n.UserId == userId)
            .Where(n => n.Status != NotificationStatus.Read)
            .ToListAsync(cancellationToken);

        foreach (var notification in unreadNotifications)
        {
            notification.MarkAsRead();
        }

        _logger.LogInformation(
            "Отмечено как прочитанные {Count} уведомлений для пользователя {UserId}",
            unreadNotifications.Count, userId);
    }

    /// <summary>
    /// Удалить старые уведомления
    /// </summary>
    public async Task<int> DeleteOldNotificationsAsync(DateTime olderThan, CancellationToken cancellationToken = default)
    {
        var oldNotifications = await _context.Notifications
            .Where(n => n.CreatedAt < olderThan)
            .Where(n => n.Status == NotificationStatus.Read || n.Status == NotificationStatus.Failed)
            .ToListAsync(cancellationToken);

        _context.Notifications.RemoveRange(oldNotifications);
        
        _logger.LogInformation(
            "Удалено {Count} старых уведомлений (старше {Date})",
            oldNotifications.Count, olderThan);

        return oldNotifications.Count;
    }

    /// <summary>
    /// Получить статистику уведомлений по пользователю
    /// </summary>
    public async Task<NotificationStatistics> GetUserStatisticsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId)
            .Select(n => new { n.Type, n.Priority, n.Status })
            .ToListAsync(cancellationToken);

        return new NotificationStatistics
        {
            TotalCount = notifications.Count,
            UnreadCount = notifications.Count(n => n.Status != NotificationStatus.Read),
            CountByType = notifications
                .GroupBy(n => n.Type)
                .ToDictionary(g => g.Key, g => g.Count()),
            CountByPriority = notifications
                .GroupBy(n => n.Priority)
                .ToDictionary(g => g.Key, g => g.Count())
        };
    }
}