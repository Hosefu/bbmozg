using BuddyBot.Domain.Entities.Flows;
using BuddyBot.Domain.Entities.Snapshots;

namespace BuddyBot.Domain.Services;

/// <summary>
/// Интерфейс сервиса для работы со снапшотами потоков
/// </summary>
public interface IFlowSnapshotService
{
    /// <summary>
    /// Создать полный снапшот потока с все его шагами и компонентами
    /// </summary>
    /// <param name="flow">Поток для создания снапшота</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Созданный снапшот потока</returns>
    Task<FlowSnapshot> CreateFlowSnapshotAsync(Flow flow, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создать снапшот конкретного потока с определенной версией
    /// </summary>
    /// <param name="flowId">ID потока</param>
    /// <param name="version">Версия снапшота</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Созданный снапшот потока</returns>
    Task<FlowSnapshot> CreateFlowSnapshotAsync(Guid flowId, int version, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить снапшот потока по ID
    /// </summary>
    /// <param name="snapshotId">ID снапшота</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Снапшот потока или null если не найден</returns>
    Task<FlowSnapshot?> GetFlowSnapshotAsync(Guid snapshotId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить снапшот потока по назначению
    /// </summary>
    /// <param name="assignmentId">ID назначения потока</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Снапшот потока или null если не найден</returns>
    Task<FlowSnapshot?> GetFlowSnapshotByAssignmentAsync(Guid assignmentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверить целостность снапшота
    /// </summary>
    /// <param name="snapshotId">ID снапшота</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат проверки целостности</returns>
    Task<bool> ValidateSnapshotIntegrityAsync(Guid snapshotId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить различия между оригинальным потоком и снапшотом
    /// </summary>
    /// <param name="flowId">ID оригинального потока</param>
    /// <param name="snapshotId">ID снапшота</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список различий</returns>
    Task<List<string>> GetSnapshotDifferencesAsync(Guid flowId, Guid snapshotId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить устаревшие снапшоты
    /// </summary>
    /// <param name="olderThanDays">Удалить снапшоты старше указанного количества дней</param>
    /// <param name="keepMinimum">Минимальное количество снапшотов для сохранения</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Количество удаленных снапшотов</returns>
    Task<int> CleanupOldSnapshotsAsync(int olderThanDays = 365, int keepMinimum = 1, CancellationToken cancellationToken = default);
}