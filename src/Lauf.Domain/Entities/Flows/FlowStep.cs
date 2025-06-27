using Lauf.Domain.Enums;
using Lauf.Domain.Entities.Components;
using Lauf.Shared.Helpers;

namespace Lauf.Domain.Entities.Flows;

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
    /// LexoRank позиция шага в потоке для динамической сортировки
    /// </summary>
    public string Order { get; set; } = string.Empty;

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
    public virtual ICollection<ComponentBase> Components { get; set; } = new List<ComponentBase>();

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Конструктор для создания нового шага потока
    /// </summary>
    /// <param name="flowId">Идентификатор потока</param>
    /// <param name="title">Название шага</param>
    /// <param name="description">Описание шага</param>
    /// <param name="order">Порядковый номер</param>
    /// <param name="isRequired">Обязательный ли шаг</param>
    /// <param name="estimatedDurationMinutes">Приблизительное время выполнения в минутах</param>
    public FlowStep(Guid flowId, string title, string description, string order, bool isRequired = true, int estimatedDurationMinutes = 30)
    {
        Id = Guid.NewGuid();
        FlowId = flowId;
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Order = order;
        IsRequired = isRequired;
        EstimatedDurationMinutes = estimatedDurationMinutes;
        Status = StepStatus.Draft;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Конструктор для EF Core
    /// </summary>
    protected FlowStep() { }

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
    public void AddComponent(ComponentBase component)
    {
        if (component == null) throw new ArgumentNullException(nameof(component));
        
        // Устанавливаем связь с шагом
        if (component.FlowStepId == Guid.Empty)
        {
            // Если компонент создан без FlowStepId, устанавливаем его
            var componentType = component.GetType();
            var flowStepIdProperty = componentType.GetProperty(nameof(ComponentBase.FlowStepId));
            flowStepIdProperty?.SetValue(component, Id);
        }
        
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
    /// Переупорядочивает компоненты после удаления (для LexoRank это не требуется)
    /// </summary>
    private void ReorderComponents()
    {
        // LexoRank не требует переупорядочивания после удаления
        // Каждый элемент уже имеет свою уникальную позицию
    }
}