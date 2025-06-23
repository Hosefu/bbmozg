using System.ComponentModel;

namespace Lauf.Domain.Enums;

/// <summary>
/// Тип компонента контента
/// </summary>
public enum ComponentType
{
    /// <summary>
    /// Статья для чтения
    /// </summary>
    [Description("Статья")]
    Article = 0,

    /// <summary>
    /// Квиз с вопросами и вариантами ответов
    /// </summary>
    [Description("Квиз")]
    Quiz = 1,

    /// <summary>
    /// Практическое задание
    /// </summary>
    [Description("Задание")]
    Task = 2,
}