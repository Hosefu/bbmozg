namespace Lauf.Application.DTOs.Components;

/// <summary>
/// DTO для компонента статьи (упрощенная версия)
/// </summary>
public class ArticleComponentDto
{
    /// <summary>
    /// Уникальный идентификатор компонента
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название статьи
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание статьи
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Содержимое статьи в формате Markdown
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Тип компонента
    /// </summary>
    public string Type { get; set; } = "ARTICLE";

    /// <summary>
    /// Время чтения в минутах
    /// </summary>
    public int ReadingTimeMinutes { get; set; }
}

