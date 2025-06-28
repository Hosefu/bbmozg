using Lauf.Domain.Enums;
using Lauf.Application.DTOs.Users;

namespace Lauf.Application.DTOs.Flows;

/// <summary>
/// DTO для назначения потока пользователю (новая архитектура)
/// </summary>
public class FlowAssignmentDto
{
    /// <summary>
    /// Уникальный идентификатор назначения
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Идентификатор потока
    /// </summary>
    public Guid FlowId { get; set; }

    /// <summary>
    /// Статус назначения (новая архитектура)
    /// </summary>
    public ProgressStatus Status { get; set; }

    /// <summary>
    /// Дата назначения (AssignedAt в новой архитектуре)
    /// </summary>
    public DateTime AssignedAt { get; set; }

    /// <summary>
    /// Дата завершения прохождения
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Крайний срок выполнения (Deadline в новой архитектуре)
    /// </summary>
    public DateTime Deadline { get; set; }

    /// <summary>
    /// Идентификатор назначившего пользователя (AssignedBy в новой архитектуре)
    /// </summary>
    public Guid AssignedBy { get; set; }

    /// <summary>
    /// Идентификатор buddy (наставника), может быть null
    /// </summary>
    public Guid? Buddy { get; set; }

    /// <summary>
    /// Заметки о назначении
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Пользователь (может быть null, если не загружен)
    /// </summary>
    public UserDto? User { get; set; }

    /// <summary>
    /// Поток обучения (может быть null, если не загружен)
    /// </summary>
    public FlowDto? Flow { get; set; }
}