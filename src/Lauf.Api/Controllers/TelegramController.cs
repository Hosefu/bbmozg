using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using Lauf.Infrastructure.ExternalServices.TelegramBot;
using Microsoft.AspNetCore.Authorization;

namespace Lauf.Api.Controllers;

/// <summary>
/// Контроллер для обработки Telegram Bot webhook'ов
/// </summary>
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class TelegramController : ControllerBase
{
    private readonly ILogger<TelegramController> _logger;
    private readonly TelegramWebhookHandler _webhookHandler;

    public TelegramController(
        ILogger<TelegramController> logger,
        TelegramWebhookHandler webhookHandler)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _webhookHandler = webhookHandler ?? throw new ArgumentNullException(nameof(webhookHandler));
    }

    /// <summary>
    /// Webhook endpoint для получения обновлений от Telegram
    /// </summary>
    /// <param name="update">Обновление от Telegram Bot API</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Результат обработки</returns>
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook([FromBody] Update update, CancellationToken cancellationToken = default)
    {
        try
        {
            if (update == null)
            {
                _logger.LogWarning("Получен пустой webhook от Telegram");
                return BadRequest("Update object is null");
            }

            _logger.LogInformation("Получен webhook от Telegram. Update ID: {UpdateId}, Type: {UpdateType}", 
                update.Id, update.Type);

            // Обрабатываем обновление через webhook handler
            await _webhookHandler.HandleUpdateAsync(update, cancellationToken);

            _logger.LogInformation("Webhook успешно обработан. Update ID: {UpdateId}", update.Id);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка обработки Telegram webhook. Update ID: {UpdateId}", update?.Id);
            
            // Возвращаем 200, чтобы Telegram не повторял отправку
            return Ok();
        }
    }

    /// <summary>
    /// Endpoint для проверки статуса webhook'а
    /// </summary>
    /// <returns>Статус webhook'а</returns>
    [HttpGet("webhook/status")]
    public IActionResult GetWebhookStatus()
    {
        return Ok(new
        {
            Status = "Active",
            Timestamp = DateTime.UtcNow,
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
        });
    }

    /// <summary>
    /// Endpoint для тестирования webhook'а в development
    /// </summary>
    /// <param name="testMessage">Тестовое сообщение</param>
    /// <returns>Результат теста</returns>
    [HttpPost("webhook/test")]
    public IActionResult TestWebhook([FromBody] string testMessage)
    {
        _logger.LogInformation("Тестовый webhook вызов: {TestMessage}", testMessage);
        
        return Ok(new
        {
            Message = "Webhook тест прошел успешно",
            ReceivedMessage = testMessage,
            Timestamp = DateTime.UtcNow
        });
    }
} 