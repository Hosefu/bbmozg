using System.ComponentModel;

namespace Lauf.Domain.Enums;

/// <summary>
/// Статус прогресса (обновленный)
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
    /// Завершен
    /// </summary>
    [Description("Завершен")]
    Completed = 2,

    /// <summary>
    /// Неудачная попытка
    /// </summary>
    [Description("Неудачно")]
    Failed = 3,

    /// <summary>
    /// Отменено
    /// </summary>
    [Description("Отменено")]
    Cancelled = 4
}