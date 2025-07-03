using HotChocolate.Types;
using Lauf.Application.DTOs.Components;
using Lauf.Domain.Enums;

namespace Lauf.Api.GraphQL.Types.Components;

/// <summary>
/// Wrapper для компонента с метаданными
/// </summary>
public class ComponentWithMetadata
{
    /// <summary>
    /// Идентификатор компонента
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Тип компонента
    /// </summary>
    public ComponentType ComponentType { get; set; }

    /// <summary>
    /// Название компонента
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание компонента
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Порядковый номер компонента
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Обязательный ли компонент
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Включен ли компонент
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Данные компонента
    /// </summary>
    public object Component { get; set; } = null!;
}

/// <summary>
/// GraphQL тип для компонента с метаданными
/// </summary>
public class ComponentWithMetadataType : ObjectType<ComponentWithMetadata>
{
    protected override void Configure(IObjectTypeDescriptor<ComponentWithMetadata> descriptor)
    {
        descriptor.Name("ComponentWithMetadata");
        descriptor.Description("Компонент с метаданными");

        descriptor.Field(f => f.Id)
            .Description("Уникальный идентификатор компонента");

        descriptor.Field(f => f.ComponentType)
            .Description("Тип компонента");

        descriptor.Field(f => f.Title)
            .Description("Название компонента");

        descriptor.Field(f => f.Description)
            .Description("Описание компонента");

        descriptor.Field(f => f.Order)
            .Description("Порядковый номер компонента");

        descriptor.Field(f => f.IsRequired)
            .Description("Обязательный ли компонент");

        descriptor.Field(f => f.IsEnabled)
            .Description("Включен ли компонент");

        descriptor.Field(f => f.Component)
            .Description("Данные компонента")
            .Type<ComponentUnionType>();
    }
} 