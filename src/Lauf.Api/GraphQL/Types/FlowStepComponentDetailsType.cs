using Lauf.Application.Queries.Flows;

namespace Lauf.Api.GraphQL.Types;

/// <summary>
/// GraphQL тип для детальной информации о компоненте шага
/// </summary>
public class FlowStepComponentDetailsType : ObjectType<FlowStepComponentDetailsDto>
{
    protected override void Configure(IObjectTypeDescriptor<FlowStepComponentDetailsDto> descriptor)
    {
        descriptor.Name("FlowStepComponentDetails");
        descriptor.Description("Детальная информация о компоненте шага потока");

        descriptor.Field(f => f.Id)
            .Description("Идентификатор компонента");

        descriptor.Field(f => f.Title)
            .Description("Название компонента");

        descriptor.Field(f => f.Description)
            .Description("Описание компонента");

        descriptor.Field(f => f.ComponentType)
            .Description("Тип компонента");

        descriptor.Field(f => f.Content)
            .Description("Контент компонента");

        descriptor.Field(f => f.Settings)
            .Description("Настройки компонента");

        descriptor.Field(f => f.IsRequired)
            .Description("Обязателен ли компонент");

        descriptor.Field(f => f.Order)
            .Description("Порядок сортировки");

        descriptor.Field(f => f.Progress)
            .Description("Прогресс выполнения компонента")
            .Type<ComponentProgressType>();
    }
}