using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Entities.Progress;
using Lauf.Domain.Entities.Versions;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Domain.Services;
using Lauf.Domain.ValueObjects;

namespace Lauf.Domain.Services;

/// <summary>
/// Сервис для расчета прогресса пользователей
/// </summary>
public class ProgressCalculationService
{
    private readonly IUserProgressRepository _progressRepository;
    private readonly IFlowAssignmentRepository _assignmentRepository;
    private readonly IVersioningService _versioningService;

    public ProgressCalculationService(
        IUserProgressRepository progressRepository,
        IFlowAssignmentRepository assignmentRepository,
        IVersioningService versioningService)
    {
        _progressRepository = progressRepository;
        _assignmentRepository = assignmentRepository;
        _versioningService = versioningService;
    }

    /// <summary>
    /// Пересчитать прогресс по потоку
    /// </summary>
    /// <param name="flowAssignmentId">Идентификатор назначения потока</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Обновленный прогресс</returns>
    public async Task<FlowProgress?> RecalculateFlowProgressAsync(
        Guid flowAssignmentId, 
        CancellationToken cancellationToken = default)
    {
        var flowProgress = await _progressRepository.GetFlowProgressByAssignmentIdAsync(
            flowAssignmentId, cancellationToken);

        if (flowProgress == null)
            return null;

        // Пересчитываем прогресс
        flowProgress.RecalculateProgress();
        await _progressRepository.UpdateFlowProgressAsync(flowProgress, cancellationToken);
        
        return flowProgress;
    }

    /// <summary>
    /// Создать начальный прогресс для нового назначения потока
    /// </summary>
    /// <param name="assignment">Назначение потока</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Созданный прогресс</returns>
    public async Task<FlowProgress> CreateInitialProgressAsync(
        FlowAssignment assignment,
        CancellationToken cancellationToken = default)
    {
        // Получаем версию потока
        var flowVersion = await _versioningService.GetActiveFlowVersionAsync(assignment.FlowId, cancellationToken);
        if (flowVersion == null)
            throw new InvalidOperationException($"Активная версия потока {assignment.FlowId} не найдена");
        
        var totalStepsCount = flowVersion.StepVersions?.Count ?? 0;
        var totalComponentsCount = flowVersion.StepVersions?
            .SelectMany(sv => sv.ComponentVersions ?? new List<ComponentVersion>())
            .Count() ?? 0;

        // Создаем прогресс по потоку
        var flowProgress = new FlowProgress(
            assignment.UserId,
            assignment.Id,
            flowVersion.Id,
            totalStepsCount,
            totalComponentsCount);

        // Создаем прогресс по шагам
        if (flowVersion.StepVersions != null)
        {
            var stepIndex = 1;
            foreach (var stepVersion in flowVersion.StepVersions.OrderBy(s => s.Order))
            {
                var componentsInStep = stepVersion.ComponentVersions?.Count ?? 0;
                var stepProgress = new StepProgress(
                    flowProgress.Id,
                    stepVersion.Id,
                    stepIndex,
                    componentsInStep,
                    stepIndex == 1); // Первый шаг разблокирован

                // Создаем прогресс по компонентам
                if (stepVersion.ComponentVersions != null)
                {
                    var componentIndex = 1;
                    foreach (var componentVersion in stepVersion.ComponentVersions.OrderBy(c => c.Order))
                    {
                        var componentProgress = new ComponentProgress(
                            stepProgress.Id,
                            componentVersion.Id,
                            componentIndex,
                            componentVersion.IsRequired);

                        stepProgress.ComponentProgresses.Add(componentProgress);
                        componentIndex++;
                    }
                }

                flowProgress.StepProgresses.Add(stepProgress);
                stepIndex++;
            }
        }

        await _progressRepository.AddFlowProgressAsync(flowProgress, cancellationToken);
        return flowProgress;
    }

    /// <summary>
    /// Завершить компонент и обновить связанный прогресс
    /// </summary>
    /// <param name="componentSnapshotId">Идентификатор снапшота компонента</param>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="timeSpentMinutes">Время, потраченное на компонент</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Результат завершения</returns>
    public async Task<ComponentCompletionResult> CompleteComponentAsync(
        Guid componentSnapshotId,
        Guid userId,
        int timeSpentMinutes,
        CancellationToken cancellationToken = default)
    {
        var componentProgress = await _progressRepository.GetComponentProgressAsync(
            componentSnapshotId, userId, cancellationToken);

        if (componentProgress == null)
            throw new InvalidOperationException("Прогресс компонента не найден");

        if (componentProgress.IsCompleted)
            return new ComponentCompletionResult { AlreadyCompleted = true };

        // Завершаем компонент
        componentProgress.Complete();
        componentProgress.AddTimeSpent(timeSpentMinutes);
        await _progressRepository.UpdateComponentProgressAsync(componentProgress, cancellationToken);

        var result = new ComponentCompletionResult
        {
            ComponentCompleted = true,
            ComponentId = componentSnapshotId
        };

        // Пересчитываем прогресс шага
        var stepProgress = componentProgress.StepProgress;
        stepProgress.RecalculateProgress();
        
        if (stepProgress.IsCompleted())
        {
            await _progressRepository.UpdateStepProgressAsync(stepProgress, cancellationToken);
            
            result.StepCompleted = true;
            result.StepId = stepProgress.StepVersionId;

            // Проверяем разблокировку следующего шага
            var nextStep = await GetNextStepToUnlockAsync(stepProgress, cancellationToken);
            if (nextStep != null)
            {
                nextStep.Unlock();
                await _progressRepository.UpdateStepProgressAsync(nextStep, cancellationToken);
                
                result.NextStepUnlocked = true;
                result.NextStepId = nextStep.StepVersionId;
            }
        }

        // Пересчитываем прогресс по потоку
        var flowProgress = await RecalculateFlowProgressAsync(
            stepProgress.FlowProgressId, cancellationToken);

        if (flowProgress != null && flowProgress.Progress.Value >= 100)
        {
            result.FlowCompleted = true;
            result.FlowId = flowProgress.FlowVersionId;
        }

        return result;
    }

    /// <summary>
    /// Получить следующий шаг для разблокировки
    /// </summary>
    /// <param name="completedStep">Завершенный шаг</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Следующий шаг для разблокировки</returns>
    private async Task<StepProgress?> GetNextStepToUnlockAsync(
        StepProgress completedStep,
        CancellationToken cancellationToken)
    {
        var flowProgress = await _progressRepository.GetFlowProgressByAssignmentIdAsync(
            completedStep.FlowProgressId, cancellationToken);

        if (flowProgress == null)
            return null;

        // Находим следующий заблокированный шаг
        var nextOrder = completedStep.Order + 1;
        return flowProgress.StepProgresses
            .FirstOrDefault(sp => sp.Order == nextOrder && !sp.IsUnlocked);
    }

    /// <summary>
    /// Пересчитать общий прогресс пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Обновленный прогресс пользователя</returns>
    public async Task<UserProgress?> RecalculateUserProgressAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var userProgress = await _progressRepository.GetUserProgressAsync(userId, cancellationToken);
        if (userProgress == null)
        {
            // Создаем новый прогресс пользователя если его нет
            userProgress = new UserProgress(userId);
            await _progressRepository.AddUserProgressAsync(userProgress, cancellationToken);
        }

        // Здесь будет логика пересчета общего прогресса
        // На основе всех FlowProgress пользователя
        
        await _progressRepository.UpdateUserProgressAsync(userProgress, cancellationToken);
        return userProgress;
    }

    /// <summary>
    /// Получить статистику прогресса пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Статистика прогресса</returns>
    public async Task<UserProgressStatistics> GetUserProgressStatisticsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var userProgress = await _progressRepository.GetUserProgressAsync(userId, cancellationToken);
        
        // Возвращаем базовую статистику
        return new UserProgressStatistics
        {
            UserId = userId,
            OverallProgress = (double)(userProgress?.OverallProgress.Value ?? 0),
            AssignedFlowsCount = 0,
            CompletedFlowsCount = 0,
            ActiveFlowsCount = 0,
            OverdueFlowsCount = 0,
            TotalLearningHours = 0,
            DaysActive = 0,
            AverageSessionTimeMinutes = 0,
            AchievementsCount = 0,
            LastActivityAt = DateTime.UtcNow
        };
    }
}

/// <summary>
/// Результат завершения компонента
/// </summary>
public class ComponentCompletionResult
{
    /// <summary>
    /// Компонент уже был завершен
    /// </summary>
    public bool AlreadyCompleted { get; set; }

    /// <summary>
    /// Компонент был завершен в результате операции
    /// </summary>
    public bool ComponentCompleted { get; set; }

    /// <summary>
    /// Идентификатор завершенного компонента
    /// </summary>
    public Guid ComponentId { get; set; }

    /// <summary>
    /// Шаг был завершен в результате операции
    /// </summary>
    public bool StepCompleted { get; set; }

    /// <summary>
    /// Идентификатор завершенного шага
    /// </summary>
    public Guid? StepId { get; set; }

    /// <summary>
    /// Следующий шаг был разблокирован
    /// </summary>
    public bool NextStepUnlocked { get; set; }

    /// <summary>
    /// Идентификатор разблокированного шага
    /// </summary>
    public Guid? NextStepId { get; set; }

    /// <summary>
    /// Поток был завершен в результате операции
    /// </summary>
    public bool FlowCompleted { get; set; }

    /// <summary>
    /// Идентификатор завершенного потока
    /// </summary>
    public Guid? FlowId { get; set; }
}

/// <summary>
/// Статистика прогресса пользователя
/// </summary>
public class UserProgressStatistics
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Общий прогресс пользователя (в процентах)
    /// </summary>
    public double OverallProgress { get; set; }

    /// <summary>
    /// Количество назначенных потоков
    /// </summary>
    public int AssignedFlowsCount { get; set; }

    /// <summary>
    /// Количество завершенных потоков
    /// </summary>
    public int CompletedFlowsCount { get; set; }

    /// <summary>
    /// Количество активных потоков
    /// </summary>
    public int ActiveFlowsCount { get; set; }

    /// <summary>
    /// Количество просроченных потоков
    /// </summary>
    public int OverdueFlowsCount { get; set; }

    /// <summary>
    /// Общее время обучения в часах
    /// </summary>
    public int TotalLearningHours { get; set; }

    /// <summary>
    /// Количество активных дней
    /// </summary>
    public int DaysActive { get; set; }

    /// <summary>
    /// Среднее время сессии в минутах
    /// </summary>
    public double AverageSessionTimeMinutes { get; set; }

    /// <summary>
    /// Количество достижений
    /// </summary>
    public int AchievementsCount { get; set; }

    /// <summary>
    /// Время последней активности
    /// </summary>
    public DateTime LastActivityAt { get; set; }
}