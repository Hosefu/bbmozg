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
        CreateMap<ArticleComponent, ArticleComponentDto>();

        CreateMap<QuizComponent, QuizComponentDto>();

        CreateMap<TaskComponent, TaskComponentDto>();

        CreateMap<QuestionOption, QuestionOptionDto>();

        // Маппинг Command DTOs -> Domain Objects (новая архитектура - полный конструктор)
        CreateMap<CreateQuestionOptionDto, QuestionOption>()
            .ConstructUsing(src => new QuestionOption(
                Guid.Empty, // QuizQuestionId - будет задан при создании
                src.Text, 
                src.IsCorrect, 
                src.Points, 
                "a0")); // Порядок по умолчанию - будет перезаписан
    }
}