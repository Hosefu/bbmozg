using AutoMapper;
using System.Text.Json;
using Lauf.Domain.Entities.Notifications;
using Lauf.Domain.Enums;
using Lauf.Application.Queries.Notifications;

namespace Lauf.Application.Mappings;

/// <summary>
/// Профиль маппинга для уведомлений (новая архитектура)
/// </summary>
public class NotificationMappingProfile : Profile
{
    public NotificationMappingProfile()
    {
        CreateMap<Notification, NotificationDto>()
            .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.Status == NotificationStatus.Sent))
            .ForMember(dest => dest.Data, opt => opt.MapFrom(src => new Dictionary<string, object>())) // Metadata убран
            .ForMember(dest => dest.ActionUrl, opt => opt.MapFrom(src => GenerateActionUrl(src)));
    }

    /// <summary>
    /// Генерирует URL действия на основе типа уведомления
    /// </summary>
    private string? GenerateActionUrl(Notification notification)
    {
        return notification.Type switch
        {
            NotificationType.FlowAssigned => $"/flows/{notification.RelatedEntityId}",
            NotificationType.DeadlineReminder => $"/assignments/{notification.RelatedEntityId}",
            NotificationType.SystemNotification => null,
            _ => null
        };
    }
}