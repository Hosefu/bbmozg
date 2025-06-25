using MediatR;
using Lauf.Application.DTOs.Users;

namespace Lauf.Application.Queries.Users;

/// <summary>
/// Запрос получения списка пользователей
/// </summary>
public class GetUsersQuery : IRequest<GetUsersQueryResult>
{
    /// <summary>
    /// Количество пропускаемых записей
    /// </summary>
    public int Skip { get; set; } = 0;

    /// <summary>
    /// Количество записей для получения
    /// </summary>
    public int Take { get; set; } = 50;

    /// <summary>
    /// Фильтр по активности
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Поисковый запрос
    /// </summary>
    public string? SearchTerm { get; set; }
}

/// <summary>
/// Результат запроса получения списка пользователей
/// </summary>
public class GetUsersQueryResult
{
    /// <summary>
    /// Список пользователей
    /// </summary>
    public IReadOnlyList<UserDto> Users { get; set; } = new List<UserDto>();

    /// <summary>
    /// Общее количество пользователей
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Успешность операции
    /// </summary>
    public bool Success { get; set; } = true;

    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public string? ErrorMessage { get; set; }
}