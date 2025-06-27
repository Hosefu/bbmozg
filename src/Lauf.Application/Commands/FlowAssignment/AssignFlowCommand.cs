using MediatR;

namespace Lauf.Application.Commands.FlowAssignment;

/// <summary>
/// Команда для назначения потока пользователю
/// </summary>
public record AssignFlowCommand : IRequest<AssignFlowCommandResult>
{
    /// <summary>
    /// Идентификатор пользователя, которому назначается поток
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Идентификатор потока для назначения
    /// </summary>
    public Guid FlowId { get; init; }

    /// <summary>
    /// Дедлайн прохождения потока
    /// </summary>
    public DateTime? Deadline { get; init; }

    /// <summary>
    /// Идентификатор buddy (наставника)
    /// </summary>
    public Guid? BuddyId { get; init; }

    /// <summary>
    /// Идентификатор пользователя, создающего назначение
    /// </summary>
    public Guid CreatedById { get; init; }

    /// <summary>
    /// Дополнительные заметки к назначению
    /// </summary>
    public string? Notes { get; init; }
}

/// <summary>
/// Результат выполнения команды назначения потока
/// </summary>
public class AssignFlowCommandResult
{
    /// <summary>
    /// Идентификатор созданного назначения
    /// </summary>
    public Guid AssignmentId { get; set; }

    /// <summary>
    /// Идентификатор содержимого потока, назначенного пользователю
    /// </summary>
    public Guid FlowContentId { get; set; }

    /// <summary>
    /// Успешно ли выполнено назначение
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Сообщение о результате
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Расчетная дата завершения
    /// </summary>
    public DateTime EstimatedCompletionDate { get; set; }
}