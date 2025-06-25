using MediatR;
using Lauf.Application.DTOs.Flows;

namespace Lauf.Application.Queries.FlowAssignments;

/// <summary>
/// Запрос получения просроченных назначений
/// </summary>
public class GetOverdueAssignmentsQuery : IRequest<GetOverdueAssignmentsQueryResult>
{
}

/// <summary>
/// Результат запроса получения просроченных назначений
/// </summary>
public class GetOverdueAssignmentsQueryResult
{
    /// <summary>
    /// Просроченные назначения
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