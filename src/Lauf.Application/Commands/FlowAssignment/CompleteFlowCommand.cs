using MediatR;
using Lauf.Application.DTOs.Flows;

namespace Lauf.Application.Commands.FlowAssignment;

/// <summary>
/// Команда завершения прохождения потока
/// </summary>
public class CompleteFlowCommand : IRequest<CompleteFlowCommandResult>
{
    /// <summary>
    /// ID назначения потока
    /// </summary>
    public Guid AssignmentId { get; set; }

    /// <summary>
    /// Заметки о завершении
    /// </summary>
    public string? CompletionNotes { get; set; }
}

/// <summary>
/// Результат команды завершения прохождения потока
/// </summary>
public class CompleteFlowCommandResult
{
    /// <summary>
    /// Завершенное назначение
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