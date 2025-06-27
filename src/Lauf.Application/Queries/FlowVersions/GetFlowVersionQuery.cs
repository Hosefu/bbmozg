using Lauf.Application.DTOs.Flows;
using MediatR;
using System;

namespace Lauf.Application.Queries.FlowVersions;

/// <summary>
/// Запрос для получения версии потока
/// </summary>
public class GetFlowVersionQuery : IRequest<FlowVersionDto?>
{
    /// <summary>
    /// Идентификатор версии потока
    /// </summary>
    public Guid FlowVersionId { get; set; }

    /// <summary>
    /// Включать ли полную загрузку зависимостей
    /// </summary>
    public bool IncludeDetails { get; set; } = true;
}

/// <summary>
/// Запрос для получения активной версии потока
/// </summary>
public class GetActiveFlowVersionQuery : IRequest<FlowVersionDto?>
{
    /// <summary>
    /// Идентификатор оригинального потока
    /// </summary>
    public Guid OriginalFlowId { get; set; }

    /// <summary>
    /// Включать ли полную загрузку зависимостей
    /// </summary>
    public bool IncludeDetails { get; set; } = true;
}

/// <summary>
/// Запрос для получения конкретной версии потока
/// </summary>
public class GetSpecificFlowVersionQuery : IRequest<FlowVersionDto?>
{
    /// <summary>
    /// Идентификатор оригинального потока
    /// </summary>
    public Guid OriginalFlowId { get; set; }

    /// <summary>
    /// Номер версии
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Включать ли полную загрузку зависимостей
    /// </summary>
    public bool IncludeDetails { get; set; } = true;
}

/// <summary>
/// Запрос для получения всех версий потока
/// </summary>
public class GetAllFlowVersionsQuery : IRequest<IList<FlowVersionDto>>
{
    /// <summary>
    /// Идентификатор оригинального потока
    /// </summary>
    public Guid OriginalFlowId { get; set; }

    /// <summary>
    /// Включать ли полную загрузку зависимостей
    /// </summary>
    public bool IncludeDetails { get; set; } = false;

    /// <summary>
    /// Максимальное количество версий для возврата
    /// </summary>
    public int? Limit { get; set; }
}