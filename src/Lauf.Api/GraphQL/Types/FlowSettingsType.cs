using Lauf.Application.DTOs.Flows;

namespace Lauf.Api.GraphQL.Types;

/// <summary>
/// GraphQL тип для настроек потока
/// </summary>
public class FlowSettingsType : ObjectType<FlowSettingsDto>
{
    protected override void Configure(IObjectTypeDescriptor<FlowSettingsDto> descriptor)
    {
        descriptor.Name("FlowSettings");
        descriptor.Description("Настройки потока обучения");

        descriptor.Field(f => f.DaysPerStep)
            .Description("Дней на шаг");

        descriptor.Field(f => f.RequireSequentialCompletionComponents)
            .Description("Требовать последовательное прохождение компонентов");

        descriptor.Field(f => f.AllowSelfRestart)
            .Description("Разрешить самостоятельный перезапуск");

        descriptor.Field(f => f.AllowSelfPause)
            .Description("Разрешить самостоятельную паузу");
    }
}