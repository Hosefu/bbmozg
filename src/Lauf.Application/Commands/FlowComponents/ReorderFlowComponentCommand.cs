using MediatR;

namespace Lauf.Application.Commands.FlowComponents;

/// <summary>
/// Команда для изменения порядка компонента в шаге
/// </summary>
public class ReorderFlowComponentCommand : IRequest<ReorderFlowComponentCommandResult>
{
    /// <summary>
    /// Идентификатор компонента
    /// </summary>
    public Guid ComponentId { get; set; }

    /// <summary>
    /// Новая позиция (0-based индекс)
    /// </summary>
    public int NewPosition { get; set; }
}

/// <summary>
/// Результат команды перестановки компонента
/// </summary>
public class ReorderFlowComponentCommandResult
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
    public static ReorderFlowComponentCommandResult Success(string message = "Порядок компонента успешно изменен")
    {
        return new ReorderFlowComponentCommandResult
        {
            IsSuccess = true,
            Message = message
        };
    }

    /// <summary>
    /// Создать результат с ошибкой
    /// </summary>
    public static ReorderFlowComponentCommandResult Failure(string message)
    {
        return new ReorderFlowComponentCommandResult
        {
            IsSuccess = false,
            Message = message
        };
    }
} 