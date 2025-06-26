using Microsoft.EntityFrameworkCore;
using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Domain.Enums;
using Lauf.Infrastructure.Persistence;

namespace Lauf.Infrastructure.Persistence.Repositories;

/// <summary>
/// Репозиторий для работы с потоками
/// </summary>
public class FlowRepository : IFlowRepository
{
    private readonly ApplicationDbContext _context;

    public FlowRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Flow?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Flows
            .Include(x => x.Settings)
            .Include(x => x.Steps)
                .ThenInclude(s => s.Components)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Flow?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<Flow?> GetByIdWithStepsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Flows
            .Include(x => x.Steps)
                .ThenInclude(s => s.Components)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Flow?> GetByTitleAsync(string title, CancellationToken cancellationToken = default)
    {
        return await _context.Flows
            .Include(x => x.Settings)
            .FirstOrDefaultAsync(x => x.Title == title, cancellationToken);
    }

    public async Task<IReadOnlyList<Flow>> GetByStatusAsync(FlowStatus status, int skip = 0, int take = 50, CancellationToken cancellationToken = default)
    {
        return await _context.Flows
            .Include(x => x.Settings)
            .Where(x => x.Status == status)
            .OrderBy(x => x.Title)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Flow>> GetPublishedAsync(int skip = 0, int take = 50, CancellationToken cancellationToken = default)
    {
        return await _context.Flows
            .Include(x => x.Settings)
            .Where(x => x.Status == FlowStatus.Published)
            .OrderBy(x => x.Title)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Flow>> GetByCategoryAsync(string category, int skip = 0, int take = 50, CancellationToken cancellationToken = default)
    {
        // Пока категории нет в модели, возвращаем все опубликованные потоки
        return await GetByStatusAsync(FlowStatus.Published, skip, take, cancellationToken);
    }

    public async Task<IReadOnlyList<Flow>> GetRequiredFlowsAsync(CancellationToken cancellationToken = default)
    {
        // Пока логики обязательных потоков нет, возвращаем все опубликованные
        return await GetByStatusAsync(FlowStatus.Published, cancellationToken: cancellationToken);
    }

    public async Task<int> CountByStatusAsync(FlowStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Flows
            .CountAsync(x => x.Status == status, cancellationToken);
    }

    public async Task<Flow> AddAsync(Flow flow, CancellationToken cancellationToken = default)
    {
        _context.Flows.Add(flow);
        await _context.SaveChangesAsync(cancellationToken);
        return flow;
    }

    public async Task<Flow> UpdateAsync(Flow flow, CancellationToken cancellationToken = default)
    {
        _context.Flows.Update(flow);
        await _context.SaveChangesAsync(cancellationToken);
        return flow;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var flow = await GetByIdAsync(id, cancellationToken);
        if (flow != null)
        {
            await DeleteAsync(flow, cancellationToken);
        }
    }

    public async Task DeleteAsync(Flow flow, CancellationToken cancellationToken = default)
    {
        _context.Flows.Remove(flow);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Flows.AnyAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Flow>> SearchAsync(string searchTerm, int skip = 0, int take = 50, CancellationToken cancellationToken = default)
    {
        return await _context.Flows
            .Include(x => x.Settings)
            .Where(x => x.Title.Contains(searchTerm) || x.Description.Contains(searchTerm))
            .OrderBy(x => x.Title)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByTitleAsync(string title, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Flows.Where(x => x.Title == title);
        
        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Flow>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Flows
            .Include(f => f.Steps)
            .ToListAsync(cancellationToken);
    }

    public async Task<Flow?> GetFlowByStepIdAsync(Guid stepId, CancellationToken cancellationToken = default)
    {
        return await _context.Flows
            .Include(f => f.Steps)
                .ThenInclude(s => s.Components)
            .Where(f => f.Steps.Any(s => s.Id == stepId))
            .FirstOrDefaultAsync(cancellationToken);
    }
}