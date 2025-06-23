using AutoMapper;
using BuddyBot.Application.DTOs.Flows;
using BuddyBot.Domain.Entities.Flows;
using BuddyBot.Shared.Helpers;

namespace BuddyBot.Application.Mappings;

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
            .ForMember(dest => dest.TotalSteps, opt => opt.MapFrom(src => src.TotalSteps))
            .ForMember(dest => dest.EstimatedDurationMinutes, opt => opt.MapFrom(src => src.EstimatedDurationMinutes));

        // TODO: Маппинг FlowSettings -> FlowSettingsDto будет добавлен позже
        // CreateMap<FlowSettings, FlowSettingsDto>();

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