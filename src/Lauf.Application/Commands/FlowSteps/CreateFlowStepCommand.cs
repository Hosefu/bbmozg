using MediatR;
using Lauf.Application.DTOs.Flows;

namespace Lauf.Application.Commands.FlowSteps;

/// <summary>
/// Команда для создания шага потока
/// </summary>
public class CreateFlowStepCommand : IRequest<CreateFlowStepCommandResult>
{
    /// <summary>
    /// Идентификатор потока
    /// </summary>
    public Guid FlowId { get; set; }

    /// <summary>
    /// Название шага
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание шага
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Порядковый номер шага (если не указан, добавится в конец)
    /// </summary>
    public int? Order { get; set; }

    /// <summary>
    /// Обязательный ли шаг
    /// </summary>
    public bool IsRequired { get; set; } = true;

    /// <summary>
    /// Инструкции для прохождения шага
    /// </summary>
    public string Instructions { get; set; } = string.Empty;

    /// <summary>
    /// Дополнительные заметки
    /// </summary>
    public string Notes { get; set; } = string.Empty;
}

/// <summary>
/// Результат создания шага потока
/// </summary>
public class CreateFlowStepCommandResult
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
    /// Идентификатор созданного шага
    /// </summary>
    public Guid StepId { get; set; }

    /// <summary>
    /// Данные созданного шага
    /// </summary>
    public FlowStepDto? Step { get; set; }

    /// <summary>
    /// Создать успешный результат
    /// </summary>
    public static CreateFlowStepCommandResult Success(Guid stepId, FlowStepDto step, string message = "Шаг успешно создан")
    {
        return new CreateFlowStepCommandResult
        {
            IsSuccess = true,
            Message = message,
            StepId = stepId,
            Step = step
        };
    }

    /// <summary>
    /// Создать результат с ошибкой
    /// </summary>
    public static CreateFlowStepCommandResult Failure(string message)
    {
        return new CreateFlowStepCommandResult
        {
            IsSuccess = false,
            Message = message
        };
    }
}