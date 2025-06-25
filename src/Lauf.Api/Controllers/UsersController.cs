using Microsoft.AspNetCore.Mvc;
using MediatR;
using Lauf.Application.Commands.Users;
using Lauf.Application.Queries.Users;
using Lauf.Application.DTOs.Users;

namespace Lauf.Api.Controllers;

/// <summary>
/// REST API контроллер для управления пользователями
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IMediator mediator, ILogger<UsersController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Получить пользователя по ID
    /// </summary>
    /// <param name="id">ID пользователя</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Данные пользователя</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id, CancellationToken cancellationToken = default)
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
            _logger.LogError(ex, "Ошибка получения пользователя с ID {UserId}", id);
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    /// <summary>
    /// Создать нового пользователя
    /// </summary>
    /// <param name="command">Команда создания пользователя</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Созданный пользователь</returns>
    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка создания пользователя");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    /// <summary>
    /// Обновить данные пользователя
    /// </summary>
    /// <param name="id">ID пользователя</param>
    /// <param name="command">Команда обновления пользователя</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Обновленный пользователь</returns>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserDto>> Update(Guid id, [FromBody] UpdateUserCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            command.UserId = id;
            var result = await _mediator.Send(command, cancellationToken);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка обновления пользователя с ID {UserId}", id);
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }
}