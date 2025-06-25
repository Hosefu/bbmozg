namespace Lauf.Application.Services.Interfaces;

/// <summary>
/// Сервис для работы с текущим пользователем
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Получить идентификатор текущего пользователя
    /// </summary>
    Guid? GetCurrentUserId();

    /// <summary>
    /// Получить email текущего пользователя
    /// </summary>
    string? GetCurrentUserEmail();

    /// <summary>
    /// Получить роли текущего пользователя
    /// </summary>
    IEnumerable<string> GetCurrentUserRoles();

    /// <summary>
    /// Проверить, аутентифицирован ли пользователь
    /// </summary>
    bool IsAuthenticated();

    /// <summary>
    /// Проверить, есть ли у пользователя роль
    /// </summary>
    bool IsInRole(string role);

    /// <summary>
    /// Проверить, является ли пользователь администратором
    /// </summary>
    bool IsAdmin();

    /// <summary>
    /// Проверить, является ли пользователь бадди
    /// </summary>
    bool IsBuddy();

    /// <summary>
    /// Получить все claims пользователя
    /// </summary>
    Dictionary<string, string> GetUserClaims();
}