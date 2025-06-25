using Microsoft.AspNetCore.Mvc;
using Lauf.Api.Services;

namespace Lauf.Api.Controllers;

/// <summary>
/// Контроллер для тестирования в Development окружении
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly TelegramAuthValidator _telegramValidator;

    public TestController(TelegramAuthValidator telegramValidator)
    {
        _telegramValidator = telegramValidator;
    }

    /// <summary>
    /// Тестирование валидации Telegram данных (только Development)
    /// </summary>
    [HttpPost("telegram-validate")]
    public IActionResult TestTelegramValidation([FromBody] TestTelegramRequest request)
    {
        if (!Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.Equals("Development", StringComparison.OrdinalIgnoreCase) == true)
        {
            return BadRequest("Test endpoints are only available in Development environment");
        }

        var isValid = _telegramValidator.ValidateInitData(request.InitData);
        var isDateValid = _telegramValidator.ValidateAuthDate(request.InitData);
        var userData = _telegramValidator.ExtractUserData(request.InitData);

        return Ok(new
        {
            isSignatureValid = isValid,
            isAuthDateValid = isDateValid,
            userData = userData,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Генерирует тестовые данные для проверки (только Development)
    /// </summary>
    [HttpGet("telegram-sample")]
    public IActionResult GenerateTelegramSample()
    {
        if (!Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.Equals("Development", StringComparison.OrdinalIgnoreCase) == true)
        {
            return BadRequest("Test endpoints are only available in Development environment");
        }

        var sampleData = new
        {
            example = "Пример данных для тестирования Telegram Web App",
            structure = new
            {
                user = @"{""id"":123456789,""first_name"":""Test"",""last_name"":""User"",""username"":""testuser"",""language_code"":""ru""}",
                auth_date = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                hash = "computed_hash_value"
            },
            note = "В реальном приложении эти данные приходят от Telegram Web App",
            validation_steps = new[]
            {
                "1. Telegram Web App отправляет initData",
                "2. Сервер проверяет подпись через HMAC-SHA256",
                "3. Сервер проверяет актуальность по auth_date",
                "4. Сервер извлекает пользовательские данные",
                "5. Сервер создает/обновляет пользователя",
                "6. Сервер выдает JWT токен"
            }
        };

        return Ok(sampleData);
    }
}

/// <summary>
/// Запрос для тестирования Telegram валидации
/// </summary>
public record TestTelegramRequest(string InitData);