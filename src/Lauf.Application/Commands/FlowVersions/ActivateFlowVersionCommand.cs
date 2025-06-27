using MediatR;
using System;

namespace Lauf.Application.Commands.FlowVersions;

/// <summary>
/// Команда для активации версии потока
/// </summary>
public class ActivateFlowVersionCommand : IRequest<ActivateFlowVersionResponse>
{
    /// <summary>
    /// Идентификатор версии потока для активации
    /// </summary>
    public Guid FlowVersionId { get; set; }

    /// <summary>
    /// Идентификатор пользователя, выполняющего активацию
    /// </summary>
    public Guid ActivatedById { get; set; }

    /// <summary>
    /// Принудительная активация (игнорировать предупреждения)
    /// </summary>
    public bool ForceActivation { get; set; } = false;
}

/// <summary>
/// Ответ на команду активации версии потока
/// </summary>
public class ActivateFlowVersionResponse
{
    /// <summary>
    /// Идентификатор активированной версии потока
    /// </summary>
    public Guid FlowVersionId { get; set; }

    /// <summary>
    /// Номер активированной версии
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Идентификатор оригинального потока
    /// </summary>
    public Guid OriginalFlowId { get; set; }

    /// <summary>
    /// Идентификатор предыдущей активной версии (если была)
    /// </summary>
    public Guid? PreviousActiveVersionId { get; set; }

    /// <summary>
    /// Дата активации
    /// </summary>
    public DateTime ActivatedAt { get; set; }

    /// <summary>
    /// Количество связанных назначений, которые будут затронуты
    /// </summary>
    public int AffectedAssignmentsCount { get; set; }

    /// <summary>
    /// Сообщение об успешной активации
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Предупреждения (если есть)
    /// </summary>
    public string[] Warnings { get; set; } = Array.Empty<string>();
}