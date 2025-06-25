using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Domain.ValueObjects;

namespace Lauf.Api.Controllers;

/// <summary>
/// Контроллер авторизации для мини-апп
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;

    public AuthController(IConfiguration configuration, IUserRepository userRepository)
    {
        _configuration = configuration;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Dev-логин для разработки (только в Development окружении)
    /// </summary>
    [HttpPost("dev-login")]
    public async Task<IActionResult> DevLogin([FromBody] DevLoginRequest request)
    {
        // Проверяем, что мы в Development окружении
        if (!Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.Equals("Development", StringComparison.OrdinalIgnoreCase) == true)
        {
            return BadRequest("Dev login is only available in Development environment");
        }

        // Ищем пользователя по Telegram ID или создаем тестового
        var user = await _userRepository.GetByTelegramIdAsync(new TelegramUserId(request.TelegramId));
        
        if (user == null)
        {
            // Создаем тестового пользователя для разработки
            user = new Domain.Entities.Users.User
            {
                Id = Guid.NewGuid(),
                TelegramUserId = new TelegramUserId(request.TelegramId),
                Email = request.Email ?? $"dev{request.TelegramId}@test.com",
                FirstName = request.FirstName ?? "Dev",
                LastName = request.LastName ?? "User",
                Position = request.Position ?? "Developer",
                Language = "ru",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            await _userRepository.AddAsync(user);
        }

        var token = GenerateJwtToken(user);
        
        return Ok(new
        {
            token,
            user = new
            {
                id = user.Id,
                telegramId = user.TelegramUserId.Value,
                email = user.Email,
                firstName = user.FirstName,
                lastName = user.LastName,
                position = user.Position,
                isActive = user.IsActive
            }
        });
    }

    /// <summary>
    /// Telegram Web App авторизация (для продакшена)
    /// </summary>
    [HttpPost("telegram")]
    public async Task<IActionResult> TelegramAuth(
        [FromBody] TelegramWebAppAuthRequest request,
        [FromServices] Services.TelegramAuthValidator validator)
    {
        // Валидация данных от Telegram Web App
        if (!validator.ValidateInitData(request.InitData))
        {
            return Unauthorized("Invalid Telegram data signature");
        }

        // Проверка времени авторизации (не старше 24 часов)
        if (!validator.ValidateAuthDate(request.InitData))
        {
            return Unauthorized("Telegram auth data is too old");
        }

        // Извлекаем данные пользователя
        var userData = validator.ExtractUserData(request.InitData);
        if (userData == null)
        {
            return BadRequest("Cannot extract user data from Telegram");
        }

        // Ищем или создаем пользователя
        var user = await _userRepository.GetByTelegramIdAsync(new TelegramUserId(userData.Id));
        
        if (user == null)
        {
            // Создаем нового пользователя на основе данных Telegram
            user = new Domain.Entities.Users.User
            {
                Id = Guid.NewGuid(),
                TelegramUserId = new TelegramUserId(userData.Id),
                Email = $"user{userData.Id}@telegram.local", // Telegram не предоставляет email
                FirstName = userData.FirstName ?? "Пользователь",
                LastName = userData.LastName ?? "",
                TelegramUsername = userData.Username,
                Language = userData.LanguageCode ?? "ru",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            await _userRepository.AddAsync(user);
        }
        else
        {
            // Обновляем данные существующего пользователя
            user.FirstName = userData.FirstName ?? user.FirstName;
            user.LastName = userData.LastName ?? user.LastName;
            user.TelegramUsername = userData.Username ?? user.TelegramUsername;
            user.Language = userData.LanguageCode ?? user.Language;
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdateLastActivity();
        }

        var token = GenerateJwtToken(user);
        
        return Ok(new
        {
            token,
            user = new
            {
                id = user.Id,
                telegramId = user.TelegramUserId.Value,
                email = user.Email,
                firstName = user.FirstName,
                lastName = user.LastName,
                position = user.Position,
                isActive = user.IsActive
            }
        });
    }

    private string GenerateJwtToken(Domain.Entities.Users.User user)
    {
        var jwtSettings = _configuration.GetSection("JWT");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            jwtSettings["Key"] ?? "default-secret-key-for-development-only-32chars"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("telegram_id", user.TelegramUserId.Value.ToString()),
            new Claim("first_name", user.FirstName),
            new Claim("last_name", user.LastName),
            new Claim("position", user.Position ?? string.Empty),
            new Claim("is_active", user.IsActive.ToString())
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpireMinutes"] ?? "1440")),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

/// <summary>
/// Запрос для dev-логина
/// </summary>
public record DevLoginRequest(
    long TelegramId,
    string? Email = null,
    string? FirstName = null,
    string? LastName = null,
    string? Position = null);

/// <summary>
/// Запрос для Telegram Web App авторизации
/// </summary>
public record TelegramWebAppAuthRequest(
    string InitData);