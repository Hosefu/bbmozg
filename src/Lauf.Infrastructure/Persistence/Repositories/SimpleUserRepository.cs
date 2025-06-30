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
            .FirstOrDefaultAsync(x => x.TelegramUserId.Value == telegramUserId.Value, cancellationToken);
    }

    public async Task<User?> GetByTelegramUserIdAsync(long telegramUserId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.TelegramUserId != null && x.TelegramUserId.Value == telegramUserId, cancellationToken);
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
            .AnyAsync(x => x.TelegramUserId.Value == telegramUserId.Value, cancellationToken);
    }

    public Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Add(user);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.Roles)
            .ToListAsync(cancellationToken);
    }

    public async Task<IList<User>> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Users.Include(u => u.Roles).AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLowerInvariant();
            query = query.Where(u => 
                u.FirstName.ToLowerInvariant().Contains(term) ||
                u.LastName.ToLowerInvariant().Contains(term));
        }

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetTotalCountAsync(string? searchTerm = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Users.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLowerInvariant();
            query = query.Where(u => 
                u.FirstName.ToLowerInvariant().Contains(term) ||
                u.LastName.ToLowerInvariant().Contains(term));
        }

        return await query.CountAsync(cancellationToken);
    }

    public async Task<IList<User>> GetByIdsAsync(IEnumerable<Guid> userIds, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.Roles)
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync(cancellationToken);
    }
}