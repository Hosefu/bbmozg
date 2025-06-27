using Lauf.Domain.Enums;
using Lauf.Domain.ValueObjects;

namespace Lauf.Domain.Entities.Notifications;

/// <summary>
/// Уведомление для пользователя
/// </summary>
public class Notification
{
    /// <summary>
    /// Уникальный идентификатор уведомления
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор пользователя-получателя
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Тип уведомления
    /// </summary>
    public NotificationType Type { get; set; }

    /// <summary>
    /// Канал доставки
    /// </summary>
    public NotificationChannel Channel { get; set; }

    /// <summary>
    /// Приоритет уведомления
    /// </summary>
    public NotificationPriority Priority { get; set; }

    /// <summary>
    /// Заголовок уведомления
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Содержимое уведомления
    /// </summary>
    public string Content { get; set; } = string.Empty;


    /// <summary>
    /// Статус уведомления
    /// </summary>
    public NotificationStatus Status { get; set; } = NotificationStatus.Pending;

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Дата запланированной отправки
    /// </summary>
    public DateTime ScheduledAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Дата фактической отправки
    /// </summary>
    public DateTime? SentAt { get; set; }


    /// <summary>
    /// Количество попыток отправки
    /// </summary>
    public int AttemptCount { get; set; } = 0;

    /// <summary>
    /// Максимальное количество попыток
    /// </summary>
    public int MaxAttempts { get; set; } = 3;


    /// <summary>
    /// Связанная сущность (поток, назначение, достижение)
    /// </summary>
    public Guid? RelatedEntityId { get; set; }

    /// <summary>
    /// Тип связанной сущности
    /// </summary>
    public string? RelatedEntityType { get; set; }

    /// <summary>
    /// Навигационное свойство - пользователь
    /// </summary>
    public virtual Users.User User { get; set; } = null!;

    /// <summary>
    /// Отметить как отправленное
    /// </summary>
    public void MarkAsSent()
    {
        Status = NotificationStatus.Sent;
        SentAt = DateTime.UtcNow;
    }


    /// <summary>
    /// Отметить как неудачную попытку
    /// </summary>
    public void MarkAsFailed()
    {
        AttemptCount++;
        
        if (AttemptCount >= MaxAttempts)
        {
            Status = NotificationStatus.Failed;
        }
        else
        {
            Status = NotificationStatus.Pending;
            // Увеличиваем задержку для следующей попытки
            ScheduledAt = DateTime.UtcNow.AddMinutes(Math.Pow(2, AttemptCount) * 5);
        }
    }

    /// <summary>
    /// Проверить, готово ли уведомление к отправке
    /// </summary>
    public bool IsReadyToSend()
    {
        return Status == NotificationStatus.Pending && 
               ScheduledAt <= DateTime.UtcNow && 
               AttemptCount < MaxAttempts;
    }

    /// <summary>
    /// Создать уведомление о назначении потока
    /// </summary>
    public static Notification CreateFlowAssigned(
        Guid userId, 
        string flowTitle, 
        DateTime deadline,
        Guid flowAssignmentId)
    {
        return new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = NotificationType.FlowAssigned,
            Channel = NotificationChannel.Telegram,
            Priority = NotificationPriority.High,
            Title = "📚 Новое обучение назначено!",
            Content = $"Вам назначен поток обучения \"{flowTitle}\". Срок завершения: {deadline:dd.MM.yyyy}",
            RelatedEntityId = flowAssignmentId,
            RelatedEntityType = "FlowAssignment"
        };
    }

    /// <summary>
    /// Создать уведомление о приближении дедлайна
    /// </summary>
    public static Notification CreateDeadlineReminder(
        Guid userId,
        string flowTitle,
        DateTime deadline,
        int daysLeft,
        Guid flowAssignmentId)
    {
        var priority = daysLeft <= 1 ? NotificationPriority.Critical : 
                      daysLeft <= 3 ? NotificationPriority.High : 
                      NotificationPriority.Medium;

        var emoji = daysLeft <= 1 ? "🚨" : daysLeft <= 3 ? "⚠️" : "⏰";

        return new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = NotificationType.DeadlineReminder,
            Channel = NotificationChannel.Telegram,
            Priority = priority,
            Title = $"{emoji} Напоминание о дедлайне",
            Content = $"До завершения обучения \"{flowTitle}\" осталось {daysLeft} дн. Дедлайн: {deadline:dd.MM.yyyy}",
            RelatedEntityId = flowAssignmentId,
            RelatedEntityType = "FlowAssignment"
        };
    }

    /// <summary>
    /// Создать уведомление о получении достижения
    /// </summary>
    public static Notification CreateAchievementUnlocked(
        Guid userId,
        string achievementTitle,
        string achievementDescription,
        Guid achievementId)
    {
        return new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = NotificationType.AchievementUnlocked,
            Channel = NotificationChannel.Telegram,
            Priority = NotificationPriority.Medium,
            Title = "🏆 Новое достижение!",
            Content = $"Поздравляем! Вы получили достижение \"{achievementTitle}\": {achievementDescription}",
            RelatedEntityId = achievementId,
            RelatedEntityType = "Achievement"
        };
    }
}