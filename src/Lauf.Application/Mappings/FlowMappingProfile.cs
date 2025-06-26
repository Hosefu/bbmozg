using AutoMapper;
using Lauf.Application.DTOs.Flows;
using Lauf.Domain.Entities.Flows;
using Lauf.Shared.Helpers;

namespace Lauf.Application.Mappings;

/// <summary>
/// Профиль маппинга для потоков
/// </summary>
public class FlowMappingProfile : Profile
{
    public FlowMappingProfile()
    {
        // Маппинг Flow -> FlowDto
        CreateMap<Flow, FlowDto>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => ParseTags(src.Tags)))
            .ForMember(dest => dest.TotalSteps, opt => opt.MapFrom(src => src.TotalSteps));

        // Маппинг FlowSettings -> FlowSettingsDto
        CreateMap<FlowSettings, FlowSettingsDto>()
            .ForMember(dest => dest.AllowSkipping, opt => opt.MapFrom(src => !src.RequiresBuddy))
            .ForMember(dest => dest.RequireSequentialCompletion, opt => opt.MapFrom(src => !src.AllowSelfPaced))
            .ForMember(dest => dest.MaxAttempts, opt => opt.Ignore())
            .ForMember(dest => dest.TimeToCompleteWorkingDays, opt => opt.MapFrom(src => src.DaysToComplete))
            .ForMember(dest => dest.ShowProgress, opt => opt.MapFrom(src => src.SendDailyProgress))
            .ForMember(dest => dest.AllowRetry, opt => opt.MapFrom(src => src.AllowPause))
            .ForMember(dest => dest.SendReminders, opt => opt.MapFrom(src => src.SendDeadlineReminders))
            .ForMember(dest => dest.AdditionalSettings, opt => opt.MapFrom(src => ParseAdditionalSettings(src.CustomSettings)));

        // Маппинг FlowStep -> FlowStepDto
        CreateMap<FlowStep, FlowStepDto>()
            .ForMember(dest => dest.TotalComponents, opt => opt.MapFrom(src => src.TotalComponents))
            .ForMember(dest => dest.RequiredComponents, opt => opt.MapFrom(src => src.RequiredComponents));

        // Маппинг FlowStepComponent -> FlowStepComponentDto
        CreateMap<FlowStepComponent, FlowStepComponentDto>()
            .ForMember(dest => dest.Settings, opt => opt.MapFrom(src => ParseSettings(src.Settings)));
    }

    private static List<string> ParseTags(string tagsJson)
    {
        try
        {
            return JsonHelper.Deserialize<List<string>>(tagsJson) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    private static Dictionary<string, object> ParseAdditionalSettings(string settingsJson)
    {
        try
        {
            return JsonHelper.Deserialize<Dictionary<string, object>>(settingsJson) ?? new Dictionary<string, object>();
        }
        catch
        {
            return new Dictionary<string, object>();
        }
    }

    private static Dictionary<string, object> ParseSettings(string settingsJson)
    {
        try
        {
            return JsonHelper.Deserialize<Dictionary<string, object>>(settingsJson) ?? new Dictionary<string, object>();
        }
        catch
        {
            return new Dictionary<string, object>();
        }
    }
}