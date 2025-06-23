using MediatR;

namespace BuddyBot.Application.Commands.FlowManagement;

/// <summary>
/// Команда для создания нового потока обучения
/// </summary>
public record CreateFlowCommand : IRequest<CreateFlowCommandResult>
{
    /// <summary>
    /// Название потока
    /// </summary>
    public string Title { get; init; } = string.Empty;

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
/// Настройки для создаваемого потока
/// </summary>
public record CreateFlowSettingsCommand
{
    /// <summary>
    /// Разрешить пропуск шагов
    /// </summary>
    public bool AllowSkipping { get; init; } = false;

    /// <summary>
    /// Требовать последовательное прохождение
    /// </summary>
    public bool RequireSequentialCompletion { get; init; } = true;

    /// <summary>
    /// Максимальное количество попыток
    /// </summary>
    public int? MaxAttempts { get; init; }

    /// <summary>
    /// Время на выполнение в рабочих днях
    /// </summary>
    public int? TimeToCompleteWorkingDays { get; init; }

    /// <summary>
    /// Показывать прогресс
    /// </summary>
    public bool ShowProgress { get; init; } = true;

    /// <summary>
    /// Разрешить повторное прохождение
    /// </summary>
    public bool AllowRetry { get; init; } = false;

    /// <summary>
    /// Отправлять напоминания
    /// </summary>
    public bool SendReminders { get; init; } = true;

    /// <summary>
    /// Дополнительные настройки в JSON
    /// </summary>
    public string AdditionalSettings { get; init; } = "{}";
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
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Статус созданного потока
    /// </summary>
    public string Status { get; set; } = string.Empty;
}