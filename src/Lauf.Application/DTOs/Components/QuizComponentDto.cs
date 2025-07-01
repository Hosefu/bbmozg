namespace Lauf.Application.DTOs.Components;

/// <summary>
/// DTO для компонента квиза (упрощенная версия)
/// </summary>
public class QuizComponentDto
{
    /// <summary>
    /// Уникальный идентификатор компонента
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название квиза
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание квиза
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Содержимое квиза
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Тип компонента
    /// </summary>
    public string Type { get; set; } = "QUIZ";

    /// <summary>
    /// Вопросы квиза
    /// </summary>
    public List<QuizQuestionDto> Questions { get; set; } = new();
}

/// <summary>
/// DTO для вопроса квиза
/// </summary>
public class QuizQuestionDto
{
    /// <summary>
    /// Идентификатор вопроса
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Текст вопроса
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Является ли вопрос обязательным
    /// </summary>
    public bool IsRequired { get; set; } = true;

    /// <summary>
    /// Порядковый номер вопроса
    /// </summary>
    public string Order { get; set; } = string.Empty;

    /// <summary>
    /// Варианты ответов
    /// </summary>
    public List<QuestionOptionDto> Options { get; set; } = new();
}

/// <summary>
/// DTO для варианта ответа
/// </summary>
public class QuestionOptionDto
{
    /// <summary>
    /// Идентификатор варианта ответа
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Текст варианта ответа
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Является ли этот вариант правильным ответом
    /// </summary>
    public bool IsCorrect { get; set; }

    /// <summary>
    /// Порядковый номер варианта
    /// </summary>
    public string Order { get; set; } = string.Empty;

    /// <summary>
    /// Очки за правильный ответ
    /// </summary>
    public int Score { get; set; } = 1;
}