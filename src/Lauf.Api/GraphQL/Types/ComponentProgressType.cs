using Lauf.Application.Queries.Flows;

namespace Lauf.Api.GraphQL.Types;

/// <summary>
/// GraphQL тип для прогресса компонента
/// </summary>
public class ComponentProgressType : ObjectType<ComponentProgressDto>
{
    protected override void Configure(IObjectTypeDescriptor<ComponentProgressDto> descriptor)
    {
        descriptor.Name("ComponentProgress");
        descriptor.Description("Прогресс выполнения компонента");

        descriptor.Field(f => f.IsCompleted)
            .Description("Завершен ли компонент");

        descriptor.Field(f => f.Progress)
            .Description("Прогресс выполнения (0-100)");

        descriptor.Field(f => f.Data)
            .Description("Данные прогресса");

        descriptor.Field(f => f.CompletedAt)
            .Description("Время завершения");
    }
}