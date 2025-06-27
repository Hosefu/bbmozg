namespace Lauf.Domain.Enums;

/// <summary>
/// Статус уведомления (упрощенный)
/// </summary>
public enum NotificationStatus
{
    /// <summary>
    /// Ожидает отправки
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Отправлено
    /// </summary>
    Sent = 1,

    /// <summary>
    /// Ошибка отправки
    /// </summary>
    Failed = 2
}