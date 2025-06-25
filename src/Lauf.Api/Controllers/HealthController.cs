using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lauf.Infrastructure.Persistence;

namespace Lauf.Api.Controllers;

/// <summary>
/// Контроллер для проверки состояния приложения
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HealthController> _logger;

    public HealthController(ApplicationDbContext context, ILogger<HealthController> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Базовая проверка состояния приложения
    /// </summary>
    /// <returns>Статус приложения</returns>
    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            var response = new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = GetType().Assembly.GetName().Version?.ToString() ?? "Unknown",
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при проверке состояния приложения");
            return StatusCode(500, new { Status = "Unhealthy", Error = ex.Message });
        }
    }

    /// <summary>
    /// Детальная проверка состояния приложения с проверкой БД
    /// </summary>
    /// <returns>Детальный статус приложения</returns>
    [HttpGet("detailed")]
    public async Task<IActionResult> GetDetailed()
    {
        var checks = new List<object>();

        // Проверка базы данных
        try
        {
            var canConnect = await _context.Database.CanConnectAsync();
            checks.Add(new
            {
                Component = "Database",
                Status = canConnect ? "Healthy" : "Unhealthy",
                Message = canConnect ? "Соединение с БД установлено" : "Не удалось подключиться к БД"
            });
        }
        catch (Exception ex)
        {
            checks.Add(new
            {
                Component = "Database",
                Status = "Unhealthy",
                Message = $"Ошибка подключения к БД: {ex.Message}"
            });
        }

        // Проверка памяти
        var memoryBefore = GC.GetTotalMemory(false);
        GC.Collect();
        GC.WaitForPendingFinalizers();
        var memoryAfter = GC.GetTotalMemory(true);

        checks.Add(new
        {
            Component = "Memory",
            Status = "Healthy",
            Message = $"Используется {memoryAfter / 1024 / 1024} MB памяти"
        });

        var overallStatus = checks.All(c => c.GetType().GetProperty("Status")?.GetValue(c)?.ToString() == "Healthy") 
            ? "Healthy" : "Degraded";

        var response = new
        {
            Status = overallStatus,
            Timestamp = DateTime.UtcNow,
            Version = GetType().Assembly.GetName().Version?.ToString() ?? "Unknown",
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
            Checks = checks
        };

        return Ok(response);
    }

    /// <summary>
    /// Проверка готовности приложения
    /// </summary>
    /// <returns>Статус готовности</returns>
    [HttpGet("ready")]
    public async Task<IActionResult> Ready()
    {
        try
        {
            // Проверяем критичные компоненты
            var canConnectToDb = await _context.Database.CanConnectAsync();
            
            if (canConnectToDb)
            {
                return Ok(new { Status = "Ready", Timestamp = DateTime.UtcNow });
            }
            else
            {
                return StatusCode(503, new { Status = "Not Ready", Reason = "Database connection failed" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при проверке готовности приложения");
            return StatusCode(503, new { Status = "Not Ready", Reason = ex.Message });
        }
    }

    /// <summary>
    /// Проверка активности приложения
    /// </summary>
    /// <returns>Статус активности</returns>
    [HttpGet("live")]
    public IActionResult Live()
    {
        // Базовая проверка - если приложение может отвечать на запросы, значит оно живо
        return Ok(new { Status = "Alive", Timestamp = DateTime.UtcNow });
    }
}