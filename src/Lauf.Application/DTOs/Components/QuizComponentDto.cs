namespace Lauf.Application.DTOs.Components;

/// <summary>
/// DTO для компонента квиза
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
    /// Вопросы квиза
    /// </summary>
    public List<QuizQuestionDto> Questions { get; set; } = new();

    /// <summary>
    /// Ограничение по времени в секундах
    /// </summary>
    public int? TimeLimit { get; set; }

    /// <summary>
    /// Проходной балл в процентах
    /// </summary>
    public int PassingScore { get; set; }

    /// <summary>
    /// Перемешивать ли вопросы
    /// </summary>
    public bool RandomizeQuestions { get; set; }

    /// <summary>
    /// Перемешивать ли варианты ответов
    /// </summary>
    public bool RandomizeOptions { get; set; }

    /// <summary>
    /// Показывать ли правильные ответы после завершения
    /// </summary>
    public bool ShowCorrectAnswers { get; set; }

    /// <summary>
    /// Показывать ли объяснения к ответам
    /// </summary>
    public bool ShowExplanations { get; set; }

    /// <summary>
    /// Разрешить ли просмотр результатов
    /// </summary>
    public bool AllowReview { get; set; }

    /// <summary>
    /// Включить ли защиту от списывания
    /// </summary>
    public bool PreventCheating { get; set; }

    /// <summary>
    /// Показывать ли прогресс-бар
    /// </summary>
    public bool ShowProgressBar { get; set; }

    /// <summary>
    /// Разрешить ли пропуск вопросов
    /// </summary>
    public bool AllowSkipping { get; set; }

    /// <summary>
    /// Количество вопросов на страницу
    /// </summary>
    public int QuestionsPerPage { get; set; }

    /// <summary>
    /// Автоматическая отправка при истечении времени
    /// </summary>
    public bool AutoSubmit { get; set; }

    /// <summary>
    /// Показывать ли таймер
    /// </summary>
    public bool ShowTimer { get; set; }

    /// <summary>
    /// Уровень сложности квиза
    /// </summary>
    public string Difficulty { get; set; } = string.Empty;

    /// <summary>
    /// Категории квиза
    /// </summary>
    public List<string> Categories { get; set; } = new();

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
    /// Тип вопроса
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Варианты ответов
    /// </summary>
    public List<QuestionOptionDto> Options { get; set; } = new();

    /// <summary>
    /// Порядковый номер вопроса
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Баллы за правильный ответ
    /// </summary>
    public int Points { get; set; }

    /// <summary>
    /// Обязательный ли вопрос
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Объяснение к ответу
    /// </summary>
    public string? Explanation { get; set; }

    /// <summary>
    /// Подсказка к вопросу
    /// </summary>
    public string? Hint { get; set; }

    /// <summary>
    /// URL изображения к вопросу
    /// </summary>
    public string? ImageUrl { get; set; }
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
    /// Порядковый номер варианта
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Объяснение к варианту ответа
    /// </summary>
    public string? Explanation { get; set; }

    /// <summary>
    /// URL изображения к варианту
    /// </summary>
    public string? ImageUrl { get; set; }
}