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
    /// Обновить общий прогресс на основе данных назначений (упрощено)
    /// </summary>
    public void RecalculateProgress(int assignedCount, int completedCount, int activeCount, int overdueCount)
    {
        AssignedFlowsCount = assignedCount;
        CompletedFlowsCount = completedCount;
        ActiveFlowsCount = activeCount;
        OverdueFlowsCount = overdueCount;

        // Рассчитываем общий прогресс
        if (assignedCount == 0)
        {
            OverallProgress = new ProgressPercentage(0);
        }
        else
        {
            var progressPercent = (completedCount * 100) / assignedCount;
            OverallProgress = new ProgressPercentage(progressPercent);
        }

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
    /// Получить статистику активности пользователя (упрощено)
    /// </summary>
    public (int TotalHours, double AverageSessionTime) GetActivityStats()
    {
        var totalHours = TotalLearningTimeMinutes / 60;
        var daysSinceStart = (DateTime.UtcNow - LastActivityAt).Days;
        var averageSessionTime = daysSinceStart > 0 ? (double)TotalLearningTimeMinutes / daysSinceStart : 0;

        return (totalHours, averageSessionTime);
    }
}