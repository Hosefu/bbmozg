using Lauf.Application.DTOs.Flows;

namespace Lauf.Api.GraphQL.Types;

/// <summary>
/// GraphQL тип для назначения потока обучения
/// </summary>
public class FlowAssignmentType : ObjectType<FlowAssignmentDto>
{
    protected override void Configure(IObjectTypeDescriptor<FlowAssignmentDto> descriptor)
    {
        descriptor.Name("FlowAssignment");
        descriptor.Description("Назначение потока обучения пользователю");

        descriptor.Field(f => f.Id)
            .Description("Уникальный идентификатор назначения");

        descriptor.Field(f => f.UserId)
            .Description("Идентификатор пользователя");

        descriptor.Field(f => f.FlowId)
            .Description("Идентификатор потока");

        descriptor.Field(f => f.Status)
            .Description("Статус прохождения");

        descriptor.Field(f => f.CreatedAt)
            .Description("Дата создания назначения");

        descriptor.Field(f => f.StartedAt)
            .Description("Дата начала прохождения");

        descriptor.Field(f => f.CompletedAt)
            .Description("Дата завершения прохождения");

        descriptor.Field(f => f.DueDate)
            .Description("Крайний срок выполнения");

        descriptor.Field(f => f.ProgressPercentage)
            .Description("Процент прогресса выполнения");

        descriptor.Field(f => f.Notes)
            .Description("Заметки о назначении");

        descriptor.Field(f => f.Priority)
            .Description("Приоритет выполнения");

        descriptor.Field(f => f.User)
            .Description("Пользователь")
            .Type<UserType>();

        descriptor.Field(f => f.Flow)
            .Description("Поток обучения")
            .Type<FlowType>();

        descriptor.Field(f => f.AssignedBy)
            .Description("Назначивший пользователь")
            .Type<UserType>();

        descriptor.Field(f => f.Buddy)
            .Description("Куратор")
            .Type<UserType>();
    }
}