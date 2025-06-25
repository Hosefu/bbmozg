using AutoMapper;
using System.Text.Json;
using Lauf.Domain.Entities.Notifications;
using Lauf.Domain.Enums;
using Lauf.Application.Queries.Notifications;

namespace Lauf.Application.Mappings;

/// <summary>
/// Профиль маппинга для уведомлений
/// </summary>
public class NotificationMappingProfile : Profile
{
    public NotificationMappingProfile()
    {
        CreateMap<Notification, NotificationDto>()
            .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.Status == NotificationStatus.Read))
            .ForMember(dest => dest.Data, opt => opt.MapFrom(src => ParseMetadata(src.Metadata)))
            .ForMember(dest => dest.ActionUrl, opt => opt.MapFrom(src => GenerateActionUrl(src)));
    }

    /// <summary>
    /// Парсит метаданные из JSON строки в словарь
    /// </summary>
    private Dictionary<string, object>? ParseMetadata(string? metadata)
    {
        if (string.IsNullOrEmpty(metadata))
            return null;

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, object>>(metadata);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Генерирует URL для действия на основе типа уведомления
    /// </summary>
    private string? GenerateActionUrl(Notification notification)
    {
        return notification.Type switch
        {
            NotificationType.FlowAssigned => $"/flow-assignments/{notification.RelatedEntityId}",
            NotificationType.DeadlineReminder => $"/flow-assignments/{notification.RelatedEntityId}",
            NotificationType.DeadlineApproaching => $"/flow-assignments/{notification.RelatedEntityId}",
            NotificationType.ComponentCompleted => $"/flows/{notification.RelatedEntityId}",
            NotificationType.StepCompleted => $"/flows/{notification.RelatedEntityId}",
            NotificationType.FlowCompleted => $"/flow-assignments/{notification.RelatedEntityId}",
            NotificationType.StepUnlocked => $"/flows/{notification.RelatedEntityId}",
            NotificationType.AchievementEarned => $"/achievements/{notification.RelatedEntityId}",
            NotificationType.AchievementUnlocked => $"/achievements/{notification.RelatedEntityId}",
            NotificationType.BuddyMessage => $"/messages/{notification.RelatedEntityId}",
            _ => null
        };
    }
}