using MediatR;
using Lauf.Application.DTOs.Flows;

namespace Lauf.Application.Queries.Flows;

/// <summary>
/// Запрос получения опубликованных потоков
/// </summary>
public class GetPublishedFlowsQuery : IRequest<GetPublishedFlowsQueryResult>
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
    /// Фильтр по категории
    /// </summary>
    public string? Category { get; set; }
}

/// <summary>
/// Результат запроса получения опубликованных потоков
/// </summary>
public class GetPublishedFlowsQueryResult
{
    /// <summary>
    /// Опубликованные потоки
    /// </summary>
    public IReadOnlyList<FlowDto> Flows { get; set; } = new List<FlowDto>();

    /// <summary>
    /// Общее количество потоков
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