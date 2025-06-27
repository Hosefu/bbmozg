using Lauf.Domain.Entities.Users;

namespace Lauf.Domain.Entities.Flows;

/// <summary>
/// Версионируемый контент потока (заменяет FlowVersion)
/// </summary>
public class FlowContent
{
    /// <summary>
    /// Уникальный идентификатор версии контента
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор Flow-координатора
    /// </summary>
    public Guid FlowId { get; set; }

    /// <summary>
    /// Номер версии контента
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Дата создания версии
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Идентификатор создателя версии
    /// </summary>
    public Guid CreatedBy { get; set; }

    /// <summary>
    /// Flow-координатор
    /// </summary>
    public virtual Flow Flow { get; set; } = null!;

    /// <summary>
    /// Создатель версии
    /// </summary>
    public virtual User CreatedByUser { get; set; } = null!;

    /// <summary>
    /// Шаги, привязанные к этой версии контента
    /// </summary>
    public virtual ICollection<FlowStep> Steps { get; set; } = new List<FlowStep>();

    /// <summary>
    /// Назначения, использующие эту версию контента
    /// </summary>
    public virtual ICollection<FlowAssignment> Assignments { get; set; } = new List<FlowAssignment>();

    /// <summary>
    /// Конструктор для создания новой версии контента
    /// </summary>
    /// <param name="flowId">Идентификатор потока</param>
    /// <param name="version">Номер версии</param>
    /// <param name="createdBy">Создатель версии</param>
    public FlowContent(Guid flowId, int version, Guid createdBy)
    {
        Id = Guid.NewGuid();
        FlowId = flowId;
        Version = version;
        CreatedBy = createdBy;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Конструктор для EF Core
    /// </summary>
    protected FlowContent() { }

    /// <summary>
    /// Общее количество шагов в версии контента
    /// </summary>
    public int TotalSteps => Steps.Count;
}