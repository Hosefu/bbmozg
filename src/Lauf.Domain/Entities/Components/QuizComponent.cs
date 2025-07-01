using Lauf.Domain.Enums;
using Lauf.Shared.Helpers;

namespace Lauf.Domain.Entities.Components;

/// <summary>
/// Компонент квиза
/// </summary>
public class QuizComponent : ComponentBase
{
    /// <summary>
    /// Тип компонента
    /// </summary>
    public override ComponentType Type => ComponentType.Quiz;

    /// <summary>
    /// Тесты дают очки
    /// </summary>
    public override bool HasScore => true;

    /// <summary>
    /// Вопросы квиза
    /// </summary>
    public virtual ICollection<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();

    /// <summary>
    /// Конструктор для создания нового квиза
    /// </summary>
    /// <param name="flowStepId">Идентификатор шага потока</param>
    /// <param name="title">Название квиза</param>
    /// <param name="description">Описание квиза</param>
    /// <param name="content">Содержимое квиза</param>
    /// <param name="order">Порядковый номер компонента</param>
    /// <param name="isRequired">Обязательный ли компонент</param>
    public QuizComponent(Guid flowStepId, string title, string description, string content, string order, bool isRequired = true)
        : base(flowStepId, title, description, content, order, isRequired)
    {
    }

    /// <summary>
    /// Конструктор для EF Core
    /// </summary>
    protected QuizComponent() { }

    /// <summary>
    /// Подсчитывает общий балл
    /// </summary>
    /// <returns>Общее количество баллов</returns>
    public override int GetTotalScore()
    {
        return Questions?.Sum(q => q.GetMaxScore()) ?? 0;
    }
}

/// <summary>
/// Вопрос теста
/// </summary>
public class QuizQuestion
{
    /// <summary>
    /// Уникальный идентификатор вопроса
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор квиза
    /// </summary>
    public Guid QuizComponentId { get; set; }

    /// <summary>
    /// Текст вопроса
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Обязательный ли вопрос
    /// </summary>
    public bool IsRequired { get; set; } = true;

    /// <summary>
    /// Порядок вопроса
    /// </summary>
    public string Order { get; set; } = string.Empty;

    /// <summary>
    /// Квиз
    /// </summary>
    public virtual QuizComponent QuizComponent { get; set; } = null!;

    /// <summary>
    /// Варианты ответов
    /// </summary>
    public virtual ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();

    /// <summary>
    /// Конструктор для создания нового вопроса
    /// </summary>
    /// <param name="quizComponentId">Идентификатор квиза</param>
    /// <param name="text">Текст вопроса</param>
    /// <param name="order">Порядок</param>
    public QuizQuestion(Guid quizComponentId, string text, string order)
    {
        Id = Guid.NewGuid();
        QuizComponentId = quizComponentId;
        Text = text ?? throw new ArgumentNullException(nameof(text));
        Order = order ?? throw new ArgumentNullException(nameof(order));
        IsRequired = true;
    }

    /// <summary>
    /// Конструктор для EF Core
    /// </summary>
    protected QuizQuestion() { }

    /// <summary>
    /// Максимальный балл за вопрос
    /// </summary>
    public int GetMaxScore()
    {
        return Options?.Where(o => o.IsCorrect).Sum(o => o.Score) ?? 0;
    }
}

/// <summary>
/// Вариант ответа
/// </summary>
public class QuestionOption
{
    /// <summary>
    /// Уникальный идентификатор варианта
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор вопроса
    /// </summary>
    public Guid QuizQuestionId { get; set; }

    /// <summary>
    /// Текст варианта ответа
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Является ли вариант правильным
    /// </summary>
    public bool IsCorrect { get; set; }

    /// <summary>
    /// Количество баллов (переименовано с Points)
    /// </summary>
    public int Score { get; set; } = 1;

    /// <summary>
    /// Порядок варианта
    /// </summary>
    public string Order { get; set; } = string.Empty;

    /// <summary>
    /// Вопрос
    /// </summary>
    public virtual QuizQuestion QuizQuestion { get; set; } = null!;

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="quizQuestionId">Идентификатор вопроса</param>
    /// <param name="text">Текст варианта</param>
    /// <param name="isCorrect">Правильный ли вариант</param>
    /// <param name="score">Количество баллов</param>
    /// <param name="order">Порядок</param>
    public QuestionOption(Guid quizQuestionId, string text, bool isCorrect, int score, string order)
    {
        Id = Guid.NewGuid();
        QuizQuestionId = quizQuestionId;
        Text = text ?? throw new ArgumentNullException(nameof(text));
        IsCorrect = isCorrect;
        Score = score > 0 ? score : 1;
        Order = order ?? throw new ArgumentNullException(nameof(order));
    }

    /// <summary>
    /// Конструктор для EF Core
    /// </summary>
    protected QuestionOption() { }
}