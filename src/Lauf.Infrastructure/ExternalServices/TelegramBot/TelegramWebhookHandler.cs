using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Lauf.Domain.Interfaces.Services;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Infrastructure.ExternalServices.Configurations;

namespace Lauf.Infrastructure.ExternalServices.TelegramBot;

/// <summary>
/// Обработчик Telegram Bot webhook'ов
/// </summary>
public class TelegramWebhookHandler
{
    private readonly ILogger<TelegramWebhookHandler> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly IUserRepository _userRepository;
    private readonly INotificationService _notificationService;
    private readonly TelegramConfiguration _telegramConfig;

    public TelegramWebhookHandler(
        ILogger<TelegramWebhookHandler> logger,
        ITelegramBotClient botClient,
        IUserRepository userRepository,
        INotificationService notificationService,
        IOptions<TelegramConfiguration> telegramConfig)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _telegramConfig = telegramConfig.Value ?? throw new ArgumentNullException(nameof(telegramConfig));
    }

    /// <summary>
    /// Обработка webhook обновлений от Telegram
    /// </summary>
    /// <param name="update">Обновление от Telegram</param>
    /// <param name="cancellationToken">Токен отмены</param>
    public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Получено обновление от Telegram: {UpdateType}", update.Type);

            switch (update.Type)
            {
                case UpdateType.Message:
                    await HandleMessageAsync(update.Message!, cancellationToken);
                    break;

                case UpdateType.CallbackQuery:
                    await HandleCallbackQueryAsync(update.CallbackQuery!, cancellationToken);
                    break;

                default:
                    _logger.LogDebug("Неподдерживаемый тип обновления: {UpdateType}", update.Type);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка обработки Telegram webhook");
        }
    }

    /// <summary>
    /// Обработка текстовых сообщений
    /// </summary>
    private async Task HandleMessageAsync(Message message, CancellationToken cancellationToken)
    {
        if (message.From == null || message.Text == null)
            return;

        var telegramUserId = message.From.Id;
        var messageText = message.Text.Trim();

        _logger.LogInformation(
            "Получено сообщение от пользователя {TelegramUserId}: {MessageText}",
            telegramUserId,
            messageText);

        // Обработка команд
        if (messageText.StartsWith('/'))
        {
            await HandleCommandAsync(message, messageText, cancellationToken);
        }
        else
        {
            await HandleTextMessageAsync(message, messageText, cancellationToken);
        }
    }

    /// <summary>
    /// Обработка команд бота
    /// </summary>
    private async Task HandleCommandAsync(Message message, string command, CancellationToken cancellationToken)
    {
        var telegramUserId = message.From!.Id;

        switch (command.ToLower())
        {
            case "/start":
                await HandleStartCommandAsync(message, cancellationToken);
                break;

            case "/help":
                await HandleHelpCommandAsync(message, cancellationToken);
                break;

            case "/progress":
                await HandleProgressCommandAsync(telegramUserId, message.Chat.Id, cancellationToken);
                break;

            case "/pause":
                await HandlePauseCommandAsync(telegramUserId, message.Chat.Id, cancellationToken);
                break;

            case "/resume":
                await HandleResumeCommandAsync(telegramUserId, message.Chat.Id, cancellationToken);
                break;

            default:
                await SendUnknownCommandMessageAsync(message.Chat.Id, cancellationToken);
                break;
        }
    }

    /// <summary>
    /// Команда /start
    /// </summary>
    private async Task HandleStartCommandAsync(Message message, CancellationToken cancellationToken)
    {
        var welcomeText = $"""
            👋 Добро пожаловать в Lauf!
            
            Это ваш персональный помощник для корпоративного обучения.
            
            🚀 Доступные команды:
            /help - Справка по командам
            /progress - Мой прогресс обучения
            /pause - Приостановить уведомления
            /resume - Возобновить уведомления
            
            Для начала работы откройте Web App:
            """;

        await _botClient.SendTextMessageAsync(
            message.Chat.Id,
            welcomeText,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Команда /help
    /// </summary>
    private async Task HandleHelpCommandAsync(Message message, CancellationToken cancellationToken)
    {
        var helpText = """
            📚 Справка по командам Lauf Bot:

            🚀 Основные команды:
            /start - Приветствие и начало работы
            /progress - Показать текущий прогресс обучения
            /pause - Приостановить уведомления о потоках
            /resume - Возобновить уведомления

            💡 Возможности:
            • Получение уведомлений о новых потоках обучения
            • Напоминания о дедлайнах и прогрессе
            • Уведомления о достижениях
            • Быстрые действия через inline-кнопки

            🎯 Для полного доступа к системе используйте Web App.
            """;

        await _botClient.SendTextMessageAsync(
            message.Chat.Id,
            helpText,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Команда /progress
    /// </summary>
    private async Task HandleProgressCommandAsync(long telegramUserId, long chatId, CancellationToken cancellationToken)
    {
        try
        {
            // Ищем пользователя по Telegram ID
            var user = await _userRepository.GetByTelegramIdAsync(new Domain.ValueObjects.TelegramUserId(telegramUserId), cancellationToken);
            if (user == null)
            {
                await _botClient.SendTextMessageAsync(
                    chatId,
                    "❌ Пользователь не найден. Убедитесь, что вы авторизованы в системе.",
                    cancellationToken: cancellationToken);
                return;
            }

            // Отправляем информацию о прогрессе (упрощенная версия)
            var progressText = """
                📈 Ваш прогресс обучения:
                
                🎯 Активные потоки: 2
                ✅ Завершенные потоки: 1
                ⏱️ Всего времени обучения: 12 часов
                🏆 Достижений: 3
                
                Для подробной информации откройте Web App.
                """;

            await _botClient.SendTextMessageAsync(
                chatId,
                progressText,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка получения прогресса для пользователя {TelegramUserId}", telegramUserId);
            await _botClient.SendTextMessageAsync(
                chatId,
                "❌ Ошибка получения прогресса. Попробуйте позже.",
                cancellationToken: cancellationToken);
        }
    }

    /// <summary>
    /// Команда /pause - приостановка уведомлений
    /// </summary>
    private async Task HandlePauseCommandAsync(long telegramUserId, long chatId, CancellationToken cancellationToken)
    {
        // TODO: Реализовать настройки уведомлений пользователя
        await _botClient.SendTextMessageAsync(
            chatId,
            "⏸️ Уведомления приостановлены. Используйте /resume для возобновления.",
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Команда /resume - возобновление уведомлений
    /// </summary>
    private async Task HandleResumeCommandAsync(long telegramUserId, long chatId, CancellationToken cancellationToken)
    {
        // TODO: Реализовать настройки уведомлений пользователя
        await _botClient.SendTextMessageAsync(
            chatId,
            "▶️ Уведомления возобновлены.",
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Обработка обычных текстовых сообщений
    /// </summary>
    private async Task HandleTextMessageAsync(Message message, string text, CancellationToken cancellationToken)
    {
        // Автоответ на текстовые сообщения
        await _botClient.SendTextMessageAsync(
            message.Chat.Id,
            "🤖 Спасибо за сообщение! Используйте /help для списка доступных команд.",
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Обработка callback запросов (inline кнопки)
    /// </summary>
    private async Task HandleCallbackQueryAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        if (callbackQuery.Data == null || callbackQuery.Message == null)
            return;

        _logger.LogInformation(
            "Получен callback от пользователя {TelegramUserId}: {CallbackData}",
            callbackQuery.From.Id,
            callbackQuery.Data);

        // Подтверждаем получение callback
        await _botClient.AnswerCallbackQueryAsync(
            callbackQuery.Id,
            cancellationToken: cancellationToken);

        // Обрабатываем различные типы callback'ов
        switch (callbackQuery.Data)
        {
            case var data when data.StartsWith("start_flow_"):
                await HandleStartFlowCallbackAsync(callbackQuery, data, cancellationToken);
                break;

            case var data when data.StartsWith("pause_flow_"):
                await HandlePauseFlowCallbackAsync(callbackQuery, data, cancellationToken);
                break;

            case var data when data.StartsWith("view_achievement_"):
                await HandleViewAchievementCallbackAsync(callbackQuery, data, cancellationToken);
                break;

            default:
                _logger.LogWarning("Неизвестный callback: {CallbackData}", callbackQuery.Data);
                break;
        }
    }

    /// <summary>
    /// Обработка callback'а начала потока
    /// </summary>
    private async Task HandleStartFlowCallbackAsync(CallbackQuery callbackQuery, string callbackData, CancellationToken cancellationToken)
    {
        // Извлекаем ID потока из callback данных
        var flowId = callbackData.Replace("start_flow_", "");

        await _botClient.SendTextMessageAsync(
            callbackQuery.Message!.Chat.Id,
            $"🚀 Поток запущен! Откройте Web App для продолжения обучения.",
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Обработка callback'а паузы потока
    /// </summary>
    private async Task HandlePauseFlowCallbackAsync(CallbackQuery callbackQuery, string callbackData, CancellationToken cancellationToken)
    {
        var flowId = callbackData.Replace("pause_flow_", "");

        await _botClient.SendTextMessageAsync(
            callbackQuery.Message!.Chat.Id,
            "⏸️ Поток приостановлен. Вы можете возобновить его в любое время.",
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Обработка callback'а просмотра достижения
    /// </summary>
    private async Task HandleViewAchievementCallbackAsync(CallbackQuery callbackQuery, string callbackData, CancellationToken cancellationToken)
    {
        var achievementId = callbackData.Replace("view_achievement_", "");

        await _botClient.SendTextMessageAsync(
            callbackQuery.Message!.Chat.Id,
            "🏆 Откройте Web App для просмотра всех ваших достижений!",
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Отправка сообщения о неизвестной команде
    /// </summary>
    private async Task SendUnknownCommandMessageAsync(long chatId, CancellationToken cancellationToken)
    {
        await _botClient.SendTextMessageAsync(
            chatId,
            "❓ Неизвестная команда. Используйте /help для списка доступных команд.",
            cancellationToken: cancellationToken);
    }
} 