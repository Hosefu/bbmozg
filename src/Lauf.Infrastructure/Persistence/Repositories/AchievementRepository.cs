using Lauf.Domain.Entities.Users;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Lauf.Infrastructure.Persistence.Repositories;

/// <summary>
/// Репозиторий для работы с достижениями
/// </summary>
public class AchievementRepository : IAchievementRepository
{
    private readonly ApplicationDbContext _context;

    public AchievementRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Achievement?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Achievement>()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Achievement>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<Achievement>()
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Achievement>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<Achievement>()
            .Where(a => a.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Achievement>> GetByTypeAsync(string type, CancellationToken cancellationToken = default)
    {
        // Фильтрация по типу - можно расширить в зависимости от требований
        return await _context.Set<Achievement>()
            .Where(a => a.Title.Contains(type) || a.Description.Contains(type))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Achievement>> GetByRarityAsync(Domain.Enums.AchievementRarity rarity, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Achievement>()
            .Where(a => a.Rarity == rarity)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Achievement achievement, CancellationToken cancellationToken = default)
    {
        achievement.CreatedAt = DateTime.UtcNow;
        achievement.UpdatedAt = DateTime.UtcNow;
        
        await _context.Set<Achievement>().AddAsync(achievement, cancellationToken);
    }

    public Task UpdateAsync(Achievement achievement, CancellationToken cancellationToken = default)
    {
        achievement.UpdatedAt = DateTime.UtcNow;
        _context.Set<Achievement>().Update(achievement);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Achievement achievement, CancellationToken cancellationToken = default)
    {
        _context.Set<Achievement>().Remove(achievement);
        return Task.CompletedTask;
    }
}