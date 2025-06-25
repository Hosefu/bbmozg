using MediatR;
using AutoMapper;
using Lauf.Domain.Enums;
using Lauf.Domain.Interfaces.Repositories;

namespace Lauf.Application.Queries.Notifications;

/// <summary>
/// Запрос для получения уведомлений пользователя
/// </summary>
public record GetUserNotificationsQuery : IRequest<IEnumerable<NotificationDto>>
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Включать только непрочитанные
    /// </summary>
    public bool OnlyUnread { get; init; } = false;

    /// <summary>
    /// Тип уведомлений
    /// </summary>
    public NotificationType? Type { get; init; }

    /// <summary>
    /// Количество записей для пропуска
    /// </summary>
    public int Skip { get; init; } = 0;

    /// <summary>
    /// Количество записей для получения
    /// </summary>
    public int Take { get; init; } = 50;

    public GetUserNotificationsQuery(
        Guid userId,
        bool onlyUnread = false,
        NotificationType? type = null,
        int skip = 0,
        int take = 50)
    {
        UserId = userId;
        OnlyUnread = onlyUnread;
        Type = type;
        Skip = skip;
        Take = take;
    }
}

/// <summary>
/// DTO для уведомления
/// </summary>
public class NotificationDto
{
    /// <summary>
    /// Идентификатор уведомления
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Заголовок уведомления
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Содержание уведомления
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Тип уведомления
    /// </summary>
    public NotificationType Type { get; set; }

    /// <summary>
    /// Прочитано ли уведомление
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата прочтения
    /// </summary>
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// Связанные данные (JSON)
    /// </summary>
    public Dictionary<string, object>? Data { get; set; }

    /// <summary>
    /// URL для действия
    /// </summary>
    public string? ActionUrl { get; set; }
}

/// <summary>
/// Обработчик запроса получения уведомлений пользователя
/// </summary>
public class GetUserNotificationsQueryHandler : IRequestHandler<GetUserNotificationsQuery, IEnumerable<NotificationDto>>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IMapper _mapper;
    
    public GetUserNotificationsQueryHandler(
        INotificationRepository notificationRepository,
        IMapper mapper)
    {
        _notificationRepository = notificationRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Обработка запроса получения уведомлений пользователя
    /// </summary>
    public async Task<IEnumerable<NotificationDto>> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
    {
        var includeRead = !request.OnlyUnread;
        var notifications = await _notificationRepository.GetUserNotificationsAsync(
            request.UserId, 
            includeRead, 
            request.Take,
            cancellationToken);

        // Если указан тип уведомлений, фильтруем
        if (request.Type.HasValue)
        {
            notifications = notifications.Where(n => n.Type == request.Type.Value).ToList();
        }

        // Применяем пагинацию
        var pagedNotifications = notifications.Skip(request.Skip).Take(request.Take).ToList();

        return _mapper.Map<IEnumerable<NotificationDto>>(pagedNotifications);
    }
}