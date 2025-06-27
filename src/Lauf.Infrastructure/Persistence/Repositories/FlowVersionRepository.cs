using Lauf.Domain.Entities.Versions;
using Lauf.Domain.Enums;
using Lauf.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lauf.Infrastructure.Persistence.Repositories;

/// <summary>
/// Реализация репозитория для работы с версиями потоков
/// </summary>
public class FlowVersionRepository : IFlowVersionRepository
{
    private readonly ApplicationDbContext _context;

    public FlowVersionRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Получить версию потока по ID
    /// </summary>
    public async Task<FlowVersion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.FlowVersions
            .FirstOrDefaultAsync(fv => fv.Id == id, cancellationToken);
    }

    /// <summary>
    /// Получить версию потока по ID с полной загрузкой зависимостей
    /// </summary>
    public async Task<FlowVersion?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.FlowVersions
            .Include(fv => fv.StepVersions.OrderBy(sv => sv.Order))
                .ThenInclude(sv => sv.ComponentVersions.OrderBy(cv => cv.Order))
                    .ThenInclude(cv => cv.ArticleVersion)
            .Include(fv => fv.StepVersions)
                .ThenInclude(sv => sv.ComponentVersions)
                    .ThenInclude(cv => cv.QuizVersion)
                        .ThenInclude(qv => qv!.Options.OrderBy(o => o.Order))
            .Include(fv => fv.StepVersions)
                .ThenInclude(sv => sv.ComponentVersions)
                    .ThenInclude(cv => cv.TaskVersion)
            .FirstOrDefaultAsync(fv => fv.Id == id, cancellationToken);
    }

    /// <summary>
    /// Получить активную версию потока
    /// </summary>
    public async Task<FlowVersion?> GetActiveVersionAsync(Guid originalFlowId, CancellationToken cancellationToken = default)
    {
        return await _context.FlowVersions
            .FirstOrDefaultAsync(fv => fv.OriginalId == originalFlowId && fv.IsActive, cancellationToken);
    }

    /// <summary>
    /// Получить активную версию потока с полной загрузкой зависимостей
    /// </summary>
    public async Task<FlowVersion?> GetActiveVersionWithDetailsAsync(Guid originalFlowId, CancellationToken cancellationToken = default)
    {
        return await _context.FlowVersions
            .Include(fv => fv.StepVersions.OrderBy(sv => sv.Order))
                .ThenInclude(sv => sv.ComponentVersions.OrderBy(cv => cv.Order))
                    .ThenInclude(cv => cv.ArticleVersion)
            .Include(fv => fv.StepVersions)
                .ThenInclude(sv => sv.ComponentVersions)
                    .ThenInclude(cv => cv.QuizVersion)
                        .ThenInclude(qv => qv!.Options.OrderBy(o => o.Order))
            .Include(fv => fv.StepVersions)
                .ThenInclude(sv => sv.ComponentVersions)
                    .ThenInclude(cv => cv.TaskVersion)
            .FirstOrDefaultAsync(fv => fv.OriginalId == originalFlowId && fv.IsActive, cancellationToken);
    }

    /// <summary>
    /// Получить конкретную версию потока
    /// </summary>
    public async Task<FlowVersion?> GetVersionAsync(Guid originalFlowId, int version, CancellationToken cancellationToken = default)
    {
        return await _context.FlowVersions
            .FirstOrDefaultAsync(fv => fv.OriginalId == originalFlowId && fv.Version == version, cancellationToken);
    }

    /// <summary>
    /// Получить конкретную версию потока с полной загрузкой зависимостей
    /// </summary>
    public async Task<FlowVersion?> GetVersionWithDetailsAsync(Guid originalFlowId, int version, CancellationToken cancellationToken = default)
    {
        return await _context.FlowVersions
            .Include(fv => fv.StepVersions.OrderBy(sv => sv.Order))
                .ThenInclude(sv => sv.ComponentVersions.OrderBy(cv => cv.Order))
                    .ThenInclude(cv => cv.ArticleVersion)
            .Include(fv => fv.StepVersions)
                .ThenInclude(sv => sv.ComponentVersions)
                    .ThenInclude(cv => cv.QuizVersion)
                        .ThenInclude(qv => qv!.Options.OrderBy(o => o.Order))
            .Include(fv => fv.StepVersions)
                .ThenInclude(sv => sv.ComponentVersions)
                    .ThenInclude(cv => cv.TaskVersion)
            .FirstOrDefaultAsync(fv => fv.OriginalId == originalFlowId && fv.Version == version, cancellationToken);
    }

    /// <summary>
    /// Получить все версии потока
    /// </summary>
    public async Task<IList<FlowVersion>> GetAllVersionsAsync(Guid originalFlowId, CancellationToken cancellationToken = default)
    {
        return await _context.FlowVersions
            .Where(fv => fv.OriginalId == originalFlowId)
            .OrderByDescending(fv => fv.Version)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Получить последние N версий потока
    /// </summary>
    public async Task<IList<FlowVersion>> GetLatestVersionsAsync(Guid originalFlowId, int count, CancellationToken cancellationToken = default)
    {
        return await _context.FlowVersions
            .Where(fv => fv.OriginalId == originalFlowId)
            .OrderByDescending(fv => fv.Version)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Получить максимальный номер версии для потока
    /// </summary>
    public async Task<int> GetMaxVersionAsync(Guid originalFlowId, CancellationToken cancellationToken = default)
    {
        return await _context.FlowVersions
            .Where(fv => fv.OriginalId == originalFlowId)
            .MaxAsync(fv => (int?)fv.Version, cancellationToken) ?? 0;
    }

    /// <summary>
    /// Проверить, существует ли активная версия потока
    /// </summary>
    public async Task<bool> HasActiveVersionAsync(Guid originalFlowId, CancellationToken cancellationToken = default)
    {
        return await _context.FlowVersions
            .AnyAsync(fv => fv.OriginalId == originalFlowId && fv.IsActive, cancellationToken);
    }

    /// <summary>
    /// Получить все активные версии потоков
    /// </summary>
    public async Task<IList<FlowVersion>> GetAllActiveVersionsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.FlowVersions
            .Where(fv => fv.IsActive)
            .OrderBy(fv => fv.Title)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Получить версии потоков по статусу
    /// </summary>
    public async Task<IList<FlowVersion>> GetByStatusAsync(FlowStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.FlowVersions
            .Where(fv => fv.Status == status)
            .OrderBy(fv => fv.Title)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Получить версии потоков, созданные пользователем
    /// </summary>
    public async Task<IList<FlowVersion>> GetByCreatedByAsync(Guid createdById, CancellationToken cancellationToken = default)
    {
        return await _context.FlowVersions
            .Where(fv => fv.CreatedById == createdById)
            .OrderByDescending(fv => fv.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Получить версии, которые используются в активных назначениях
    /// </summary>
    public async Task<IList<FlowVersion>> GetUsedInActiveAssignmentsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.FlowVersions
            .Where(fv => fv.Assignments.Any(a => a.Status == AssignmentStatus.InProgress || a.Status == AssignmentStatus.Assigned))
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Получить неиспользуемые версии (не связанные с назначениями)
    /// </summary>
    public async Task<IList<FlowVersion>> GetUnusedVersionsAsync(Guid originalFlowId, CancellationToken cancellationToken = default)
    {
        return await _context.FlowVersions
            .Where(fv => fv.OriginalId == originalFlowId && !fv.Assignments.Any())
            .OrderBy(fv => fv.Version)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Добавить новую версию потока
    /// </summary>
    public async Task<FlowVersion> AddAsync(FlowVersion flowVersion, CancellationToken cancellationToken = default)
    {
        var entry = await _context.FlowVersions.AddAsync(flowVersion, cancellationToken);
        return entry.Entity;
    }

    /// <summary>
    /// Обновить версию потока
    /// </summary>
    public async Task<FlowVersion> UpdateAsync(FlowVersion flowVersion, CancellationToken cancellationToken = default)
    {
        _context.FlowVersions.Update(flowVersion);
        return flowVersion;
    }

    /// <summary>
    /// Удалить версию потока
    /// </summary>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var flowVersion = await GetByIdAsync(id, cancellationToken);
        if (flowVersion != null)
        {
            _context.FlowVersions.Remove(flowVersion);
        }
    }

    /// <summary>
    /// Удалить версии потока
    /// </summary>
    public async Task DeleteRangeAsync(IEnumerable<FlowVersion> flowVersions, CancellationToken cancellationToken = default)
    {
        _context.FlowVersions.RemoveRange(flowVersions);
    }

    /// <summary>
    /// Деактивировать все версии потока
    /// </summary>
    public async Task DeactivateAllVersionsAsync(Guid originalFlowId, CancellationToken cancellationToken = default)
    {
        var versions = await _context.FlowVersions
            .Where(fv => fv.OriginalId == originalFlowId && fv.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var version in versions)
        {
            version.Deactivate();
        }
    }

    /// <summary>
    /// Активировать конкретную версию (деактивирует все остальные)
    /// </summary>
    public async Task ActivateVersionAsync(Guid flowVersionId, CancellationToken cancellationToken = default)
    {
        var flowVersion = await GetByIdAsync(flowVersionId, cancellationToken);
        if (flowVersion == null)
            throw new InvalidOperationException($"Версия потока с ID {flowVersionId} не найдена");

        // Деактивируем все остальные версии этого потока
        await DeactivateAllVersionsAsync(flowVersion.OriginalId, cancellationToken);

        // Активируем выбранную версию
        flowVersion.Activate();
    }

    /// <summary>
    /// Получить количество версий потока
    /// </summary>
    public async Task<int> GetVersionCountAsync(Guid originalFlowId, CancellationToken cancellationToken = default)
    {
        return await _context.FlowVersions
            .CountAsync(fv => fv.OriginalId == originalFlowId, cancellationToken);
    }

    /// <summary>
    /// Проверить, существует ли версия потока
    /// </summary>
    public async Task<bool> ExistsAsync(Guid originalFlowId, int version, CancellationToken cancellationToken = default)
    {
        return await _context.FlowVersions
            .AnyAsync(fv => fv.OriginalId == originalFlowId && fv.Version == version, cancellationToken);
    }

    /// <summary>
    /// Найти версии потоков по названию
    /// </summary>
    public async Task<IList<FlowVersion>> SearchByTitleAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var normalizedSearchTerm = searchTerm.ToLowerInvariant();
        
        return await _context.FlowVersions
            .Where(fv => fv.Title.ToLower().Contains(normalizedSearchTerm))
            .OrderBy(fv => fv.Title)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Найти версии потоков по тегам
    /// </summary>
    public async Task<IList<FlowVersion>> SearchByTagsAsync(IEnumerable<string> tags, CancellationToken cancellationToken = default)
    {
        var normalizedTags = tags.Select(t => t.ToLowerInvariant()).ToList();
        
        return await _context.FlowVersions
            .Where(fv => normalizedTags.Any(tag => fv.Tags.ToLower().Contains(tag)))
            .OrderBy(fv => fv.Title)
            .ToListAsync(cancellationToken);
    }
}