using System;
using System.Threading;
using System.Threading.Tasks;
using Lauf.Domain.Entities.Versions;

namespace Lauf.Domain.Services;

/// <summary>
/// Сервис для управления версионированием сущностей
/// </summary>
public interface IVersioningService
{
    /// <summary>
    /// Создать новую версию потока и всех его компонентов
    /// </summary>
    /// <param name="sourceFlowVersion">Исходная версия потока</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Новая версия потока со всеми вложенными версиями</returns>
    Task<FlowVersion> CreateNewFlowVersionAsync(FlowVersion sourceFlowVersion, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создать новую версию этапа и всех его компонентов
    /// </summary>
    /// <param name="sourceStepVersion">Исходная версия этапа</param>
    /// <param name="newFlowVersionId">ID новой версии потока</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Новая версия этапа со всеми вложенными версиями</returns>
    Task<FlowStepVersion> CreateNewStepVersionAsync(FlowStepVersion sourceStepVersion, Guid newFlowVersionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создать новую версию компонента
    /// </summary>
    /// <param name="sourceComponentVersion">Исходная версия компонента</param>
    /// <param name="newStepVersionId">ID новой версии этапа</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Новая версия компонента</returns>
    Task<ComponentVersion> CreateNewComponentVersionAsync(ComponentVersion sourceComponentVersion, Guid newStepVersionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Активировать версию потока (деактивировать предыдущую активную версию)
    /// </summary>
    /// <param name="flowVersionId">ID версии потока для активации</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task ActivateFlowVersionAsync(Guid flowVersionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Активировать версию этапа (деактивировать предыдущую активную версию)
    /// </summary>
    /// <param name="stepVersionId">ID версии этапа для активации</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task ActivateStepVersionAsync(Guid stepVersionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Активировать версию компонента (деактивировать предыдущую активную версию)
    /// </summary>
    /// <param name="componentVersionId">ID версии компонента для активации</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task ActivateComponentVersionAsync(Guid componentVersionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить активную версию потока
    /// </summary>
    /// <param name="originalFlowId">ID оригинального потока</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Активная версия потока или null</returns>
    Task<FlowVersion?> GetActiveFlowVersionAsync(Guid originalFlowId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить активную версию этапа
    /// </summary>
    /// <param name="originalStepId">ID оригинального этапа</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Активная версия этапа или null</returns>
    Task<FlowStepVersion?> GetActiveStepVersionAsync(Guid originalStepId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить активную версию компонента
    /// </summary>
    /// <param name="originalComponentId">ID оригинального компонента</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Активная версия компонента или null</returns>
    Task<ComponentVersion?> GetActiveComponentVersionAsync(Guid originalComponentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить конкретную версию потока
    /// </summary>
    /// <param name="originalFlowId">ID оригинального потока</param>
    /// <param name="version">Номер версии</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Версия потока или null</returns>
    Task<FlowVersion?> GetFlowVersionAsync(Guid originalFlowId, int version, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить все версии потока
    /// </summary>
    /// <param name="originalFlowId">ID оригинального потока</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список всех версий потока</returns>
    Task<IList<FlowVersion>> GetAllFlowVersionsAsync(Guid originalFlowId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить неиспользуемые версии (не связанные с активными назначениями)
    /// </summary>
    /// <param name="originalFlowId">ID оригинального потока</param>
    /// <param name="keepMinimumVersions">Минимальное количество версий для сохранения</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Количество удаленных версий</returns>
    Task<int> CleanupUnusedVersionsAsync(Guid originalFlowId, int keepMinimumVersions = 3, CancellationToken cancellationToken = default);
}