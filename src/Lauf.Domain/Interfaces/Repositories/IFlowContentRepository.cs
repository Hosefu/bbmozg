using Lauf.Domain.Entities.Flows;

namespace Lauf.Domain.Interfaces.Repositories;

/// <summary>
/// Интерфейс репозитория содержимого потоков
/// </summary>
public interface IFlowContentRepository
{
    /// <summary>
    /// Получает содержимое потока по ID
    /// </summary>
    /// <param name="id">Идентификатор содержимого</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Содержимое потока или null</returns>
    Task<FlowContent?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает содержимое потока со всеми шагами
    /// </summary>
    /// <param name="id">Идентификатор содержимого</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Содержимое со шагами или null</returns>
    Task<FlowContent?> GetWithStepsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает активное содержимое потока
    /// </summary>
    /// <param name="flowId">Идентификатор потока</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Активное содержимое или null</returns>
    Task<FlowContent?> GetActiveByFlowIdAsync(Guid flowId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает все версии содержимого потока
    /// </summary>
    /// <param name="flowId">Идентификатор потока</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список всех версий</returns>
    Task<IReadOnlyList<FlowContent>> GetVersionsByFlowIdAsync(Guid flowId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает содержимое потока по версии
    /// </summary>
    /// <param name="flowId">Идентификатор потока</param>
    /// <param name="version">Номер версии</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Содержимое указанной версии или null</returns>
    Task<FlowContent?> GetByFlowIdAndVersionAsync(Guid flowId, int version, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает последнюю версию содержимого
    /// </summary>
    /// <param name="flowId">Идентификатор потока</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Последняя версия содержимого или null</returns>
    Task<FlowContent?> GetLatestVersionAsync(Guid flowId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавляет новое содержимое потока
    /// </summary>
    /// <param name="content">Содержимое</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Добавленное содержимое</returns>
    Task<FlowContent> AddAsync(FlowContent content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет содержимое потока
    /// </summary>
    /// <param name="content">Содержимое</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Обновленное содержимое</returns>
    Task<FlowContent> UpdateAsync(FlowContent content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет содержимое потока
    /// </summary>
    /// <param name="content">Содержимое</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task DeleteAsync(FlowContent content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создает новую версию содержимого
    /// </summary>
    /// <param name="flowId">Идентификатор потока</param>
    /// <param name="createdBy">Создатель версии</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Новая версия содержимого</returns>
    Task<FlowContent> CreateNewVersionAsync(Guid flowId, Guid createdBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает количество версий содержимого
    /// </summary>
    /// <param name="flowId">Идентификатор потока</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Количество версий</returns>
    Task<int> GetVersionCountAsync(Guid flowId, CancellationToken cancellationToken = default);
}