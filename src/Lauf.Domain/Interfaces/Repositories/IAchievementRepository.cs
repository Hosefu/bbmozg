using Lauf.Domain.Entities.Users;
using Lauf.Domain.Enums;

namespace Lauf.Domain.Interfaces.Repositories;

/// <summary>
/// Интерфейс репозитория для работы с достижениями
/// </summary>
public interface IAchievementRepository
{
    /// <summary>
    /// Получить все достижения
    /// </summary>
    Task<IEnumerable<Achievement>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить достижения по редкости
    /// </summary>
    Task<IEnumerable<Achievement>> GetByRarityAsync(AchievementRarity rarity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить активные достижения
    /// </summary>
    Task<IEnumerable<Achievement>> GetActiveAsync(CancellationToken cancellationToken = default);
}