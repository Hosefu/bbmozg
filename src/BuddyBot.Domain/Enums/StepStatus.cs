using System.ComponentModel;

namespace BuddyBot.Domain.Enums;

/// <summary>
/// Статус шага в потоке
/// </summary>
public enum StepStatus
{
    /// <summary>
    /// Черновик - шаг в разработке
    /// </summary>
    [Description("Черновик")]
    Draft = 0,

    /// <summary>
    /// Активный - доступен для прохождения
    /// </summary>
    [Description("Активный")]
    Active = 1,

    /// <summary>
    /// Неактивный - временно отключен
    /// </summary>
    [Description("Неактивный")]
    Inactive = 2
}