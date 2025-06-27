using MediatR;

namespace Lauf.Application.Commands.FlowManagement;

/// <summary>
/// Команда для создания нового потока обучения
/// </summary>
public record CreateFlowCommand : IRequest<CreateFlowCommandResult>
{
    /// <summary>
    /// Название потока
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Описание потока
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Категория потока
    /// </summary>
    public string Category { get; init; } = string.Empty;

    /// <summary>
    /// Теги потока (JSON массив)
    /// </summary>
    public string Tags { get; init; } = "[]";

    /// <summary>
    /// Приоритет потока (0-10)
    /// </summary>
    public int Priority { get; init; } = 5;

    /// <summary>
    /// Обязательный ли поток
    /// </summary>
    public bool IsRequired { get; init; } = false;

    /// <summary>
    /// Идентификатор создателя потока
    /// </summary>
    public Guid CreatedById { get; init; }

    /// <summary>
    /// Настройки потока
    /// </summary>
    public CreateFlowSettingsCommand? Settings { get; init; }
}

/// <summary>
/// Настройки для создаваемого потока (упрощенные)
/// </summary>
public record CreateFlowSettingsCommand
{
    /// <summary>
    /// Дней на шаг
    /// </summary>
    public int DaysPerStep { get; init; } = 7;

    /// <summary>
    /// Требовать последовательное прохождение компонентов
    /// </summary>
    public bool RequireSequentialCompletionComponents { get; init; } = false;

    /// <summary>
    /// Разрешить самостоятельный перезапуск
    /// </summary>
    public bool AllowSelfRestart { get; init; } = false;

    /// <summary>
    /// Разрешить самостоятельную паузу
    /// </summary>
    public bool AllowSelfPause { get; init; } = true;
}

/// <summary>
/// Результат выполнения команды создания потока
/// </summary>
public class CreateFlowCommandResult
{
    /// <summary>
    /// Идентификатор созданного потока
    /// </summary>
    public Guid FlowId { get; set; }

    /// <summary>
    /// Идентификатор созданного содержимого
    /// </summary>
    public Guid ContentId { get; set; }

    /// <summary>
    /// Номер созданной версии
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Успешно ли создан поток
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Сообщение о результате
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Название созданного потока
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Статус созданного потока
    /// </summary>
    public string Status { get; set; } = string.Empty;
}