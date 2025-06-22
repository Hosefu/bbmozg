using System.ComponentModel;

namespace BuddyBot.Domain.Enums;

/// <summary>
/// Статус компонента в шаге
/// </summary>
public enum ComponentStatus
{
    /// <summary>
    /// Черновик - компонент в разработке
    /// </summary>
    [Description("Черновик")]
    Draft = 0,

    /// <summary>
    /// Активный - доступен для взаимодействия
    /// </summary>
    [Description("Активный")]
    Active = 1,

    /// <summary>
    /// Неактивный - временно отключен
    /// </summary>
    [Description("Неактивный")]
    Inactive = 2
}