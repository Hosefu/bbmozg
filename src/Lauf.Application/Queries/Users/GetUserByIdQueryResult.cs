using Lauf.Application.DTOs.Users;

namespace Lauf.Application.Queries.Users;

/// <summary>
/// Результат запроса получения пользователя по ID
/// </summary>
public class GetUserByIdQueryResult
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