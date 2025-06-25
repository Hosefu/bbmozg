namespace Lauf.Domain.Enums;

/// <summary>
/// Каналы доставки уведомлений
/// </summary>
public enum NotificationChannel
{
    /// <summary>
    /// Telegram бот
    /// </summary>
    Telegram = 1,

    /// <summary>
    /// Email
    /// </summary>
    Email = 2,

    /// <summary>
    /// Push уведомления в веб-приложении
    /// </summary>
    WebPush = 3,

    /// <summary>
    /// In-app уведомления в интерфейсе
    /// </summary>
    InApp = 4,

    /// <summary>
    /// SMS (для критических уведомлений)
    /// </summary>
    Sms = 5
}