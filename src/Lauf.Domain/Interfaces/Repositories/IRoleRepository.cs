using Lauf.Domain.Entities.Users;

namespace Lauf.Domain.Interfaces.Repositories;

/// <summary>
/// Интерфейс репозитория ролей
/// </summary>
public interface IRoleRepository
{
    /// <summary>
    /// Получает роль по ID
    /// </summary>
    /// <param name="id">Идентификатор роли</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Роль или null</returns>
    Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает роль по имени
    /// </summary>
    /// <param name="name">Имя роли</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Роль или null</returns>
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает все активные роли
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список ролей</returns>
    Task<IReadOnlyList<Role>> GetAllActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает все роли
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список ролей</returns>
    Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает роли по списку ID
    /// </summary>
    /// <param name="roleIds">Список ID ролей</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список ролей</returns>
    Task<IList<Role>> GetByIdsAsync(IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавляет новую роль
    /// </summary>
    /// <param name="role">Роль</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Добавленная роль</returns>
    Task<Role> AddAsync(Role role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет роль
    /// </summary>
    /// <param name="role">Роль</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Обновленная роль</returns>
    Task<Role> UpdateAsync(Role role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет роль
    /// </summary>
    /// <param name="role">Роль</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task DeleteAsync(Role role, CancellationToken cancellationToken = default);
} 