using AutoMapper;
using Lauf.Api.GraphQL.Types.Components;
using Lauf.Application.Commands.Components;
using Lauf.Application.DTOs.Components;

namespace Lauf.Api.GraphQL.Mappings;

/// <summary>
/// Профиль маппинга GraphQL типов в команды Application слоя
/// </summary>
public class ComponentApiMappingProfile : Profile
{
    public ComponentApiMappingProfile()
    {
        // Маппинг GraphQL Input -> Commands
        CreateMap<CreateArticleComponentInput, CreateArticleComponentCommand>();
        
        CreateMap<CreateQuizComponentInput, CreateQuizComponentCommand>()
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options));
            
        CreateMap<CreateTaskComponentInput, CreateTaskComponentCommand>();
        
        CreateMap<CreateQuestionOptionInput, CreateQuestionOptionDto>();
    }
}