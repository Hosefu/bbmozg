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
    /// Идентификатор версии контента потока
    /// </summary>
    public Guid FlowContentId { get; set; }

    /// <summary>
    /// Версия контента потока
    /// </summary>
    public virtual FlowContent FlowContent { get; set; } = null!;

    /// <summary>
    /// Название шага
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Описание шага
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// LexoRank позиция шага для динамической сортировки
    /// </summary>
    public string Order { get; set; } = string.Empty;

    /// <summary>
    /// Включен ли шаг (заменяет StepStatus)
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Компоненты шага
    /// </summary>
    public virtual ICollection<ComponentBase> Components { get; set; } = new List<ComponentBase>();

    /// <summary>
    /// Конструктор для создания нового шага потока
    /// </summary>
    /// <param name="flowContentId">Идентификатор версии контента</param>
    /// <param name="name">Название шага</param>
    /// <param name="description">Описание шага</param>
    /// <param name="order">Порядковый номер</param>
    public FlowStep(Guid flowContentId, string name, string description, string order)
    {
        Id = Guid.NewGuid();
        FlowContentId = flowContentId;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Order = order;
        IsEnabled = true;
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
    /// Включает шаг
    /// </summary>
    public void Enable()
    {
        IsEnabled = true;
    }

    /// <summary>
    /// Отключает шаг
    /// </summary>
    public void Disable()
    {
        IsEnabled = false;
    }

    /// <summary>
    /// Добавляет компонент к шагу
    /// </summary>
    /// <param name="component">Компонент для добавления</param>
    public void AddComponent(ComponentBase component)
    {
        if (component == null) throw new ArgumentNullException(nameof(component));
        
        Components.Add(component);
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
        }
    }
}