using System;

namespace Lauf.Application.DTOs.Flows;

/// <summary>
/// DTO для версии компонента статьи
/// </summary>
public class ArticleComponentVersionDto
{
    /// <summary>
    /// Идентификатор версии компонента
    /// </summary>
    public Guid ComponentVersionId { get; set; }

    /// <summary>
    /// Содержимое статьи в формате Markdown
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Время чтения в минутах
    /// </summary>
    public int ReadingTimeMinutes { get; set; }
}