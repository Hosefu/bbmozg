using Microsoft.EntityFrameworkCore;
using Lauf.Domain.Entities.Users;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Domain.ValueObjects;
using Lauf.Infrastructure.Persistence;

namespace Lauf.Infrastructure.Persistence.Repositories;

/// <summary>
/// Упрощенная реализация репозитория пользователей для этапа 7
/// </summary>
public class SimpleUserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public SimpleUserRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<User?> GetByTelegramIdAsync(TelegramUserId telegramUserId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.TelegramUserId == telegramUserId, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
    }

    public async Task<User?> GetWithRolesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetByRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(x => x.Roles)
            .Where(x => x.IsActive && x.Roles.Any(r => r.Name == roleName))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetActiveUsersAsync(int skip = 0, int take = 50, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(x => x.Roles)
            .Where(x => x.IsActive)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetActiveUsersCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .CountAsync(x => x.IsActive, cancellationToken);
    }

    public async Task<bool> ExistsByTelegramIdAsync(TelegramUserId telegramUserId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AnyAsync(x => x.TelegramUserId == telegramUserId, cancellationToken);
    }

    public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task DeleteAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);
    }
}