namespace BuddyBot.Domain.Exceptions;

/// <summary>
/// Исключение, выбрасываемое при отсутствии потока
/// </summary>
public class FlowNotFoundException : DomainException
{
    /// <summary>
    /// Код ошибки
    /// </summary>
    public override string ErrorCode => "FLOW_NOT_FOUND";

    /// <summary>
    /// Создает новое исключение с идентификатором потока
    /// </summary>
    /// <param name="flowId">Идентификатор потока</param>
    public FlowNotFoundException(Guid flowId) 
        : base($"Поток с ID {flowId} не найден", "FLOW_NOT_FOUND")
    {
        WithEntityId(flowId).WithEntityType("Flow");
    }

    /// <summary>
    /// Создает новое исключение с названием потока
    /// </summary>
    /// <param name="title">Название потока</param>
    public FlowNotFoundException(string title) 
        : base($"Поток с названием '{title}' не найден", "FLOW_NOT_FOUND")
    {
        WithDetail("Title", title).WithEntityType("Flow");
    }

    /// <summary>
    /// Создает общее исключение о ненайденном потоке
    /// </summary>
    public FlowNotFoundException() 
        : base("Поток не найден", "FLOW_NOT_FOUND")
    {
        WithEntityType("Flow");
    }
}