using MediatR;
using Lauf.Application.DTOs.Flows;

namespace Lauf.Application.Commands.FlowAssignment;

/// <summary>
/// Команда начала прохождения потока
/// </summary>
public class StartFlowCommand : IRequest<StartFlowCommandResult>
{
    /// <summary>
    /// ID назначения потока
    /// </summary>
    public Guid AssignmentId { get; set; }
}

/// <summary>
/// Результат команды начала прохождения потока
/// </summary>
public class StartFlowCommandResult
{
    /// <summary>
    /// Обновленное назначение
    /// </summary>
    public FlowAssignmentDto Assignment { get; set; } = null!;

    /// <summary>
    /// Успешность операции
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public string? ErrorMessage { get; set; }
}