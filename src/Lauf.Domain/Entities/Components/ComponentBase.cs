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
    /// Содержимое компонента
    /// </summary>
    public string Content { get; protected set; } = string.Empty;

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
    /// Включен ли компонент (заменяет ComponentStatus)
    /// </summary>
    public bool IsEnabled { get; protected set; } = true;

    /// <summary>
    /// Конструктор для создания нового компонента
    /// </summary>
    /// <param name="flowStepId">Идентификатор шага потока</param>
    /// <param name="title">Название компонента</param>
    /// <param name="description">Описание компонента</param>
    /// <param name="content">Содержимое компонента</param>
    /// <param name="order">Порядковый номер компонента</param>
    /// <param name="isRequired">Обязательный ли компонент</param>
    protected ComponentBase(Guid flowStepId, string title, string description, string content, string order, bool isRequired = true)
    {
        Id = Guid.NewGuid();
        FlowStepId = flowStepId;
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Content = content ?? throw new ArgumentNullException(nameof(content));
        Order = order ?? throw new ArgumentNullException(nameof(order));
        IsRequired = isRequired;
        IsEnabled = true;
    }

    /// <summary>
    /// Конструктор для EF Core
    /// </summary>
    protected ComponentBase() { }

    /// <summary>
    /// Включает компонент
    /// </summary>
    public void Enable()
    {
        IsEnabled = true;
    }

    /// <summary>
    /// Отключает компонент
    /// </summary>
    public void Disable()
    {
        IsEnabled = false;
    }
}