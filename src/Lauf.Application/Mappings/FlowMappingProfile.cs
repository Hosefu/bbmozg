using AutoMapper;
using Lauf.Application.DTOs.Flows;
using Lauf.Domain.Entities.Flows;
using Lauf.Shared.Helpers;
using Lauf.Application.DTOs.Components;
using Lauf.Domain.Entities.Components;
using Lauf.Application.Queries.Flows;

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
            .ForMember(dest => dest.TotalSteps, opt => opt.MapFrom(src => src.TotalSteps))
            .ForMember(dest => dest.Steps, opt => opt.MapFrom((src, dest, destMember, context) => 
                MapStepsWithOrder(src.Steps, context)));

        // Маппинг Flow -> FlowDetailsDto
        CreateMap<Flow, FlowDetailsDto>()
            .IncludeBase<Flow, FlowDto>()
            .ForMember(dest => dest.Steps, opt => opt.MapFrom((src, dest, destMember, context) => 
                MapStepsWithOrderDetailed(src.Steps, context)))
            .ForMember(dest => dest.Statistics, opt => opt.Ignore())
            .ForMember(dest => dest.UserProgress, opt => opt.Ignore());

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
            .ForMember(dest => dest.Order, opt => opt.Ignore())
            .ForMember(dest => dest.TotalComponents, opt => opt.MapFrom(src => src.TotalComponents))
            .ForMember(dest => dest.RequiredComponents, opt => opt.MapFrom(src => src.RequiredComponents))
            .ForMember(dest => dest.Components, opt => opt.MapFrom((src, dest, destMember, context) => 
                MapComponentsWithOrder(src.Components, context)));

        // Маппинг FlowStep -> FlowStepDetailsDto
        CreateMap<FlowStep, FlowStepDetailsDto>()
            .IncludeBase<FlowStep, FlowStepDto>()
            .ForMember(dest => dest.Components, opt => opt.MapFrom((src, dest, destMember, context) => 
                MapComponentsWithOrderDetailed(src.Components, context)));

        // Маппинг ComponentBase -> FlowStepComponentDto
        CreateMap<ComponentBase, FlowStepComponentDto>()
            .ForMember(dest => dest.Order, opt => opt.Ignore())
            .ForMember(dest => dest.Settings, opt => opt.MapFrom(src => new Dictionary<string, object>()))
            .ForMember(dest => dest.Component, opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.ComponentId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ComponentType, opt => opt.MapFrom(src => src.Type));

        // Маппинг ComponentBase -> FlowStepComponentDetailsDto
        CreateMap<ComponentBase, FlowStepComponentDetailsDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.ComponentType, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => MapComponentContent(src)))
            .ForMember(dest => dest.Settings, opt => opt.MapFrom(src => new Dictionary<string, object>()))
            .ForMember(dest => dest.IsRequired, opt => opt.MapFrom(src => src.IsRequired))
            .ForMember(dest => dest.Order, opt => opt.Ignore())
            .ForMember(dest => dest.Progress, opt => opt.Ignore());

        // Маппинг компонентов
        CreateMap<ArticleComponent, ArticleComponentDto>();
        CreateMap<QuizComponent, QuizComponentDto>()
            .ForMember(dest => dest.Options, opt => opt.MapFrom((src, dest, destMember, context) => 
                MapOptionsWithOrder(src.Options, context)));
        CreateMap<TaskComponent, TaskComponentDto>();

        // QuestionOption -> QuestionOptionDto с преобразованием LexoRank в order
        CreateMap<QuestionOption, QuestionOptionDto>()
            .ForMember(dest => dest.Order, opt => opt.Ignore());
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

    /// <summary>
    /// Маппинг шагов с преобразованием LexoRank в числовые order
    /// </summary>
    private ICollection<FlowStepDto> MapStepsWithOrder(ICollection<FlowStep> steps, ResolutionContext context)
    {
        var orderedSteps = steps.OrderBy(s => s.Order).ToArray();
        var result = new List<FlowStepDto>();

        for (int i = 0; i < orderedSteps.Length; i++)
        {
            var stepDto = context.Mapper.Map<FlowStepDto>(orderedSteps[i]);
            stepDto.Order = i; // Порядковый номер начиная с 0
            result.Add(stepDto);
        }

        return result;
    }

    /// <summary>
    /// Маппинг компонентов с преобразованием LexoRank в числовые order
    /// </summary>
    private ICollection<FlowStepComponentDto> MapComponentsWithOrder(ICollection<ComponentBase> components, ResolutionContext context)
    {
        var orderedComponents = components.OrderBy(c => c.Order).ToArray();
        var result = new List<FlowStepComponentDto>();

        for (int i = 0; i < orderedComponents.Length; i++)
        {
            var componentDto = context.Mapper.Map<FlowStepComponentDto>(orderedComponents[i]);
            componentDto.Order = i; // Порядковый номер начиная с 0
            result.Add(componentDto);
        }

        return result;
    }

    /// <summary>
    /// Маппинг вариантов ответов с преобразованием LexoRank в числовые order
    /// </summary>
    private List<QuestionOptionDto> MapOptionsWithOrder(List<QuestionOption> options, ResolutionContext context)
    {
        var orderedOptions = options.OrderBy(o => o.Order).ToArray();
        var result = new List<QuestionOptionDto>();

        for (int i = 0; i < orderedOptions.Length; i++)
        {
            var optionDto = context.Mapper.Map<QuestionOptionDto>(orderedOptions[i]);
            optionDto.Order = i; // Порядковый номер начиная с 0
            result.Add(optionDto);
        }

        return result;
    }

    /// <summary>
    /// Маппинг шагов для детального просмотра с преобразованием LexoRank в числовые order
    /// </summary>
    private ICollection<FlowStepDetailsDto> MapStepsWithOrderDetailed(ICollection<FlowStep> steps, ResolutionContext context)
    {
        var orderedSteps = steps.OrderBy(s => s.Order).ToArray();
        var result = new List<FlowStepDetailsDto>();

        for (int i = 0; i < orderedSteps.Length; i++)
        {
            var stepDto = context.Mapper.Map<FlowStepDetailsDto>(orderedSteps[i]);
            stepDto.Order = i; // Порядковый номер начиная с 0
            result.Add(stepDto);
        }

        return result;
    }

    /// <summary>
    /// Маппинг компонентов для детального просмотра с преобразованием LexoRank в числовые order
    /// </summary>
    private ICollection<FlowStepComponentDetailsDto> MapComponentsWithOrderDetailed(ICollection<ComponentBase> components, ResolutionContext context)
    {
        var orderedComponents = components.OrderBy(c => c.Order).ToArray();
        var result = new List<FlowStepComponentDetailsDto>();

        for (int i = 0; i < orderedComponents.Length; i++)
        {
            var componentDto = context.Mapper.Map<FlowStepComponentDetailsDto>(orderedComponents[i]);
            componentDto.Order = i; // Порядковый номер начиная с 0
            result.Add(componentDto);
        }

        return result;
    }

    /// <summary>
    /// Извлекает содержимое компонента в зависимости от его типа
    /// </summary>
    private static string MapComponentContent(ComponentBase component)
    {
        return component switch
        {
            ArticleComponent article => article.Content,
            QuizComponent quiz => quiz.QuestionText,
            TaskComponent task => task.Instruction,
            _ => component.Description
        };
    }
}