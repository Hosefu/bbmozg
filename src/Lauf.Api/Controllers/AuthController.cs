using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Domain.Interfaces;
using Lauf.Domain.ValueObjects;
using Lauf.Shared.Constants;

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
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IConfiguration configuration, IUserRepository userRepository, IRoleRepository roleRepository, IUnitOfWork unitOfWork, ILogger<AuthController> logger)
    {
        _configuration = configuration;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Dev-логин для разработки (только в Development окружении)
    /// Имитирует Telegram Web App авторизацию без проверки подписи
    /// </summary>
    [HttpPost("dev-login")]
    public async Task<IActionResult> DevLogin([FromBody] DevLoginRequest request)
    {
        _logger.LogInformation("Получен запрос dev-login: {@Request}", request);
        
        // Проверяем, что мы в Development окружении
        if (!Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.Equals("Development", StringComparison.OrdinalIgnoreCase) == true)
        {
            return BadRequest("Dev login is only available in Development environment");
        }

        // В dev режиме принимаем данные пользователя как есть, без проверки подписи
        var telegramUserId = new TelegramUserId(request.TelegramId);
        _logger.LogInformation("Ищем пользователя с TelegramId: {TelegramId}", telegramUserId.Value);
        
        var user = await _userRepository.GetByTelegramIdAsync(telegramUserId);
        _logger.LogInformation("Пользователь найден: {UserFound}", user != null);
        
        if (user == null)
        {
            // Создаем пользователя на основе переданных данных (как в prod)
            user = new Domain.Entities.Users.User
            {
                Id = Guid.NewGuid(),
                TelegramUserId = telegramUserId,
                FirstName = request.FirstName ?? "Пользователь",
                LastName = request.LastName ?? "",
                TelegramUsername = request.Username,
                // Language поле убрано из новой архитектуры
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // В dev режиме позволяем указать роль, иначе Employee по умолчанию
            var roleName = !string.IsNullOrEmpty(request.Role) ? request.Role : Roles.Employee;
            var role = await _roleRepository.GetByNameAsync(roleName);
            if (role != null)
            {
                user.Roles.Add(role);
                _logger.LogInformation("Пользователю назначена роль: {Role}", roleName);
            }
            else
            {
                _logger.LogWarning("Роль {Role} не найдена. Пользователь создан без роли", roleName);
            }
            
            _logger.LogInformation("Создаем нового пользователя с TelegramId: {TelegramId}", request.TelegramId);
            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Пользователь создан с Id: {UserId}", user.Id);
        }
        else
        {
            _logger.LogInformation("Обновляем существующего пользователя с Id: {UserId}", user.Id);
            // Обновляем данные существующего пользователя (как в prod)
            user.FirstName = request.FirstName ?? user.FirstName;
            user.LastName = request.LastName ?? user.LastName;
            user.TelegramUsername = request.Username ?? user.TelegramUsername;
            // Language поле убрано из новой архитектуры
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdateLastActivity();
            await _unitOfWork.SaveChangesAsync();
        }

        _logger.LogInformation("Генерируем JWT токен для пользователя: {UserId}", user.Id);
        var token = GenerateJwtToken(user);
        
        return Ok(new
        {
            token,
            user = new
            {
                id = user.Id,
                telegramId = user.TelegramUserId.Value,
                firstName = user.FirstName,
                lastName = user.LastName,
                // position поле убрано из новой архитектуры
                isActive = user.IsActive,
                roles = user.Roles.Select(r => r.Name).ToArray()
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
                FirstName = userData.FirstName ?? "Пользователь",
                LastName = userData.LastName ?? "",
                TelegramUsername = userData.Username,
                // Language поле убрано из новой архитектуры
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Назначаем роль Employee по умолчанию при первом обращении
            var employeeRole = await _roleRepository.GetByNameAsync(Roles.Employee);
            if (employeeRole != null)
            {
                user.Roles.Add(employeeRole);
                _logger.LogInformation("Новому пользователю назначена роль Employee по умолчанию");
            }
            else
            {
                _logger.LogWarning("Роль Employee не найдена в системе. Пользователь создан без роли");
            }
            
            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }
        else
        {
            // Обновляем данные существующего пользователя
            user.FirstName = userData.FirstName ?? user.FirstName;
            user.LastName = userData.LastName ?? user.LastName;
            user.TelegramUsername = userData.Username ?? user.TelegramUsername;
            // Language поле убрано из новой архитектуры
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdateLastActivity();
            await _unitOfWork.SaveChangesAsync();
        }

        var token = GenerateJwtToken(user);
        
        return Ok(new
        {
            token,
            user = new
            {
                id = user.Id,
                telegramId = user.TelegramUserId.Value,
                firstName = user.FirstName,
                lastName = user.LastName,
                // position поле убрано из новой архитектуры
                isActive = user.IsActive,
                roles = user.Roles.Select(r => r.Name).ToArray()
            }
        });
    }

    private string GenerateJwtToken(Domain.Entities.Users.User user)
    {
        var jwtSettings = _configuration.GetSection("JWT");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            jwtSettings["Key"] ?? "default-secret-key-for-development-only-32chars"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("telegram_id", user.TelegramUserId.Value.ToString()),
            new Claim("first_name", user.FirstName),
            new Claim("last_name", user.LastName),
            // position поле убрано из новой архитектуры
            new Claim("is_active", user.IsActive.ToString())
        };

        // Добавляем роли как отдельные claims
        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Name));
        }

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpireMinutes"] ?? "1440")),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

/// <summary>
/// Запрос для dev-логина (имитирует данные Telegram Web App)
/// </summary>
public record DevLoginRequest(
    long TelegramId,
    string? FirstName = null,
    string? LastName = null,
    string? Username = null,
    string? LanguageCode = null,
    string? Role = null);

/// <summary>
/// Запрос для Telegram Web App авторизации
/// </summary>
public record TelegramWebAppAuthRequest(
    string InitData);