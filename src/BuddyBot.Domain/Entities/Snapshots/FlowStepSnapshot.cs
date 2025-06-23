using BuddyBot.Domain.Enums;

namespace BuddyBot.Domain.Entities.Snapshots;

/// <summary>
/// Снапшот шага потока
/// </summary>
public class FlowStepSnapshot
{
    /// <summary>
    /// Идентификатор снапшота шага
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Идентификатор оригинального шага
    /// </summary>
    public Guid OriginalStepId { get; private set; }

    /// <summary>
    /// Идентификатор снапшота потока
    /// </summary>
    public Guid FlowSnapshotId { get; private set; }

    /// <summary>
    /// Снапшот потока
    /// </summary>
    public FlowSnapshot FlowSnapshot { get; private set; } = null!;

    /// <summary>
    /// Название шага
    /// </summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// Описание шага
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Порядковый номер шага
    /// </summary>
    public int Order { get; private set; }

    /// <summary>
    /// Требуется ли последовательное выполнение
    /// </summary>
    public bool RequiresSequentialCompletion { get; private set; }

    /// <summary>
    /// Расчетное время выполнения в минутах
    /// </summary>
    public int EstimatedMinutes { get; private set; }

    /// <summary>
    /// Дата создания снапшота
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Компоненты шага в снапшоте
    /// </summary>
    public List<ComponentSnapshot> Components { get; private set; } = new();

    /// <summary>
    /// Приватный конструктор для EF Core
    /// </summary>
    private FlowStepSnapshot() { }

    /// <summary>
    /// Конструктор для создания снапшота шага
    /// </summary>
    /// <param name="originalStepId">ID оригинального шага</param>
    /// <param name="flowSnapshotId">ID снапшота потока</param>
    /// <param name="title">Название шага</param>
    /// <param name="description">Описание шага</param>
    /// <param name="order">Порядковый номер</param>
    /// <param name="requiresSequentialCompletion">Требует последовательного выполнения</param>
    /// <param name="estimatedMinutes">Расчетное время в минутах</param>
    public FlowStepSnapshot(
        Guid originalStepId,
        Guid flowSnapshotId,
        string title,
        string description,
        int order,
        bool requiresSequentialCompletion,
        int estimatedMinutes)
    {
        Id = Guid.NewGuid();
        OriginalStepId = originalStepId;
        FlowSnapshotId = flowSnapshotId;
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? string.Empty;
        Order = order >= 0 ? order : throw new ArgumentException("Порядковый номер не может быть отрицательным");
        RequiresSequentialCompletion = requiresSequentialCompletion;
        EstimatedMinutes = estimatedMinutes >= 0 ? estimatedMinutes : throw new ArgumentException("Время не может быть отрицательным");
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Добавить снапшот компонента
    /// </summary>
    /// <param name="componentSnapshot">Снапшот компонента</param>
    public void AddComponentSnapshot(ComponentSnapshot componentSnapshot)
    {
        ArgumentNullException.ThrowIfNull(componentSnapshot);
        Components.Add(componentSnapshot);
    }

    /// <summary>
    /// Получить снапшоты компонентов, отсортированные по порядку
    /// </summary>
    public List<ComponentSnapshot> GetOrderedComponents()
    {
        return Components.OrderBy(component => component.Order).ToList();
    }

    /// <summary>
    /// Получить снапшоты компонентов определенного типа
    /// </summary>
    /// <param name="componentType">Тип компонента</param>
    public List<ComponentSnapshot> GetComponentsByType(ComponentType componentType)
    {
        return Components.Where(c => c.Type == componentType).OrderBy(c => c.Order).ToList();
    }
}