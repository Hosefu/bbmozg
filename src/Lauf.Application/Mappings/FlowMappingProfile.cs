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
        // Маппинг Flow -> FlowDto (обновленная архитектура)
        CreateMap<Flow, FlowDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.TotalSteps, opt => opt.MapFrom(src => src.ActiveContent != null ? src.ActiveContent.Steps.Count : 0))
            .ForMember(dest => dest.Steps, opt => opt.MapFrom((src, dest, destMember, context) => 
                src.ActiveContent != null ? MapStepsWithOrder(src.ActiveContent.Steps, context) : new List<FlowStepDto>()));

        // Маппинг FlowContent -> FlowContentDto
        CreateMap<FlowContent, FlowContentDto>()
            .ForMember(dest => dest.Steps, opt => opt.MapFrom((src, dest, destMember, context) => 
                MapStepsWithOrder(src.Steps, context)));

        // Маппинг FlowSettings -> FlowSettingsDto (упрощенный)
        CreateMap<FlowSettings, FlowSettingsDto>();

        // Маппинг FlowStep -> FlowStepDto (обновленная архитектура)
        CreateMap<FlowStep, FlowStepDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.FlowContentId, opt => opt.MapFrom(src => src.FlowContentId))
            .ForMember(dest => dest.IsEnabled, opt => opt.MapFrom(src => src.IsEnabled))
            .ForMember(dest => dest.Order, opt => opt.Ignore())
            .ForMember(dest => dest.TotalComponents, opt => opt.MapFrom(src => src.Components.Count))
            .ForMember(dest => dest.RequiredComponents, opt => opt.MapFrom(src => src.Components.Count(c => c.IsRequired)))
            .ForMember(dest => dest.Components, opt => opt.MapFrom((src, dest, destMember, context) => 
                MapComponentsWithOrder(src.Components, context)));

        // Маппинг FlowStep -> FlowStepDetailsDto
        CreateMap<FlowStep, FlowStepDetailsDto>()
            .IncludeBase<FlowStep, FlowStepDto>()
            .ForMember(dest => dest.Components, opt => opt.MapFrom((src, dest, destMember, context) => 
                MapComponentsWithOrderDetailed(src.Components, context)));

        // Маппинг ComponentBase -> FlowStepComponentDto (обновленная архитектура)
        CreateMap<ComponentBase, FlowStepComponentDto>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.IsEnabled, opt => opt.MapFrom(src => src.IsEnabled))
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

        // Маппинг компонентов (обновленная архитектура)
        CreateMap<ArticleComponent, ArticleComponentDto>();
        CreateMap<QuizComponent, QuizComponentDto>()
            .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.Questions));
        CreateMap<TaskComponent, TaskComponentDto>();

        // Маппинг вопросов и вариантов ответов
        CreateMap<QuizQuestion, QuizQuestionDto>()
            .ForMember(dest => dest.Options, opt => opt.MapFrom((src, dest, destMember, context) => 
                MapOptionsWithOrder(src.Options.ToList(), context)));
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
    /// Извлекает содержимое компонента в зависимости от его типа (обновленная архитектура)
    /// </summary>
    private static string MapComponentContent(ComponentBase component)
    {
        return component switch
        {
            ArticleComponent article => article.Content,
            QuizComponent quiz => quiz.Content,
            TaskComponent task => task.Content,
            _ => component.Description
        };
    }
}