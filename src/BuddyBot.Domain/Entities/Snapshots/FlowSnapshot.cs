using BuddyBot.Domain.Enums;

namespace BuddyBot.Domain.Entities.Snapshots;

/// <summary>
/// Снапшот потока - неизменяемая копия потока на момент назначения
/// </summary>
public class FlowSnapshot
{
    /// <summary>
    /// Идентификатор снапшота
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Идентификатор оригинального потока
    /// </summary>
    public Guid OriginalFlowId { get; private set; }

    /// <summary>
    /// Название потока на момент создания снапшота
    /// </summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// Описание потока на момент создания снапшота
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Статус потока на момент создания снапшота
    /// </summary>
    public FlowStatus Status { get; private set; }

    /// <summary>
    /// Расчетное время прохождения в часах
    /// </summary>
    public int EstimatedHours { get; private set; }

    /// <summary>
    /// Количество рабочих дней для завершения
    /// </summary>
    public int WorkingDaysToComplete { get; private set; }

    /// <summary>
    /// Является ли поток обязательным
    /// </summary>
    public bool IsRequired { get; private set; }

    /// <summary>
    /// Теги потока (JSON)
    /// </summary>
    public string Tags { get; private set; } = "[]";

    /// <summary>
    /// Дата создания снапшота
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Версия снапшота
    /// </summary>
    public int Version { get; private set; }

    /// <summary>
    /// Шаги потока в снапшоте
    /// </summary>
    public List<FlowStepSnapshot> Steps { get; private set; } = new();

    /// <summary>
    /// Приватный конструктор для EF Core
    /// </summary>
    private FlowSnapshot() { }

    /// <summary>
    /// Конструктор для создания снапшота потока
    /// </summary>
    /// <param name="originalFlowId">ID оригинального потока</param>
    /// <param name="title">Название потока</param>
    /// <param name="description">Описание потока</param>
    /// <param name="status">Статус потока</param>
    /// <param name="estimatedHours">Расчетное время в часах</param>
    /// <param name="workingDaysToComplete">Рабочие дни для завершения</param>
    /// <param name="isRequired">Является ли обязательным</param>
    /// <param name="tags">Теги потока</param>
    /// <param name="version">Версия снапшота</param>
    public FlowSnapshot(
        Guid originalFlowId,
        string title,
        string description,
        FlowStatus status,
        int estimatedHours,
        int workingDaysToComplete,
        bool isRequired,
        string tags,
        int version = 1)
    {
        Id = Guid.NewGuid();
        OriginalFlowId = originalFlowId;
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? string.Empty;
        Status = status;
        EstimatedHours = estimatedHours >= 0 ? estimatedHours : throw new ArgumentException("Время не может быть отрицательным");
        WorkingDaysToComplete = workingDaysToComplete >= 0 ? workingDaysToComplete : throw new ArgumentException("Количество дней не может быть отрицательным");
        IsRequired = isRequired;
        Tags = tags ?? "[]";
        Version = version;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Добавить снапшот шага
    /// </summary>
    /// <param name="stepSnapshot">Снапшот шага</param>
    public void AddStepSnapshot(FlowStepSnapshot stepSnapshot)
    {
        ArgumentNullException.ThrowIfNull(stepSnapshot);
        Steps.Add(stepSnapshot);
    }

    /// <summary>
    /// Получить общее количество компонентов в снапшоте
    /// </summary>
    public int GetTotalComponentsCount()
    {
        return Steps.Sum(step => step.Components.Count);
    }

    /// <summary>
    /// Получить снапшоты шагов, отсортированные по порядку
    /// </summary>
    public List<FlowStepSnapshot> GetOrderedSteps()
    {
        return Steps.OrderBy(step => step.Order).ToList();
    }
}