namespace BuddyBot.Domain.Exceptions;

/// <summary>
/// Исключение, выбрасываемое при попытке повторного назначения потока
/// </summary>
public class FlowAlreadyAssignedException : DomainException
{
    /// <summary>
    /// Код ошибки
    /// </summary>
    public override string ErrorCode => "FLOW_ALREADY_ASSIGNED";

    /// <summary>
    /// Создает новое исключение с идентификаторами пользователя и потока
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="flowId">Идентификатор потока</param>
    public FlowAlreadyAssignedException(Guid userId, Guid flowId) 
        : base($"Поток {flowId} уже назначен пользователю {userId}", "FLOW_ALREADY_ASSIGNED")
    {
        WithDetail("UserId", userId)
            .WithDetail("FlowId", flowId)
            .WithEntityType("FlowAssignment");
    }

    /// <summary>
    /// Создает новое исключение с идентификатором назначения
    /// </summary>
    /// <param name="assignmentId">Идентификатор назначения</param>
    public FlowAlreadyAssignedException(Guid assignmentId) 
        : base($"Поток уже назначен (назначение {assignmentId})", "FLOW_ALREADY_ASSIGNED")
    {
        WithEntityId(assignmentId).WithEntityType("FlowAssignment");
    }

    /// <summary>
    /// Создает общее исключение о повторном назначении
    /// </summary>
    public FlowAlreadyAssignedException() 
        : base("Поток уже назначен этому пользователю", "FLOW_ALREADY_ASSIGNED")
    {
        WithEntityType("FlowAssignment");
    }
}