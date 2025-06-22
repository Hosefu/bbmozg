using BuddyBot.Domain.Entities.Users;
using BuddyBot.Domain.ValueObjects;

namespace BuddyBot.Domain.Interfaces.Repositories;

/// <summary>
/// Интерфейс репозитория пользователей
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Получает пользователя по ID
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Пользователь или null</returns>
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает пользователя по Telegram ID
    /// </summary>
    /// <param name="telegramUserId">Telegram ID пользователя</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Пользователь или null</returns>
    Task<User?> GetByTelegramIdAsync(TelegramUserId telegramUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает пользователя по email
    /// </summary>
    /// <param name="email">Email пользователя</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Пользователь или null</returns>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

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
    /// Добавляет нового пользователя
    /// </summary>
    /// <param name="user">Пользователь</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Добавленный пользователь</returns>
    Task<User> AddAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет пользователя
    /// </summary>
    /// <param name="user">Пользователь</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Обновленный пользователь</returns>
    Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет пользователя
    /// </summary>
    /// <param name="user">Пользователь</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task DeleteAsync(User user, CancellationToken cancellationToken = default);
}