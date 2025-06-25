namespace Lauf.Domain.Enums;

/// <summary>
/// Статус уведомления
/// </summary>
public enum NotificationStatus
{
    /// <summary>
    /// Ожидает отправки
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Отправлено
    /// </summary>
    Sent = 2,

    /// <summary>
    /// Доставлено получателю
    /// </summary>
    Delivered = 3,

    /// <summary>
    /// Прочитано пользователем
    /// </summary>
    Read = 4,

    /// <summary>
    /// Ошибка отправки
    /// </summary>
    Failed = 5,

    /// <summary>
    /// Отменено
    /// </summary>
    Cancelled = 6
}