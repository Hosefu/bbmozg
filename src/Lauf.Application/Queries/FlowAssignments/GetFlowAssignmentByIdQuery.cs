using MediatR;
using Lauf.Application.DTOs.Flows;

namespace Lauf.Application.Queries.FlowAssignments;

/// <summary>
/// Запрос получения назначения потока по ID
/// </summary>
public class GetFlowAssignmentByIdQuery : IRequest<GetFlowAssignmentByIdQueryResult>
{
    /// <summary>
    /// ID назначения
    /// </summary>
    public Guid AssignmentId { get; set; }
}

/// <summary>
/// Результат запроса получения назначения потока по ID
/// </summary>
public class GetFlowAssignmentByIdQueryResult
{
    /// <summary>
    /// Назначение потока
    /// </summary>
    public FlowAssignmentDto? Assignment { get; set; }

    /// <summary>
    /// Успешность операции
    /// </summary>
    public bool Success { get; set; } = true;

    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public string? ErrorMessage { get; set; }
}