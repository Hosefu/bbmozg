using Lauf.Domain.Entities.Users;
using Lauf.Domain.Enums;

namespace Lauf.Domain.Entities.Flows;

/// <summary>
/// Поток обучения - координатор версий контента
/// </summary>
public class Flow
{
    /// <summary>
    /// Уникальный идентификатор потока
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Неизменное административное имя потока
    /// </summary>    
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Базовое описание потока
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Идентификатор создателя потока
    /// </summary>
    public Guid CreatedBy { get; set; }

    /// <summary>
    /// Дата создания потока
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Можно ли создавать назначения
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Идентификатор активной версии контента
    /// </summary>
    public Guid? ActiveContentId { get; set; }

    /// <summary>
    /// Создатель потока
    /// </summary>
    public virtual User CreatedByUser { get; set; } = null!;

    /// <summary>
    /// Настройки потока (не версионируются)
    /// </summary>
    public virtual FlowSettings Settings { get; set; } = null!;

    /// <summary>
    /// Активная версия контента
    /// </summary>
    public virtual FlowContent ActiveContent { get; set; } = null!;

    /// <summary>
    /// Все версии контента
    /// </summary>
    public virtual ICollection<FlowContent> Contents { get; set; } = new List<FlowContent>();

    /// <summary>
    /// Назначения потока
    /// </summary>
    public virtual ICollection<FlowAssignment> Assignments { get; set; } = new List<FlowAssignment>();

    /// <summary>
    /// Конструктор для создания нового потока
    /// </summary>
    /// <param name="name">Название потока</param>
    /// <param name="description">Описание потока</param>
    /// <param name="createdBy">Создатель потока</param>
    public Flow(string name, string description, Guid createdBy)
    {
        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        CreatedBy = createdBy;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    /// <summary>
    /// Конструктор для EF Core
    /// </summary>
    protected Flow() { }

    /// <summary>
    /// Получает общее количество шагов в активной версии контента
    /// </summary>
    public int TotalSteps => ActiveContent?.TotalSteps ?? 0;

    /// <summary>
    /// Проверяет, может ли поток быть активирован для создания назначений
    /// </summary>
    /// <returns>true, если поток готов к использованию</returns>
    public bool CanBeActivated()
    {
        return ActiveContent != null && 
               ActiveContent.Steps.Any() && 
               Settings != null && 
               !string.IsNullOrWhiteSpace(Name) &&
               !string.IsNullOrWhiteSpace(Description);
    }

    /// <summary>
    /// Активирует поток для создания назначений
    /// </summary>
    public void Activate()
    {
        if (!CanBeActivated())
            throw new InvalidOperationException("Поток не может быть активирован в текущем состоянии");

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Деактивирует поток (новые назначения нельзя создавать)
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Устанавливает активную версию контента
    /// </summary>
    /// <param name="contentId">Идентификатор версии контента</param>
    public void SetActiveContent(Guid? contentId)
    {
        ActiveContentId = contentId;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Создает новую версию контента
    /// </summary>
    /// <param name="createdBy">Создатель версии</param>
    /// <returns>Новая версия контента</returns>
    public FlowContent CreateNewContentVersion(Guid createdBy)
    {
        var nextVersion = Contents.Any() ? Contents.Max(c => c.Version) + 1 : 1;
        var newContent = new FlowContent(Id, nextVersion, createdBy);
        Contents.Add(newContent);
        return newContent;
    }
}