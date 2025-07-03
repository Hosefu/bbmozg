using MediatR;
using Lauf.Application.DTOs.Components;

namespace Lauf.Application.Commands.Components;

/// <summary>
/// Команда для создания компонента квиза
/// </summary>
public class CreateQuizComponentCommand : IRequest<CreateQuizComponentResult>
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
    public List<CreateQuestionOptionDto> Options { get; set; } = new();



    /// <summary>
    /// Обязательный ли компонент
    /// </summary>
    public bool IsRequired { get; set; } = true;

    /// <summary>
    /// Приблизительное время выполнения в минутах
    /// </summary>
    public int EstimatedDurationMinutes { get; set; } = 5;

    /// <summary>
    /// Ограничение по времени в секундах
    /// </summary>
    public int? TimeLimit { get; set; }

    /// <summary>
    /// Проходной балл в процентах
    /// </summary>
    public int PassingScore { get; set; } = 80;

    /// <summary>
    /// Максимальное количество попыток
    /// </summary>
    public int? MaxAttempts { get; set; }

    /// <summary>
    /// Перемешивать ли вопросы
    /// </summary>
    public bool RandomizeQuestions { get; set; } = false;

    /// <summary>
    /// Перемешивать ли варианты ответов
    /// </summary>
    public bool RandomizeOptions { get; set; } = false;

    /// <summary>
    /// Показывать ли правильные ответы после завершения
    /// </summary>
    public bool ShowCorrectAnswers { get; set; } = false;

    /// <summary>
    /// Показывать ли объяснения к ответам
    /// </summary>
    public bool ShowExplanations { get; set; } = true;

    /// <summary>
    /// Разрешить ли просмотр результатов
    /// </summary>
    public bool AllowReview { get; set; } = true;

    /// <summary>
    /// Включить ли защиту от списывания
    /// </summary>
    public bool PreventCheating { get; set; } = true;

    /// <summary>
    /// Показывать ли прогресс-бар
    /// </summary>
    public bool ShowProgressBar { get; set; } = true;

    /// <summary>
    /// Разрешить ли пропуск вопросов
    /// </summary>
    public bool AllowSkipping { get; set; } = false;

    /// <summary>
    /// Количество вопросов на страницу
    /// </summary>
    public int QuestionsPerPage { get; set; } = 1;

    /// <summary>
    /// Автоматическая отправка при истечении времени
    /// </summary>
    public bool AutoSubmit { get; set; } = false;

    /// <summary>
    /// Показывать ли таймер
    /// </summary>
    public bool ShowTimer { get; set; } = true;

    /// <summary>
    /// Уровень сложности квиза
    /// </summary>
    public string Difficulty { get; set; } = "intermediate";

    /// <summary>
    /// Категории квиза
    /// </summary>
    public List<string> Categories { get; set; } = new();

    /// <summary>
    /// Дополнительные инструкции
    /// </summary>
    public string Instructions { get; set; } = string.Empty;
}

/// <summary>
/// DTO для создания варианта ответа
/// </summary>
public class CreateQuestionOptionDto
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
}

/// <summary>
/// Результат команды создания компонента квиза
/// </summary>
public class CreateQuizComponentResult
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
    public QuizComponentDto? Component { get; private set; }

    /// <summary>
    /// Конструктор для успешного результата
    /// </summary>
    /// <param name="componentId">Идентификатор компонента</param>
    /// <param name="component">DTO компонента</param>
    /// <param name="message">Сообщение об успехе</param>
    private CreateQuizComponentResult(Guid componentId, QuizComponentDto component, string message = "Компонент квиза успешно создан")
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
    private CreateQuizComponentResult(string message)
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
    public static CreateQuizComponentResult Success(Guid componentId, QuizComponentDto component, string? message = null)
    {
        return new CreateQuizComponentResult(componentId, component, message ?? "Компонент квиза успешно создан");
    }

    /// <summary>
    /// Создает неуспешный результат
    /// </summary>
    /// <param name="message">Сообщение об ошибке</param>
    /// <returns>Неуспешный результат</returns>
    public static CreateQuizComponentResult Failure(string message)
    {
        return new CreateQuizComponentResult(message);
    }
}