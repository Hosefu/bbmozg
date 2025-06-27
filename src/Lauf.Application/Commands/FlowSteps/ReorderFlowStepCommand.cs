using MediatR;

namespace Lauf.Application.Commands.FlowSteps;

/// <summary>
/// Команда для изменения порядка шага в потоке
/// </summary>
public class ReorderFlowStepCommand : IRequest<ReorderFlowStepCommandResult>
{
    /// <summary>
    /// Идентификатор шага
    /// </summary>
    public Guid StepId { get; set; }

    /// <summary>
    /// Новая позиция (0-based индекс)
    /// </summary>
    public int NewPosition { get; set; }
}

/// <summary>
/// Результат команды перестановки шага
/// </summary>
public class ReorderFlowStepCommandResult
{
    /// <summary>
    /// Успешность операции
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Сообщение о результате
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Создать успешный результат
    /// </summary>
    public static ReorderFlowStepCommandResult Success(string message = "Порядок шага успешно изменен")
    {
        return new ReorderFlowStepCommandResult
        {
            IsSuccess = true,
            Message = message
        };
    }

    /// <summary>
    /// Создать результат с ошибкой
    /// </summary>
    public static ReorderFlowStepCommandResult Failure(string message)
    {
        return new ReorderFlowStepCommandResult
        {
            IsSuccess = false,
            Message = message
        };
    }
} 