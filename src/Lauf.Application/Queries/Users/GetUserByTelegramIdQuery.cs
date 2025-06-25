using MediatR;
using Lauf.Application.DTOs.Users;

namespace Lauf.Application.Queries.Users;

/// <summary>
/// Запрос получения пользователя по Telegram ID
/// </summary>
public class GetUserByTelegramIdQuery : IRequest<GetUserByTelegramIdQueryResult>
{
    /// <summary>
    /// Telegram ID пользователя
    /// </summary>
    public long TelegramId { get; set; }
}

/// <summary>
/// Результат запроса получения пользователя по Telegram ID
/// </summary>
public class GetUserByTelegramIdQueryResult
{
    /// <summary>
    /// Пользователь
    /// </summary>
    public UserDto? User { get; set; }

    /// <summary>
    /// Успешность операции
    /// </summary>
    public bool Success { get; set; } = true;

    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public string? ErrorMessage { get; set; }
}