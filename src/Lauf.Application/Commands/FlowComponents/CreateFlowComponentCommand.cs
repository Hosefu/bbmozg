using MediatR;
using Lauf.Domain.Enums;
using Lauf.Application.DTOs.Flows;

namespace Lauf.Application.Commands.FlowComponents;

/// <summary>
/// Команда для создания компонента шага потока
/// </summary>
public class CreateFlowComponentCommand : IRequest<CreateFlowComponentCommandResult>
{
    /// <summary>
    /// Идентификатор шага потока
    /// </summary>
    public Guid FlowStepId { get; set; }

    /// <summary>
    /// Название компонента
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание компонента
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Тип компонента
    /// </summary>
    public ComponentType Type { get; set; }

    /// <summary>
    /// Содержимое компонента в формате JSON
    /// </summary>
    public string Content { get; set; } = "{}";

    /// <summary>
    /// LexoRank позиция компонента (если не указана, добавится в конец)
    /// </summary>
    public string? Order { get; set; }

    /// <summary>
    /// Обязательный ли компонент
    /// </summary>
    public bool IsRequired { get; set; } = true;

}

/// <summary>
/// Результат создания компонента шага потока
/// </summary>
public class CreateFlowComponentCommandResult
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
    /// Идентификатор созданного компонента
    /// </summary>
    public Guid ComponentId { get; set; }

    /// <summary>
    /// Данные созданного компонента
    /// </summary>
    public FlowStepComponentDto? Component { get; set; }

    /// <summary>
    /// Создать успешный результат
    /// </summary>
    public static CreateFlowComponentCommandResult Success(Guid componentId, FlowStepComponentDto component, string message = "Компонент успешно создан")
    {
        return new CreateFlowComponentCommandResult
        {
            IsSuccess = true,
            Message = message,
            ComponentId = componentId,
            Component = component
        };
    }

    /// <summary>
    /// Создать результат с ошибкой
    /// </summary>
    public static CreateFlowComponentCommandResult Failure(string message)
    {
        return new CreateFlowComponentCommandResult
        {
            IsSuccess = false,
            Message = message,
            ComponentId = Guid.Empty,
            Component = null
        };
    }
}