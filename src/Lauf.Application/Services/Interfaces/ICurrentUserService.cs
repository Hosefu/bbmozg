namespace Lauf.Application.Services.Interfaces;

/// <summary>
/// Интерфейс для получения информации о текущем пользователе
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Получить ID текущего пользователя
    /// </summary>
    Guid? GetCurrentUserId();

    /// <summary>
    /// Получить Telegram ID текущего пользователя
    /// </summary>
    long? GetCurrentUserTelegramId();

    /// <summary>
    /// Получить роли текущего пользователя
    /// </summary>
    IEnumerable<string> GetCurrentUserRoles();

    /// <summary>
    /// Проверить, авторизован ли пользователь
    /// </summary>
    bool IsAuthenticated();

    /// <summary>
    /// Проверить, имеет ли пользователь указанную роль
    /// </summary>
    bool IsInRole(string role);



    /// <summary>
    /// Получить все claims пользователя
    /// </summary>
    IDictionary<string, string> GetUserClaims();
}