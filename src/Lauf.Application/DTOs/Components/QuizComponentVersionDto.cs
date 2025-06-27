using System;
using System.Collections.Generic;

namespace Lauf.Application.DTOs.Flows;

/// <summary>
/// DTO для версии компонента теста
/// </summary>
public class QuizComponentVersionDto
{
    /// <summary>
    /// Идентификатор версии компонента
    /// </summary>
    public Guid ComponentVersionId { get; set; }

    /// <summary>
    /// Проходной балл (в процентах)
    /// </summary>
    public decimal PassingScore { get; set; }

    /// <summary>
    /// Ограничение по времени в минутах
    /// </summary>
    public int? TimeLimitMinutes { get; set; }

    /// <summary>
    /// Разрешены ли повторные попытки
    /// </summary>
    public bool AllowMultipleAttempts { get; set; }

    /// <summary>
    /// Показывать ли правильные ответы после завершения
    /// </summary>
    public bool ShowCorrectAnswers { get; set; }

    /// <summary>
    /// Перемешивать ли вопросы
    /// </summary>
    public bool ShuffleQuestions { get; set; }

    /// <summary>
    /// Перемешивать ли варианты ответов
    /// </summary>
    public bool ShuffleAnswers { get; set; }

    /// <summary>
    /// Варианты ответов
    /// </summary>
    public IList<QuizOptionVersionDto> Options { get; set; } = new List<QuizOptionVersionDto>();
}

/// <summary>
/// DTO для версии варианта ответа в тесте
/// </summary>
public class QuizOptionVersionDto
{
    /// <summary>
    /// Идентификатор варианта ответа
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор версии компонента теста
    /// </summary>
    public Guid ComponentVersionId { get; set; }

    /// <summary>
    /// Текст варианта ответа
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Является ли вариант правильным
    /// </summary>
    public bool IsCorrect { get; set; }

    /// <summary>
    /// Количество баллов за выбор этого варианта
    /// </summary>
    public decimal Points { get; set; }

    /// <summary>
    /// Порядок отображения
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Объяснение (показывается после ответа)
    /// </summary>
    public string Explanation { get; set; } = string.Empty;
}