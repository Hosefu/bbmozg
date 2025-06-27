using MediatR;
using System;

namespace Lauf.Application.Commands.FlowVersions;

/// <summary>
/// Команда для создания новой версии потока
/// </summary>
public class CreateFlowVersionCommand : IRequest<CreateFlowVersionResponse>
{
    /// <summary>
    /// Идентификатор оригинального потока
    /// </summary>
    public Guid OriginalFlowId { get; set; }

    /// <summary>
    /// Название потока
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание потока
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Теги потока (разделенные запятыми)
    /// </summary>
    public string Tags { get; set; } = string.Empty;

    /// <summary>
    /// Приоритет потока
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Является ли поток обязательным
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Идентификатор создателя
    /// </summary>
    public Guid CreatedById { get; set; }

    /// <summary>
    /// Активировать ли новую версию сразу
    /// </summary>
    public bool ActivateImmediately { get; set; } = false;
}

/// <summary>
/// Ответ на команду создания новой версии потока
/// </summary>
public class CreateFlowVersionResponse
{
    /// <summary>
    /// Идентификатор созданной версии потока
    /// </summary>
    public Guid FlowVersionId { get; set; }

    /// <summary>
    /// Номер созданной версии
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Является ли версия активной
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Дата создания версии
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Сообщение об успешном создании
    /// </summary>
    public string Message { get; set; } = string.Empty;
}