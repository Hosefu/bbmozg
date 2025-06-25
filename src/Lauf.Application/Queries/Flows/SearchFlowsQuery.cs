using MediatR;
using Lauf.Application.DTOs.Flows;

namespace Lauf.Application.Queries.Flows;

/// <summary>
/// Запрос поиска потоков
/// </summary>
public class SearchFlowsQuery : IRequest<SearchFlowsQueryResult>
{
    /// <summary>
    /// Поисковый запрос
    /// </summary>
    public string SearchTerm { get; set; } = string.Empty;

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
/// Результат запроса поиска потоков
/// </summary>
public class SearchFlowsQueryResult
{
    /// <summary>
    /// Найденные потоки
    /// </summary>
    public IReadOnlyList<FlowDto> Flows { get; set; } = new List<FlowDto>();

    /// <summary>
    /// Общее количество найденных потоков
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