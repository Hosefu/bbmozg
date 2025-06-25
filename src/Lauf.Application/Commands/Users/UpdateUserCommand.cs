using MediatR;
using Lauf.Application.DTOs.Users;

namespace Lauf.Application.Commands.Users;

/// <summary>
/// Команда обновления пользователя
/// </summary>
public class UpdateUserCommand : IRequest<UpdateUserCommandResult>
{
    /// <summary>
    /// ID пользователя
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Email пользователя
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Полное имя пользователя
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// Должность пользователя
    /// </summary>
    public string? Position { get; set; }

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