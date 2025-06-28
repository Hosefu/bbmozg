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
    /// Текст вопроса
    /// </summary>
    public string QuestionText { get; set; } = string.Empty;

    /// <summary>
    /// Вопросы квиза
    /// </summary>
    public List<QuizQuestionDto> Questions { get; set; } = new();

    /// <summary>
    /// Варианты ответов (ровно 5)
    /// </summary>
    public List<QuestionOptionDto> Options { get; set; } = new();

    /// <summary>
    /// Статус компонента
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Приблизительное время выполнения в минутах
    /// </summary>
    public int EstimatedDurationMinutes { get; set; }

    /// <summary>
    /// Максимальное количество попыток
    /// </summary>
    public int? MaxAttempts { get; set; }

    /// <summary>
    /// Минимальный проходной балл
    /// </summary>
    public int? MinPassingScore { get; set; }

    /// <summary>
    /// Дополнительные инструкции
    /// </summary>
    public string Instructions { get; set; } = string.Empty;

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO для вопроса квиза
/// </summary>
public class QuizQuestionDto
{
    /// <summary>
    /// Уникальный идентификатор вопроса
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Текст вопроса
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Варианты ответов
    /// </summary>
    public List<QuestionOptionDto> Options { get; set; } = new();
}

/// <summary>
/// DTO для варианта ответа на вопрос
/// </summary>
public class QuestionOptionDto
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
    /// Порядковый номер варианта (для фронтенда, начиная с 0)
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Сообщение, показываемое при выборе этого варианта
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Количество баллов за правильный ответ
    /// </summary>
    public int Points { get; set; } = 1;
}