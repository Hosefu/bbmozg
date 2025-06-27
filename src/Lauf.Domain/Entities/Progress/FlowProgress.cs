using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Entities.Versions;
using Lauf.Domain.Entities.Users;
using Lauf.Domain.ValueObjects;

namespace Lauf.Domain.Entities.Progress;

/// <summary>
/// Прогресс пользователя по конкретному потоку
/// </summary>
public class FlowProgress
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
    /// Идентификатор назначения потока
    /// </summary>
    public Guid FlowAssignmentId { get; private set; }

    /// <summary>
    /// Назначение потока
    /// </summary>
    public FlowAssignment FlowAssignment { get; private set; } = null!;

    /// <summary>
    /// Идентификатор версии потока
    /// </summary>
    public Guid FlowVersionId { get; private set; }

    /// <summary>
    /// Версия потока
    /// </summary>
    public FlowVersion FlowVersion { get; private set; } = null!;

    /// <summary>
    /// Общий процент прогресса по потоку
    /// </summary>
    public ProgressPercentage Progress { get; private set; }

    /// <summary>
    /// Количество завершенных шагов
    /// </summary>
    public int CompletedStepsCount { get; private set; }

    /// <summary>
    /// Общее количество шагов
    /// </summary>
    public int TotalStepsCount { get; private set; }

    /// <summary>
    /// Количество завершенных компонентов
    /// </summary>
    public int CompletedComponentsCount { get; private set; }

    /// <summary>
    /// Общее количество компонентов
    /// </summary>
    public int TotalComponentsCount { get; private set; }

    /// <summary>
    /// Время, потраченное на обучение в минутах
    /// </summary>
    public int TimeSpentMinutes { get; private set; }

    /// <summary>
    /// Дата начала прохождения
    /// </summary>
    public DateTime StartedAt { get; private set; }

    /// <summary>
    /// Дата завершения (если завершен)
    /// </summary>
    public DateTime? CompletedAt { get; private set; }

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime LastUpdatedAt { get; private set; }

    /// <summary>
    /// Идентификатор текущего активного шага
    /// </summary>
    public Guid? CurrentStepId { get; private set; }

    /// <summary>
    /// Прогресс по шагам
    /// </summary>
    public List<StepProgress> StepProgresses { get; private set; } = new();

    /// <summary>
    /// Приватный конструктор для EF Core
    /// </summary>
    private FlowProgress() { }

    /// <summary>
    /// Конструктор для создания записи прогресса по потоку
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="flowAssignmentId">ID назначения потока</param>
    /// <param name="flowVersionId">ID версии потока</param>
    /// <param name="totalStepsCount">Общее количество шагов</param>
    /// <param name="totalComponentsCount">Общее количество компонентов</param>
    public FlowProgress(
        Guid userId,
        Guid flowAssignmentId,
        Guid flowVersionId,
        int totalStepsCount,
        int totalComponentsCount)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        FlowAssignmentId = flowAssignmentId;
        FlowVersionId = flowVersionId;
        Progress = new ProgressPercentage(0);
        CompletedStepsCount = 0;
        TotalStepsCount = totalStepsCount > 0 ? totalStepsCount : throw new ArgumentException("Количество шагов должно быть положительным");
        CompletedComponentsCount = 0;
        TotalComponentsCount = totalComponentsCount > 0 ? totalComponentsCount : throw new ArgumentException("Количество компонентов должно быть положительным");
        TimeSpentMinutes = 0;
        StartedAt = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Пересчитать прогресс на основе прогресса по шагам
    /// </summary>
    public void RecalculateProgress()
    {
        if (StepProgresses.Count == 0)
        {
            Progress = new ProgressPercentage(0);
            return;
        }

        // Подсчитываем завершенные шаги и компоненты
        CompletedStepsCount = StepProgresses.Count(sp => sp.Progress.Value >= 100);
        CompletedComponentsCount = StepProgresses.Sum(sp => sp.CompletedComponentsCount);
        TimeSpentMinutes = StepProgresses.Sum(sp => sp.TimeSpentMinutes);

        // Рассчитываем общий прогресс как среднее от прогресса по шагам
        var totalStepProgress = StepProgresses.Sum(sp => sp.Progress.Value);
        var averageProgress = totalStepProgress / StepProgresses.Count;
        Progress = new ProgressPercentage(averageProgress);

        // Проверяем завершение потока
        if (Progress.Value >= 100 && !CompletedAt.HasValue)
        {
            CompletedAt = DateTime.UtcNow;
        }

        LastUpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Установить текущий активный шаг
    /// </summary>
    /// <param name="stepId">ID шага</param>
    public void SetCurrentStep(Guid stepId)
    {
        CurrentStepId = stepId;
        LastUpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Добавить время обучения
    /// </summary>
    /// <param name="minutes">Количество минут</param>
    public void AddLearningTime(int minutes)
    {
        if (minutes > 0)
        {
            TimeSpentMinutes += minutes;
            LastUpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Проверить, просрочен ли поток
    /// </summary>
    public bool IsOverdue()
    {
        return FlowAssignment.IsOverdue();
    }

    /// <summary>
    /// Проверить, завершен ли поток
    /// </summary>
    public bool IsCompleted()
    {
        return CompletedAt.HasValue || Progress.Value >= 100;
    }

    /// <summary>
    /// Получить следующий доступный шаг
    /// </summary>
    public Guid? GetNextAvailableStepId()
    {
        // Находим первый незавершенный шаг
        var incompleteStep = StepProgresses
            .Where(sp => sp.Progress.Value < 100)
            .OrderBy(sp => sp.Order)
            .FirstOrDefault();

        return incompleteStep?.StepVersionId;
    }

    /// <summary>
    /// Получить статистику прогресса
    /// </summary>
    public (double CompletionRate, int DaysSpent, int EstimatedDaysLeft) GetProgressStats()
    {
        var completionRate = (double)Progress.Value / 100;
        var daysSpent = (DateTime.UtcNow - StartedAt).Days;
        
        var estimatedDaysLeft = 0;
        if (completionRate > 0 && completionRate < 1)
        {
            var totalEstimatedDays = daysSpent / completionRate;
            estimatedDaysLeft = (int)Math.Ceiling(totalEstimatedDays - daysSpent);
        }

        return (completionRate, daysSpent, estimatedDaysLeft);
    }
}