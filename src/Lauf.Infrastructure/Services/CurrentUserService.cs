using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Lauf.Application.Services.Interfaces;

namespace Lauf.Infrastructure.Services;

/// <summary>
/// Сервис для получения информации о текущем пользователе
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Получить ID текущего пользователя
    /// </summary>
    public Guid? GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;

        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    /// <summary>
    /// Получить Telegram ID текущего пользователя
    /// </summary>
    public long? GetCurrentUserTelegramId()
    {
        var telegramIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("telegram_id")?.Value;
        return long.TryParse(telegramIdClaim, out var telegramId) ? telegramId : null;
    }

    /// <summary>
    /// Получить роли текущего пользователя
    /// </summary>
    public IEnumerable<string> GetCurrentUserRoles()
    {
        return _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role)?.Select(c => c.Value) ?? Enumerable.Empty<string>();
    }

    /// <summary>
    /// Проверить, авторизован ли пользователь
    /// </summary>
    public bool IsAuthenticated()
    {
        return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }

    /// <summary>
    /// Проверить, имеет ли пользователь указанную роль
    /// </summary>
    public bool IsInRole(string role)
    {
        return _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
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
    public IDictionary<string, string> GetUserClaims()
    {
        var claims = new Dictionary<string, string>();
        
        if (_httpContextAccessor.HttpContext?.User?.Claims != null)
        {
            foreach (var claim in _httpContextAccessor.HttpContext.User.Claims)
            {
                claims[claim.Type] = claim.Value;
            }
        }

        return claims;
    }
}