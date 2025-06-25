using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Enums;

namespace Lauf.Domain.Interfaces.Repositories;

/// <summary>
/// Интерфейс репозитория назначений потоков
/// </summary>
public interface IFlowAssignmentRepository
{
    /// <summary>
    /// Получает назначение по ID
    /// </summary>
    /// <param name="id">Идентификатор назначения</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Назначение или null</returns>
    Task<FlowAssignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает назначение со всеми связанными данными
    /// </summary>
    /// <param name="id">Идентификатор назначения</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Назначение с данными или null</returns>
    Task<FlowAssignment?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает назначение по пользователю и потоку
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="flowId">Идентификатор потока</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Назначение или null</returns>
    Task<FlowAssignment?> GetByUserAndFlowAsync(Guid userId, Guid flowId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает назначения пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="skip">Количество пропускаемых записей</param>
    /// <param name="take">Количество возвращаемых записей</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список назначений</returns>
    Task<IReadOnlyList<FlowAssignment>> GetByUserAsync(Guid userId, int skip = 0, int take = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает назначения пользователя (все)
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список назначений</returns>
    Task<IEnumerable<FlowAssignment>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает назначения по потоку
    /// </summary>
    /// <param name="flowId">Идентификатор потока</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список назначений</returns>
    Task<IEnumerable<FlowAssignment>> GetByFlowIdAsync(Guid flowId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает назначения по статусу
    /// </summary>
    /// <param name="status">Статус назначения</param>
    /// <param name="skip">Количество пропускаемых записей</param>
    /// <param name="take">Количество возвращаемых записей</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список назначений</returns>
    Task<IReadOnlyList<FlowAssignment>> GetByStatusAsync(AssignmentStatus status, int skip = 0, int take = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает назначения с истекающим дедлайном
    /// </summary>
    /// <param name="daysThreshold">Количество дней до дедлайна</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список назначений</returns>
    Task<IReadOnlyList<FlowAssignment>> GetWithApproachingDeadlineAsync(int daysThreshold, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает просроченные назначения
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список просроченных назначений</returns>
    Task<IReadOnlyList<FlowAssignment>> GetOverdueAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает назначения по бадди
    /// </summary>
    /// <param name="buddyId">Идентификатор бадди</param>
    /// <param name="skip">Количество пропускаемых записей</param>
    /// <param name="take">Количество возвращаемых записей</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список назначений</returns>
    Task<IReadOnlyList<FlowAssignment>> GetByBuddyAsync(Guid buddyId, int skip = 0, int take = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает количество назначений по статусу
    /// </summary>
    /// <param name="status">Статус назначения</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Количество назначений</returns>
    Task<int> CountByStatusAsync(AssignmentStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет существование назначения
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="flowId">Идентификатор потока</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>true, если назначение существует</returns>
    Task<bool> ExistsAsync(Guid userId, Guid flowId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет наличие активного назначения
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="flowId">Идентификатор потока</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>true, если есть активное назначение</returns>
    Task<bool> HasActiveAssignmentAsync(Guid userId, Guid flowId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавляет новое назначение
    /// </summary>
    /// <param name="assignment">Назначение</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Добавленное назначение</returns>
    Task<FlowAssignment> AddAsync(FlowAssignment assignment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет назначение
    /// </summary>
    /// <param name="assignment">Назначение</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Обновленное назначение</returns>
    Task<FlowAssignment> UpdateAsync(FlowAssignment assignment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет назначение
    /// </summary>
    /// <param name="assignment">Назначение</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task DeleteAsync(FlowAssignment assignment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает все назначения
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список всех назначений</returns>
    Task<IEnumerable<FlowAssignment>> GetAllAsync(CancellationToken cancellationToken = default);
}