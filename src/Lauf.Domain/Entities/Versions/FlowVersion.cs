using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Enums;
using Lauf.Domain.Interfaces;
using Lauf.Domain.ValueObjects;

namespace Lauf.Domain.Entities.Versions;

/// <summary>
/// Версия потока обучения
/// </summary>
public class FlowVersion : IVersionedEntity<FlowVersion>
{
    /// <summary>
    /// Уникальный идентификатор версии
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Идентификатор оригинального потока
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
    /// Название потока
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// Описание потока
    /// </summary>
    [Required]
    [StringLength(1000)]
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Теги потока (разделенные запятыми)
    /// </summary>
    public string Tags { get; private set; } = string.Empty;

    /// <summary>
    /// Статус потока
    /// </summary>
    public FlowStatus Status { get; private set; } = FlowStatus.Draft;

    /// <summary>
    /// Приоритет потока
    /// </summary>
    public int Priority { get; private set; }

    /// <summary>
    /// Является ли поток обязательным
    /// </summary>
    public bool IsRequired { get; private set; }

    /// <summary>
    /// Идентификатор создателя
    /// </summary>
    public Guid CreatedById { get; private set; }

    /// <summary>
    /// Дата создания версии
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Дата последнего обновления версии
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Дата публикации (если опубликован)
    /// </summary>
    public DateTime? PublishedAt { get; private set; }

    /// <summary>
    /// Коллекция версий этапов
    /// </summary>
    public virtual ICollection<FlowStepVersion> StepVersions { get; set; } = new List<FlowStepVersion>();

    /// <summary>
    /// Коллекция назначений, использующих эту версию
    /// </summary>
    public virtual ICollection<FlowAssignment> Assignments { get; set; } = new List<FlowAssignment>();

    /// <summary>
    /// Конструктор для EF Core
    /// </summary>
    protected FlowVersion() { }

    /// <summary>
    /// Конструктор для создания новой версии
    /// </summary>
    public FlowVersion(
        Guid originalId,
        int version,
        string title,
        string description,
        string tags,
        FlowStatus status,
        int priority,
        bool isRequired,
        Guid createdById,
        bool isActive = false)
    {
        Id = Guid.NewGuid();
        OriginalId = originalId;
        Version = version;
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Tags = tags ?? string.Empty;
        Status = status;
        Priority = priority;
        IsRequired = isRequired;
        CreatedById = createdById;
        IsActive = isActive;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        if (status == FlowStatus.Published)
        {
            PublishedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Создать новую версию на основе текущей
    /// </summary>
    public FlowVersion CreateNewVersion()
    {
        return new FlowVersion(
            OriginalId,
            Version + 1,
            Title,
            Description,
            Tags,
            FlowStatus.Draft, // Новая версия всегда начинается как черновик
            Priority,
            IsRequired,
            CreatedById,
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
    /// Обновить метаданные версии
    /// </summary>
    public void UpdateMetadata(
        string title,
        string description,
        string tags,
        int priority,
        bool isRequired)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Tags = tags ?? string.Empty;
        Priority = priority;
        IsRequired = isRequired;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Изменить статус версии
    /// </summary>
    public void ChangeStatus(FlowStatus newStatus)
    {
        var oldStatus = Status;
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;

        // Установить дату публикации при первой публикации
        if (newStatus == FlowStatus.Published && oldStatus != FlowStatus.Published)
        {
            PublishedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Добавить версию этапа
    /// </summary>
    public void AddStepVersion(FlowStepVersion stepVersion)
    {
        if (stepVersion == null)
            throw new ArgumentNullException(nameof(stepVersion));

        StepVersions.Add(stepVersion);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Удалить версию этапа
    /// </summary>
    public void RemoveStepVersion(FlowStepVersion stepVersion)
    {
        if (stepVersion == null)
            throw new ArgumentNullException(nameof(stepVersion));

        StepVersions.Remove(stepVersion);
        UpdatedAt = DateTime.UtcNow;
    }
}