using MediatR;
using Lauf.Application.DTOs.Flows;

namespace Lauf.Application.Queries.FlowAssignments;

/// <summary>
/// Запрос получения активных назначений пользователя
/// </summary>
public class GetActiveAssignmentsQuery : IRequest<GetActiveAssignmentsQueryResult>
{
    /// <summary>
    /// ID пользователя
    /// </summary>
    public Guid UserId { get; set; }
}

/// <summary>
/// Результат запроса получения активных назначений пользователя
/// </summary>
public class GetActiveAssignmentsQueryResult
{
    /// <summary>
    /// Активные назначения
    /// </summary>
    public IReadOnlyList<FlowAssignmentDto> Assignments { get; set; } = new List<FlowAssignmentDto>();

    /// <summary>
    /// Успешность операции
    /// </summary>
    public bool Success { get; set; } = true;

    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public string? ErrorMessage { get; set; }
}