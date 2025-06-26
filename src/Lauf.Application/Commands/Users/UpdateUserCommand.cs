using MediatR;
using Lauf.Application.DTOs.Users;

namespace Lauf.Application.Commands.Users;

/// <summary>
/// Команда обновления данных пользователя
/// </summary>
public class UpdateUserCommand : IRequest<UserDto>
{
    /// <summary>
    /// Идентификатор пользователя для обновления
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Фамилия пользователя
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Telegram ID пользователя
    /// </summary>
    public long? TelegramUserId { get; set; }

    /// <summary>
    /// Список ID ролей для назначения пользователю
    /// </summary>
    public List<Guid>? RoleIds { get; set; }

    /// <summary>
    /// Активность пользователя
    /// </summary>
    public bool? IsActive { get; set; }
}

/// <summary>
/// Результат команды обновления пользователя
/// </summary>
public class UpdateUserCommandResult
{
    /// <summary>
    /// Обновленный пользователь
    /// </summary>
    public UserDto User { get; set; } = null!;

    /// <summary>
    /// Успешность операции
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public string? ErrorMessage { get; set; }
}