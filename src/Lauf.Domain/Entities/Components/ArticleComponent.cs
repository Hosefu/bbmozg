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
    /// Статьи не дают очков
    /// </summary>
    public override bool HasScore => false;

    /// <summary>
    /// Конструктор для создания новой статьи
    /// </summary>
    /// <param name="flowStepId">Идентификатор шага потока</param>
    /// <param name="title">Название статьи</param>
    /// <param name="description">Описание статьи</param>
    /// <param name="content">Содержимое статьи в Markdown</param>
    /// <param name="order">Порядковый номер компонента</param>
    /// <param name="isRequired">Обязательный ли компонент</param>
    public ArticleComponent(Guid flowStepId, string title, string description, string content, string order, bool isRequired = true)
        : base(flowStepId, title, description, content, order, isRequired)
    {
    }

    /// <summary>
    /// Конструктор для EF Core
    /// </summary>
    protected ArticleComponent() { }

    /// <summary>
    /// Вычисляемое время чтения в минутах (примерно 200 слов в минуту)
    /// </summary>
    public int ReadingTimeMinutes => CalculateReadingTime(Content);

    private int CalculateReadingTime(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return 1;
            
        var wordCount = content.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        return Math.Max(1, wordCount / 200);
    }
}