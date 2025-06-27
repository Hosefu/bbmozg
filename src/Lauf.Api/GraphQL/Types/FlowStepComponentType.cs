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

        descriptor.Field(f => f.Status)
            .Description("Статус компонента");

        descriptor.Field(f => f.EstimatedDurationMinutes)
            .Description("Приблизительное время выполнения в минутах");

        descriptor.Field(f => f.MaxAttempts)
            .Description("Максимальное количество попыток");

        descriptor.Field(f => f.MinPassingScore)
            .Description("Минимальный проходной балл");

        descriptor.Field(f => f.Settings)
            .Description("Настройки компонента");

        descriptor.Field(f => f.Instructions)
            .Description("Инструкции для компонента");

        descriptor.Field(f => f.CreatedAt)
            .Description("Дата создания");

        descriptor.Field(f => f.UpdatedAt)
            .Description("Дата последнего обновления");

        // Поле компонента с данными
        descriptor.Field(f => f.Component)
            .Type<ComponentUnionType>()
            .Description("Данные компонента (статья, квиз или задание)");
    }
}