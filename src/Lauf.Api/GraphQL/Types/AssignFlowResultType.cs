using Lauf.Application.Commands.FlowAssignment;

namespace Lauf.Api.GraphQL.Types;

/// <summary>
/// GraphQL тип для результата назначения потока
/// </summary>
public class AssignFlowResultType : ObjectType<AssignFlowCommandResult>
{
    protected override void Configure(IObjectTypeDescriptor<AssignFlowCommandResult> descriptor)
    {
        descriptor.Name("AssignFlowResult");
        descriptor.Description("Результат назначения потока пользователю");

        descriptor.Field(f => f.AssignmentId)
            .Description("Идентификатор созданного назначения");

        descriptor.Field(f => f.FlowContentId)
            .Description("Идентификатор содержимого потока");

        descriptor.Field(f => f.IsSuccess)
            .Description("Успешно ли выполнено назначение");

        descriptor.Field(f => f.Message)
            .Description("Сообщение о результате");

        descriptor.Field(f => f.EstimatedCompletionDate)
            .Description("Расчетная дата завершения");
    }
}