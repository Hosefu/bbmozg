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

        descriptor.Field(f => f.AllowSkipping)
            .Description("Разрешить пропуск шагов");

        descriptor.Field(f => f.RequireSequentialCompletion)
            .Description("Требовать последовательное прохождение");

        descriptor.Field(f => f.MaxAttempts)
            .Description("Максимальное количество попыток");

        descriptor.Field(f => f.TimeToCompleteWorkingDays)
            .Description("Время на выполнение в рабочих днях");

        descriptor.Field(f => f.ShowProgress)
            .Description("Показывать прогресс");

        descriptor.Field(f => f.AllowRetry)
            .Description("Разрешить повторное прохождение");

        descriptor.Field(f => f.SendReminders)
            .Description("Отправлять напоминания");

        descriptor.Field(f => f.AdditionalSettings)
            .Description("Дополнительные настройки");
    }
}