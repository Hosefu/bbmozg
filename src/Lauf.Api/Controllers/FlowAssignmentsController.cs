using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Lauf.Application.Commands.FlowAssignment;
using Lauf.Application.Queries.FlowAssignments;
using Lauf.Domain.Enums;

namespace Lauf.Api.Controllers;

/// <summary>
/// Контроллер для управления назначениями потоков обучения
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FlowAssignmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<FlowAssignmentsController> _logger;

    public FlowAssignmentsController(IMediator mediator, ILogger<FlowAssignmentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Получить список назначений потоков
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<GetFlowAssignmentsQueryResult>> GetFlowAssignments(
        [FromQuery] Guid? userId = null,
        [FromQuery] Guid? flowId = null,
        [FromQuery] AssignmentStatus? status = null,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (userId.HasValue)
            {
                var userQuery = new GetFlowAssignmentsByUserQuery 
                { 
                    UserId = userId.Value, 
                    Skip = skip, 
                    Take = take 
                };
                var userResult = await _mediator.Send(userQuery, cancellationToken);
                return Ok(new GetFlowAssignmentsQueryResult { Assignments = userResult.Assignments });
            }

            if (flowId.HasValue)
            {
                var flowQuery = new GetFlowAssignmentsByFlowQuery 
                { 
                    FlowId = flowId.Value, 
                    Skip = skip, 
                    Take = take 
                };
                var flowResult = await _mediator.Send(flowQuery, cancellationToken);
                return Ok(new GetFlowAssignmentsQueryResult { Assignments = flowResult.Assignments });
            }

            var query = new GetFlowAssignmentsQuery { Skip = skip, Take = take };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении списка назначений");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    /// <summary>
    /// Получить назначение по ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetFlowAssignmentByIdQueryResult>> GetFlowAssignment(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetFlowAssignmentByIdQuery { AssignmentId = id };
            var result = await _mediator.Send(query, cancellationToken);
            
            if (result?.Assignment == null)
            {
                return NotFound($"Назначение с ID {id} не найдено");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении назначения {AssignmentId}", id);
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    /// <summary>
    /// Назначить поток пользователю
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<AssignFlowCommandResult>> AssignFlow(
        [FromBody] AssignFlowRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new AssignFlowCommand
            {
                UserId = request.UserId,
                FlowId = request.FlowId,
                Deadline = request.DueDate,
                CreatedById = request.AssignedBy ?? Guid.Empty
            };

            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(
                nameof(GetFlowAssignment), 
                new { id = result.AssignmentId }, 
                result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при назначении потока");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    /// <summary>
    /// Начать прохождение потока
    /// </summary>
    [HttpPost("{id:guid}/start")]
    public async Task<ActionResult<StartFlowCommandResult>> StartFlow(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new StartFlowCommand { AssignmentId = id };
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при начале прохождения потока {AssignmentId}", id);
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    /// <summary>
    /// Завершить прохождение потока
    /// </summary>
    [HttpPost("{id:guid}/complete")]
    public async Task<ActionResult<CompleteFlowCommandResult>> CompleteFlow(
        Guid id,
        [FromBody] CompleteFlowRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new CompleteFlowCommand 
            { 
                AssignmentId = id,
                CompletionNotes = request.CompletionNotes
            };
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при завершении прохождения потока {AssignmentId}", id);
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    /// <summary>
    /// Получить активные назначения пользователя
    /// </summary>
    [HttpGet("active/{userId:guid}")]
    public async Task<ActionResult<GetActiveAssignmentsQueryResult>> GetActiveAssignments(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetActiveAssignmentsQuery { UserId = userId };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении активных назначений пользователя {UserId}", userId);
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    /// <summary>
    /// Получить просроченные назначения
    /// </summary>
    [HttpGet("overdue")]
    public async Task<ActionResult<GetOverdueAssignmentsQueryResult>> GetOverdueAssignments(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetOverdueAssignmentsQuery();
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении просроченных назначений");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    /// <summary>
    /// Получить назначения, приближающиеся к дедлайну
    /// </summary>
    [HttpGet("upcoming-deadline")]
    public async Task<ActionResult<GetUpcomingDeadlineAssignmentsQueryResult>> GetUpcomingDeadlineAssignments(
        [FromQuery] int daysAhead = 3,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetUpcomingDeadlineAssignmentsQuery { DaysAhead = daysAhead };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении назначений с приближающимся дедлайном");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }
}

/// <summary>
/// Модели запросов для REST API
/// </summary>
public class AssignFlowRequest
{
    public Guid UserId { get; set; }
    public Guid FlowId { get; set; }
    public DateTime? DueDate { get; set; }
    public Guid? AssignedBy { get; set; }
}

public class CompleteFlowRequest
{
    public string? CompletionNotes { get; set; }
}