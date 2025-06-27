using Lauf.Application.DTOs.Flows;
using MediatR;
using System;
using System.Collections.Generic;

namespace Lauf.Application.Queries.ComponentVersions;

/// <summary>
/// Запрос для получения версии компонента по ID
/// </summary>
public class GetComponentVersionQuery : IRequest<ComponentVersionDto?>
{
    /// <summary>
    /// Идентификатор версии компонента
    /// </summary>
    public Guid ComponentVersionId { get; set; }

    /// <summary>
    /// Включать ли специализированные данные
    /// </summary>
    public bool IncludeSpecializedData { get; set; } = true;
}

/// <summary>
/// Запрос для получения активной версии компонента
/// </summary>
public class GetActiveComponentVersionQuery : IRequest<ComponentVersionDto?>
{
    /// <summary>
    /// Идентификатор оригинального компонента
    /// </summary>
    public Guid OriginalComponentId { get; set; }

    /// <summary>
    /// Включать ли специализированные данные
    /// </summary>
    public bool IncludeSpecializedData { get; set; } = true;
}

/// <summary>
/// Запрос для получения конкретной версии компонента
/// </summary>
public class GetSpecificComponentVersionQuery : IRequest<ComponentVersionDto?>
{
    /// <summary>
    /// Идентификатор оригинального компонента
    /// </summary>
    public Guid OriginalComponentId { get; set; }

    /// <summary>
    /// Номер версии
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Включать ли специализированные данные
    /// </summary>
    public bool IncludeSpecializedData { get; set; } = true;
}

/// <summary>
/// Запрос для получения всех версий компонента
/// </summary>
public class GetAllComponentVersionsQuery : IRequest<IList<ComponentVersionDto>>
{
    /// <summary>
    /// Идентификатор оригинального компонента
    /// </summary>
    public Guid OriginalComponentId { get; set; }

    /// <summary>
    /// Включать ли специализированные данные
    /// </summary>
    public bool IncludeSpecializedData { get; set; } = false;

    /// <summary>
    /// Максимальное количество версий для возврата
    /// </summary>
    public int? Limit { get; set; }
}

/// <summary>
/// Запрос для получения версий компонентов для версии этапа
/// </summary>
public class GetComponentVersionsByStepVersionQuery : IRequest<IList<ComponentVersionDto>>
{
    /// <summary>
    /// Идентификатор версии этапа
    /// </summary>
    public Guid StepVersionId { get; set; }

    /// <summary>
    /// Включать ли специализированные данные
    /// </summary>
    public bool IncludeSpecializedData { get; set; } = true;

    /// <summary>
    /// Сортировать ли по порядку
    /// </summary>
    public bool OrderBySequence { get; set; } = true;
}