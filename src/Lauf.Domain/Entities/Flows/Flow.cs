using Lauf.Domain.Enums;

namespace Lauf.Domain.Entities.Flows;

/// <summary>
/// Поток обучения - шаблон обучающей программы
/// </summary>
public class Flow
{
    /// <summary>
    /// Уникальный идентификатор потока
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название потока
    /// </summary>    
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание потока
    /// </summary>
    public string Description { get; set; } = string.Empty;


    /// <summary>
    /// Теги для поиска
    /// </summary>
    public string Tags { get; set; } = string.Empty;

    /// <summary>
    /// Статус потока
    /// </summary>
    public FlowStatus Status { get; set; } = FlowStatus.Draft;


    /// <summary>
    /// Приоритет отображения
    /// </summary>
    public int Priority { get; set; } = 0;

    /// <summary>
    /// Обязательный ли поток
    /// </summary>
    public bool IsRequired { get; set; } = false;

    /// <summary>
    /// Настройки потока
    /// </summary>
    public virtual FlowSettings Settings { get; set; } = null!;

    /// <summary>
    /// Шаги потока
    /// </summary>
    public virtual ICollection<FlowStep> Steps { get; set; } = new List<FlowStep>();

    /// <summary>
    /// Назначения потока
    /// </summary>
    public virtual ICollection<FlowAssignment> Assignments { get; set; } = new List<FlowAssignment>();

    /// <summary>
    /// Автор потока
    /// </summary>
    public Guid CreatedById { get; set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Дата публикации
    /// </summary>
    public DateTime? PublishedAt { get; set; }

    /// <summary>
    /// Конструктор для создания нового потока
    /// </summary>
    /// <param name="title">Название потока</param>
    /// <param name="description">Описание потока</param>
    public Flow(string title, string description)
    {
        Id = Guid.NewGuid();
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Status = FlowStatus.Draft;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Конструктор для EF Core
    /// </summary>
    protected Flow() { }

    /// <summary>
    /// Получает общее количество шагов в потоке
    /// </summary>
    public int TotalSteps => Steps.Count;


    /// <summary>
    /// Проверяет, может ли поток быть опубликован
    /// </summary>
    /// <returns>true, если поток готов к публикации</returns>
    public bool CanBePublished()
    {
        return Status == FlowStatus.Draft && 
               Steps.Any() && 
               Settings != null && 
               !string.IsNullOrWhiteSpace(Title) &&
               !string.IsNullOrWhiteSpace(Description);
    }

    /// <summary>
    /// Публикует поток
    /// </summary>
    public void Publish()
    {
        if (!CanBePublished())
            throw new InvalidOperationException("Поток не может быть опубликован в текущем состоянии");

        Status = FlowStatus.Published;
        PublishedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Архивирует поток
    /// </summary>
    public void Archive()
    {
        Status = FlowStatus.Archived;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Возвращает поток в черновик
    /// </summary>
    public void ReturnToDraft()
    {
        Status = FlowStatus.Draft;
        PublishedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }


    /// <summary>
    /// Добавляет шаг в поток
    /// </summary>
    /// <param name="step">Шаг для добавления</param>
    public void AddStep(FlowStep step)
    {
        step.Order = Steps.Count + 1;
        step.FlowId = Id;
        Steps.Add(step);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Удаляет шаг из потока
    /// </summary>
    /// <param name="stepId">Идентификатор шага</param>
    public void RemoveStep(Guid stepId)
    {
        var step = Steps.FirstOrDefault(s => s.Id == stepId);
        if (step != null)
        {
            Steps.Remove(step);
            ReorderSteps();
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Переупорядочивает шаги после удаления
    /// </summary>
    private void ReorderSteps()
    {
        var orderedSteps = Steps.OrderBy(s => s.Order).ToList();
        for (int i = 0; i < orderedSteps.Count; i++)
        {
            orderedSteps[i].Order = i + 1;
        }
    }
}