using System;
using System.ComponentModel.DataAnnotations;

namespace Lauf.Domain.Entities.Versions;

/// <summary>
/// Версия варианта ответа в квизе
/// </summary>
public class QuizOptionVersion
{
    /// <summary>
    /// Уникальный идентификатор варианта ответа
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Идентификатор версии квиза
    /// </summary>
    public Guid QuizVersionId { get; private set; }

    /// <summary>
    /// Текст варианта ответа
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Text { get; private set; } = string.Empty;

    /// <summary>
    /// Является ли ответ правильным
    /// </summary>
    public bool IsCorrect { get; private set; }

    /// <summary>
    /// Количество баллов за этот ответ
    /// </summary>
    [Range(0, int.MaxValue)]
    public int Points { get; private set; }

    /// <summary>
    /// Порядковый номер варианта ответа
    /// </summary>
    public int Order { get; private set; }

    /// <summary>
    /// Объяснение ответа (почему правильный/неправильный)
    /// </summary>
    [StringLength(1000)]
    public string? Explanation { get; private set; }

    /// <summary>
    /// Версия квиза, к которой принадлежит этот вариант ответа
    /// </summary>
    public virtual QuizComponentVersion QuizVersion { get; set; } = null!;

    /// <summary>
    /// Конструктор для EF Core
    /// </summary>
    protected QuizOptionVersion() { }

    /// <summary>
    /// Конструктор для создания варианта ответа
    /// </summary>
    public QuizOptionVersion(
        Guid quizVersionId,
        string text,
        bool isCorrect,
        int points,
        int order,
        string? explanation = null)
    {
        Id = Guid.NewGuid();
        QuizVersionId = quizVersionId;
        Text = text ?? throw new ArgumentNullException(nameof(text));
        IsCorrect = isCorrect;
        Points = points;
        Order = order;
        Explanation = explanation;

        ValidateOption();
    }

    /// <summary>
    /// Обновить вариант ответа
    /// </summary>
    public void Update(
        string text,
        bool isCorrect,
        int points,
        int order,
        string? explanation = null)
    {
        Text = text ?? throw new ArgumentNullException(nameof(text));
        IsCorrect = isCorrect;
        Points = points;
        Order = order;
        Explanation = explanation;

        ValidateOption();
    }

    /// <summary>
    /// Валидация варианта ответа
    /// </summary>
    private void ValidateOption()
    {
        if (string.IsNullOrWhiteSpace(Text))
        {
            throw new ArgumentException("Текст варианта ответа не может быть пустым", nameof(Text));
        }

        if (Text.Length > 500)
        {
            throw new ArgumentException("Текст варианта ответа не может превышать 500 символов", nameof(Text));
        }

        if (Points < 0)
        {
            throw new ArgumentException("Количество баллов не может быть отрицательным", nameof(Points));
        }

        if (Order < 1 || Order > 5)
        {
            throw new ArgumentException("Порядковый номер должен быть от 1 до 5", nameof(Order));
        }

        if (!string.IsNullOrEmpty(Explanation) && Explanation.Length > 1000)
        {
            throw new ArgumentException("Объяснение не может превышать 1000 символов", nameof(Explanation));
        }

        // Логическая валидация: правильные ответы должны иметь больше 0 баллов
        if (IsCorrect && Points == 0)
        {
            throw new ArgumentException("Правильный ответ должен иметь баллы больше 0", nameof(Points));
        }
    }

    /// <summary>
    /// Получить краткое описание варианта ответа для логирования
    /// </summary>
    public string GetShortDescription()
    {
        var statusText = IsCorrect ? "Правильный" : "Неправильный";
        var truncatedText = Text.Length > 50 ? Text.Substring(0, 47) + "..." : Text;
        
        return $"{statusText} ({Points} баллов): {truncatedText}";
    }

    /// <summary>
    /// Проверить, является ли вариант ответа валидным для использования
    /// </summary>
    public bool IsValid()
    {
        try
        {
            ValidateOption();
            return !string.IsNullOrWhiteSpace(Text);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Клонировать вариант ответа для новой версии квиза
    /// </summary>
    public QuizOptionVersion CloneForQuiz(Guid newQuizVersionId)
    {
        return new QuizOptionVersion(
            newQuizVersionId,
            Text,
            IsCorrect,
            Points,
            Order,
            Explanation);
    }
}