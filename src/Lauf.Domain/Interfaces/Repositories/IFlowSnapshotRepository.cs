using Lauf.Domain.Entities.Snapshots;

namespace Lauf.Domain.Interfaces.Repositories;

/// <summary>
/// Интерфейс репозитория для работы со снапшотами потоков
/// </summary>
public interface IFlowSnapshotRepository
{
    /// <summary>
    /// Добавить снапшот потока
    /// </summary>
    /// <param name="snapshot">Снапшот для добавления</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task AddAsync(FlowSnapshot snapshot, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить снапшот по ID с полной детализацией
    /// </summary>
    /// <param name="id">ID снапшота</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Снапшот с полной детализацией или null</returns>
    Task<FlowSnapshot?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить снапшот по ID назначения потока
    /// </summary>
    /// <param name="assignmentId">ID назначения</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Снапшот или null</returns>
    Task<FlowSnapshot?> GetByAssignmentIdAsync(Guid assignmentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить все снапшоты для оригинального потока
    /// </summary>
    /// <param name="originalFlowId">ID оригинального потока</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список снапшотов</returns>
    Task<List<FlowSnapshot>> GetByOriginalFlowIdAsync(Guid originalFlowId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить старые снапшоты для очистки
    /// </summary>
    /// <param name="cutoffDate">Дата, старше которой считаются снапшоты устаревшими</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список старых снапшотов</returns>
    Task<List<FlowSnapshot>> GetOldSnapshotsAsync(DateTime cutoffDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверить, используется ли снапшот активными назначениями
    /// </summary>
    /// <param name="snapshotId">ID снапшота</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True если используется</returns>
    Task<bool> HasActiveAssignmentsAsync(Guid snapshotId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить снапшот
    /// </summary>
    /// <param name="id">ID снапшота</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить последний снапшот для потока
    /// </summary>
    /// <param name="originalFlowId">ID оригинального потока</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Последний снапшот или null</returns>
    Task<FlowSnapshot?> GetLatestSnapshotAsync(Guid originalFlowId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить снапшот определенной версии
    /// </summary>
    /// <param name="originalFlowId">ID оригинального потока</param>
    /// <param name="version">Версия снапшота</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Снапшот указанной версии или null</returns>
    Task<FlowSnapshot?> GetSnapshotByVersionAsync(Guid originalFlowId, int version, CancellationToken cancellationToken = default);
}