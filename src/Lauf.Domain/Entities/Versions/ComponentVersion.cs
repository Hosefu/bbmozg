using System;
using System.ComponentModel.DataAnnotations;
using Lauf.Domain.Enums;
using Lauf.Domain.Interfaces;
using Lauf.Domain.ValueObjects;

namespace Lauf.Domain.Entities.Versions;

/// <summary>
/// Версия компонента обучения
/// </summary>
public class ComponentVersion : IVersionedEntity<ComponentVersion>
{
    /// <summary>
    /// Уникальный идентификатор версии компонента
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Идентификатор оригинального компонента
    /// </summary>
    public Guid OriginalId { get; private set; }

    /// <summary>
    /// Номер версии
    /// </summary>
    public int Version { get; private set; }

    /// <summary>
    /// Является ли данная версия активной
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Идентификатор версии этапа
    /// </summary>
    public Guid StepVersionId { get; private set; }

    /// <summary>
    /// Название компонента
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// Описание компонента
    /// </summary>
    [Required]
    [StringLength(1000)]
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Тип компонента
    /// </summary>
    public ComponentType ComponentType { get; private set; }

    /// <summary>
    /// Статус компонента
    /// </summary>
    public ComponentStatus Status { get; private set; } = ComponentStatus.Draft;

    /// <summary>
    /// Порядок компонента (LexoRank)
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Order { get; private set; } = string.Empty;

    /// <summary>
    /// Является ли компонент обязательным
    /// </summary>
    public bool IsRequired { get; private set; } = true;

    /// <summary>
    /// Оценочное время выполнения в минутах
    /// </summary>
    public int EstimatedDurationMinutes { get; private set; } = 15;

    /// <summary>
    /// Максимальное количество попыток
    /// </summary>
    public int? MaxAttempts { get; private set; }

    /// <summary>
    /// Минимальный проходной балл
    /// </summary>
    public int? MinPassingScore { get; private set; }

    /// <summary>
    /// Инструкции для компонента
    /// </summary>
    [StringLength(2000)]
    public string Instructions { get; private set; } = string.Empty;

    /// <summary>
    /// Дата создания версии
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Дата последнего обновления версии
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Версия этапа, к которой принадлежит этот компонент
    /// </summary>
    public virtual FlowStepVersion StepVersion { get; set; } = null!;

    /// <summary>
    /// Версия статьи (если тип = Article)
    /// </summary>
    public virtual ArticleComponentVersion? ArticleVersion { get; set; }

    /// <summary>
    /// Версия квиза (если тип = Quiz)
    /// </summary>
    public virtual QuizComponentVersion? QuizVersion { get; set; }

    /// <summary>
    /// Версия задания (если тип = Task)
    /// </summary>
    public virtual TaskComponentVersion? TaskVersion { get; set; }

    /// <summary>
    /// Конструктор для EF Core
    /// </summary>
    protected ComponentVersion() { }

    /// <summary>
    /// Конструктор для создания новой версии компонента
    /// </summary>
    public ComponentVersion(
        Guid originalId,
        int version,
        Guid stepVersionId,
        string title,
        string description,
        ComponentType componentType,
        ComponentStatus status,
        string order,
        bool isRequired,
        int estimatedDurationMinutes,
        int? maxAttempts,
        int? minPassingScore,
        string instructions,
        bool isActive = false)
    {
        Id = Guid.NewGuid();
        OriginalId = originalId;
        Version = version;
        StepVersionId = stepVersionId;
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        ComponentType = componentType;
        Status = status;
        Order = order ?? throw new ArgumentNullException(nameof(order));
        IsRequired = isRequired;
        EstimatedDurationMinutes = estimatedDurationMinutes;
        MaxAttempts = maxAttempts;
        MinPassingScore = minPassingScore;
        Instructions = instructions ?? string.Empty;
        IsActive = isActive;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Создать новую версию на основе текущей
    /// </summary>
    public ComponentVersion CreateNewVersion()
    {
        return new ComponentVersion(
            OriginalId,
            Version + 1,
            StepVersionId,
            Title,
            Description,
            ComponentType,
            ComponentStatus.Draft, // Новая версия всегда начинается как черновик
            Order,
            IsRequired,
            EstimatedDurationMinutes,
            MaxAttempts,
            MinPassingScore,
            Instructions,
            false); // Новая версия не активна по умолчанию
    }

    /// <summary>
    /// Активировать эту версию
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Деактивировать эту версию
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Обновить метаданные версии компонента
    /// </summary>
    public void UpdateMetadata(
        string title,
        string description,
        string order,
        bool isRequired,
        int estimatedDurationMinutes,
        int? maxAttempts,
        int? minPassingScore,
        string instructions)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Order = order ?? throw new ArgumentNullException(nameof(order));
        IsRequired = isRequired;
        EstimatedDurationMinutes = estimatedDurationMinutes;
        MaxAttempts = maxAttempts;
        MinPassingScore = minPassingScore;
        Instructions = instructions ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Изменить статус версии компонента
    /// </summary>
    public void ChangeStatus(ComponentStatus newStatus)
    {
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }
}