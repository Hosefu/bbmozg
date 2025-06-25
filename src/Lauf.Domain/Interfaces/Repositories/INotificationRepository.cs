using Lauf.Domain.Entities.Notifications;
using Lauf.Domain.Enums;

namespace Lauf.Domain.Interfaces.Repositories;

/// <summary>
/// Репозиторий для работы с уведомлениями
/// </summary>
public interface INotificationRepository
{
    /// <summary>
    /// Получить уведомление по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор уведомления</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Уведомление или null</returns>
    Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить уведомления пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="includeRead">Включать прочитанные уведомления</param>
    /// <param name="limit">Максимальное количество уведомлений</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список уведомлений</returns>
    Task<IReadOnlyList<Notification>> GetUserNotificationsAsync(
        Guid userId, 
        bool includeRead = false, 
        int limit = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить уведомления готовые к отправке
    /// </summary>
    /// <param name="batchSize">Размер батча</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список уведомлений для отправки</returns>
    Task<IReadOnlyList<Notification>> GetPendingNotificationsAsync(
        int batchSize = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить количество непрочитанных уведомлений пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Количество непрочитанных уведомлений</returns>
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавить уведомление
    /// </summary>
    /// <param name="notification">Уведомление</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task AddAsync(Notification notification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавить несколько уведомлений
    /// </summary>
    /// <param name="notifications">Список уведомлений</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task AddRangeAsync(IEnumerable<Notification> notifications, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить уведомление
    /// </summary>
    /// <param name="notification">Уведомление</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отметить уведомление как прочитанное
    /// </summary>
    /// <param name="notificationId">Идентификатор уведомления</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task MarkAsReadAsync(Guid notificationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отметить все уведомления пользователя как прочитанные
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить старые уведомления
    /// </summary>
    /// <param name="olderThan">Удалить уведомления старше указанной даты</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Количество удаленных уведомлений</returns>
    Task<int> DeleteOldNotificationsAsync(DateTime olderThan, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить статистику уведомлений по пользователю
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Статистика уведомлений</returns>
    Task<NotificationStatistics> GetUserStatisticsAsync(Guid userId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Статистика уведомлений пользователя
/// </summary>
public class NotificationStatistics
{
    /// <summary>
    /// Общее количество уведомлений
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Количество непрочитанных уведомлений
    /// </summary>
    public int UnreadCount { get; set; }

    /// <summary>
    /// Количество уведомлений по типам
    /// </summary>
    public Dictionary<NotificationType, int> CountByType { get; set; } = new();

    /// <summary>
    /// Количество уведомлений по приоритетам
    /// </summary>
    public Dictionary<NotificationPriority, int> CountByPriority { get; set; } = new();
}