using System.ComponentModel;

namespace BuddyBot.Domain.Enums;

/// <summary>
/// Статус прогресса компонента
/// </summary>
public enum ProgressStatus
{
    /// <summary>
    /// Не начат
    /// </summary>
    [Description("Не начат")]
    NotStarted = 0,

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
    /// Завершен
    /// </summary>
    [Description("Завершен")]
    Completed = 3,

    /// <summary>
    /// Пропущен
    /// </summary>
    [Description("Пропущен")]
    Skipped = 4
}