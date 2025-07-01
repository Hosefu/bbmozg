using AutoMapper;
using Lauf.Application.Commands.Components;
using Lauf.Application.DTOs.Components;
using Lauf.Domain.Entities.Components;

namespace Lauf.Application.Mappings;

/// <summary>
/// Профиль маппинга для компонентов
/// </summary>
public class ComponentMappingProfile : Profile
{
    public ComponentMappingProfile()
    {
        // Маппинг Domain Entities -> DTOs (новая архитектура - Status убран)
        CreateMap<ArticleComponent, ArticleComponentDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "ARTICLE"));

        CreateMap<QuizComponent, QuizComponentDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "QUIZ"))
            .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.Questions));

        CreateMap<TaskComponent, TaskComponentDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "TASK"));

        // Маппинг для вопросов и вариантов ответов
        CreateMap<QuizQuestion, QuizQuestionDto>()
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options));

        CreateMap<QuestionOption, QuestionOptionDto>();
    }
}