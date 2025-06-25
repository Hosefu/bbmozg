using MediatR;
using Lauf.Application.DTOs.Flows;

namespace Lauf.Application.Queries.FlowAssignments;

/// <summary>
/// Запрос получения назначений с приближающимся дедлайном
/// </summary>
public class GetUpcomingDeadlineAssignmentsQuery : IRequest<GetUpcomingDeadlineAssignmentsQueryResult>
{
    /// <summary>
    /// Количество дней до дедлайна
    /// </summary>
    public int DaysAhead { get; set; } = 3;
}

/// <summary>
/// Результат запроса получения назначений с приближающимся дедлайном
/// </summary>
public class GetUpcomingDeadlineAssignmentsQueryResult
{
    /// <summary>
    /// Назначения с приближающимся дедлайном
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