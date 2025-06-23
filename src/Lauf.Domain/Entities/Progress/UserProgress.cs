using Lauf.Domain.Entities.Users;
using Lauf.Domain.ValueObjects;

namespace Lauf.Domain.Entities.Progress;

/// <summary>
/// Общий прогресс пользователя по всем потокам
/// </summary>
public class UserProgress
{
    /// <summary>
    /// Идентификатор записи прогресса
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Пользователь
    /// </summary>
    public User User { get; private set; } = null!;

    /// <summary>
    /// Общий процент прогресса пользователя
    /// </summary>
    public ProgressPercentage OverallProgress { get; private set; }

    /// <summary>
    /// Количество назначенных потоков
    /// </summary>
    public int AssignedFlowsCount { get; private set; }

    /// <summary>
    /// Количество завершенных потоков
    /// </summary>
    public int CompletedFlowsCount { get; private set; }

    /// <summary>
    /// Количество активных потоков
    /// </summary>
    public int ActiveFlowsCount { get; private set; }

    /// <summary>
    /// Количество просроченных потоков
    /// </summary>
    public int OverdueFlowsCount { get; private set; }

    /// <summary>
    /// Общее время обучения в минутах
    /// </summary>
    public int TotalLearningTimeMinutes { get; private set; }

    /// <summary>
    /// Количество заработанных достижений
    /// </summary>
    public int AchievementsCount { get; private set; }

    /// <summary>
    /// Последняя дата активности
    /// </summary>
    public DateTime LastActivityAt { get; private set; }

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Прогресс по потокам
    /// </summary>
    public List<FlowProgress> FlowProgresses { get; private set; } = new();

    /// <summary>
    /// Приватный конструктор для EF Core
    /// </summary>
    private UserProgress() { }

    /// <summary>
    /// Конструктор для создания записи прогресса пользователя
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    public UserProgress(Guid userId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        OverallProgress = new ProgressPercentage(0);
        AssignedFlowsCount = 0;
        CompletedFlowsCount = 0;
        ActiveFlowsCount = 0;
        OverdueFlowsCount = 0;
        TotalLearningTimeMinutes = 0;
        AchievementsCount = 0;
        LastActivityAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Обновить общий прогресс на основе прогресса по потокам
    /// </summary>
    public void RecalculateProgress()
    {
        if (FlowProgresses.Count == 0)
        {
            OverallProgress = new ProgressPercentage(0);
            return;
        }

        var totalProgress = FlowProgresses.Sum(fp => fp.Progress.Value);
        var averageProgress = totalProgress / FlowProgresses.Count;
        OverallProgress = new ProgressPercentage(averageProgress);

        AssignedFlowsCount = FlowProgresses.Count;
        CompletedFlowsCount = FlowProgresses.Count(fp => fp.Progress.Value >= 100);
        ActiveFlowsCount = FlowProgresses.Count(fp => fp.Progress.Value > 0 && fp.Progress.Value < 100);
        OverdueFlowsCount = FlowProgresses.Count(fp => fp.IsOverdue());
        TotalLearningTimeMinutes = FlowProgresses.Sum(fp => fp.TimeSpentMinutes);

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Обновить время последней активности
    /// </summary>
    public void UpdateLastActivity()
    {
        LastActivityAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Увеличить количество достижений
    /// </summary>
    public void IncrementAchievementsCount()
    {
        AchievementsCount++;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Получить статистику активности пользователя
    /// </summary>
    public (int TotalHours, int DaysActive, double AverageSessionTime) GetActivityStats()
    {
        var totalHours = TotalLearningTimeMinutes / 60;
        var daysActive = FlowProgresses
            .SelectMany(fp => fp.StepProgresses)
            .SelectMany(sp => sp.ComponentProgresses)
            .Select(cp => cp.LastUpdatedAt.Date)
            .Distinct()
            .Count();

        var averageSessionTime = daysActive > 0 ? (double)TotalLearningTimeMinutes / daysActive : 0;

        return (totalHours, daysActive, averageSessionTime);
    }
}