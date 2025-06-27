using MediatR;
using Lauf.Application.DTOs.Components;

namespace Lauf.Application.Commands.Components;

/// <summary>
/// Команда для создания компонента статьи
/// </summary>
public class CreateArticleComponentCommand : IRequest<CreateArticleComponentResult>
{
    /// <summary>
    /// Идентификатор шага потока
    /// </summary>
    public Guid FlowStepId { get; set; }

    /// <summary>
    /// Название статьи
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание статьи
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Содержимое статьи в формате Markdown
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Время чтения в минутах
    /// </summary>
    public int ReadingTimeMinutes { get; set; } = 15;



    /// <summary>
    /// Обязательный ли компонент
    /// </summary>
    public bool IsRequired { get; set; } = true;
}

/// <summary>
/// Результат команды создания компонента статьи
/// </summary>
public class CreateArticleComponentResult
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
    public ArticleComponentDto? Component { get; private set; }

    /// <summary>
    /// Конструктор для успешного результата
    /// </summary>
    /// <param name="componentId">Идентификатор компонента</param>
    /// <param name="component">DTO компонента</param>
    /// <param name="message">Сообщение об успехе</param>
    private CreateArticleComponentResult(Guid componentId, ArticleComponentDto component, string message = "Компонент статьи успешно создан")
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
    private CreateArticleComponentResult(string message)
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
    public static CreateArticleComponentResult Success(Guid componentId, ArticleComponentDto component, string? message = null)
    {
        return new CreateArticleComponentResult(componentId, component, message ?? "Компонент статьи успешно создан");
    }

    /// <summary>
    /// Создает неуспешный результат
    /// </summary>
    /// <param name="message">Сообщение об ошибке</param>
    /// <returns>Неуспешный результат</returns>
    public static CreateArticleComponentResult Failure(string message)
    {
        return new CreateArticleComponentResult(message);
    }
}