using BuddyBot.Domain.Enums;

namespace BuddyBot.Domain.Entities.Flows;

/// <summary>
/// Шаг в потоке обучения
/// </summary>
public class FlowStep
{
    /// <summary>
    /// Уникальный идентификатор шага
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор потока
    /// </summary>
    public Guid FlowId { get; set; }

    /// <summary>
    /// Поток, которому принадлежит шаг
    /// </summary>
    public virtual Flow Flow { get; set; } = null!;

    /// <summary>
    /// Название шага
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание шага
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Порядковый номер шага в потоке
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Обязательный ли шаг
    /// </summary>
    public bool IsRequired { get; set; } = true;

    /// <summary>
    /// Приблизительное время выполнения в минутах
    /// </summary>
    public int EstimatedDurationMinutes { get; set; } = 30;

    /// <summary>
    /// Статус шага
    /// </summary>
    public StepStatus Status { get; set; } = StepStatus.Active;

    /// <summary>
    /// Инструкции для прохождения шага
    /// </summary>
    public string Instructions { get; set; } = string.Empty;

    /// <summary>
    /// Дополнительные заметки
    /// </summary>
    public string Notes { get; set; } = string.Empty;

    /// <summary>
    /// Компоненты шага
    /// </summary>
    public virtual ICollection<FlowStepComponent> Components { get; set; } = new List<FlowStepComponent>();

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Получает общее количество компонентов в шаге
    /// </summary>
    public int TotalComponents => Components.Count;

    /// <summary>
    /// Получает количество обязательных компонентов
    /// </summary>
    public int RequiredComponents => Components.Count(c => c.IsRequired);

    /// <summary>
    /// Проверяет, может ли шаг быть активирован
    /// </summary>
    /// <returns>true, если шаг может быть активирован</returns>
    public bool CanBeActivated()
    {
        return Status == StepStatus.Draft && 
               Components.Any() && 
               !string.IsNullOrWhiteSpace(Title);
    }

    /// <summary>
    /// Активирует шаг
    /// </summary>
    public void Activate()
    {
        if (!CanBeActivated())
            throw new InvalidOperationException("Шаг не может быть активирован в текущем состоянии");

        Status = StepStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Деактивирует шаг
    /// </summary>
    public void Deactivate()
    {
        Status = StepStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Возвращает шаг в черновик
    /// </summary>
    public void ReturnToDraft()
    {
        Status = StepStatus.Draft;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Добавляет компонент к шагу
    /// </summary>
    /// <param name="component">Компонент для добавления</param>
    public void AddComponent(FlowStepComponent component)
    {
        component.Order = Components.Count + 1;
        component.FlowStepId = Id;
        Components.Add(component);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Удаляет компонент из шага
    /// </summary>
    /// <param name="componentId">Идентификатор компонента</param>
    public void RemoveComponent(Guid componentId)
    {
        var component = Components.FirstOrDefault(c => c.Id == componentId);
        if (component != null)
        {
            Components.Remove(component);
            ReorderComponents();
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Переупорядочивает компоненты после удаления
    /// </summary>
    private void ReorderComponents()
    {
        var orderedComponents = Components.OrderBy(c => c.Order).ToList();
        for (int i = 0; i < orderedComponents.Count; i++)
        {
            orderedComponents[i].Order = i + 1;
        }
    }
}