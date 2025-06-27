using Lauf.Domain.Entities.Components;
using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Enums;
using System;

namespace Lauf.Application.DTOs.Flows;

/// <summary>
/// DTO для версии компонента
/// </summary>
public class ComponentVersionDto
{
    /// <summary>
    /// Идентификатор версии компонента
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор оригинального компонента
    /// </summary>
    public Guid OriginalId { get; set; }

    /// <summary>
    /// Номер версии
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Идентификатор версии этапа
    /// </summary>
    public Guid StepVersionId { get; set; }

    /// <summary>
    /// Название компонента
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание компонента
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Тип компонента
    /// </summary>
    public ComponentType ComponentType { get; set; }

    /// <summary>
    /// Статус компонента
    /// </summary>
    public ComponentStatus Status { get; set; }

    /// <summary>
    /// Порядок в этапе
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Является ли компонент обязательным
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Оценочное время выполнения в минутах
    /// </summary>
    public int EstimatedDurationMinutes { get; set; }

    /// <summary>
    /// Максимальное количество попыток
    /// </summary>
    public int MaxAttempts { get; set; }

    /// <summary>
    /// Минимальный проходной балл
    /// </summary>
    public decimal MinPassingScore { get; set; }

    /// <summary>
    /// Инструкции по выполнению
    /// </summary>
    public string Instructions { get; set; } = string.Empty;

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
    /// Специализированные данные статьи (если тип = Article)
    /// </summary>
    public ArticleComponentVersionDto? ArticleVersion { get; set; }

    /// <summary>
    /// Специализированные данные теста (если тип = Quiz)
    /// </summary>
    public QuizComponentVersionDto? QuizVersion { get; set; }

    /// <summary>
    /// Специализированные данные задания (если тип = Task)
    /// </summary>
    public TaskComponentVersionDto? TaskVersion { get; set; }
}