using Lauf.Application.DTOs.Flows;

namespace Lauf.Api.GraphQL.Types;

/// <summary>
/// GraphQL тип для потока обучения
/// </summary>
public class FlowType : ObjectType<FlowDto>
{
    protected override void Configure(IObjectTypeDescriptor<FlowDto> descriptor)
    {
        descriptor.Name("Flow");
        descriptor.Description("Поток обучения");

        descriptor.Field(f => f.Id)
            .Description("Уникальный идентификатор потока");

        descriptor.Field(f => f.Title)
            .Description("Название потока");

        descriptor.Field(f => f.Description)
            .Description("Описание потока");


        descriptor.Field(f => f.Tags)
            .Description("Теги потока");

        descriptor.Field(f => f.Status)
            .Description("Статус потока");


        descriptor.Field(f => f.Priority)
            .Description("Приоритет потока");

        descriptor.Field(f => f.IsRequired)
            .Description("Обязательный ли поток");

        descriptor.Field(f => f.CreatedById)
            .Description("Автор потока");

        descriptor.Field(f => f.CreatedAt)
            .Description("Дата создания потока");

        descriptor.Field(f => f.UpdatedAt)
            .Description("Дата последнего обновления");

        descriptor.Field(f => f.PublishedAt)
            .Description("Дата публикации");

        descriptor.Field(f => f.TotalSteps)
            .Description("Общее количество шагов");


        descriptor.Field(f => f.Settings)
            .Description("Настройки потока")
            .Type<FlowSettingsType>();

        descriptor.Field(f => f.Steps)
            .Description("Шаги потока")
            .Type<ListType<FlowStepType>>();
    }
}