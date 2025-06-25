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
    /// Получить email текущего пользователя
    /// </summary>
    public string? GetCurrentUserEmail()
    {
        return "test@example.com";
    }

    /// <summary>
    /// Получить роли текущего пользователя
    /// </summary>
    public IEnumerable<string> GetCurrentUserRoles()
    {
        return new[] { "Admin" };
    }

    /// <summary>
    /// Проверить, аутентифицирован ли пользователь
    /// </summary>
    public bool IsAuthenticated()
    {
        return true;
    }

    /// <summary>
    /// Проверить, есть ли у пользователя определенная роль
    /// </summary>
    public bool IsInRole(string role)
    {
        return GetCurrentUserRoles().Contains(role, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Проверить, является ли пользователь администратором
    /// </summary>
    public bool IsAdmin()
    {
        return IsInRole("Admin");
    }

    /// <summary>
    /// Проверить, является ли пользователь бадди
    /// </summary>
    public bool IsBuddy()
    {
        return IsInRole("Buddy");
    }

    /// <summary>
    /// Получить все claims пользователя
    /// </summary>
    public Dictionary<string, string> GetUserClaims()
    {
        return new Dictionary<string, string>
        {
            { "sub", GetCurrentUserId()?.ToString() ?? "" },
            { "email", GetCurrentUserEmail() ?? "" },
            { "role", "Admin" }
        };
    }
}