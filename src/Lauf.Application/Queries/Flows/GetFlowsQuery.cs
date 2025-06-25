using MediatR;
using Lauf.Application.DTOs.Flows;
using Lauf.Domain.Enums;

namespace Lauf.Application.Queries.Flows;

/// <summary>
/// Запрос получения списка потоков
/// </summary>
public class GetFlowsQuery : IRequest<GetFlowsQueryResult>
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
    public FlowStatus? Status { get; set; }

    /// <summary>
    /// Поисковый запрос
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Фильтр по категории
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Включить шаги в результат
    /// </summary>
    public bool IncludeSteps { get; set; } = false;
}

/// <summary>
/// Результат запроса получения списка потоков
/// </summary>
public class GetFlowsQueryResult
{
    /// <summary>
    /// Список потоков
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