using Lauf.Domain.Enums;

namespace Lauf.Domain.Entities.Components;

/// <summary>
/// Базовый класс для всех компонентов контента
/// </summary>
public abstract class ComponentBase
{
    /// <summary>
    /// Уникальный идентификатор компонента
    /// </summary>
    public Guid Id { get; protected set; }

    /// <summary>
    /// Тип компонента
    /// </summary>
    public abstract ComponentType Type { get; }

    /// <summary>
    /// Название компонента
    /// </summary>
    public string Title { get; protected set; } = string.Empty;

    /// <summary>
    /// Описание компонента
    /// </summary>
    public string Description { get; protected set; } = string.Empty;

    /// <summary>
    /// Статус компонента
    /// </summary>
    public ComponentStatus Status { get; protected set; } = ComponentStatus.Draft;

    /// <summary>
    /// Приблизительное время выполнения в минутах
    /// </summary>
    public int EstimatedDurationMinutes { get; protected set; } = 15;

    /// <summary>
    /// Максимальное количество попыток
    /// </summary>
    public int? MaxAttempts { get; protected set; }

    /// <summary>
    /// Минимальный проходной балл (для тестов и квизов)
    /// </summary>
    public int? MinPassingScore { get; protected set; }

    /// <summary>
    /// Дополнительные инструкции для компонента
    /// </summary>
    public string Instructions { get; protected set; } = string.Empty;

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;

    /// <summary>
    /// Идентификатор шага потока, к которому принадлежит компонент
    /// </summary>
    public Guid FlowStepId { get; protected set; }

    /// <summary>
    /// Порядковый номер компонента в шаге (LexoRank для динамической сортировки)
    /// </summary>
    public string Order { get; protected set; } = string.Empty;

    /// <summary>
    /// Обязательный ли компонент для завершения шага
    /// </summary>
    public bool IsRequired { get; protected set; } = true;

    /// <summary>
    /// Конструктор для создания нового компонента
    /// </summary>
    /// <param name="flowStepId">Идентификатор шага потока</param>
    /// <param name="title">Название компонента</param>
    /// <param name="description">Описание компонента</param>
    /// <param name="order">Порядковый номер компонента</param>
    /// <param name="isRequired">Обязательный ли компонент</param>
    /// <param name="estimatedDurationMinutes">Приблизительное время выполнения в минутах</param>
    protected ComponentBase(Guid flowStepId, string title, string description, string order, bool isRequired = true, int estimatedDurationMinutes = 15)
    {
        Id = Guid.NewGuid();
        FlowStepId = flowStepId;
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Order = order ?? throw new ArgumentNullException(nameof(order));
        IsRequired = isRequired;
        EstimatedDurationMinutes = estimatedDurationMinutes;
        Status = ComponentStatus.Draft;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Конструктор для EF Core
    /// </summary>
    protected ComponentBase() { }

    /// <summary>
    /// Проверяет, может ли компонент быть активирован
    /// </summary>
    /// <returns>true, если компонент может быть активирован</returns>
    public virtual bool CanBeActivated()
    {
        return Status == ComponentStatus.Draft && 
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
    /// Обновляет основную информацию компонента
    /// </summary>
    /// <param name="title">Новое название</param>
    /// <param name="description">Новое описание</param>
    /// <param name="instructions">Новые инструкции</param>
    public void UpdateBasicInfo(string title, string description, string? instructions = null)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        if (instructions != null)
            Instructions = instructions;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Устанавливает настройки оценивания
    /// </summary>
    /// <param name="maxAttempts">Максимальное количество попыток</param>
    /// <param name="minPassingScore">Минимальный проходной балл</param>
    public void SetScoringSettings(int? maxAttempts, int? minPassingScore)
    {
        MaxAttempts = maxAttempts;
        MinPassingScore = minPassingScore;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Проверяет, является ли компонент интерактивным (требует взаимодействия)
    /// </summary>
    /// <returns>true, если компонент интерактивный</returns>
    public virtual bool IsInteractive()
    {
        return Type is ComponentType.Quiz or ComponentType.Task;
    }

    /// <summary>
    /// Проверяет, поддерживает ли компонент отслеживание прогресса
    /// </summary>
    /// <returns>true, если поддерживает отслеживание прогресса</returns>
    public virtual bool SupportsProgress()
    {
        return Type is ComponentType.Article or ComponentType.Task;
    }

    /// <summary>
    /// Проверяет, имеет ли компонент систему оценок
    /// </summary>
    /// <returns>true, если имеет систему оценок</returns>
    public virtual bool HasScoring()
    {
        return Type is ComponentType.Quiz or ComponentType.Task && MinPassingScore.HasValue;
    }

    /// <summary>
    /// Обновляет время последнего изменения
    /// </summary>
    public void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}