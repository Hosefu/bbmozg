using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Enums;
using System;
using System.Collections.Generic;

namespace Lauf.Application.DTOs.Flows;

/// <summary>
/// DTO для версии этапа потока
/// </summary>
public class FlowStepVersionDto
{
    /// <summary>
    /// Идентификатор версии этапа
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор оригинального этапа
    /// </summary>
    public Guid OriginalId { get; set; }

    /// <summary>
    /// Номер версии
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Идентификатор версии потока
    /// </summary>
    public Guid FlowVersionId { get; set; }

    /// <summary>
    /// Название этапа
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание этапа
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Порядок в потоке
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Является ли этап обязательным
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Оценочное время выполнения в минутах
    /// </summary>
    public int EstimatedDurationMinutes { get; set; }

    /// <summary>
    /// Статус этапа
    /// </summary>
    public StepStatus Status { get; set; }

    /// <summary>
    /// Инструкции по выполнению
    /// </summary>
    public string Instructions { get; set; } = string.Empty;

    /// <summary>
    /// Заметки наставника
    /// </summary>
    public string Notes { get; set; } = string.Empty;

    /// <summary>
    /// Является ли версия активной
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Версии компонентов (если включены детали)
    /// </summary>
    public IList<ComponentVersionDto> ComponentVersions { get; set; } = new List<ComponentVersionDto>();
}