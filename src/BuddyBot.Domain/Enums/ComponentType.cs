using System.ComponentModel;

namespace BuddyBot.Domain.Enums;

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
    /// Видеоматериал
    /// </summary>
    [Description("Видео")]
    Video = 1,

    /// <summary>
    /// Квиз с вопросами и вариантами ответов
    /// </summary>
    [Description("Квиз")]
    Quiz = 2,

    /// <summary>
    /// Практическое задание
    /// </summary>
    [Description("Задание")]
    Task = 3,

    /// <summary>
    /// Опрос или анкета
    /// </summary>
    [Description("Опрос")]
    Survey = 4,

    /// <summary>
    /// Файл для скачивания
    /// </summary>
    [Description("Файл")]
    File = 5,

    /// <summary>
    /// Ссылка на внешний ресурс
    /// </summary>
    [Description("Ссылка")]
    Link = 6,

    /// <summary>
    /// Интерактивное задание
    /// </summary>
    [Description("Интерактив")]
    Interactive = 7
}