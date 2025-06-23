using System.ComponentModel;

namespace Lauf.Domain.Enums;

/// <summary>
/// Статус потока обучения
/// </summary>
public enum FlowStatus
{
    /// <summary>
    /// Черновик - поток в разработке
    /// </summary>
    [Description("Черновик")]
    Draft = 0,

    /// <summary>
    /// Опубликован и доступен для назначения
    /// </summary>
    [Description("Опубликован")]
    Published = 1,

    /// <summary>
    /// Архивный - недоступен для новых назначений
    /// </summary>
    [Description("Архивный")]
    Archived = 2
}