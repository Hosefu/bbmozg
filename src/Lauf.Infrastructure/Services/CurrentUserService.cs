using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Lauf.Application.Services.Interfaces;

namespace Lauf.Infrastructure.Services;

/// <summary>
/// Сервис для работы с текущим пользователем
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Получить идентификатор текущего пользователя
    /// </summary>
    public Guid? GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;

        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }

        return null;
    }

    /// <summary>
    /// Получить email текущего пользователя
    /// </summary>
    public string? GetCurrentUserEmail()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value
               ?? _httpContextAccessor.HttpContext?.User?.FindFirst("email")?.Value;
    }

    /// <summary>
    /// Получить роли текущего пользователя
    /// </summary>
    public IEnumerable<string> GetCurrentUserRoles()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null)
        {
            return Enumerable.Empty<string>();
        }

        return user.FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .Concat(user.FindAll("role").Select(c => c.Value))
            .Distinct();
    }

    /// <summary>
    /// Проверить, аутентифицирован ли пользователь
    /// </summary>
    public bool IsAuthenticated()
    {
        return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;
    }

    /// <summary>
    /// Проверить, есть ли у пользователя роль
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
        return IsInRole("Admin") || IsInRole("Administrator");
    }

    /// <summary>
    /// Проверить, является ли пользователь бадди
    /// </summary>
    public bool IsBuddy()
    {
        return IsInRole("Buddy") || IsInRole("Mentor");
    }

    /// <summary>
    /// Получить все claims пользователя
    /// </summary>
    public Dictionary<string, string> GetUserClaims()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null)
        {
            return new Dictionary<string, string>();
        }

        return user.Claims.ToDictionary(c => c.Type, c => c.Value);
    }
}