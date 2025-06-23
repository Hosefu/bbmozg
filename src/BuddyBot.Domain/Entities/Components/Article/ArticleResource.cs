namespace BuddyBot.Domain.Entities.Components.Article;

/// <summary>
/// Ресурс статьи (ссылка, документ, видео и т.д.)
/// </summary>
public class ArticleResource
{
    /// <summary>
    /// Название ресурса
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// URL ресурса
    /// </summary>
    public string Url { get; private set; }

    /// <summary>
    /// Тип ресурса
    /// </summary>
    public string Type { get; private set; }

    /// <summary>
    /// Описание ресурса
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Конструктор для создания ресурса статьи
    /// </summary>
    /// <param name="title">Название ресурса</param>
    /// <param name="url">URL ресурса</param>
    /// <param name="type">Тип ресурса (link, document, video, etc.)</param>
    /// <param name="description">Описание ресурса</param>
    public ArticleResource(string title, string url, string type, string? description = null)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Url = url ?? throw new ArgumentNullException(nameof(url));
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Description = description;

        // Валидация URL
        if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
        {
            throw new ArgumentException("Некорректный URL ресурса", nameof(url));
        }
    }

    /// <summary>
    /// Обновить информацию о ресурсе
    /// </summary>
    /// <param name="title">Новое название</param>
    /// <param name="description">Новое описание</param>
    /// <param name="type">Новый тип</param>
    public void Update(string? title = null, string? description = null, string? type = null)
    {
        if (!string.IsNullOrWhiteSpace(title))
        {
            Title = title;
        }

        if (description != null)
        {
            Description = description;
        }

        if (!string.IsNullOrWhiteSpace(type))
        {
            Type = type;
        }
    }

    /// <summary>
    /// Проверить, является ли ресурс внешней ссылкой
    /// </summary>
    public bool IsExternalLink()
    {
        return Type.Equals("link", StringComparison.OrdinalIgnoreCase) ||
               Type.Equals("external", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Проверить, является ли ресурс документом
    /// </summary>
    public bool IsDocument()
    {
        return Type.Equals("document", StringComparison.OrdinalIgnoreCase) ||
               Type.Equals("pdf", StringComparison.OrdinalIgnoreCase) ||
               Type.Equals("doc", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Проверить, является ли ресурс видео
    /// </summary>
    public bool IsVideo()
    {
        return Type.Equals("video", StringComparison.OrdinalIgnoreCase) ||
               Type.Equals("youtube", StringComparison.OrdinalIgnoreCase) ||
               Type.Equals("vimeo", StringComparison.OrdinalIgnoreCase);
    }
}