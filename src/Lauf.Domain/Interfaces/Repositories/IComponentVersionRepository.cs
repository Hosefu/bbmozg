using Lauf.Domain.Entities.Versions;
using Lauf.Domain.Enums;
using Lauf.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lauf.Domain.Interfaces.Repositories;

/// <summary>
/// Репозиторий для работы с версиями компонентов
/// </summary>
public interface IComponentVersionRepository
{
    /// <summary>
    /// Получить версию компонента по ID
    /// </summary>
    Task<ComponentVersion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить версию компонента по ID с полной загрузкой зависимостей
    /// </summary>
    Task<ComponentVersion?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить активную версию компонента
    /// </summary>
    Task<ComponentVersion?> GetActiveVersionAsync(Guid originalComponentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить активную версию компонента с полной загрузкой зависимостей
    /// </summary>
    Task<ComponentVersion?> GetActiveVersionWithDetailsAsync(Guid originalComponentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить конкретную версию компонента
    /// </summary>
    Task<ComponentVersion?> GetVersionAsync(Guid originalComponentId, int version, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить конкретную версию компонента с полной загрузкой зависимостей
    /// </summary>
    Task<ComponentVersion?> GetVersionWithDetailsAsync(Guid originalComponentId, int version, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить все версии компонента
    /// </summary>
    Task<IList<ComponentVersion>> GetAllVersionsAsync(Guid originalComponentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить версии компонентов для этапа
    /// </summary>
    Task<IList<ComponentVersion>> GetByStepVersionAsync(Guid stepVersionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить версии компонентов для этапа с полной загрузкой зависимостей
    /// </summary>
    Task<IList<ComponentVersion>> GetByStepVersionWithDetailsAsync(Guid stepVersionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить версии компонентов по типу
    /// </summary>
    Task<IList<ComponentVersion>> GetByTypeAsync(ComponentType componentType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить версии компонентов по статусу
    /// </summary>
    Task<IList<ComponentVersion>> GetByStatusAsync(ComponentStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить максимальный номер версии для компонента
    /// </summary>
    Task<int> GetMaxVersionAsync(Guid originalComponentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверить, существует ли активная версия компонента
    /// </summary>
    Task<bool> HasActiveVersionAsync(Guid originalComponentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить все активные версии компонентов
    /// </summary>
    Task<IList<ComponentVersion>> GetAllActiveVersionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить неиспользуемые версии компонента
    /// </summary>
    Task<IList<ComponentVersion>> GetUnusedVersionsAsync(Guid originalComponentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавить новую версию компонента
    /// </summary>
    Task<ComponentVersion> AddAsync(ComponentVersion componentVersion, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить версию компонента
    /// </summary>
    Task<ComponentVersion> UpdateAsync(ComponentVersion componentVersion, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить версию компонента
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить версии компонента
    /// </summary>
    Task DeleteRangeAsync(IEnumerable<ComponentVersion> componentVersions, CancellationToken cancellationToken = default);

    /// <summary>
    /// Деактивировать все версии компонента
    /// </summary>
    Task DeactivateAllVersionsAsync(Guid originalComponentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Активировать конкретную версию (деактивирует все остальные)
    /// </summary>
    Task ActivateVersionAsync(Guid componentVersionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить количество версий компонента
    /// </summary>
    Task<int> GetVersionCountAsync(Guid originalComponentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверить, существует ли версия компонента
    /// </summary>
    Task<bool> ExistsAsync(Guid originalComponentId, int version, CancellationToken cancellationToken = default);

    /// <summary>
    /// Найти версии компонентов по названию
    /// </summary>
    Task<IList<ComponentVersion>> SearchByTitleAsync(string searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить статистику по типам компонентов
    /// </summary>
    Task<Dictionary<ComponentType, int>> GetComponentTypeStatisticsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить компоненты с истекшим временем выполнения
    /// </summary>
    Task<IList<ComponentVersion>> GetWithExpiredDurationAsync(int maxDurationMinutes, CancellationToken cancellationToken = default);
}