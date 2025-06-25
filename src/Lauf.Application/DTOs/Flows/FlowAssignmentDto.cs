using Lauf.Domain.Enums;
using Lauf.Application.DTOs.Users;

namespace Lauf.Application.DTOs.Flows;

/// <summary>
/// DTO для назначения потока пользователю
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
    /// Статус назначения
    /// </summary>
    public AssignmentStatus Status { get; set; }

    /// <summary>
    /// Дата создания назначения
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата начала прохождения
    /// </summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// Дата завершения прохождения
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Крайний срок выполнения
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Процент прогресса выполнения
    /// </summary>
    public int ProgressPercentage { get; set; }

    /// <summary>
    /// Заметки о назначении
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Приоритет выполнения
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Пользователь (может быть null, если не загружен)
    /// </summary>
    public UserDto? User { get; set; }

    /// <summary>
    /// Поток обучения (может быть null, если не загружен)
    /// </summary>
    public FlowDto? Flow { get; set; }

    /// <summary>
    /// ID назначившего пользователя
    /// </summary>
    public Guid? AssignedById { get; set; }

    /// <summary>
    /// Назначивший пользователь (может быть null, если не загружен)
    /// </summary>
    public UserDto? AssignedBy { get; set; }

    /// <summary>
    /// ID куратора (бадди)
    /// </summary>
    public Guid? BuddyId { get; set; }

    /// <summary>
    /// Куратор (может быть null, если не загружен)
    /// </summary>
    public UserDto? Buddy { get; set; }
}