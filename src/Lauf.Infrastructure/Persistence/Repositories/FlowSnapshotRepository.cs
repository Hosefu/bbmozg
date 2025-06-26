using Lauf.Domain.Entities.Snapshots;
using Lauf.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Lauf.Infrastructure.Persistence.Repositories;

/// <summary>
/// Репозиторий для работы со снапшотами потоков
/// </summary>
public class FlowSnapshotRepository : IFlowSnapshotRepository
{
    private readonly ApplicationDbContext _context;

    public FlowSnapshotRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(FlowSnapshot snapshot, CancellationToken cancellationToken = default)
    {
        await _context.FlowSnapshots.AddAsync(snapshot, cancellationToken);
    }

    public async Task<FlowSnapshot?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.FlowSnapshots
            .Include(fs => fs.Steps)
                .ThenInclude(s => s.Components)
            .FirstOrDefaultAsync(fs => fs.Id == id, cancellationToken);
    }

    public async Task<FlowSnapshot?> GetByAssignmentIdAsync(Guid assignmentId, CancellationToken cancellationToken = default)
    {
        // Найти FlowSnapshot через FlowAssignment
        var assignment = await _context.FlowAssignments
            .Where(fa => fa.Id == assignmentId)
            .Select(fa => fa.FlowSnapshotId)
            .FirstOrDefaultAsync(cancellationToken);

        if (assignment == null)
            return null;

        return await _context.FlowSnapshots
            .Include(fs => fs.Steps)
                .ThenInclude(s => s.Components)
            .FirstOrDefaultAsync(fs => fs.Id == assignment, cancellationToken);
    }

    public async Task<List<FlowSnapshot>> GetByOriginalFlowIdAsync(Guid originalFlowId, CancellationToken cancellationToken = default)
    {
        return await _context.FlowSnapshots
            .Where(fs => fs.OriginalFlowId == originalFlowId)
            .OrderBy(fs => fs.Version)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<FlowSnapshot>> GetOldSnapshotsAsync(DateTime cutoffDate, CancellationToken cancellationToken = default)
    {
        return await _context.FlowSnapshots
            .Where(fs => fs.CreatedAt < cutoffDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasActiveAssignmentsAsync(Guid snapshotId, CancellationToken cancellationToken = default)
    {
        return await _context.FlowAssignments
            .AnyAsync(fa => fa.FlowSnapshotId == snapshotId && 
                           (fa.Status == Domain.Enums.AssignmentStatus.Assigned || 
                            fa.Status == Domain.Enums.AssignmentStatus.InProgress), 
                      cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var snapshot = await _context.FlowSnapshots.FindAsync(new object[] { id }, cancellationToken);
        if (snapshot != null)
        {
            _context.FlowSnapshots.Remove(snapshot);
        }
    }

    public async Task<FlowSnapshot?> GetLatestSnapshotAsync(Guid originalFlowId, CancellationToken cancellationToken = default)
    {
        return await _context.FlowSnapshots
            .Include(fs => fs.Steps)
                .ThenInclude(s => s.Components)
            .Where(fs => fs.OriginalFlowId == originalFlowId)
            .OrderByDescending(fs => fs.Version)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<FlowSnapshot?> GetSnapshotByVersionAsync(Guid originalFlowId, int version, CancellationToken cancellationToken = default)
    {
        return await _context.FlowSnapshots
            .Include(fs => fs.Steps)
                .ThenInclude(s => s.Components)
            .FirstOrDefaultAsync(fs => fs.OriginalFlowId == originalFlowId && fs.Version == version, cancellationToken);
    }
} 