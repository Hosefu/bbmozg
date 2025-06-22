using BuddyBot.Domain.Enums;

namespace BuddyBot.Domain.Entities.Flows;

/// <summary>
/// Компонент шага в потоке - связь шага с конкретным компонентом контента
/// </summary>
public class FlowStepComponent
{
    /// <summary>
    /// Уникальный идентификатор связи
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор шага
    /// </summary>
    public Guid FlowStepId { get; set; }

    /// <summary>
    /// Шаг, которому принадлежит компонент
    /// </summary>
    public virtual FlowStep FlowStep { get; set; } = null!;

    /// <summary>
    /// Идентификатор компонента контента
    /// </summary>
    public Guid ComponentId { get; set; }

    /// <summary>
    /// Тип компонента
    /// </summary>
    public ComponentType ComponentType { get; set; }

    /// <summary>
    /// Название компонента
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание компонента
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Порядковый номер компонента в шаге
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Обязательный ли компонент
    /// </summary>
    public bool IsRequired { get; set; } = true;

    /// <summary>
    /// Статус компонента
    /// </summary>
    public ComponentStatus Status { get; set; } = ComponentStatus.Active;

    /// <summary>
    /// Приблизительное время выполнения в минутах
    /// </summary>
    public int EstimatedDurationMinutes { get; set; } = 15;

    /// <summary>
    /// Максимальное количество попыток
    /// </summary>
    public int? MaxAttempts { get; set; }

    /// <summary>
    /// Минимальный проходной балл (для тестов и квизов)
    /// </summary>
    public int? MinPassingScore { get; set; }

    /// <summary>
    /// Настройки компонента в формате JSON
    /// </summary>
    public string Settings { get; set; } = "{}";

    /// <summary>
    /// Дополнительные инструкции для компонента
    /// </summary>
    public string Instructions { get; set; } = string.Empty;

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Проверяет, может ли компонент быть активирован
    /// </summary>
    /// <returns>true, если компонент может быть активирован</returns>
    public bool CanBeActivated()
    {
        return Status == ComponentStatus.Draft && 
               ComponentId != Guid.Empty && 
               !string.IsNullOrWhiteSpace(Title);
    }

    /// <summary>
    /// Активирует компонент
    /// </summary>
    public void Activate()
    {
        if (!CanBeActivated())
            throw new InvalidOperationException("Компонент не может быть активирован в текущем состоянии");

        Status = ComponentStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Деактивирует компонент
    /// </summary>
    public void Deactivate()
    {
        Status = ComponentStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Возвращает компонент в черновик
    /// </summary>
    public void ReturnToDraft()
    {
        Status = ComponentStatus.Draft;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Проверяет, является ли компонент интерактивным (требует взаимодействия)
    /// </summary>
    /// <returns>true, если компонент интерактивный</returns>
    public bool IsInteractive()
    {
        return ComponentType is ComponentType.Quiz or ComponentType.Task or ComponentType.Survey;
    }

    /// <summary>
    /// Проверяет, поддерживает ли компонент отслеживание прогресса
    /// </summary>
    /// <returns>true, если поддерживает отслеживание прогресса</returns>
    public bool SupportsProgress()
    {
        return ComponentType is ComponentType.Video or ComponentType.Article or ComponentType.Task;
    }

    /// <summary>
    /// Проверяет, имеет ли компонент систему оценок
    /// </summary>
    /// <returns>true, если имеет систему оценок</returns>
    public bool HasScoring()
    {
        return ComponentType is ComponentType.Quiz or ComponentType.Task && MinPassingScore.HasValue;
    }
}