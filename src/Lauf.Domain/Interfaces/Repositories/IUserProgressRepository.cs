using Lauf.Domain.Entities.Progress;

namespace Lauf.Domain.Interfaces.Repositories;

/// <summary>
/// Интерфейс репозитория для работы с прогрессом пользователей
/// </summary>
public interface IUserProgressRepository
{
    /// <summary>
    /// Получить прогресс пользователя по назначению
    /// </summary>
    Task<UserProgress?> GetByAssignmentIdAsync(Guid assignmentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить весь прогресс пользователя
    /// </summary>
    Task<IEnumerable<UserProgress>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить активный прогресс пользователя (незавершенные назначения)
    /// </summary>
    Task<IEnumerable<UserProgress>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    // Flow Progress methods
    Task<FlowProgress?> GetFlowProgressByAssignmentIdAsync(Guid assignmentId, CancellationToken cancellationToken = default);
    Task AddFlowProgressAsync(FlowProgress flowProgress, CancellationToken cancellationToken = default);
    Task UpdateFlowProgressAsync(FlowProgress flowProgress, CancellationToken cancellationToken = default);

    // Component Progress methods
    Task<ComponentProgress?> GetComponentProgressAsync(Guid componentSnapshotId, Guid userId, CancellationToken cancellationToken = default);
    Task UpdateComponentProgressAsync(ComponentProgress componentProgress, CancellationToken cancellationToken = default);

    // Step Progress methods
    Task UpdateStepProgressAsync(StepProgress stepProgress, CancellationToken cancellationToken = default);

    // User Progress methods
    Task<UserProgress?> GetUserProgressAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddUserProgressAsync(UserProgress userProgress, CancellationToken cancellationToken = default);
    Task UpdateUserProgressAsync(UserProgress userProgress, CancellationToken cancellationToken = default);
}