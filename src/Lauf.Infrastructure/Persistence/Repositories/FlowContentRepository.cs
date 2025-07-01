using Microsoft.EntityFrameworkCore;
using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Entities.Components;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Infrastructure.Persistence;

namespace Lauf.Infrastructure.Persistence.Repositories;

/// <summary>
/// Репозиторий для работы с содержимым потоков
/// </summary>
public class FlowContentRepository : IFlowContentRepository
{
    private readonly ApplicationDbContext _context;

    public FlowContentRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<FlowContent?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.FlowContents
            .Include(fc => fc.Steps.OrderBy(s => s.Order))
                .ThenInclude(s => s.Components.OrderBy(c => c.Order))
                    .ThenInclude(c => ((QuizComponent)c).Questions)
                        .ThenInclude(q => q.Options.OrderBy(o => o.Order))
            .FirstOrDefaultAsync(fc => fc.Id == id, cancellationToken);
    }

    public async Task<FlowContent?> GetWithStepsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<FlowContent?> GetActiveByFlowIdAsync(Guid flowId, CancellationToken cancellationToken = default)
    {
        var flow = await _context.Flows
            .Include(f => f.ActiveContent)
                .ThenInclude(ac => ac.Steps.OrderBy(s => s.Order))
                    .ThenInclude(s => s.Components.OrderBy(c => c.Order))
                        .ThenInclude(c => ((QuizComponent)c).Questions)
                            .ThenInclude(q => q.Options.OrderBy(o => o.Order))
            .FirstOrDefaultAsync(f => f.Id == flowId, cancellationToken);

        return flow?.ActiveContent;
    }

    public async Task<IReadOnlyList<FlowContent>> GetVersionsByFlowIdAsync(Guid flowId, CancellationToken cancellationToken = default)
    {
        return await _context.FlowContents
            .Where(fc => fc.FlowId == flowId)
            .OrderBy(fc => fc.Version)
            .ToListAsync(cancellationToken);
    }

    public async Task<FlowContent?> GetByFlowIdAndVersionAsync(Guid flowId, int version, CancellationToken cancellationToken = default)
    {
        return await _context.FlowContents
            .Include(fc => fc.Steps.OrderBy(s => s.Order))
                .ThenInclude(s => s.Components.OrderBy(c => c.Order))
                    .ThenInclude(c => ((QuizComponent)c).Questions)
                        .ThenInclude(q => q.Options.OrderBy(o => o.Order))
            .FirstOrDefaultAsync(fc => fc.FlowId == flowId && fc.Version == version, cancellationToken);
    }

    public async Task<FlowContent?> GetLatestVersionAsync(Guid flowId, CancellationToken cancellationToken = default)
    {
        return await _context.FlowContents
            .Include(fc => fc.Steps.OrderBy(s => s.Order))
                .ThenInclude(s => s.Components.OrderBy(c => c.Order))
                    .ThenInclude(c => ((QuizComponent)c).Questions)
                        .ThenInclude(q => q.Options.OrderBy(o => o.Order))
            .Where(fc => fc.FlowId == flowId)
            .OrderByDescending(fc => fc.Version)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<FlowContent> AddAsync(FlowContent content, CancellationToken cancellationToken = default)
    {
        _context.FlowContents.Add(content);
        await _context.SaveChangesAsync(cancellationToken);
        return content;
    }

    public async Task<FlowContent> UpdateAsync(FlowContent content, CancellationToken cancellationToken = default)
    {
        var entry = _context.Entry(content);
        if (entry.State == EntityState.Detached)
        {
            _context.FlowContents.Update(content);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return content;
    }

    public async Task DeleteAsync(FlowContent content, CancellationToken cancellationToken = default)
    {
        _context.FlowContents.Remove(content);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<FlowContent> CreateNewVersionAsync(Guid flowId, Guid createdBy, CancellationToken cancellationToken = default)
    {
        // Получаем следующий номер версии
        var maxVersion = await _context.FlowContents
            .Where(fc => fc.FlowId == flowId)
            .MaxAsync(fc => (int?)fc.Version, cancellationToken) ?? 0;

        var newVersion = maxVersion + 1;

        // Создаем новую версию содержимого
        var newContent = new FlowContent(flowId, newVersion, createdBy);

        return await AddAsync(newContent, cancellationToken);
    }

    public async Task<int> GetVersionCountAsync(Guid flowId, CancellationToken cancellationToken = default)
    {
        return await _context.FlowContents
            .CountAsync(fc => fc.FlowId == flowId, cancellationToken);
    }
}