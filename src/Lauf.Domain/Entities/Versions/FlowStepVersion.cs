using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Lauf.Domain.Enums;
using Lauf.Domain.Interfaces;
using Lauf.Domain.ValueObjects;

namespace Lauf.Domain.Entities.Versions;

/// <summary>
/// Версия этапа потока обучения
/// </summary>
public class FlowStepVersion : IVersionedEntity<FlowStepVersion>
{
    /// <summary>
    /// Уникальный идентификатор версии этапа
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Идентификатор оригинального этапа
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
    /// Идентификатор версии потока
    /// </summary>
    public Guid FlowVersionId { get; private set; }

    /// <summary>
    /// Название этапа
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// Описание этапа
    /// </summary>
    [Required]
    [StringLength(1000)]
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Порядок этапа (LexoRank)
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Order { get; private set; } = string.Empty;

    /// <summary>
    /// Является ли этап обязательным
    /// </summary>
    public bool IsRequired { get; private set; } = true;

    /// <summary>
    /// Оценочное время выполнения в минутах
    /// </summary>
    public int EstimatedDurationMinutes { get; private set; }

    /// <summary>
    /// Статус этапа
    /// </summary>
    public StepStatus Status { get; private set; } = StepStatus.Draft;

    /// <summary>
    /// Инструкции для этапа
    /// </summary>
    [StringLength(2000)]
    public string Instructions { get; private set; } = string.Empty;

    /// <summary>
    /// Заметки по этапу
    /// </summary>
    public string Notes { get; private set; } = string.Empty;

    /// <summary>
    /// Дата создания версии
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Дата последнего обновления версии
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Версия потока, к которой принадлежит этот этап
    /// </summary>
    public virtual FlowVersion FlowVersion { get; set; } = null!;

    /// <summary>
    /// Коллекция версий компонентов этого этапа
    /// </summary>
    public virtual ICollection<ComponentVersion> ComponentVersions { get; set; } = new List<ComponentVersion>();

    /// <summary>
    /// Конструктор для EF Core
    /// </summary>
    protected FlowStepVersion() { }

    /// <summary>
    /// Конструктор для создания новой версии этапа
    /// </summary>
    public FlowStepVersion(
        Guid originalId,
        int version,
        Guid flowVersionId,
        string title,
        string description,
        string order,
        bool isRequired,
        int estimatedDurationMinutes,
        StepStatus status,
        string instructions,
        string notes,
        bool isActive = false)
    {
        Id = Guid.NewGuid();
        OriginalId = originalId;
        Version = version;
        FlowVersionId = flowVersionId;
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Order = order ?? throw new ArgumentNullException(nameof(order));
        IsRequired = isRequired;
        EstimatedDurationMinutes = estimatedDurationMinutes;
        Status = status;
        Instructions = instructions ?? string.Empty;
        Notes = notes ?? string.Empty;
        IsActive = isActive;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Создать новую версию на основе текущей
    /// </summary>
    public FlowStepVersion CreateNewVersion()
    {
        return new FlowStepVersion(
            OriginalId,
            Version + 1,
            FlowVersionId,
            Title,
            Description,
            Order,
            IsRequired,
            EstimatedDurationMinutes,
            StepStatus.Draft, // Новая версия всегда начинается как черновик
            Instructions,
            Notes,
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
    /// Обновить метаданные версии этапа
    /// </summary>
    public void UpdateMetadata(
        string title,
        string description,
        string order,
        bool isRequired,
        int estimatedDurationMinutes,
        string instructions,
        string notes)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Order = order ?? throw new ArgumentNullException(nameof(order));
        IsRequired = isRequired;
        EstimatedDurationMinutes = estimatedDurationMinutes;
        Instructions = instructions ?? string.Empty;
        Notes = notes ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Изменить статус версии этапа
    /// </summary>
    public void ChangeStatus(StepStatus newStatus)
    {
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Добавить версию компонента
    /// </summary>
    public void AddComponentVersion(ComponentVersion componentVersion)
    {
        if (componentVersion == null)
            throw new ArgumentNullException(nameof(componentVersion));

        ComponentVersions.Add(componentVersion);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Удалить версию компонента
    /// </summary>
    public void RemoveComponentVersion(ComponentVersion componentVersion)
    {
        if (componentVersion == null)
            throw new ArgumentNullException(nameof(componentVersion));

        ComponentVersions.Remove(componentVersion);
        UpdatedAt = DateTime.UtcNow;
    }
}