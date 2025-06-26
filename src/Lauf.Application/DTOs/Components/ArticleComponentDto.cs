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
    /// Время чтения в минутах
    /// </summary>
    public int ReadingTimeMinutes { get; set; }

    /// <summary>
    /// Статус компонента
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

