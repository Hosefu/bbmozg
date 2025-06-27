using Lauf.Domain.Enums;
using Lauf.Shared.Helpers;

namespace Lauf.Domain.Entities.Components;

/// <summary>
/// Компонент квиза - один вопрос с 5 вариантами ответов
/// </summary>
public class QuizComponent : ComponentBase
{
    /// <summary>
    /// Тип компонента
    /// </summary>
    public override ComponentType Type => ComponentType.Quiz;

    /// <summary>
    /// Текст вопроса
    /// </summary>
    public string QuestionText { get; private set; } = string.Empty;

    /// <summary>
    /// Варианты ответов (ровно 5)
    /// </summary>
    public List<QuestionOption> Options { get; private set; } = new();

    /// <summary>
    /// Конструктор для создания нового квиза
    /// </summary>
    /// <param name="flowStepId">Идентификатор шага потока</param>
    /// <param name="title">Название квиза</param>
    /// <param name="description">Описание квиза</param>
    /// <param name="questionText">Текст вопроса</param>
    /// <param name="order">Порядковый номер компонента</param>
    /// <param name="isRequired">Обязательный ли компонент</param>
    /// <param name="estimatedDurationMinutes">Приблизительное время выполнения</param>
    public QuizComponent(Guid flowStepId, string title, string description, string questionText, string order, bool isRequired = true, int estimatedDurationMinutes = 5)
        : base(flowStepId, title, description, order, isRequired, estimatedDurationMinutes)
    {
        QuestionText = questionText ?? throw new ArgumentNullException(nameof(questionText));
    }

    /// <summary>
    /// Конструктор для создания нового квиза с вариантами ответов
    /// </summary>
    /// <param name="flowStepId">Идентификатор шага потока</param>
    /// <param name="title">Название квиза</param>
    /// <param name="description">Описание квиза</param>
    /// <param name="questionText">Текст вопроса</param>
    /// <param name="options">Варианты ответов</param>
    /// <param name="order">Порядковый номер компонента</param>
    /// <param name="isRequired">Обязательный ли компонент</param>
    /// <param name="estimatedDurationMinutes">Приблизительное время выполнения</param>
    public QuizComponent(Guid flowStepId, string title, string description, string questionText, List<QuestionOption> options, string order, bool isRequired = true, int estimatedDurationMinutes = 5)
        : base(flowStepId, title, description, order, isRequired, estimatedDurationMinutes)
    {
        QuestionText = questionText ?? throw new ArgumentNullException(nameof(questionText));
        Options = options ?? new List<QuestionOption>();
    }

    /// <summary>
    /// Конструктор для EF Core
    /// </summary>
    protected QuizComponent() { }

    /// <summary>
    /// Устанавливает вопрос и варианты ответов
    /// </summary>
    /// <param name="questionText">Текст вопроса</param>
    /// <param name="options">Варианты ответов (ровно 5)</param>
    public void SetQuestion(string questionText, List<QuestionOption> options)
    {
        if (string.IsNullOrWhiteSpace(questionText))
            throw new ArgumentException("Текст вопроса не может быть пустым", nameof(questionText));
        
        if (options == null || options.Count != 5)
            throw new ArgumentException("Должно быть ровно 5 вариантов ответа", nameof(options));

        QuestionText = questionText;
        Options.Clear();
        
        for (int i = 0; i < options.Count; i++)
        {
            options[i].Order = i == 0 ? LexoRankHelper.Middle() : LexoRankHelper.Next(options[i-1].Order);
            Options.Add(options[i]);
        }
        
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Добавляет вариант ответа
    /// </summary>
    /// <param name="option">Вариант ответа</param>
    public void AddOption(QuestionOption option)
    {
        if (option == null) throw new ArgumentNullException(nameof(option));
        if (Options.Count >= 5) throw new InvalidOperationException("Максимум 5 вариантов ответа");
        
        var lastOption = Options.OrderBy(o => o.Order).LastOrDefault();
        option.Order = lastOption != null ? LexoRankHelper.Next(lastOption.Order) : LexoRankHelper.Middle();
        Options.Add(option);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Подсчитывает общий балл
    /// </summary>
    /// <returns>Общее количество баллов</returns>
    public int GetTotalScore()
    {
        return Options.Where(o => o.IsCorrect).Sum(o => o.Points);
    }

    /// <summary>
    /// Проверяет, может ли компонент быть активирован
    /// </summary>
    public override bool CanBeActivated()
    {
        return !string.IsNullOrWhiteSpace(QuestionText) && 
               Options.Count == 5 && 
               Options.Any(o => o.IsCorrect);
    }
}

/// <summary>
/// Вариант ответа на вопрос
/// </summary>
public class QuestionOption
{
    /// <summary>
    /// Уникальный идентификатор варианта
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Текст варианта ответа
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Является ли вариант правильным
    /// </summary>
    public bool IsCorrect { get; set; }

    /// <summary>
    /// LexoRank позиция варианта для динамической сортировки
    /// </summary>
    public string Order { get; set; } = string.Empty;

    /// <summary>
    /// Сообщение, показываемое при выборе этого варианта
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Количество баллов за правильный ответ
    /// </summary>
    public int Points { get; set; } = 1;

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="text">Текст варианта</param>
    /// <param name="isCorrect">Правильный ли вариант</param>
    /// <param name="message">Сообщение при выборе</param>
    /// <param name="points">Количество баллов</param>
    public QuestionOption(string text, bool isCorrect, string message, int points = 1)
    {
        Id = Guid.NewGuid();
        Text = text ?? throw new ArgumentNullException(nameof(text));
        IsCorrect = isCorrect;
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Points = points > 0 ? points : 1;
    }

    /// <summary>
    /// Конструктор для EF Core
    /// </summary>
    protected QuestionOption() { }
}