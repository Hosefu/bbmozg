namespace Lauf.Application.DTOs.Flows;

/// <summary>
/// DTO для содержимого потока (версионированное)
/// </summary>
public class FlowContentDto
{
    /// <summary>
    /// Идентификатор содержимого
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор потока
    /// </summary>
    public Guid FlowId { get; set; }

    /// <summary>
    /// Номер версии
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Дата создания версии
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Создатель версии
    /// </summary>
    public Guid CreatedBy { get; set; }

    /// <summary>
    /// Шаги содержимого
    /// </summary>
    public List<FlowStepDto>? Steps { get; set; }
}