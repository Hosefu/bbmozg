namespace Lauf.Domain.Enums;

/// <summary>
/// Каналы доставки уведомлений (упрощенный)
/// </summary>
public enum NotificationChannel
{
    /// <summary>
    /// Telegram бот
    /// </summary>
    Telegram = 0,

    /// <summary>
    /// Системные уведомления
    /// </summary>
    System = 1
}