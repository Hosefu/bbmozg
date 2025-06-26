using HotChocolate.Types;

namespace Lauf.Api.GraphQL.Types.Components;

/// <summary>
/// Input Union для создания компонентов
/// </summary>
public class CreateComponentInput
{
    /// <summary>
    /// Данные для создания компонента статьи
    /// </summary>
    public CreateArticleComponentInput? Article { get; set; }

    /// <summary>
    /// Данные для создания компонента квиза
    /// </summary>
    public CreateQuizComponentInput? Quiz { get; set; }

    /// <summary>
    /// Данные для создания компонента задания
    /// </summary>
    public CreateTaskComponentInput? Task { get; set; }
}

/// <summary>
/// Input для создания компонента статьи
/// </summary>
public class CreateArticleComponentInput
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
    /// Порядковый номер компонента в шаге
    /// </summary>
    public int? Order { get; set; }

    /// <summary>
    /// Обязательный ли компонент
    /// </summary>
    public bool IsRequired { get; set; } = true;
}

/// <summary>
/// Input для создания компонента квиза
/// </summary>
public class CreateQuizComponentInput
{
    /// <summary>
    /// Идентификатор шага потока
    /// </summary>
    public Guid FlowStepId { get; set; }

    /// <summary>
    /// Название квиза
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание квиза
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Текст вопроса
    /// </summary>
    public string QuestionText { get; set; } = string.Empty;

    /// <summary>
    /// Варианты ответов (ровно 5)
    /// </summary>
    public List<CreateQuestionOptionInput> Options { get; set; } = new();

    /// <summary>
    /// Порядковый номер компонента в шаге
    /// </summary>
    public int? Order { get; set; }

    /// <summary>
    /// Обязательный ли компонент
    /// </summary>
    public bool IsRequired { get; set; } = true;

    /// <summary>
    /// Приблизительное время выполнения в минутах
    /// </summary>
    public int EstimatedDurationMinutes { get; set; } = 5;
}

/// <summary>
/// Input для создания компонента задания
/// </summary>
public class CreateTaskComponentInput
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
    /// Порядковый номер компонента в шаге
    /// </summary>
    public int? Order { get; set; }

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
/// Input для создания варианта ответа
/// </summary>
public class CreateQuestionOptionInput
{
    /// <summary>
    /// Текст варианта ответа
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Является ли вариант правильным
    /// </summary>
    public bool IsCorrect { get; set; }

    /// <summary>
    /// Сообщение, показываемое при выборе этого варианта
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Количество баллов за правильный ответ
    /// </summary>
    public int Points { get; set; } = 1;
}