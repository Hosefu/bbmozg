using AutoMapper;
using Lauf.Application.DTOs.Flows;
using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Enums;
using Lauf.Shared.Helpers;
using Lauf.Application.DTOs.Components;
using Lauf.Domain.Entities.Components;
using Lauf.Application.Queries.Flows;

namespace Lauf.Application.Mappings;

/// <summary>
/// Профиль маппинга для сущностей потоков обучения
/// </summary>
public class FlowMappingProfile : Profile
{
    public FlowMappingProfile()
    {
        // Маппинг Flow -> FlowDto
        CreateMap<Flow, FlowDto>()
            .ForMember(dest => dest.TotalSteps, opt => opt.MapFrom(src => src.TotalSteps))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.IsActive ? FlowStatus.Published : FlowStatus.Draft))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => 1))
            .ForMember(dest => dest.IsRequired, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => new List<string>()))
            .ForMember(dest => dest.PublishedAt, opt => opt.MapFrom(src => src.IsActive ? src.CreatedAt : (DateTime?)null))
            .ForMember(dest => dest.Settings, opt => opt.MapFrom(src => src.Settings))
            .ForMember(dest => dest.Steps, opt => opt.MapFrom(src => src.ActiveContent != null ? src.ActiveContent.Steps : new List<FlowStep>()));

        // Маппинг FlowSettings -> FlowSettingsDto
        CreateMap<FlowSettings, FlowSettingsDto>()
            .ForMember(dest => dest.DaysPerStep, opt => opt.MapFrom(src => src.DaysPerStep))
            .ForMember(dest => dest.RequireSequentialCompletionComponents, opt => opt.MapFrom(src => src.RequireSequentialCompletionComponents))
            .ForMember(dest => dest.AllowSelfRestart, opt => opt.MapFrom(src => src.AllowSelfRestart))
            .ForMember(dest => dest.AllowSelfPause, opt => opt.MapFrom(src => src.AllowSelfPause));

        // Маппинг FlowStep -> FlowStepDto (упрощенный без промежуточного маппинга)
        CreateMap<FlowStep, FlowStepDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.FlowContentId, opt => opt.MapFrom(src => src.FlowContentId))
            .ForMember(dest => dest.IsEnabled, opt => opt.MapFrom(src => src.IsEnabled))
            .ForMember(dest => dest.Order, opt => opt.Ignore())
            .ForMember(dest => dest.TotalComponents, opt => opt.MapFrom(src => src.Components.Count))
            .ForMember(dest => dest.RequiredComponents, opt => opt.MapFrom(src => src.Components.Count(c => c.IsRequired)))
            .ForMember(dest => dest.Components, opt => opt.MapFrom((src, dest, destMember, context) => 
                MapComponentsDirectly(src.Components, context)));

        // Маппинг FlowStep -> FlowStepDetailsDto
        CreateMap<FlowStep, FlowStepDetailsDto>()
            .IncludeBase<FlowStep, FlowStepDto>()
            .ForMember(dest => dest.Components, opt => opt.MapFrom((src, dest, destMember, context) => 
                MapComponentsDetailed(src.Components, context)));

        // Маппинг FlowAssignment -> FlowAssignmentDto (новая архитектура)
        CreateMap<FlowAssignment, FlowAssignmentDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => MapAssignmentStatusToProgressStatus(src.Status)))
            .ForMember(dest => dest.AssignedAt, opt => opt.MapFrom(src => src.AssignedAt))
            .ForMember(dest => dest.CompletedAt, opt => opt.MapFrom(src => src.CompletedAt))
            .ForMember(dest => dest.Deadline, opt => opt.MapFrom(src => src.Deadline))
            .ForMember(dest => dest.AssignedBy, opt => opt.MapFrom(src => src.AssignedBy))
            .ForMember(dest => dest.Buddy, opt => opt.MapFrom(src => src.Buddy))
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => default(string))); // Заметки пока не добавлены

        // Маппинг компонентов
        CreateMap<ArticleComponent, ArticleComponentDto>()
            .ForMember(dest => dest.ReadingTimeMinutes, opt => opt.MapFrom(src => src.ReadingTimeMinutes));

        CreateMap<QuizComponent, QuizComponentDto>();

        CreateMap<TaskComponent, TaskComponentDto>();

        CreateMap<QuizQuestion, QuizQuestionDto>();

        CreateMap<QuestionOption, QuestionOptionDto>();
    }

    /// <summary>
    /// Прямой маппинг компонентов в FlowStepComponentDto для обратной совместимости
    /// </summary>
    private List<FlowStepComponentDto> MapComponentsDirectly(ICollection<ComponentBase> components, ResolutionContext context)
    {
        if (components == null || !components.Any())
            return new List<FlowStepComponentDto>();

        return components
            .OrderBy(c => c.Order)
            .Select((component, index) =>
            {
                var dto = new FlowStepComponentDto
                {
                    Id = component.Id,
                    FlowStepId = component.FlowStepId,
                    ComponentId = component.Id,
                    ComponentType = component.Type,
                    Title = component.Title,
                    Description = component.Description,
                    Order = index,
                    IsRequired = component.IsRequired,
                    IsEnabled = component.IsEnabled,
                    Component = MapComponentToDto(component, context)
                };
                return dto;
            })
            .ToList();
    }

    /// <summary>
    /// Маппинг компонентов для детального просмотра
    /// </summary>
    private List<FlowStepComponentDetailsDto> MapComponentsDetailed(ICollection<ComponentBase> components, ResolutionContext context)
    {
        if (components == null || !components.Any())
            return new List<FlowStepComponentDetailsDto>();

        return components
            .OrderBy(c => c.Order)
            .Select((component, index) =>
            {
                var dto = new FlowStepComponentDetailsDto
                {
                    Id = component.Id,
                    Title = component.Title,
                    Description = component.Description,
                    ComponentType = component.Type,
                    Content = component.Content,
                    Settings = null, // Настройки пока не определены
                    IsRequired = component.IsRequired,
                    Order = index,
                    Progress = null // Прогресс будет добавлен позже
                };
                return dto;
            })
            .ToList();
    }

    /// <summary>
    /// Маппинг компонента в DTO в зависимости от типа
    /// </summary>
    private object? MapComponentToDto(ComponentBase component, ResolutionContext context)
    {
        return component switch
        {
            ArticleComponent article => context.Mapper.Map<ArticleComponentDto>(article),
            QuizComponent quiz => context.Mapper.Map<QuizComponentDto>(quiz),
            TaskComponent task => context.Mapper.Map<TaskComponentDto>(task),
            _ => null
        };
    }

    /// <summary>
    /// Преобразование AssignmentStatus в ProgressStatus
    /// </summary>
    private ProgressStatus MapAssignmentStatusToProgressStatus(AssignmentStatus assignmentStatus)
    {
        return assignmentStatus switch
        {
            AssignmentStatus.Assigned => ProgressStatus.NotStarted,
            AssignmentStatus.InProgress => ProgressStatus.InProgress,
            AssignmentStatus.Completed => ProgressStatus.Completed,
            AssignmentStatus.Cancelled => ProgressStatus.Cancelled,
            AssignmentStatus.Paused => ProgressStatus.InProgress,
            AssignmentStatus.Overdue => ProgressStatus.Failed,
            _ => ProgressStatus.NotStarted
        };
    }
}