using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Enums;
using System;
using System.Collections.Generic;

namespace Lauf.Application.DTOs.Flows;

/// <summary>
/// DTO для версии потока
/// </summary>
public class FlowVersionDto
{
    /// <summary>
    /// Идентификатор версии потока
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор оригинального потока
    /// </summary>
    public Guid OriginalId { get; set; }

    /// <summary>
    /// Номер версии
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Название потока
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание потока
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Теги потока
    /// </summary>
    public string Tags { get; set; } = string.Empty;

    /// <summary>
    /// Статус потока
    /// </summary>
    public FlowStatus Status { get; set; }

    /// <summary>
    /// Приоритет потока
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Является ли поток обязательным
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Является ли версия активной
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Идентификатор создателя
    /// </summary>
    public Guid CreatedById { get; set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Версии этапов (если включены детали)
    /// </summary>
    public IList<FlowStepVersionDto> StepVersions { get; set; } = new List<FlowStepVersionDto>();
}