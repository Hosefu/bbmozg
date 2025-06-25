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
}