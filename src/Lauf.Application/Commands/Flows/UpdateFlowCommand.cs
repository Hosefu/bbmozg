using MediatR;
using Lauf.Application.DTOs.Flows;
using Lauf.Domain.Enums;

namespace Lauf.Application.Commands.Flows;

/// <summary>
/// Команда обновления потока
/// </summary>
public class UpdateFlowCommand : IRequest<UpdateFlowCommandResult>
{
    /// <summary>
    /// ID потока
    /// </summary>
    public Guid FlowId { get; set; }

    /// <summary>
    /// Название потока
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Описание потока
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Статус потока
    /// </summary>
    public FlowStatus? Status { get; set; }

    /// <summary>
    /// Категория потока
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Теги потока
    /// </summary>
    public string? Tags { get; set; }

    /// <summary>
    /// Приоритет потока
    /// </summary>
    public int? Priority { get; set; }

    /// <summary>
    /// Обязательность потока
    /// </summary>
    public bool? IsRequired { get; set; }
}

/// <summary>
/// Результат команды обновления потока
/// </summary>
public class UpdateFlowCommandResult
{
    /// <summary>
    /// Обновленный поток
    /// </summary>
    public FlowDto Flow { get; set; } = null!;

    /// <summary>
    /// Успешность операции
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public string? ErrorMessage { get; set; }
}