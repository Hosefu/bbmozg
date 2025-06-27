using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Lauf.Domain.Entities.Versions;

/// <summary>
/// Версия компонента квиза
/// </summary>
public class QuizComponentVersion
{
    /// <summary>
    /// Идентификатор версии компонента (Foreign Key)
    /// </summary>
    public Guid ComponentVersionId { get; private set; }

    /// <summary>
    /// Проходной балл (в процентах)
    /// </summary>
    [Range(0, 100)]
    public int PassingScore { get; private set; } = 80;

    /// <summary>
    /// Ограничение по времени в минутах (null = без ограничения)
    /// </summary>
    public int? TimeLimitMinutes { get; private set; }

    /// <summary>
    /// Разрешены ли множественные попытки
    /// </summary>
    public bool AllowMultipleAttempts { get; private set; } = true;

    /// <summary>
    /// Показывать ли правильные ответы после завершения
    /// </summary>
    public bool ShowCorrectAnswers { get; private set; } = true;

    /// <summary>
    /// Перемешивать ли вопросы
    /// </summary>
    public bool ShuffleQuestions { get; private set; } = false;

    /// <summary>
    /// Перемешивать ли варианты ответов
    /// </summary>
    public bool ShuffleAnswers { get; private set; } = false;

    /// <summary>
    /// Версия компонента, к которой принадлежит этот квиз
    /// </summary>
    public virtual ComponentVersion ComponentVersion { get; set; } = null!;

    /// <summary>
    /// Коллекция вариантов ответов квиза
    /// </summary>
    public virtual ICollection<QuizOptionVersion> Options { get; set; } = new List<QuizOptionVersion>();

    /// <summary>
    /// Конструктор для EF Core
    /// </summary>
    protected QuizComponentVersion() { }

    /// <summary>
    /// Конструктор для создания версии квиза
    /// </summary>
    public QuizComponentVersion(
        Guid componentVersionId,
        int passingScore = 80,
        int? timeLimitMinutes = null,
        bool allowMultipleAttempts = true,
        bool showCorrectAnswers = true,
        bool shuffleQuestions = false,
        bool shuffleAnswers = false)
    {
        ComponentVersionId = componentVersionId;
        PassingScore = passingScore;
        TimeLimitMinutes = timeLimitMinutes;
        AllowMultipleAttempts = allowMultipleAttempts;
        ShowCorrectAnswers = showCorrectAnswers;
        ShuffleQuestions = shuffleQuestions;
        ShuffleAnswers = shuffleAnswers;

        ValidateSettings();
    }

    /// <summary>
    /// Обновить настройки квиза
    /// </summary>
    public void UpdateSettings(
        int passingScore,
        int? timeLimitMinutes,
        bool allowMultipleAttempts,
        bool showCorrectAnswers,
        bool shuffleQuestions,
        bool shuffleAnswers)
    {
        PassingScore = passingScore;
        TimeLimitMinutes = timeLimitMinutes;
        AllowMultipleAttempts = allowMultipleAttempts;
        ShowCorrectAnswers = showCorrectAnswers;
        ShuffleQuestions = shuffleQuestions;
        ShuffleAnswers = shuffleAnswers;

        ValidateSettings();
    }

    /// <summary>
    /// Добавить вариант ответа
    /// </summary>
    public void AddOption(QuizOptionVersion option)
    {
        if (option == null)
            throw new ArgumentNullException(nameof(option));

        // Проверяем, что не превышено максимальное количество вариантов
        if (Options.Count >= 5)
            throw new InvalidOperationException("Квиз не может содержать более 5 вариантов ответа");

        Options.Add(option);
        ValidateOptions();
    }

    /// <summary>
    /// Удалить вариант ответа
    /// </summary>
    public void RemoveOption(QuizOptionVersion option)
    {
        if (option == null)
            throw new ArgumentNullException(nameof(option));

        Options.Remove(option);
        ValidateOptions();
    }

    /// <summary>
    /// Очистить все варианты ответов
    /// </summary>
    public void ClearOptions()
    {
        Options.Clear();
    }

    /// <summary>
    /// Валидация настроек квиза
    /// </summary>
    private void ValidateSettings()
    {
        if (PassingScore < 0 || PassingScore > 100)
        {
            throw new ArgumentException("Проходной балл должен быть от 0 до 100%", nameof(PassingScore));
        }

        if (TimeLimitMinutes.HasValue && TimeLimitMinutes.Value <= 0)
        {
            throw new ArgumentException("Ограничение по времени должно быть больше 0", nameof(TimeLimitMinutes));
        }

        if (TimeLimitMinutes.HasValue && TimeLimitMinutes.Value > 480) // 8 часов максимум
        {
            throw new ArgumentException("Ограничение по времени не может превышать 8 часов", nameof(TimeLimitMinutes));
        }
    }

    /// <summary>
    /// Валидация вариантов ответов
    /// </summary>
    private void ValidateOptions()
    {
        if (Options.Count == 0)
            return; // Пустой квиз может быть валидным на этапе создания

        if (Options.Count != 5)
        {
            throw new InvalidOperationException("Квиз должен содержать ровно 5 вариантов ответа");
        }

        var correctAnswers = Options.Count(o => o.IsCorrect);
        if (correctAnswers == 0)
        {
            throw new InvalidOperationException("Квиз должен содержать хотя бы один правильный ответ");
        }

        if (correctAnswers == Options.Count)
        {
            throw new InvalidOperationException("Квиз не может содержать только правильные ответы");
        }

        // Проверяем уникальность порядковых номеров
        var orders = Options.Select(o => o.Order).ToList();
        if (orders.Distinct().Count() != orders.Count)
        {
            throw new InvalidOperationException("Порядковые номера вариантов ответов должны быть уникальными");
        }

        // Проверяем, что все баллы не отрицательные
        if (Options.Any(o => o.Points < 0))
        {
            throw new InvalidOperationException("Баллы за ответы не могут быть отрицательными");
        }
    }

    /// <summary>
    /// Получить максимально возможное количество баллов
    /// </summary>
    public int GetMaxPoints()
    {
        if (!Options.Any())
            return 0;

        return Options.Where(o => o.IsCorrect).Sum(o => o.Points);
    }

    /// <summary>
    /// Получить минимальное количество баллов для прохождения
    /// </summary>
    public int GetMinPointsToPass()
    {
        var maxPoints = GetMaxPoints();
        return (int)Math.Ceiling(maxPoints * PassingScore / 100.0);
    }

    /// <summary>
    /// Проверить, готов ли квиз к использованию
    /// </summary>
    public bool IsReadyForUse()
    {
        try
        {
            ValidateOptions();
            return Options.Count == 5 && GetMaxPoints() > 0;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Получить статистику по баллам
    /// </summary>
    public (int maxPoints, int minToPass, int correctAnswers, int incorrectAnswers) GetScoreStatistics()
    {
        var maxPoints = GetMaxPoints();
        var minToPass = GetMinPointsToPass();
        var correctAnswers = Options.Count(o => o.IsCorrect);
        var incorrectAnswers = Options.Count - correctAnswers;

        return (maxPoints, minToPass, correctAnswers, incorrectAnswers);
    }
}