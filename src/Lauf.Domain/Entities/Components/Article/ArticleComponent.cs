using System.Text.Json;
using Lauf.Domain.Entities.Components.Base;
using Lauf.Domain.Enums;

namespace Lauf.Domain.Entities.Components.Article;

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
    /// URL изображения-обложки статьи
    /// </summary>
    public string? CoverImageUrl { get; private set; }

    /// <summary>
    /// Теги статьи
    /// </summary>
    public List<string> Tags { get; private set; } = new();

    /// <summary>
    /// Автор статьи
    /// </summary>
    public string? Author { get; private set; }

    /// <summary>
    /// Дата публикации статьи
    /// </summary>
    public DateTime? PublishedAt { get; private set; }

    /// <summary>
    /// Количество слов в статье (для расчета времени чтения)
    /// </summary>
    public int WordCount { get; private set; }

    /// <summary>
    /// Дополнительные ресурсы и ссылки
    /// </summary>
    public List<ArticleResource> Resources { get; private set; } = new();

    /// <summary>
    /// Приватный конструктор для EF Core
    /// </summary>
    private ArticleComponent() { }

    /// <summary>
    /// Конструктор для создания компонента статьи
    /// </summary>
    /// <param name="title">Название статьи</param>
    /// <param name="description">Описание статьи</param>
    /// <param name="order">Порядковый номер</param>
    /// <param name="isRequired">Является ли обязательным</param>
    /// <param name="content">Содержимое статьи</param>
    /// <param name="estimatedMinutes">Расчетное время чтения</param>
    /// <param name="settings">Настройки компонента</param>
    /// <param name="coverImageUrl">URL обложки</param>
    /// <param name="author">Автор статьи</param>
    /// <param name="publishedAt">Дата публикации</param>
    public ArticleComponent(
        string title,
        string description,
        int order,
        bool isRequired,
        string content,
        int estimatedMinutes,
        string settings = "{}",
        string? coverImageUrl = null,
        string? author = null,
        DateTime? publishedAt = null)
        : base(title, description, order, isRequired, estimatedMinutes, settings)
    {
        Content = content ?? throw new ArgumentNullException(nameof(content));
        CoverImageUrl = coverImageUrl;
        Author = author;
        PublishedAt = publishedAt;
        WordCount = CalculateWordCount(content);
    }

    /// <summary>
    /// Обновить содержимое статьи
    /// </summary>
    /// <param name="content">Новое содержимое</param>
    /// <param name="coverImageUrl">Новая обложка</param>
    /// <param name="author">Автор</param>
    /// <param name="publishedAt">Дата публикации</param>
    public void UpdateContent(
        string? content = null,
        string? coverImageUrl = null,
        string? author = null,
        DateTime? publishedAt = null)
    {
        if (!string.IsNullOrEmpty(content))
        {
            Content = content;
            WordCount = CalculateWordCount(content);
            
            // Пересчитываем время чтения: примерно 200 слов в минуту
            var estimatedMinutes = Math.Max(1, WordCount / 200);
            EstimatedMinutes = estimatedMinutes;
        }

        if (coverImageUrl != null)
        {
            CoverImageUrl = coverImageUrl;
        }

        if (author != null)
        {
            Author = author;
        }

        if (publishedAt != null)
        {
            PublishedAt = publishedAt;
        }

        UpdatedAt = DateTime.UtcNow;
        Version++;
    }

    /// <summary>
    /// Добавить тег к статье
    /// </summary>
    /// <param name="tag">Тег</param>
    public void AddTag(string tag)
    {
        if (!string.IsNullOrWhiteSpace(tag) && !Tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
        {
            Tags.Add(tag.Trim());
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Удалить тег из статьи
    /// </summary>
    /// <param name="tag">Тег для удаления</param>
    public void RemoveTag(string tag)
    {
        if (!string.IsNullOrWhiteSpace(tag))
        {
            Tags.RemoveAll(t => t.Equals(tag.Trim(), StringComparison.OrdinalIgnoreCase));
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Добавить ресурс к статье
    /// </summary>
    /// <param name="resource">Ресурс</param>
    public void AddResource(ArticleResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        
        if (!Resources.Any(r => r.Url.Equals(resource.Url, StringComparison.OrdinalIgnoreCase)))
        {
            Resources.Add(resource);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Удалить ресурс из статьи
    /// </summary>
    /// <param name="url">URL ресурса</param>
    public void RemoveResource(string url)
    {
        if (!string.IsNullOrWhiteSpace(url))
        {
            Resources.RemoveAll(r => r.Url.Equals(url, StringComparison.OrdinalIgnoreCase));
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Рассчитать количество слов в содержимом
    /// </summary>
    /// <param name="content">Содержимое</param>
    /// <returns>Количество слов</returns>
    private static int CalculateWordCount(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return 0;
        }

        // Убираем markdown разметку и считаем слова
        var cleanContent = content
            .Replace("#", "")
            .Replace("*", "")
            .Replace("**", "")
            .Replace("_", "")
            .Replace("`", "");

        return cleanContent
            .Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries)
            .Length;
    }

    /// <summary>
    /// Получить серилизованное содержимое компонента
    /// </summary>
    public override string SerializeContent()
    {
        var contentData = new
        {
            Content,
            CoverImageUrl,
            Tags,
            Author,
            PublishedAt,
            WordCount,
            Resources = Resources.Select(r => new
            {
                r.Title,
                r.Url,
                r.Type,
                r.Description
            }).ToList()
        };

        return JsonSerializer.Serialize(contentData);
    }

    /// <summary>
    /// Валидировать содержимое компонента
    /// </summary>
    public override List<string> ValidateContent()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Content))
        {
            errors.Add("Содержимое статьи не может быть пустым");
        }

        if (Content.Length > 50000)
        {
            errors.Add("Содержимое статьи слишком большое (максимум 50000 символов)");
        }

        if (!string.IsNullOrEmpty(CoverImageUrl) && !Uri.IsWellFormedUriString(CoverImageUrl, UriKind.Absolute))
        {
            errors.Add("Некорректный URL обложки");
        }

        foreach (var resource in Resources)
        {
            if (!Uri.IsWellFormedUriString(resource.Url, UriKind.Absolute))
            {
                errors.Add($"Некорректный URL ресурса: {resource.Title}");
            }
        }

        return errors;
    }

    /// <summary>
    /// Создать копию компонента для снапшота
    /// </summary>
    public override ComponentBase CreateSnapshot()
    {
        var snapshot = new ArticleComponent(
            Title,
            Description,
            Order,
            IsRequired,
            Content,
            EstimatedMinutes,
            Settings,
            CoverImageUrl,
            Author,
            PublishedAt);

        // Копируем теги и ресурсы
        snapshot.Tags = new List<string>(Tags);
        snapshot.Resources = Resources.Select(r => new ArticleResource(r.Title, r.Url, r.Type, r.Description)).ToList();

        return snapshot;
    }
}