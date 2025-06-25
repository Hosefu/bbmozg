using Lauf.Domain.Entities.Notifications;
using Lauf.Domain.Enums;

namespace Lauf.Domain.Interfaces.Services;

/// <summary>
/// Сервис для работы с уведомлениями
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Создать и отправить уведомление о назначении потока
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="flowTitle">Название потока</param>
    /// <param name="deadline">Дедлайн завершения</param>
    /// <param name="flowAssignmentId">Идентификатор назначения</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task NotifyFlowAssignedAsync(
        Guid userId, 
        string flowTitle, 
        DateTime deadline,
        Guid flowAssignmentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Создать и отправить напоминание о дедлайне
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="flowTitle">Название потока</param>
    /// <param name="deadline">Дедлайн</param>
    /// <param name="daysLeft">Дней до дедлайна</param>
    /// <param name="flowAssignmentId">Идентификатор назначения</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task NotifyDeadlineReminderAsync(
        Guid userId,
        string flowTitle,
        DateTime deadline,
        int daysLeft,
        Guid flowAssignmentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Создать и отправить уведомление о получении достижения
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="achievementTitle">Название достижения</param>
    /// <param name="achievementDescription">Описание достижения</param>
    /// <param name="achievementId">Идентификатор достижения</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task NotifyAchievementUnlockedAsync(
        Guid userId,
        string achievementTitle,
        string achievementDescription,
        Guid achievementId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Создать и отправить уведомление о завершении компонента
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="componentTitle">Название компонента</param>
    /// <param name="flowTitle">Название потока</param>
    /// <param name="componentId">Идентификатор компонента</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task NotifyComponentCompletedAsync(
        Guid userId,
        string componentTitle,
        string flowTitle,
        Guid componentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Создать и отправить уведомление о разблокировке шага
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="stepTitle">Название шага</param>
    /// <param name="flowTitle">Название потока</param>
    /// <param name="stepId">Идентификатор шага</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task NotifyStepUnlockedAsync(
        Guid userId,
        string stepTitle,
        string flowTitle,
        Guid stepId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Создать кастомное уведомление
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="type">Тип уведомления</param>
    /// <param name="title">Заголовок</param>
    /// <param name="content">Содержимое</param>
    /// <param name="priority">Приоритет</param>
    /// <param name="scheduledAt">Время отправки</param>
    /// <param name="metadata">Дополнительные данные</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task CreateCustomNotificationAsync(
        Guid userId,
        NotificationType type,
        string title,
        string content,
        NotificationPriority priority = NotificationPriority.Medium,
        DateTime? scheduledAt = null,
        string? metadata = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Обработать очередь уведомлений для отправки
    /// </summary>
    /// <param name="batchSize">Размер батча</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Количество обработанных уведомлений</returns>
    Task<int> ProcessPendingNotificationsAsync(
        int batchSize = 50,
        CancellationToken cancellationToken = default);

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
    /// Получить уведомления пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="includeRead">Включать прочитанные</param>
    /// <param name="limit">Максимальное количество</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список уведомлений</returns>
    Task<IReadOnlyList<Notification>> GetUserNotificationsAsync(
        Guid userId,
        bool includeRead = false,
        int limit = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить количество непрочитанных уведомлений
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Количество непрочитанных уведомлений</returns>
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default);

    // Дополнительные методы для специфичных уведомлений

    /// <summary>
    /// Уведомить бадди о назначении подопечного
    /// </summary>
    Task NotifyBuddyAssignedAsync(
        Guid buddyId,
        Guid menteeId, 
        string flowTitle,
        Guid assignmentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Напоминание о неначатом потоке
    /// </summary>
    Task NotifyReminderNotStartedAsync(
        Guid userId,
        string flowTitle,
        DateTime deadline,
        Guid assignmentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Напоминание о прогрессе потока
    /// </summary>
    Task NotifyReminderInProgressAsync(
        Guid userId,
        string flowTitle,
        int progressPercent,
        DateTime deadline,
        Guid assignmentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Напоминание о приближающемся дедлайне
    /// </summary>
    Task NotifyApproachingDeadlineAsync(
        Guid userId,
        string flowTitle,
        DateTime deadline,
        int daysLeft,
        Guid assignmentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Срочное напоминание о дедлайне
    /// </summary>
    Task NotifyUrgentDeadlineAsync(
        Guid userId,
        string flowTitle,
        DateTime deadline,
        Guid assignmentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Уведомление о заработанном достижении
    /// </summary>
    Task NotifyAchievementEarnedAsync(
        Guid userId,
        string achievementTitle,
        string achievementDescription,
        Guid achievementId,
        CancellationToken cancellationToken = default);
}