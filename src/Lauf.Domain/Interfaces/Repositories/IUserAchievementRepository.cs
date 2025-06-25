using Lauf.Domain.Entities.Users;

namespace Lauf.Domain.Interfaces.Repositories;

/// <summary>
/// Интерфейс репозитория для работы с достижениями пользователей
/// </summary>
public interface IUserAchievementRepository
{
    /// <summary>
    /// Получить достижения пользователя
    /// </summary>
    Task<IEnumerable<UserAchievement>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить конкретное достижение пользователя
    /// </summary>
    Task<UserAchievement?> GetByUserAndAchievementAsync(Guid userId, Guid achievementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверить, есть ли у пользователя достижение
    /// </summary>
    Task<bool> HasAchievementAsync(Guid userId, Guid achievementId, CancellationToken cancellationToken = default);
}