using Lauf.Domain.Entities.Versions;
using Lauf.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lauf.Domain.Interfaces.Repositories;

/// <summary>
/// Репозиторий для работы с версиями потоков
/// </summary>
public interface IFlowVersionRepository
{
    /// <summary>
    /// Получить версию потока по ID
    /// </summary>
    Task<FlowVersion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить версию потока по ID с полной загрузкой зависимостей
    /// </summary>
    Task<FlowVersion?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить активную версию потока
    /// </summary>
    Task<FlowVersion?> GetActiveVersionAsync(Guid originalFlowId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить активную версию потока с полной загрузкой зависимостей
    /// </summary>
    Task<FlowVersion?> GetActiveVersionWithDetailsAsync(Guid originalFlowId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить конкретную версию потока
    /// </summary>
    Task<FlowVersion?> GetVersionAsync(Guid originalFlowId, int version, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить конкретную версию потока с полной загрузкой зависимостей
    /// </summary>
    Task<FlowVersion?> GetVersionWithDetailsAsync(Guid originalFlowId, int version, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить все версии потока
    /// </summary>
    Task<IList<FlowVersion>> GetAllVersionsAsync(Guid originalFlowId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить последние N версий потока
    /// </summary>
    Task<IList<FlowVersion>> GetLatestVersionsAsync(Guid originalFlowId, int count, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить максимальный номер версии для потока
    /// </summary>
    Task<int> GetMaxVersionAsync(Guid originalFlowId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверить, существует ли активная версия потока
    /// </summary>
    Task<bool> HasActiveVersionAsync(Guid originalFlowId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить все активные версии потоков
    /// </summary>
    Task<IList<FlowVersion>> GetAllActiveVersionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить версии потоков по статусу
    /// </summary>
    Task<IList<FlowVersion>> GetByStatusAsync(FlowStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить версии потоков, созданные пользователем
    /// </summary>
    Task<IList<FlowVersion>> GetByCreatedByAsync(Guid createdById, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить версии, которые используются в активных назначениях
    /// </summary>
    Task<IList<FlowVersion>> GetUsedInActiveAssignmentsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить неиспользуемые версии (не связанные с назначениями)
    /// </summary>
    Task<IList<FlowVersion>> GetUnusedVersionsAsync(Guid originalFlowId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавить новую версию потока
    /// </summary>
    Task<FlowVersion> AddAsync(FlowVersion flowVersion, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить версию потока
    /// </summary>
    Task<FlowVersion> UpdateAsync(FlowVersion flowVersion, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить версию потока
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить версии потока
    /// </summary>
    Task DeleteRangeAsync(IEnumerable<FlowVersion> flowVersions, CancellationToken cancellationToken = default);

    /// <summary>
    /// Деактивировать все версии потока
    /// </summary>
    Task DeactivateAllVersionsAsync(Guid originalFlowId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Активировать конкретную версию (деактивирует все остальные)
    /// </summary>
    Task ActivateVersionAsync(Guid flowVersionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить количество версий потока
    /// </summary>
    Task<int> GetVersionCountAsync(Guid originalFlowId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверить, существует ли версия потока
    /// </summary>
    Task<bool> ExistsAsync(Guid originalFlowId, int version, CancellationToken cancellationToken = default);

    /// <summary>
    /// Найти версии потоков по названию
    /// </summary>
    Task<IList<FlowVersion>> SearchByTitleAsync(string searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Найти версии потоков по тегам
    /// </summary>
    Task<IList<FlowVersion>> SearchByTagsAsync(IEnumerable<string> tags, CancellationToken cancellationToken = default);
}