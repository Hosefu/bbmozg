using MediatR;
using Lauf.Application.DTOs.Users;

namespace Lauf.Application.Commands.Users;

/// <summary>
/// Команда создания пользователя
/// </summary>
public class CreateUserCommand : IRequest<CreateUserCommandResult>
{
    /// <summary>
    /// Telegram ID пользователя
    /// </summary>
    public long TelegramId { get; set; }

    /// <summary>
    /// Email пользователя
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Полное имя пользователя
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Должность пользователя
    /// </summary>
    public string? Position { get; set; }
}

/// <summary>
/// Результат команды создания пользователя
/// </summary>
public class CreateUserCommandResult
{
    /// <summary>
    /// Созданный пользователь
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