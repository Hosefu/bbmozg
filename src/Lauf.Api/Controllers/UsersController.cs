using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Lauf.Application.Queries.Users;
using Lauf.Application.DTOs.Users;

namespace Lauf.Api.Controllers;

/// <summary>
/// Контроллер для управления пользователями
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IMediator mediator, ILogger<UsersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Получить пользователя по ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetUser(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetUserByIdQuery(id);
            var result = await _mediator.Send(query, cancellationToken);
            
            if (result == null)
            {
                return NotFound($"Пользователь с ID {id} не найден");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении пользователя {UserId}", id);
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    /// <summary>
    /// Получить достижения пользователя
    /// </summary>
    [HttpGet("{id:guid}/achievements")]
    public async Task<ActionResult> GetUserAchievements(
        Guid id,
        [FromQuery] string? rarity = null,
        [FromQuery] bool onlyEarned = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Domain.Enums.AchievementRarity? rarityEnum = null;
            if (!string.IsNullOrEmpty(rarity) && Enum.TryParse<Domain.Enums.AchievementRarity>(rarity, true, out var parsedRarity))
            {
                rarityEnum = parsedRarity;
            }

            var query = new GetUserAchievementsQuery(id, rarityEnum, onlyEarned);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении достижений пользователя {UserId}", id);
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    /// <summary>
    /// Получить потоки пользователя
    /// </summary>
    [HttpGet("{id:guid}/flows")]
    public async Task<ActionResult> GetUserFlows(
        Guid id,
        [FromQuery] string? status = null,
        [FromQuery] bool includeCompleted = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            Domain.Enums.AssignmentStatus? statusEnum = null;
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<Domain.Enums.AssignmentStatus>(status, true, out var parsedStatus))
            {
                statusEnum = parsedStatus;
            }

            var query = new GetUserFlowsQuery(id, statusEnum, includeCompleted);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении потоков пользователя {UserId}", id);
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    /// <summary>
    /// Получить прогресс пользователя
    /// </summary>
    [HttpGet("{id:guid}/progress")]
    public async Task<ActionResult> GetUserProgress(
        Guid id,
        [FromQuery] Guid? assignmentId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetUserProgressQuery(id, assignmentId);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении прогресса пользователя {UserId}", id);
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }
}