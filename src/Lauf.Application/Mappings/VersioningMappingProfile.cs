using AutoMapper;
using Lauf.Application.DTOs.Flows;
using Lauf.Domain.Entities.Versions;
using Lauf.Domain.Entities.Flows;
using Lauf.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lauf.Application.Mappings;

/// <summary>
/// Профиль маппинга для версионируемых сущностей
/// </summary>
public class VersioningMappingProfile : Profile
{
    public VersioningMappingProfile()
    {
        // Маппинг FlowVersion -> FlowDto (для прозрачной работы API)
        CreateMap<FlowVersion, FlowDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.OriginalId)) // Возвращаем ID оригинального потока
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => ParseTags(src.Tags)))
            .ForMember(dest => dest.TotalSteps, opt => opt.MapFrom(src => src.StepVersions.Count))
            .ForMember(dest => dest.Steps, opt => opt.MapFrom((src, dest, destMember, context) => 
                MapStepVersionsToStepDtos(src.StepVersions, context)))
            .ForMember(dest => dest.Settings, opt => opt.Ignore()) // Пока игнорируем настройки
            .ForMember(dest => dest.PublishedAt, opt => opt.Ignore()); // Пока игнорируем дату публикации

        // Маппинг FlowVersion -> FlowVersionDto (для внутреннего использования)
        CreateMap<FlowVersion, FlowVersionDto>()
            .ForMember(dest => dest.StepVersions, opt => opt.MapFrom((src, dest, destMember, context) => 
                MapStepVersionsWithOrder(src.StepVersions, context)));

        // Маппинг FlowStepVersion -> FlowStepDto (для прозрачной работы API)
        CreateMap<FlowStepVersion, FlowStepDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.OriginalId)) // Возвращаем ID оригинального этапа
            .ForMember(dest => dest.FlowId, opt => opt.MapFrom(src => src.FlowVersion.OriginalId))
            .ForMember(dest => dest.TotalComponents, opt => opt.MapFrom(src => src.ComponentVersions.Count))
            .ForMember(dest => dest.RequiredComponents, opt => opt.MapFrom(src => src.ComponentVersions.Count(cv => cv.IsRequired)))
            .ForMember(dest => dest.Components, opt => opt.MapFrom((src, dest, destMember, context) => 
                MapComponentVersionsToComponentDtos(src.ComponentVersions, context)));

        // Маппинг FlowStepVersion -> FlowStepVersionDto (для внутреннего использования)
        CreateMap<FlowStepVersion, FlowStepVersionDto>()
            .ForMember(dest => dest.ComponentVersions, opt => opt.MapFrom((src, dest, destMember, context) => 
                MapComponentVersionsWithOrder(src.ComponentVersions, context)));

        // Маппинг ComponentVersion -> ComponentVersionDto
        CreateMap<ComponentVersion, ComponentVersionDto>()
            .ForMember(dest => dest.ArticleVersion, opt => opt.MapFrom(src => src.ArticleVersion))
            .ForMember(dest => dest.QuizVersion, opt => opt.MapFrom(src => src.QuizVersion))
            .ForMember(dest => dest.TaskVersion, opt => opt.MapFrom(src => src.TaskVersion));

        // Маппинг специализированных версий компонентов
        CreateMap<ArticleComponentVersion, ArticleComponentVersionDto>();
        
        CreateMap<QuizComponentVersion, QuizComponentVersionDto>()
            .ForMember(dest => dest.Options, opt => opt.MapFrom((src, dest, destMember, context) => 
                MapQuizOptionsWithOrder(src.Options, context)));
        
        CreateMap<TaskComponentVersion, TaskComponentVersionDto>();

        // Маппинг QuizOptionVersion -> QuizOptionVersionDto
        CreateMap<QuizOptionVersion, QuizOptionVersionDto>();

        // Обратные маппинги для создания/обновления
        CreateMap<FlowVersionDto, FlowVersion>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.StepVersions, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        CreateMap<FlowStepVersionDto, FlowStepVersion>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ComponentVersions, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        CreateMap<ComponentVersionDto, ComponentVersion>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ArticleVersion, opt => opt.Ignore())
            .ForMember(dest => dest.QuizVersion, opt => opt.Ignore())
            .ForMember(dest => dest.TaskVersion, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        // Обновленный маппинг FlowAssignment -> FlowAssignmentDto с поддержкой версий
        CreateMap<FlowAssignment, FlowAssignmentDto>()
            .ForMember(dest => dest.FlowVersionId, opt => opt.MapFrom(src => src.FlowVersionId))
            .ForMember(dest => dest.ProgressPercentage, opt => opt.MapFrom(src => src.ProgressPercent))
            .ForMember(dest => dest.AssignedById, opt => opt.MapFrom(src => src.AssignedById))
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Flow, opt => opt.Ignore())
            .ForMember(dest => dest.FlowVersion, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Buddy, opt => opt.Ignore());
    }

    /// <summary>
    /// Маппинг версий этапов с сохранением порядка
    /// </summary>
    private ICollection<FlowStepVersionDto> MapStepVersionsWithOrder(ICollection<FlowStepVersion> stepVersions, ResolutionContext context)
    {
        var orderedSteps = stepVersions.OrderBy(sv => sv.Order).ToArray();
        var result = new List<FlowStepVersionDto>();

        for (int i = 0; i < orderedSteps.Length; i++)
        {
            var stepDto = context.Mapper.Map<FlowStepVersionDto>(orderedSteps[i]);
            result.Add(stepDto);
        }

        return result;
    }

    /// <summary>
    /// Маппинг версий компонентов с сохранением порядка
    /// </summary>
    private ICollection<ComponentVersionDto> MapComponentVersionsWithOrder(ICollection<ComponentVersion> componentVersions, ResolutionContext context)
    {
        var orderedComponents = componentVersions.OrderBy(cv => cv.Order).ToArray();
        var result = new List<ComponentVersionDto>();

        for (int i = 0; i < orderedComponents.Length; i++)
        {
            var componentDto = context.Mapper.Map<ComponentVersionDto>(orderedComponents[i]);
            result.Add(componentDto);
        }

        return result;
    }

    /// <summary>
    /// Маппинг вариантов ответов теста с сохранением порядка
    /// </summary>
    private ICollection<QuizOptionVersionDto> MapQuizOptionsWithOrder(ICollection<QuizOptionVersion> options, ResolutionContext context)
    {
        var orderedOptions = options.OrderBy(o => o.Order).ToArray();
        var result = new List<QuizOptionVersionDto>();

        for (int i = 0; i < orderedOptions.Length; i++)
        {
            var optionDto = context.Mapper.Map<QuizOptionVersionDto>(orderedOptions[i]);
            result.Add(optionDto);
        }

        return result;
    }

    /// <summary>
    /// Маппинг версий этапов в FlowStepDto для API
    /// </summary>
    private ICollection<FlowStepDto> MapStepVersionsToStepDtos(ICollection<FlowStepVersion> stepVersions, ResolutionContext context)
    {
        var orderedSteps = stepVersions.OrderBy(sv => sv.Order).ToArray();
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
    /// Маппинг версий компонентов в FlowStepComponentDto для API  
    /// </summary>
    private ICollection<object> MapComponentVersionsToComponentDtos(ICollection<ComponentVersion> componentVersions, ResolutionContext context)
    {
        var orderedComponents = componentVersions.OrderBy(cv => cv.Order).ToArray();
        var result = new List<object>();

        for (int i = 0; i < orderedComponents.Length; i++)
        {
            // Здесь нужно создать подходящий DTO в зависимости от типа компонента
            // Пока возвращаем пустой список
            // TODO: Реализовать правильный маппинг в ComponentDto
        }

        return result;
    }

    /// <summary>
    /// Парсинг тегов из строки JSON
    /// </summary>
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
}