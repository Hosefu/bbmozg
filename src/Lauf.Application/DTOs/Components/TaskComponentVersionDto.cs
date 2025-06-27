using Lauf.Domain.Entities.Components;
using Lauf.Domain.Enums;
using System;

namespace Lauf.Application.DTOs.Flows;

/// <summary>
/// DTO для версии компонента задания
/// </summary>
public class TaskComponentVersionDto
{
    /// <summary>
    /// Идентификатор версии компонента
    /// </summary>
    public Guid ComponentVersionId { get; set; }

    /// <summary>
    /// Инструкции по выполнению задания
    /// </summary>
    public string Instructions { get; set; } = string.Empty;

    /// <summary>
    /// Тип подачи решения
    /// </summary>
    public TaskSubmissionType SubmissionType { get; set; }

    /// <summary>
    /// Максимальный размер файла (в байтах)
    /// </summary>
    public long? MaxFileSize { get; set; }

    /// <summary>
    /// Разрешенные типы файлов (расширения через запятую)
    /// </summary>
    public string AllowedFileTypes { get; set; } = string.Empty;

    /// <summary>
    /// Требуется ли одобрение наставника
    /// </summary>
    public bool RequiresMentorApproval { get; set; }

    /// <summary>
    /// Ключевые слова для автоматического одобрения
    /// </summary>
    public string AutoApprovalKeywords { get; set; } = string.Empty;
}