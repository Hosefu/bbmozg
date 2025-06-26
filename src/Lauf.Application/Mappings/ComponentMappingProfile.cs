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
        // Маппинг Domain Entities -> DTOs
        CreateMap<ArticleComponent, ArticleComponentDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<QuizComponent, QuizComponentDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<TaskComponent, TaskComponentDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<QuestionOption, QuestionOptionDto>();

        // Маппинг Command DTOs -> Domain Objects
        CreateMap<CreateQuestionOptionDto, QuestionOption>()
            .ConstructUsing(src => new QuestionOption(src.Text, src.IsCorrect, src.Message, src.Points));
    }
}