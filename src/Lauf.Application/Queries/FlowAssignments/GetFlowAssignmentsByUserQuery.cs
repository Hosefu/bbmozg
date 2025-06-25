using MediatR;
using Lauf.Application.DTOs.Flows;

namespace Lauf.Application.Queries.FlowAssignments;

/// <summary>
/// Запрос получения назначений потоков по пользователю
/// </summary>
public class GetFlowAssignmentsByUserQuery : IRequest<GetFlowAssignmentsByUserQueryResult>
{
    /// <summary>
    /// ID пользователя
    /// </summary>
    public Guid UserId { get; set; }

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
/// Результат запроса получения назначений потоков по пользователю
/// </summary>
public class GetFlowAssignmentsByUserQueryResult
{
    /// <summary>
    /// Назначения пользователя
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