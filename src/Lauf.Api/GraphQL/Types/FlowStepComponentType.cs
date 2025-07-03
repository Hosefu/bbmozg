using Lauf.Application.DTOs.Flows;
using Lauf.Api.GraphQL.Types.Components;

namespace Lauf.Api.GraphQL.Types;

/// <summary>
/// GraphQL тип для компонента шага потока
/// </summary>
public class FlowStepComponentType : ObjectType<FlowStepComponentDto>
{
    protected override void Configure(IObjectTypeDescriptor<FlowStepComponentDto> descriptor)
    {
        descriptor.Name("FlowStepComponent");
        descriptor.Description("Компонент шага потока обучения");

        descriptor.Field(f => f.Id)
            .Description("Уникальный идентификатор компонента");

        descriptor.Field(f => f.FlowStepId)
            .Description("Идентификатор шага");

        descriptor.Field(f => f.ComponentId)
            .Description("Идентификатор связанного компонента контента");

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

        // Поле компонента с данными
        descriptor.Field(f => f.Component)
            .Type<ComponentUnionType>()
            .Description("Данные компонента (статья, квиз или задание)");
    }
}