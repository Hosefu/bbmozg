using Lauf.Application.Queries.Flows;

namespace Lauf.Api.GraphQL.Types;

/// <summary>
/// GraphQL тип для детальной информации о потоке
/// </summary>
public class FlowDetailsType : ObjectType<FlowDetailsDto>
{
    protected override void Configure(IObjectTypeDescriptor<FlowDetailsDto> descriptor)
    {
        descriptor.Name("FlowDetails");
        descriptor.Description("Детальная информация о потоке обучения");

        // Наследуем все поля от FlowType
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

        // Дополнительные поля для детального просмотра
        descriptor.Field(f => f.Steps)
            .Description("Полная информация о шагах")
            .Type<ListType<FlowStepDetailsType>>();

        descriptor.Field(f => f.Statistics)
            .Description("Статистика потока")
            .Type<FlowStatisticsType>();

        descriptor.Field(f => f.UserProgress)
            .Description("Прогресс пользователя")
            .Type<UserFlowProgressType>();
    }
}