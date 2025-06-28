using Microsoft.EntityFrameworkCore;
using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Domain.Enums;
using Lauf.Infrastructure.Persistence;

namespace Lauf.Infrastructure.Persistence.Repositories;

/// <summary>
/// Репозиторий для работы с назначениями потоков
/// </summary>
public class FlowAssignmentRepository : IFlowAssignmentRepository
{
    private readonly ApplicationDbContext _context;

    public FlowAssignmentRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<FlowAssignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.FlowAssignments
            .Include(x => x.User)
            .Include(x => x.Flow)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<FlowAssignment?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<FlowAssignment>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.FlowAssignments
            .Include(x => x.User)
            .Include(x => x.Flow)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FlowAssignment>> GetByUserAsync(Guid userId, int skip = 0, int take = 50, CancellationToken cancellationToken = default)
    {
        return await _context.FlowAssignments
            .Include(x => x.User)
            .Include(x => x.Flow)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.AssignedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<FlowAssignment>> GetByFlowIdAsync(Guid flowId, CancellationToken cancellationToken = default)
    {
        return await _context.FlowAssignments
            .Include(x => x.User)
            .Include(x => x.Flow)
            .Where(x => x.FlowId == flowId)
            .OrderByDescending(x => x.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FlowAssignment>> GetByStatusAsync(AssignmentStatus status, int skip = 0, int take = 50, CancellationToken cancellationToken = default)
    {
        return await _context.FlowAssignments
            .Include(x => x.User)
            .Include(x => x.Flow)
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.AssignedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FlowAssignment>> GetWithApproachingDeadlineAsync(int daysAhead = 3, CancellationToken cancellationToken = default)
    {
        var targetDate = DateTime.UtcNow.AddDays(daysAhead);
        return await _context.FlowAssignments
            .Include(x => x.User)
            .Include(x => x.Flow)
            .Where(x => x.Status == AssignmentStatus.InProgress && 
                       x.Deadline <= targetDate)
            .OrderBy(x => x.Deadline)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FlowAssignment>> GetOverdueAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.FlowAssignments
            .Include(x => x.User)
            .Include(x => x.Flow)
            .Where(x => x.Status == AssignmentStatus.InProgress && 
                       x.Deadline < now)
            .OrderBy(x => x.Deadline)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FlowAssignment>> GetByBuddyAsync(Guid buddyId, int skip = 0, int take = 50, CancellationToken cancellationToken = default)
    {
        // Пока связь с бадди не реализована, возвращаем пустой список
        return new List<FlowAssignment>();
    }

    public async Task<int> CountByStatusAsync(AssignmentStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.FlowAssignments
            .CountAsync(x => x.Status == status, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid userId, Guid flowId, CancellationToken cancellationToken = default)
    {
        return await _context.FlowAssignments
            .AnyAsync(x => x.UserId == userId && x.FlowId == flowId, cancellationToken);
    }

    public async Task<FlowAssignment> AddAsync(FlowAssignment assignment, CancellationToken cancellationToken = default)
    {
        _context.FlowAssignments.Add(assignment);
        await _context.SaveChangesAsync(cancellationToken);
        return assignment;
    }

    public async Task<FlowAssignment> UpdateAsync(FlowAssignment assignment, CancellationToken cancellationToken = default)
    {
        _context.FlowAssignments.Update(assignment);
        await _context.SaveChangesAsync(cancellationToken);
        return assignment;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var assignment = await GetByIdAsync(id, cancellationToken);
        if (assignment != null)
        {
            await DeleteAsync(assignment, cancellationToken);
        }
    }

    public async Task DeleteAsync(FlowAssignment assignment, CancellationToken cancellationToken = default)
    {
        _context.FlowAssignments.Remove(assignment);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<FlowAssignment?> GetByUserAndFlowAsync(Guid userId, Guid flowId, CancellationToken cancellationToken = default)
    {
        return await _context.FlowAssignments
            .Include(x => x.User)
            .Include(x => x.Flow)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.FlowId == flowId, cancellationToken);
    }

    public async Task<bool> HasActiveAssignmentAsync(Guid userId, Guid flowId, CancellationToken cancellationToken = default)
    {
        return await _context.FlowAssignments
            .AnyAsync(x => x.UserId == userId && 
                          x.FlowId == flowId && 
                          x.Status == AssignmentStatus.Assigned, 
                     cancellationToken);
    }

    public async Task<IEnumerable<FlowAssignment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.FlowAssignments
            .Include(a => a.User)
            .Include(a => a.Flow)
            .ToListAsync(cancellationToken);
    }
}