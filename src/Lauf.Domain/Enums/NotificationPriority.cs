namespace Lauf.Domain.Enums;

/// <summary>
/// Приоритет уведомлений
/// </summary>
public enum NotificationPriority
{
    /// <summary>
    /// Низкий приоритет - общая информация
    /// </summary>
    Low = 1,

    /// <summary>
    /// Обычный приоритет - важная информация
    /// </summary>
    Medium = 2,

    /// <summary>
    /// Высокий приоритет - требует внимания
    /// </summary>
    High = 3,

    /// <summary>
    /// Критический приоритет - требует немедленного внимания
    /// </summary>
    Critical = 4
}