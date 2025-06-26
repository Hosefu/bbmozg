using Lauf.Domain.Entities.Users;
using Lauf.Domain.ValueObjects;

namespace Lauf.Domain.Interfaces.Repositories;

/// <summary>
/// Интерфейс репозитория пользователей
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Получает пользователя по его ID
    /// </summary>
    /// <param name="id">ID пользователя</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Пользователь или null</returns>
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает пользователя по его Telegram ID
    /// </summary>
    /// <param name="telegramUserId">Telegram ID пользователя</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Пользователь или null, если не найден</returns>
    Task<User?> GetByTelegramUserIdAsync(long telegramUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает список пользователей с пагинацией
    /// </summary>
    /// <param name="pageNumber">Номер страницы (начиная с 1)</param>
    /// <param name="pageSize">Размер страницы</param>
    /// <param name="searchTerm">Поисковый запрос (опционально)</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список пользователей</returns>
    Task<IList<User>> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает общее количество пользователей
    /// </summary>
    /// <param name="searchTerm">Поисковый запрос (опционально)</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Общее количество пользователей</returns>
    Task<int> GetTotalCountAsync(string? searchTerm = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает пользователей по списку ID
    /// </summary>
    /// <param name="userIds">Список ID пользователей</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список пользователей</returns>
    Task<IList<User>> GetByIdsAsync(IEnumerable<Guid> userIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавляет пользователя
    /// </summary>
    /// <param name="user">Пользователь для добавления</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task AddAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет пользователя
    /// </summary>
    /// <param name="user">Пользователь для обновления</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет пользователя
    /// </summary>
    /// <param name="user">Пользователь для удаления</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task DeleteAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает пользователя по Telegram ID
    /// </summary>
    /// <param name="telegramUserId">Telegram ID пользователя</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Пользователь или null</returns>
    Task<User?> GetByTelegramIdAsync(TelegramUserId telegramUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает пользователя с ролями
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Пользователь с ролями или null</returns>
    Task<User?> GetWithRolesAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает список пользователей с указанной ролью
    /// </summary>
    /// <param name="roleName">Название роли</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список пользователей</returns>
    Task<IReadOnlyList<User>> GetByRoleAsync(string roleName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает список активных пользователей
    /// </summary>
    /// <param name="skip">Количество пропускаемых записей</param>
    /// <param name="take">Количество возвращаемых записей</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список пользователей</returns>
    Task<IReadOnlyList<User>> GetActiveUsersAsync(int skip = 0, int take = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает количество активных пользователей
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Количество пользователей</returns>
    Task<int> GetActiveUsersCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет существование пользователя по Telegram ID
    /// </summary>
    /// <param name="telegramUserId">Telegram ID пользователя</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>true, если пользователь существует</returns>
    Task<bool> ExistsByTelegramIdAsync(TelegramUserId telegramUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает всех пользователей
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список всех пользователей</returns>
    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default);
}