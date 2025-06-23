using System.ComponentModel;

namespace Lauf.Domain.Enums;

/// <summary>
/// Статус назначения потока пользователю
/// </summary>
public enum AssignmentStatus
{
    /// <summary>
    /// Назначен, но не начат
    /// </summary>
    [Description("Назначен")]
    Assigned = 0,

    /// <summary>
    /// В процессе выполнения
    /// </summary>
    [Description("В процессе")]
    InProgress = 1,

    /// <summary>
    /// Приостановлен
    /// </summary>
    [Description("Приостановлен")]
    Paused = 2,

    /// <summary>
    /// Завершен успешно
    /// </summary>
    [Description("Завершен")]
    Completed = 3,

    /// <summary>
    /// Отменен
    /// </summary>
    [Description("Отменен")]
    Cancelled = 4,

    /// <summary>
    /// Просрочен
    /// </summary>
    [Description("Просрочен")]
    Overdue = 5
}