using Lauf.Domain.Entities.Components;
using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Enums;
using MediatR;
using System;

namespace Lauf.Application.Commands.ComponentVersions;

/// <summary>
/// Команда для создания новой версии компонента
/// </summary>
public class CreateComponentVersionCommand : IRequest<CreateComponentVersionResponse>
{
    /// <summary>
    /// Идентификатор оригинального компонента
    /// </summary>
    public Guid OriginalComponentId { get; set; }

    /// <summary>
    /// Идентификатор версии этапа
    /// </summary>
    public Guid StepVersionId { get; set; }

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
    public ComponentType ComponentType { get; set; }

    /// <summary>
    /// Статус компонента
    /// </summary>
    public ComponentStatus Status { get; set; } = ComponentStatus.Draft;

    /// <summary>
    /// Порядок в этапе
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Является ли компонент обязательным
    /// </summary>
    public bool IsRequired { get; set; } = true;

    /// <summary>
    /// Оценочное время выполнения в минутах
    /// </summary>
    public int EstimatedDurationMinutes { get; set; }

    /// <summary>
    /// Максимальное количество попыток
    /// </summary>
    public int MaxAttempts { get; set; } = 3;

    /// <summary>
    /// Минимальный проходной балл
    /// </summary>
    public decimal MinPassingScore { get; set; } = 70;

    /// <summary>
    /// Инструкции по выполнению
    /// </summary>
    public string Instructions { get; set; } = string.Empty;

    /// <summary>
    /// Активировать ли новую версию сразу
    /// </summary>
    public bool ActivateImmediately { get; set; } = false;

    /// <summary>
    /// Специализированные данные статьи (если тип = Article)
    /// </summary>
    public CreateArticleVersionData? ArticleData { get; set; }

    /// <summary>
    /// Специализированные данные теста (если тип = Quiz)
    /// </summary>
    public CreateQuizVersionData? QuizData { get; set; }

    /// <summary>
    /// Специализированные данные задания (если тип = Task)
    /// </summary>
    public CreateTaskVersionData? TaskData { get; set; }
}

/// <summary>
/// Данные для создания версии статьи
/// </summary>
public class CreateArticleVersionData
{
    /// <summary>
    /// Содержимое статьи в формате Markdown
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Время чтения в минутах
    /// </summary>
    public int ReadingTimeMinutes { get; set; }
}

/// <summary>
/// Данные для создания версии теста
/// </summary>
public class CreateQuizVersionData
{
    /// <summary>
    /// Проходной балл (в процентах)
    /// </summary>
    public decimal PassingScore { get; set; } = 70;

    /// <summary>
    /// Ограничение по времени в минутах
    /// </summary>
    public int? TimeLimitMinutes { get; set; }

    /// <summary>
    /// Разрешены ли повторные попытки
    /// </summary>
    public bool AllowMultipleAttempts { get; set; } = true;

    /// <summary>
    /// Показывать ли правильные ответы после завершения
    /// </summary>
    public bool ShowCorrectAnswers { get; set; } = true;

    /// <summary>
    /// Перемешивать ли вопросы
    /// </summary>
    public bool ShuffleQuestions { get; set; } = false;

    /// <summary>
    /// Перемешивать ли варианты ответов
    /// </summary>
    public bool ShuffleAnswers { get; set; } = false;

    /// <summary>
    /// Варианты ответов
    /// </summary>
    public CreateQuizOptionData[] Options { get; set; } = Array.Empty<CreateQuizOptionData>();
}

/// <summary>
/// Данные для создания варианта ответа в тесте
/// </summary>
public class CreateQuizOptionData
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
    /// Количество баллов за выбор этого варианта
    /// </summary>
    public decimal Points { get; set; }

    /// <summary>
    /// Порядок отображения
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Объяснение (показывается после ответа)
    /// </summary>
    public string Explanation { get; set; } = string.Empty;
}

/// <summary>
/// Данные для создания версии задания
/// </summary>
public class CreateTaskVersionData
{
    /// <summary>
    /// Инструкции по выполнению задания
    /// </summary>
    public string Instructions { get; set; } = string.Empty;

    /// <summary>
    /// Тип подачи решения
    /// </summary>
    public TaskSubmissionType SubmissionType { get; set; } = TaskSubmissionType.Text;

    /// <summary>
    /// Максимальный размер файла (в байтах)
    /// </summary>
    public long? MaxFileSize { get; set; }

    /// <summary>
    /// Разрешенные типы файлов (расширения через запятую)
    /// </summary>
    public string AllowedFileTypes { get; set; } = string.Empty;

    /// <summary>
    /// Требуется ли одобрение наставника
    /// </summary>
    public bool RequiresMentorApproval { get; set; } = true;

    /// <summary>
    /// Ключевые слова для автоматического одобрения
    /// </summary>
    public string AutoApprovalKeywords { get; set; } = string.Empty;
}

/// <summary>
/// Ответ на команду создания новой версии компонента
/// </summary>
public class CreateComponentVersionResponse
{
    /// <summary>
    /// Идентификатор созданной версии компонента
    /// </summary>
    public Guid ComponentVersionId { get; set; }

    /// <summary>
    /// Номер созданной версии
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Тип компонента
    /// </summary>
    public ComponentType ComponentType { get; set; }

    /// <summary>
    /// Является ли версия активной
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Дата создания версии
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Сообщение об успешном создании
    /// </summary>
    public string Message { get; set; } = string.Empty;
}