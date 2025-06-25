using MediatR;
using Lauf.Application.DTOs.Flows;

namespace Lauf.Application.Queries.FlowAssignments;

/// <summary>
/// Запрос получения назначений потока по ID потока
/// </summary>
public class GetFlowAssignmentsByFlowQuery : IRequest<GetFlowAssignmentsByFlowQueryResult>
{
    /// <summary>
    /// ID потока
    /// </summary>
    public Guid FlowId { get; set; }

    /// <summary>
    /// Количество пропускаемых записей
    /// </summary>
    public int Skip { get; set; } = 0;

    /// <summary>
    /// Количество записей для получения
    /// </summary>
    public int Take { get; set; } = 50;
}

/// <summary>
/// Результат запроса получения назначений потока по ID потока
/// </summary>
public class GetFlowAssignmentsByFlowQueryResult
{
    /// <summary>
    /// Назначения потока
    /// </summary>
    public IReadOnlyList<FlowAssignmentDto> Assignments { get; set; } = new List<FlowAssignmentDto>();

    /// <summary>
    /// Общее количество назначений
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