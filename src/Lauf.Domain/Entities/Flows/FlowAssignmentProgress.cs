namespace Lauf.Domain.Entities.Flows;

/// <summary>
/// Прогресс назначения потока
/// </summary>
public class FlowAssignmentProgress
{
    /// <summary>
    /// Уникальный идентификатор прогресса
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор назначения потока
    /// </summary>
    public Guid FlowAssignmentId { get; set; }

    /// <summary>
    /// Прогресс в процентах (0-100)
    /// </summary>
    public int ProgressPercent { get; set; } = 0;

    /// <summary>
    /// Количество завершенных шагов
    /// </summary>
    public int CompletedSteps { get; set; } = 0;

    /// <summary>
    /// Общее количество шагов
    /// </summary>
    public int TotalSteps { get; set; } = 0;

    /// <summary>
    /// Количество попыток
    /// </summary>
    public int AttemptCount { get; set; } = 1;

    /// <summary>
    /// Финальная оценка
    /// </summary>
    public int? FinalScore { get; set; }

    /// <summary>
    /// Дата начала прохождения
    /// </summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// Дата завершения
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Дата последней активности
    /// </summary>
    public DateTime? LastActivityAt { get; set; }

    /// <summary>
    /// Дата постановки на паузу
    /// </summary>
    public DateTime? PausedAt { get; set; }

    /// <summary>
    /// Причина постановки на паузу
    /// </summary>
    public string? PauseReason { get; set; }

    /// <summary>
    /// Оценка пользователя (1-5)
    /// </summary>
    public int? UserRating { get; set; }

    /// <summary>
    /// Отзыв пользователя
    /// </summary>
    public string? UserFeedback { get; set; }

    /// <summary>
    /// Назначение потока
    /// </summary>
    public virtual FlowAssignment FlowAssignment { get; set; } = null!;

    /// <summary>
    /// Конструктор для создания нового прогресса
    /// </summary>
    /// <param name="flowAssignmentId">Идентификатор назначения</param>
    /// <param name="totalSteps">Общее количество шагов</param>
    public FlowAssignmentProgress(Guid flowAssignmentId, int totalSteps)
    {
        Id = Guid.NewGuid();
        FlowAssignmentId = flowAssignmentId;
        TotalSteps = totalSteps;
        ProgressPercent = 0;
        CompletedSteps = 0;
        AttemptCount = 1;
    }

    /// <summary>
    /// Конструктор для EF Core
    /// </summary>
    protected FlowAssignmentProgress() { }

    /// <summary>
    /// Обновляет прогресс
    /// </summary>
    /// <param name="completedSteps">Завершенные шаги</param>
    public void UpdateProgress(int completedSteps)
    {
        CompletedSteps = completedSteps;
        ProgressPercent = TotalSteps > 0 ? (int)Math.Round((double)completedSteps / TotalSteps * 100) : 0;
        LastActivityAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Начинает прохождение
    /// </summary>
    public void Start()
    {
        StartedAt = DateTime.UtcNow;
        LastActivityAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Завершает прохождение
    /// </summary>
    /// <param name="finalScore">Финальная оценка</param>
    public void Complete(int? finalScore = null)
    {
        CompletedAt = DateTime.UtcNow;
        LastActivityAt = DateTime.UtcNow;
        FinalScore = finalScore;
        ProgressPercent = 100;
    }

    /// <summary>
    /// Ставит на паузу
    /// </summary>
    /// <param name="reason">Причина паузы</param>
    public void Pause(string reason)
    {
        PausedAt = DateTime.UtcNow;
        PauseReason = reason;
    }

    /// <summary>
    /// Возобновляет с паузы
    /// </summary>
    public void Resume()
    {
        PausedAt = null;
        PauseReason = null;
        LastActivityAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Добавляет отзыв пользователя
    /// </summary>
    /// <param name="feedback">Отзыв</param>
    /// <param name="rating">Оценка</param>
    public void AddUserFeedback(string feedback, int? rating = null)
    {
        UserFeedback = feedback;
        if (rating.HasValue && rating >= 1 && rating <= 5)
        {
            UserRating = rating;
        }
    }
}