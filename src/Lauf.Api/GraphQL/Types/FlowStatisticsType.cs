using Lauf.Application.Queries.Flows;

namespace Lauf.Api.GraphQL.Types;

/// <summary>
/// GraphQL тип для статистики потока
/// </summary>
public class FlowStatisticsType : ObjectType<FlowStatisticsDto>
{
    protected override void Configure(IObjectTypeDescriptor<FlowStatisticsDto> descriptor)
    {
        descriptor.Name("FlowStatistics");
        descriptor.Description("Статистика потока обучения");

        descriptor.Field(f => f.TotalAssignments)
            .Description("Общее количество назначений");

        descriptor.Field(f => f.ActiveAssignments)
            .Description("Количество активных назначений");

        descriptor.Field(f => f.CompletedAssignments)
            .Description("Количество завершенных назначений");

        descriptor.Field(f => f.AverageProgress)
            .Description("Средний прогресс выполнения");

        descriptor.Field(f => f.AverageCompletionTime)
            .Description("Среднее время прохождения");
    }
}