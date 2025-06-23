using BuddyBot.Domain.Enums;

namespace BuddyBot.Domain.Events;

/// <summary>
/// Событие завершения компонента
/// </summary>
public record ComponentCompleted : IDomainEvent
{
    /// <summary>
    /// Уникальный идентификатор события
    /// </summary>
    public Guid EventId { get; } = Guid.NewGuid();

    /// <summary>
    /// Время возникновения события
    /// </summary>
    public DateTime OccurredAt { get; } = DateTime.UtcNow;

    /// <summary>
    /// Версия события
    /// </summary>
    public int Version { get; } = 1;

    /// <summary>
    /// ID пользователя
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// ID назначения потока
    /// </summary>
    public Guid AssignmentId { get; init; }

    /// <summary>
    /// ID прогресса компонента
    /// </summary>
    public Guid ComponentProgressId { get; init; }

    /// <summary>
    /// ID снапшота компонента
    /// </summary>
    public Guid ComponentSnapshotId { get; init; }

    /// <summary>
    /// Тип компонента
    /// </summary>
    public ComponentType ComponentType { get; init; }

    /// <summary>
    /// Название компонента
    /// </summary>
    public string ComponentTitle { get; init; } = string.Empty;

    /// <summary>
    /// Был ли компонент обязательным
    /// </summary>
    public bool WasRequired { get; init; }

    /// <summary>
    /// Время выполнения в минутах
    /// </summary>
    public int TimeSpentMinutes { get; init; }

    /// <summary>
    /// Количество попыток
    /// </summary>
    public int AttemptsCount { get; init; }

    /// <summary>
    /// Финальный балл (для квизов и заданий)
    /// </summary>
    public int? FinalScore { get; init; }

    /// <summary>
    /// Лучший балл
    /// </summary>
    public int? BestScore { get; init; }

    /// <summary>
    /// Дата начала выполнения
    /// </summary>
    public DateTime StartedAt { get; init; }

    /// <summary>
    /// Дата завершения
    /// </summary>
    public DateTime CompletedAt { get; init; }

    /// <summary>
    /// ID шага, к которому относится компонент
    /// </summary>
    public Guid StepSnapshotId { get; init; }

    /// <summary>
    /// Название шага
    /// </summary>
    public string StepTitle { get; init; } = string.Empty;

    /// <summary>
    /// Дополнительные данные прогресса
    /// </summary>
    public Dictionary<string, object> ProgressData { get; init; } = new();
}