using BuddyBot.Domain.Enums;
using MediatR;

namespace BuddyBot.Application.Commands.ComponentInteraction;

/// <summary>
/// Команда для взаимодействия с компонентом
/// </summary>
public record InteractWithComponentCommand : IRequest<InteractWithComponentCommandResult>
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Идентификатор назначения потока
    /// </summary>
    public Guid AssignmentId { get; init; }

    /// <summary>
    /// Идентификатор компонента в снапшоте
    /// </summary>
    public Guid ComponentSnapshotId { get; init; }

    /// <summary>
    /// Тип взаимодействия
    /// </summary>
    public ComponentInteractionType InteractionType { get; init; }

    /// <summary>
    /// Данные взаимодействия в JSON формате
    /// </summary>
    public string InteractionData { get; init; } = "{}";

    /// <summary>
    /// Потраченное время в секундах
    /// </summary>
    public int? TimeSpentSeconds { get; init; }

    /// <summary>
    /// Дополнительные метаданные
    /// </summary>
    public string? Metadata { get; init; }
}

/// <summary>
/// Тип взаимодействия с компонентом
/// </summary>
public enum ComponentInteractionType
{
    /// <summary>
    /// Начало чтения статьи
    /// </summary>
    StartReading = 1,

    /// <summary>
    /// Завершение чтения статьи
    /// </summary>
    FinishReading = 2,

    /// <summary>
    /// Отправка ответа на задание
    /// </summary>
    SubmitTaskAnswer = 3,

    /// <summary>
    /// Отправка ответов на квиз
    /// </summary>
    SubmitQuizAnswer = 4,

    /// <summary>
    /// Просмотр компонента
    /// </summary>
    View = 5,

    /// <summary>
    /// Пропуск компонента
    /// </summary>
    Skip = 6,

    /// <summary>
    /// Повторная попытка
    /// </summary>
    Retry = 7
}

/// <summary>
/// Результат взаимодействия с компонентом
/// </summary>
public class InteractWithComponentCommandResult
{
    /// <summary>
    /// Успешно ли выполнено взаимодействие
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Сообщение о результате
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Завершен ли компонент
    /// </summary>
    public bool IsComponentCompleted { get; set; }

    /// <summary>
    /// Разблокирован ли следующий шаг
    /// </summary>
    public bool IsNextStepUnlocked { get; set; }

    /// <summary>
    /// Завершен ли поток полностью
    /// </summary>
    public bool IsFlowCompleted { get; set; }

    /// <summary>
    /// Получен ли проходной балл (для квизов и заданий)
    /// </summary>
    public bool? IsPassingScore { get; set; }

    /// <summary>
    /// Полученный балл
    /// </summary>
    public int? Score { get; set; }

    /// <summary>
    /// Максимальный балл
    /// </summary>
    public int? MaxScore { get; set; }

    /// <summary>
    /// Процент прогресса по потоку
    /// </summary>
    public decimal FlowProgress { get; set; }

    /// <summary>
    /// Обратная связь или комментарии
    /// </summary>
    public string? Feedback { get; set; }

    /// <summary>
    /// Следующие шаги или рекомендации
    /// </summary>
    public string? NextSteps { get; set; }
}