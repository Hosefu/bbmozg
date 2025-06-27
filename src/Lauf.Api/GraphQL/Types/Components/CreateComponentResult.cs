using HotChocolate.Types;
using Lauf.Application.DTOs.Components;

namespace Lauf.Api.GraphQL.Types.Components;

/// <summary>
/// OneOf Result Union для создания компонентов
/// </summary>
[OneOf]
public class CreateComponentResult
{
    /// <summary>
    /// Результат создания компонента статьи
    /// </summary>
    public ArticleComponentResult? Article { get; set; }

    /// <summary>
    /// Результат создания компонента квиза
    /// </summary>
    public QuizComponentResult? Quiz { get; set; }

    /// <summary>
    /// Результат создания компонента задания
    /// </summary>
    public TaskComponentResult? Task { get; set; }
}

/// <summary>
/// Результат создания компонента статьи для GraphQL
/// </summary>
public class ArticleComponentResult
{
    /// <summary>
    /// Успешно ли выполнена операция
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Сообщение о результате
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Идентификатор созданного компонента
    /// </summary>
    public Guid? ComponentId { get; set; }


    /// <summary>
    /// Созданный компонент статьи
    /// </summary>
    public ArticleComponentDto? Component { get; set; }
}

/// <summary>
/// Результат создания компонента квиза для GraphQL
/// </summary>
public class QuizComponentResult
{
    /// <summary>
    /// Успешно ли выполнена операция
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Сообщение о результате
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Идентификатор созданного компонента
    /// </summary>
    public Guid? ComponentId { get; set; }


    /// <summary>
    /// Созданный компонент квиза
    /// </summary>
    public QuizComponentDto? Component { get; set; }
}

/// <summary>
/// Результат создания компонента задания для GraphQL
/// </summary>
public class TaskComponentResult
{
    /// <summary>
    /// Успешно ли выполнена операция
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Сообщение о результате
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Идентификатор созданного компонента
    /// </summary>
    public Guid? ComponentId { get; set; }


    /// <summary>
    /// Созданный компонент задания
    /// </summary>
    public TaskComponentDto? Component { get; set; }
}