using BuddyBot.Domain.Entities.Progress;
using BuddyBot.Domain.Entities.Snapshots;
using BuddyBot.Domain.ValueObjects;

namespace BuddyBot.Domain.Services;

/// <summary>
/// Интерфейс сервиса для расчета прогресса пользователей
/// </summary>
public interface IProgressCalculationService
{
    /// <summary>
    /// Инициализировать прогресс для нового назначения потока
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="assignmentId">ID назначения потока</param>
    /// <param name="flowSnapshot">Снапшот потока</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Созданный прогресс потока</returns>
    Task<FlowProgress> InitializeFlowProgressAsync(
        Guid userId,
        Guid assignmentId,
        FlowSnapshot flowSnapshot,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить прогресс компонента
    /// </summary>
    /// <param name="componentProgressId">ID прогресса компонента</param>
    /// <param name="progressData">Данные прогресса</param>
    /// <param name="timeSpentMinutes">Время, потраченное в минутах</param>
    /// <param name="isCompleted">Завершен ли компонент</param>
    /// <param name="score">Балл (для квизов и заданий)</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Обновленный прогресс компонента</returns>
    Task<ComponentProgress> UpdateComponentProgressAsync(
        Guid componentProgressId,
        ComponentProgressData? progressData = null,
        int? timeSpentMinutes = null,
        bool? isCompleted = null,
        int? score = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Завершить компонент и пересчитать прогресс
    /// </summary>
    /// <param name="componentProgressId">ID прогресса компонента</param>
    /// <param name="score">Финальный балл</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Обновленный прогресс компонента</returns>
    Task<ComponentProgress> CompleteComponentAsync(
        Guid componentProgressId,
        int? score = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Разблокировать следующий шаг, если это возможно
    /// </summary>
    /// <param name="flowProgressId">ID прогресса потока</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>ID разблокированного шага или null</returns>
    Task<Guid?> UnlockNextStepAsync(Guid flowProgressId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Пересчитать весь прогресс пользователя
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Обновленный общий прогресс пользователя</returns>
    Task<UserProgress> RecalculateUserProgressAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить текущий прогресс пользователя по потоку
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="assignmentId">ID назначения потока</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Прогресс по потоку или null</returns>
    Task<FlowProgress?> GetFlowProgressAsync(
        Guid userId,
        Guid assignmentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить прогресс по шагу
    /// </summary>
    /// <param name="stepProgressId">ID прогресса шага</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Прогресс по шагу или null</returns>
    Task<StepProgress?> GetStepProgressAsync(Guid stepProgressId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить следующий доступный компонент для пользователя
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="assignmentId">ID назначения потока</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Прогресс следующего доступного компонента или null</returns>
    Task<ComponentProgress?> GetNextAvailableComponentAsync(
        Guid userId,
        Guid assignmentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверить, может ли пользователь приступить к выполнению компонента
    /// </summary>
    /// <param name="componentProgressId">ID прогресса компонента</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат проверки</returns>
    Task<(bool CanStart, string? Reason)> CanStartComponentAsync(
        Guid componentProgressId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить аналитику прогресса пользователя
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Аналитика прогресса</returns>
    Task<UserProgressAnalytics> GetUserProgressAnalyticsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Аналитика прогресса пользователя
/// </summary>
public record UserProgressAnalytics(
    ProgressPercentage OverallProgress,
    int TotalFlows,
    int CompletedFlows,
    int ActiveFlows,
    int OverdueFlows,
    int TotalLearningHours,
    int AverageCompletionDays,
    List<FlowProgressSummary> RecentActivity);

/// <summary>
/// Краткая информация о прогрессе потока
/// </summary>
public record FlowProgressSummary(
    Guid FlowId,
    string FlowTitle,
    ProgressPercentage Progress,
    DateTime LastActivity,
    bool IsOverdue,
    DateTime? DeadlineDate);