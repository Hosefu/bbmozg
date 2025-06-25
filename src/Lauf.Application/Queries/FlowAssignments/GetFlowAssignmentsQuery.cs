using MediatR;
using Lauf.Application.DTOs.Flows;
using Lauf.Domain.Enums;

namespace Lauf.Application.Queries.FlowAssignments;

/// <summary>
/// Запрос получения списка назначений потоков
/// </summary>
public class GetFlowAssignmentsQuery : IRequest<GetFlowAssignmentsQueryResult>
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
    /// Фильтр по статусу
    /// </summary>
    public AssignmentStatus? Status { get; set; }

    /// <summary>
    /// Фильтр по пользователю
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Фильтр по потоку
    /// </summary>
    public Guid? FlowId { get; set; }
}

/// <summary>
/// Результат запроса получения списка назначений потоков
/// </summary>
public class GetFlowAssignmentsQueryResult
{
    /// <summary>
    /// Список назначений
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