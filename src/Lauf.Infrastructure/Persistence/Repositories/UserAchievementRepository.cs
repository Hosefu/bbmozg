using Lauf.Domain.Entities.Users;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Lauf.Infrastructure.Persistence.Repositories;

/// <summary>
/// Репозиторий для работы с достижениями пользователей
/// </summary>
public class UserAchievementRepository : IUserAchievementRepository
{
    private readonly ApplicationDbContext _context;

    public UserAchievementRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserAchievement?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<UserAchievement>()
            .Include(ua => ua.User)
            .Include(ua => ua.Achievement)
            .FirstOrDefaultAsync(ua => ua.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<UserAchievement>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<UserAchievement>()
            .Include(ua => ua.Achievement)
            .Where(ua => ua.UserId == userId)
            .OrderByDescending(ua => ua.EarnedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserAchievement>> GetByAchievementIdAsync(Guid achievementId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<UserAchievement>()
            .Include(ua => ua.User)
            .Where(ua => ua.AchievementId == achievementId)
            .OrderByDescending(ua => ua.EarnedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserAchievement?> GetByUserAndAchievementAsync(Guid userId, Guid achievementId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<UserAchievement>()
            .Include(ua => ua.User)
            .Include(ua => ua.Achievement)
            .FirstOrDefaultAsync(ua => ua.UserId == userId && ua.AchievementId == achievementId, cancellationToken);
    }

    public async Task AddAsync(UserAchievement userAchievement, CancellationToken cancellationToken = default)
    {
        userAchievement.Id = Guid.NewGuid();
        userAchievement.EarnedAt = DateTime.UtcNow;
        
        await _context.Set<UserAchievement>().AddAsync(userAchievement, cancellationToken);
    }

    public Task UpdateAsync(UserAchievement userAchievement, CancellationToken cancellationToken = default)
    {
        _context.Set<UserAchievement>().Update(userAchievement);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(UserAchievement userAchievement, CancellationToken cancellationToken = default)
    {
        _context.Set<UserAchievement>().Remove(userAchievement);
        return Task.CompletedTask;
    }

    public async Task<bool> HasAchievementAsync(Guid userId, Guid achievementId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<UserAchievement>()
            .AnyAsync(ua => ua.UserId == userId && ua.AchievementId == achievementId, cancellationToken);
    }
}