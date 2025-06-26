using Lauf.Application.Services.Interfaces;

namespace Lauf.Api.Services;

/// <summary>
/// Временная заглушка для ICurrentUserService
/// Будет заменена на реальную реализацию в этапе 8
/// </summary>
public class MockCurrentUserService : ICurrentUserService
{
    /// <summary>
    /// Получить ID текущего пользователя
    /// </summary>
    public Guid? GetCurrentUserId()
    {
        // Возвращаем статический GUID для тестирования
        return Guid.Parse("11111111-1111-1111-1111-111111111111");
    }

    /// <summary>
    /// Получить Telegram ID текущего пользователя
    /// </summary>
    public long? GetCurrentUserTelegramId()
    {
        return 123456789;
    }

    /// <summary>
    /// Получить роли текущего пользователя
    /// </summary>
    public IEnumerable<string> GetCurrentUserRoles()
    {
        return new[] { "Admin" };
    }

    /// <summary>
    /// Проверить, авторизован ли пользователь
    /// </summary>
    public bool IsAuthenticated()
    {
        return true;
    }

    /// <summary>
    /// Проверить, имеет ли пользователь указанную роль
    /// </summary>
    public bool IsInRole(string role)
    {
        return GetCurrentUserRoles().Contains(role, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Получить все claims пользователя
    /// </summary>
    public IDictionary<string, string> GetUserClaims()
    {
        return new Dictionary<string, string>
        {
            { "sub", GetCurrentUserId()?.ToString() ?? "" },
            { "telegram_id", GetCurrentUserTelegramId()?.ToString() ?? "" },
            { "role", "Admin" }
        };
    }
}