using MediatR;
using Lauf.Application.DTOs.Users;

namespace Lauf.Application.Commands.Users;

/// <summary>
/// Команда создания пользователя
/// </summary>
public class CreateUserCommand : IRequest<UserDto>
{
    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Фамилия пользователя
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Email пользователя
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Должность пользователя
    /// </summary>
    public string? Position { get; set; }

    /// <summary>
    /// Telegram ID пользователя
    /// </summary>
    public long TelegramUserId { get; set; }

    /// <summary>
    /// Язык интерфейса
    /// </summary>
    public string Language { get; set; } = "ru";
}

