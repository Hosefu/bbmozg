namespace Lauf.Infrastructure.ExternalServices.Configurations;

/// <summary>
/// Конфигурация для Telegram Bot
/// </summary>
public class TelegramConfiguration
{
    /// <summary>
    /// Токен бота
    /// </summary>
    public string BotToken { get; set; } = string.Empty;

    /// <summary>
    /// URL webhook для получения обновлений
    /// </summary>
    public string WebhookUrl { get; set; } = string.Empty;

    /// <summary>
    /// Секретный ключ для webhook
    /// </summary>
    public string SecretToken { get; set; } = string.Empty;

    /// <summary>
    /// Максимальное количество подключений
    /// </summary>
    public int MaxConnections { get; set; } = 40;

    /// <summary>
    /// Разрешенные обновления
    /// </summary>
    public string[] AllowedUpdates { get; set; } = { "message", "callback_query" };

    /// <summary>
    /// Включить логирование запросов
    /// </summary>
    public bool EnableLogging { get; set; } = true;
} 