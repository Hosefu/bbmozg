using Lauf.Domain.Enums;

namespace Lauf.Domain.Entities.Components;

/// <summary>
/// Компонент статьи для чтения
/// </summary>
public class ArticleComponent : ComponentBase
{
    /// <summary>
    /// Тип компонента
    /// </summary>
    public override ComponentType Type => ComponentType.Article;

    /// <summary>
    /// Содержимое статьи в формате Markdown
    /// </summary>
    public string Content { get; private set; } = string.Empty;

    /// <summary>
    /// Время чтения в минутах
    /// </summary>
    public int ReadingTimeMinutes { get; private set; }

    /// <summary>
    /// Конструктор для создания новой статьи
    /// </summary>
    /// <param name="title">Название статьи</param>
    /// <param name="description">Описание статьи</param>
    /// <param name="content">Содержимое статьи в Markdown</param>
    /// <param name="readingTimeMinutes">Время чтения в минутах</param>
    public ArticleComponent(string title, string description, string content, int readingTimeMinutes = 15)
        : base(title, description, readingTimeMinutes)
    {
        Content = content ?? throw new ArgumentNullException(nameof(content));
        ReadingTimeMinutes = readingTimeMinutes;
    }

    /// <summary>
    /// Конструктор для EF Core
    /// </summary>
    protected ArticleComponent() { }

    /// <summary>
    /// Обновляет содержимое статьи
    /// </summary>
    /// <param name="content">Новое содержимое в Markdown</param>
    /// <param name="readingTimeMinutes">Новое время чтения</param>
    public void UpdateContent(string content, int? readingTimeMinutes = null)
    {
        Content = content ?? throw new ArgumentNullException(nameof(content));
        
        if (readingTimeMinutes.HasValue)
        {
            ReadingTimeMinutes = readingTimeMinutes.Value;
            EstimatedDurationMinutes = readingTimeMinutes.Value;
        }
        
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Проверяет, может ли компонент быть активирован
    /// </summary>
    public override bool CanBeActivated()
    {
        return !string.IsNullOrWhiteSpace(Content) && 
               !string.IsNullOrWhiteSpace(Title);
    }
}