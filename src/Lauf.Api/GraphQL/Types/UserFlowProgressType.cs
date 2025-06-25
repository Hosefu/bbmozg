using Lauf.Application.Queries.Flows;

namespace Lauf.Api.GraphQL.Types;

/// <summary>
/// GraphQL тип для прогресса пользователя по потоку
/// </summary>
public class UserFlowProgressType : ObjectType<UserFlowProgressDto>
{
    protected override void Configure(IObjectTypeDescriptor<UserFlowProgressDto> descriptor)
    {
        descriptor.Name("UserFlowProgress");
        descriptor.Description("Прогресс пользователя по потоку обучения");

        descriptor.Field(f => f.OverallProgress)
            .Description("Общий прогресс (0-100)");

        descriptor.Field(f => f.CurrentStep)
            .Description("Текущий шаг");

        descriptor.Field(f => f.CompletedSteps)
            .Description("Завершенные шаги");

        descriptor.Field(f => f.TotalSteps)
            .Description("Общее количество шагов");

        descriptor.Field(f => f.StartedAt)
            .Description("Дата начала");

        descriptor.Field(f => f.LastActivityAt)
            .Description("Последняя активность");

        descriptor.Field(f => f.Status)
            .Description("Статус назначения");
    }
}