using System;
using System.ComponentModel.DataAnnotations;

namespace Lauf.Domain.Entities.Versions;

/// <summary>
/// Версия компонента статьи
/// </summary>
public class ArticleComponentVersion
{
    /// <summary>
    /// Идентификатор версии компонента (Foreign Key)
    /// </summary>
    public Guid ComponentVersionId { get; private set; }

    /// <summary>
    /// Содержимое статьи в формате Markdown
    /// </summary>
    [Required]
    public string Content { get; private set; } = string.Empty;

    /// <summary>
    /// Время чтения статьи в минутах
    /// </summary>
    public int ReadingTimeMinutes { get; private set; } = 15;

    /// <summary>
    /// Версия компонента, к которой принадлежит эта статья
    /// </summary>
    public virtual ComponentVersion ComponentVersion { get; set; } = null!;

    /// <summary>
    /// Конструктор для EF Core
    /// </summary>
    protected ArticleComponentVersion() { }

    /// <summary>
    /// Конструктор для создания версии статьи
    /// </summary>
    public ArticleComponentVersion(
        Guid componentVersionId,
        string content,
        int readingTimeMinutes = 15)
    {
        ComponentVersionId = componentVersionId;
        Content = content ?? throw new ArgumentNullException(nameof(content));
        ReadingTimeMinutes = readingTimeMinutes;

        ValidateContent();
    }

    /// <summary>
    /// Обновить содержимое статьи
    /// </summary>
    public void UpdateContent(string content, int readingTimeMinutes)
    {
        Content = content ?? throw new ArgumentNullException(nameof(content));
        ReadingTimeMinutes = readingTimeMinutes;

        ValidateContent();
    }

    /// <summary>
    /// Валидация содержимого статьи
    /// </summary>
    private void ValidateContent()
    {
        if (string.IsNullOrWhiteSpace(Content))
        {
            throw new ArgumentException("Содержимое статьи не может быть пустым", nameof(Content));
        }

        if (ReadingTimeMinutes <= 0)
        {
            throw new ArgumentException("Время чтения должно быть больше 0", nameof(ReadingTimeMinutes));
        }

        if (ReadingTimeMinutes > 480) // 8 часов максимум
        {
            throw new ArgumentException("Время чтения не может превышать 8 часов", nameof(ReadingTimeMinutes));
        }
    }

    /// <summary>
    /// Получить количество слов в статье (приблизительно)
    /// </summary>
    public int GetWordCount()
    {
        if (string.IsNullOrWhiteSpace(Content))
            return 0;

        // Простой подсчет слов, исключая markdown разметку
        var words = Content
            .Replace("**", "")  // убираем жирный текст
            .Replace("*", "")   // убираем курсив  
            .Replace("#", "")   // убираем заголовки
            .Replace("`", "")   // убираем код
            .Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        return words.Length;
    }

    /// <summary>
    /// Получить примерное время чтения на основе количества слов
    /// Среднестатистический человек читает 200-250 слов в минуту
    /// </summary>
    public int GetEstimatedReadingTime()
    {
        var wordCount = GetWordCount();
        var estimatedMinutes = Math.Max(1, wordCount / 225); // 225 слов в минуту

        return estimatedMinutes;
    }

    /// <summary>
    /// Проверить, соответствует ли заданное время чтения содержимому
    /// </summary>
    public bool IsReadingTimeRealistic()
    {
        var estimatedTime = GetEstimatedReadingTime();
        var difference = Math.Abs(ReadingTimeMinutes - estimatedTime);
        
        // Допускаем отклонение до 50% от расчетного времени
        return difference <= estimatedTime * 0.5;
    }
}