using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using Lauf.Domain.Entities.Notifications;
using Lauf.Domain.Enums;

namespace Lauf.Infrastructure.Services;

/// <summary>
/// Сервис отправки уведомлений через Telegram
/// </summary>
public class TelegramNotificationService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TelegramNotificationService> _logger;
    private readonly string _botToken;

    public TelegramNotificationService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<TelegramNotificationService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _botToken = _configuration.GetSection("TelegramBot")["Token"] ?? "";
    }

    /// <summary>
    /// Отправить уведомление пользователю
    /// </summary>
    /// <param name="notification">Уведомление</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Результат отправки</returns>
    public async Task<NotificationSendResult> SendNotificationAsync(
        Notification notification, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(_botToken))
            {
                return NotificationSendResult.Failed("Telegram bot token не настроен");
            }

            // Получаем Telegram ID пользователя
            var telegramId = notification.User.TelegramUserId.Value;
            
            // Формируем сообщение
            var message = FormatMessage(notification);
            
            // Создаем inline клавиатуру если нужно
            var replyMarkup = CreateInlineKeyboard(notification);

            // Отправляем сообщение
            var success = await SendTelegramMessage(telegramId, message, replyMarkup, cancellationToken);
            
            if (success)
            {
                _logger.LogInformation(
                    "Уведомление {NotificationId} отправлено пользователю {UserId} в Telegram",
                    notification.Id, notification.UserId);
                
                return NotificationSendResult.Success();
            }
            else
            {
                return NotificationSendResult.Failed("Ошибка отправки в Telegram API");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Ошибка отправки уведомления {NotificationId} пользователю {UserId}",
                notification.Id, notification.UserId);
            
            return NotificationSendResult.Failed(ex.Message);
        }
    }

    /// <summary>
    /// Форматирует сообщение для Telegram
    /// </summary>
    private string FormatMessage(Notification notification)
    {
        var emoji = GetEmojiByType(notification.Type);
        var priority = GetPriorityText(notification.Priority);
        
        var message = new StringBuilder();
        message.AppendLine($"{emoji} *{notification.Title}*");
        message.AppendLine();
        message.AppendLine(notification.Content);
        
        if (notification.Priority >= NotificationPriority.High)
        {
            message.AppendLine();
            message.AppendLine($"🔔 {priority}");
        }

        // Добавляем метаданные если есть
        if (!string.IsNullOrEmpty("{}"))
        {
            try
            {
                var metadata = JsonSerializer.Deserialize<Dictionary<string, object>>("{}");
                if (metadata?.ContainsKey("deadline") == true)
                {
                    message.AppendLine();
                    message.AppendLine($"⏰ Дедлайн: {metadata["deadline"]}");
                }
            }
            catch
            {
                // Игнорируем ошибки парсинга метаданных
            }
        }

        return message.ToString();
    }

    /// <summary>
    /// Создает inline клавиатуру для уведомления
    /// </summary>
    private object? CreateInlineKeyboard(Notification notification)
    {
        var buttons = new List<List<object>>();

        switch (notification.Type)
        {
            case NotificationType.FlowAssigned:
                buttons.Add(new List<object>
                {
                    new { text = "🚀 Начать обучение", callback_data = $"start_flow_{notification.RelatedEntityId}" },
                    new { text = "📅 Отложить", callback_data = $"postpone_flow_{notification.RelatedEntityId}" }
                });
                break;

            case NotificationType.DeadlineReminder:
                buttons.Add(new List<object>
                {
                    new { text = "📖 Продолжить", callback_data = $"continue_flow_{notification.RelatedEntityId}" },
                    new { text = "⏰ Напомнить позже", callback_data = $"remind_later_{notification.RelatedEntityId}" }
                });
                break;

            case NotificationType.AchievementUnlocked:
                buttons.Add(new List<object>
                {
                    new { text = "🎉 Посмотреть достижения", callback_data = $"view_achievements_{notification.UserId}" }
                });
                break;

            case NotificationType.StepCompleted:
            case NotificationType.ComponentCompleted:
                buttons.Add(new List<object>
                {
                    new { text = "➡️ Следующий шаг", callback_data = $"next_step_{notification.RelatedEntityId}" },
                    new { text = "📊 Мой прогресс", callback_data = $"view_progress_{notification.UserId}" }
                });
                break;
        }

        // Общие кнопки для всех уведомлений
        buttons.Add(new List<object>
        {
            new { text = "✅ Отметить как прочитанное", callback_data = $"mark_read_{notification.Id}" }
        });

        return buttons.Any() ? new { inline_keyboard = buttons } : null;
    }

    /// <summary>
    /// Отправляет сообщение через Telegram Bot API
    /// </summary>
    private async Task<bool> SendTelegramMessage(
        long chatId, 
        string message, 
        object? replyMarkup,
        CancellationToken cancellationToken)
    {
        try
        {
            var url = $"https://api.telegram.org/bot{_botToken}/sendMessage";
            
            var payload = new
            {
                chat_id = chatId,
                text = message,
                parse_mode = "Markdown",
                reply_markup = replyMarkup
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning(
                    "Telegram API вернул ошибку {StatusCode}: {Error}",
                    response.StatusCode, errorContent);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отправке сообщения в Telegram API");
            return false;
        }
    }

    /// <summary>
    /// Получает эмодзи по типу уведомления
    /// </summary>
    private string GetEmojiByType(NotificationType type)
    {
        return type switch
        {
            NotificationType.FlowAssigned => "📚",
            NotificationType.DeadlineReminder => "⏰",
            NotificationType.DeadlineApproaching => "⚠️",
            NotificationType.DeadlineOverdue => "🚨",
            NotificationType.ComponentCompleted => "✅",
            NotificationType.StepCompleted => "🎯",
            NotificationType.FlowCompleted => "🏆",
            NotificationType.StepUnlocked => "🔓",
            NotificationType.AchievementEarned => "🏅",
            NotificationType.AchievementUnlocked => "🏆",
            NotificationType.SystemNotification => "🔔",
            NotificationType.BuddyMessage => "👥",
            _ => "📢"
        };
    }

    /// <summary>
    /// Получает текст приоритета
    /// </summary>
    private string GetPriorityText(NotificationPriority priority)
    {
        return priority switch
        {
            NotificationPriority.Critical => "Критически важно",
            NotificationPriority.High => "Высокий приоритет",
            NotificationPriority.Medium => "Средний приоритет",
            NotificationPriority.Low => "Низкий приоритет",
            _ => ""
        };
    }
}

/// <summary>
/// Результат отправки уведомления
/// </summary>
public class NotificationSendResult
{
    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }

    private NotificationSendResult(bool isSuccess, string? errorMessage = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    public static NotificationSendResult Success() => new(true);
    public static NotificationSendResult Failed(string errorMessage) => new(false, errorMessage);
}