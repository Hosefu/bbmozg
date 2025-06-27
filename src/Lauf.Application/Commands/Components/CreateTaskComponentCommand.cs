using MediatR;
using Lauf.Application.DTOs.Components;

namespace Lauf.Application.Commands.Components;

/// <summary>
/// Команда для создания компонента задания
/// </summary>
public class CreateTaskComponentCommand : IRequest<CreateTaskComponentResult>
{
    /// <summary>
    /// Идентификатор шага потока
    /// </summary>
    public Guid FlowStepId { get; set; }

    /// <summary>
    /// Название задания
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание задания
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Инструкция - как найти кодовое слово
    /// </summary>
    public string Instruction { get; set; } = string.Empty;

    /// <summary>
    /// Кодовое слово для проверки ответа
    /// </summary>
    public string CodeWord { get; set; } = string.Empty;

    /// <summary>
    /// Подсказка, доступная в любой момент
    /// </summary>
    public string Hint { get; set; } = string.Empty;



    /// <summary>
    /// Обязательный ли компонент
    /// </summary>
    public bool IsRequired { get; set; } = true;

    /// <summary>
    /// Приблизительное время выполнения в минутах
    /// </summary>
    public int EstimatedDurationMinutes { get; set; } = 30;
}

/// <summary>
/// Результат команды создания компонента задания
/// </summary>
public class CreateTaskComponentResult
{
    /// <summary>
    /// Успешно ли выполнена команда
    /// </summary>
    public bool IsSuccess { get; private set; }

    /// <summary>
    /// Сообщение о результате выполнения
    /// </summary>
    public string Message { get; private set; } = string.Empty;

    /// <summary>
    /// Идентификатор созданного компонента
    /// </summary>
    public Guid ComponentId { get; private set; }


    /// <summary>
    /// DTO созданного компонента
    /// </summary>
    public TaskComponentDto? Component { get; private set; }

    /// <summary>
    /// Конструктор для успешного результата
    /// </summary>
    /// <param name="componentId">Идентификатор компонента</param>
    /// <param name="component">DTO компонента</param>
    /// <param name="message">Сообщение об успехе</param>
    private CreateTaskComponentResult(Guid componentId, TaskComponentDto component, string message = "Компонент задания успешно создан")
    {
        IsSuccess = true;
        Message = message;
        ComponentId = componentId;
        Component = component;
    }

    /// <summary>
    /// Конструктор для неуспешного результата
    /// </summary>
    /// <param name="message">Сообщение об ошибке</param>
    private CreateTaskComponentResult(string message)
    {
        IsSuccess = false;
        Message = message;
        ComponentId = Guid.Empty;
        Component = null;
    }

    /// <summary>
    /// Создает успешный результат
    /// </summary>
    /// <param name="componentId">Идентификатор компонента</param>
    /// <param name="component">DTO компонента</param>
    /// <param name="message">Сообщение об успехе</param>
    /// <returns>Успешный результат</returns>
    public static CreateTaskComponentResult Success(Guid componentId, TaskComponentDto component, string? message = null)
    {
        return new CreateTaskComponentResult(componentId, component, message ?? "Компонент задания успешно создан");
    }

    /// <summary>
    /// Создает неуспешный результат
    /// </summary>
    /// <param name="message">Сообщение об ошибке</param>
    /// <returns>Неуспешный результат</returns>
    public static CreateTaskComponentResult Failure(string message)
    {
        return new CreateTaskComponentResult(message);
    }
}