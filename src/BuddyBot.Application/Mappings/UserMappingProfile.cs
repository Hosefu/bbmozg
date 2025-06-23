using AutoMapper;
using BuddyBot.Application.DTOs.Users;
using BuddyBot.Domain.Entities.Users;

namespace BuddyBot.Application.Mappings;

/// <summary>
/// Профиль маппинга для пользователей
/// </summary>
public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        // Маппинг User -> UserDto
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.TelegramUserId, opt => opt.MapFrom(src => src.TelegramUserId.Value))
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => MapRoles(src.Roles)));
    }

    private static List<string> MapRoles(ICollection<Role> roles)
    {
        return roles?.Select(r => r.Name).ToList() ?? new List<string>();
    }
}