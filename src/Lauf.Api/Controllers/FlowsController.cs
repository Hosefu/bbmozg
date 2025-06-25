using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Lauf.Application.Commands.FlowManagement;
using Lauf.Application.Queries.Flows;
using Lauf.Application.DTOs.Flows;

namespace Lauf.Api.Controllers;

/// <summary>
/// Контроллер для управления потоками обучения
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FlowsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<FlowsController> _logger;

    public FlowsController(IMediator mediator, ILogger<FlowsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Получить поток обучения по ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FlowDto>> GetFlow(
        Guid id,
        [FromQuery] bool includeSteps = true,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetFlowByIdQuery(id, includeSteps);
            var result = await _mediator.Send(query, cancellationToken);
            
            if (result == null)
            {
                return NotFound($"Поток с ID {id} не найден");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении потока {FlowId}", id);
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    /// <summary>
    /// Получить детальную информацию о потоке
    /// </summary>
    [HttpGet("{id:guid}/details")]
    public async Task<ActionResult> GetFlowDetails(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetFlowDetailsQuery(id, null);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении деталей потока {FlowId}", id);
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    /// <summary>
    /// Получить доступные потоки
    /// </summary>
    [HttpGet("available")]
    public async Task<ActionResult> GetAvailableFlows(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetAvailableFlowsQuery();
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении доступных потоков");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    /// <summary>
    /// Получить статистику потока
    /// </summary>
    [HttpGet("{id:guid}/stats")]
    public async Task<ActionResult> GetFlowStats(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetFlowStatsQuery(id);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении статистики потока {FlowId}", id);
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    /// <summary>
    /// Создать новый поток обучения
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CreateFlowCommandResult>> CreateFlow(
        [FromBody] CreateFlowRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new CreateFlowCommand
            {
                Title = request.Title,
                Description = request.Description,
                Category = request.Category ?? "Общее",
                Tags = request.Tags ?? "",
                Priority = request.Priority,
                IsRequired = request.IsRequired,
                CreatedById = GetCurrentUserId(), // Нужно реализовать получение текущего пользователя
                Settings = request.Settings != null ? new CreateFlowSettingsCommand
                {
                    // Здесь нужно будет маппить настройки
                } : null
            };

            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(
                nameof(GetFlow), 
                new { id = result.FlowId }, 
                result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании потока");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    private Guid GetCurrentUserId()
    {
        // Здесь нужно реализовать получение ID текущего пользователя из токена/контекста
        // Временно возвращаем пустой GUID
        return Guid.Empty;
    }
}

/// <summary>
/// Модели запросов для REST API
/// </summary>
public class CreateFlowRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? Tags { get; set; }
    public int Priority { get; set; } = 0;
    public bool IsRequired { get; set; } = false;
    public CreateFlowSettingsRequest? Settings { get; set; }
}

public class CreateFlowSettingsRequest
{
    // Здесь будут настройки потока
}