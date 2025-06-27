using MediatR;
using System;

namespace Lauf.Application.Commands.FlowVersions;

/// <summary>
/// Команда для обновления версии потока
/// </summary>
public class UpdateFlowVersionCommand : IRequest<UpdateFlowVersionResponse>
{
    /// <summary>
    /// Идентификатор версии потока для обновления
    /// </summary>
    public Guid FlowVersionId { get; set; }

    /// <summary>
    /// Новое название потока
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Новое описание потока
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Новые теги потока
    /// </summary>
    public string? Tags { get; set; }

    /// <summary>
    /// Новый приоритет потока
    /// </summary>
    public int? Priority { get; set; }

    /// <summary>
    /// Является ли поток обязательным
    /// </summary>
    public bool? IsRequired { get; set; }

    /// <summary>
    /// Идентификатор пользователя, выполняющего обновление
    /// </summary>
    public Guid UpdatedById { get; set; }
}

/// <summary>
/// Ответ на команду обновления версии потока
/// </summary>
public class UpdateFlowVersionResponse
{
    /// <summary>
    /// Идентификатор обновленной версии потока
    /// </summary>
    public Guid FlowVersionId { get; set; }

    /// <summary>
    /// Номер версии
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Дата обновления
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Сообщение об успешном обновлении
    /// </summary>
    public string Message { get; set; } = string.Empty;
}