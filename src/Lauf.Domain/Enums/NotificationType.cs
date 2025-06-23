namespace Lauf.Domain.Enums;

/// <summary>
/// Типы уведомлений в системе
/// </summary>
public enum NotificationType
{
    /// <summary>
    /// Напоминание о дедлайне
    /// </summary>
    DeadlineReminder = 1,

    /// <summary>
    /// Приближающийся дедлайн
    /// </summary>
    DeadlineApproaching = 2,

    /// <summary>
    /// Пропущенный дедлайн
    /// </summary>
    DeadlineOverdue = 3,

    /// <summary>
    /// Завершение компонента
    /// </summary>
    ComponentCompleted = 4,

    /// <summary>
    /// Завершение шага
    /// </summary>
    StepCompleted = 5,

    /// <summary>
    /// Завершение потока
    /// </summary>
    FlowCompleted = 6,

    /// <summary>
    /// Разблокировка нового шага
    /// </summary>
    StepUnlocked = 7,

    /// <summary>
    /// Получение достижения
    /// </summary>
    AchievementEarned = 8,

    /// <summary>
    /// Назначение потока
    /// </summary>
    FlowAssigned = 9,

    /// <summary>
    /// Системное уведомление
    /// </summary>
    SystemNotification = 10,

    /// <summary>
    /// Сообщение от бадди
    /// </summary>
    BuddyMessage = 11
}