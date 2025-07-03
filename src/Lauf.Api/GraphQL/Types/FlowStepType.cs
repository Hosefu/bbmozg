using Lauf.Application.DTOs.Flows;
using Lauf.Api.GraphQL.Types.Components;

namespace Lauf.Api.GraphQL.Types;

/// <summary>
/// GraphQL тип для шага потока обучения
/// </summary>
public class FlowStepType : ObjectType<FlowStepDto>
{
    protected override void Configure(IObjectTypeDescriptor<FlowStepDto> descriptor)
    {
        descriptor.Name("FlowStep");
        descriptor.Description("Шаг потока обучения");

        descriptor.Field(f => f.Id)
            .Description("Уникальный идентификатор шага");

        descriptor.Field(f => f.FlowContentId)
            .Description("Идентификатор содержимого потока");

        descriptor.Field(f => f.Name)
            .Description("Название шага");

        descriptor.Field(f => f.Description)
            .Description("Описание шага");

        descriptor.Field(f => f.Order)
            .Description("Порядковый номер шага");

        descriptor.Field(f => f.IsRequired)
            .Description("Обязательность прохождения шага");

        descriptor.Field(f => f.EstimatedDurationMinutes)
            .Description("Ожидаемое время прохождения в минутах");

        descriptor.Field(f => f.IsEnabled)
            .Description("Включен ли шаг");

        descriptor.Field(f => f.Instructions)
            .Description("Инструкции для прохождения шага");

        descriptor.Field(f => f.Notes)
            .Description("Дополнительные заметки");

        descriptor.Field(f => f.CreatedAt)
            .Description("Дата создания");

        descriptor.Field(f => f.UpdatedAt)
            .Description("Дата последнего обновления");

        descriptor.Field(f => f.TotalComponents)
            .Description("Общее количество компонентов");

        descriptor.Field(f => f.RequiredComponents)
            .Description("Количество обязательных компонентов");

        // Использование нового ComponentWithMetadata типа
        descriptor.Field("components")
            .Description("Компоненты шага с метаданными")
            .Type<ListType<ComponentWithMetadataType>>()
            .Resolve(context =>
            {
                var step = context.Parent<FlowStepDto>();
                if (step.Components == null || !step.Components.Any())
                    return new List<ComponentWithMetadata>();
                
                // Преобразуем FlowStepComponentDto в ComponentWithMetadata
                return step.Components
                    .Where(c => c.Component != null)
                    .OrderBy(c => c.Order)
                    .Select(c => new ComponentWithMetadata
                    {
                        Id = c.ComponentId,
                        ComponentType = c.ComponentType,
                        Title = c.Title,
                        Description = c.Description,
                        Order = c.Order,
                        IsRequired = c.IsRequired,
                        IsEnabled = c.IsEnabled,
                        Component = c.Component!
                    })
                    .ToList();
            });
    }
}