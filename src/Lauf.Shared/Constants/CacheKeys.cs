namespace Lauf.Shared.Constants;

/// <summary>
/// Константы ключей кэширования для Redis
/// </summary>
public static class CacheKeys
{
    /// <summary>
    /// Префикс для всех ключей кэша
    /// </summary>
    public const string Prefix = "Lauf:";
    
    /// <summary>
    /// Кэш пользователей
    /// </summary>
    public static class Users
    {
        /// <summary>
        /// Пользователь по ID: Lauf:User:{userId}
        /// </summary>
        public static string ById(int userId) => $"{Prefix}User:{userId}";
        
        /// <summary>
        /// Пользователь по Telegram ID: Lauf:User:Telegram:{telegramUserId}
        /// </summary>
        public static string ByTelegramId(long telegramUserId) => $"{Prefix}User:Telegram:{telegramUserId}";
        
        /// <summary>
        /// Роли пользователя: Lauf:User:{userId}:Roles
        /// </summary>
        public static string Roles(int userId) => $"{Prefix}User:{userId}:Roles";
        
        /// <summary>
        /// Список всех пользователей: Lauf:Users:All
        /// </summary>
        public const string All = Prefix + "Users:All";
    }
    
    /// <summary>
    /// Кэш потоков
    /// </summary>
    public static class Flows
    {
        /// <summary>
        /// Поток по ID: Lauf:Flow:{flowId}
        /// </summary>
        public static string ById(int flowId) => $"{Prefix}Flow:{flowId}";
        
        /// <summary>
        /// Активные потоки: Lauf:Flows:Active
        /// </summary>
        public const string Active = Prefix + "Flows:Active";
        
        /// <summary>
        /// Доступные потоки для пользователя: Lauf:Flows:Available:{userId}
        /// </summary>
        public static string AvailableForUser(int userId) => $"{Prefix}Flows:Available:{userId}";
        
        /// <summary>
        /// Статистика потока: Lauf:Flow:{flowId}:Stats
        /// </summary>
        public static string Stats(int flowId) => $"{Prefix}Flow:{flowId}:Stats";
    }
    
    /// <summary>
    /// Кэш назначений потоков
    /// </summary>
    public static class Assignments
    {
        /// <summary>
        /// Назначение по ID: Lauf:Assignment:{assignmentId}
        /// </summary>
        public static string ById(int assignmentId) => $"{Prefix}Assignment:{assignmentId}";
        
        /// <summary>
        /// Назначения пользователя: Lauf:User:{userId}:Assignments
        /// </summary>
        public static string ByUser(int userId) => $"{Prefix}User:{userId}:Assignments";
        
        /// <summary>
        /// Активные назначения пользователя: Lauf:User:{userId}:Assignments:Active
        /// </summary>
        public static string ActiveByUser(int userId) => $"{Prefix}User:{userId}:Assignments:Active";
    }
    
    /// <summary>
    /// Кэш прогресса
    /// </summary>
    public static class Progress
    {
        /// <summary>
        /// Прогресс по назначению: Lauf:Progress:Assignment:{assignmentId}
        /// </summary>
        public static string ByAssignment(int assignmentId) => $"{Prefix}Progress:Assignment:{assignmentId}";
        
        /// <summary>
        /// Прогресс пользователя: Lauf:Progress:User:{userId}
        /// </summary>
        public static string ByUser(int userId) => $"{Prefix}Progress:User:{userId}";
        
        /// <summary>
        /// Общий прогресс по потоку: Lauf:Progress:Flow:{flowId}
        /// </summary>
        public static string ByFlow(int flowId) => $"{Prefix}Progress:Flow:{flowId}";
    }
    
    /// <summary>
    /// Кэш уведомлений
    /// </summary>
    public static class Notifications
    {
        /// <summary>
        /// Уведомления пользователя: Lauf:Notifications:User:{userId}
        /// </summary>
        public static string ByUser(int userId) => $"{Prefix}Notifications:User:{userId}";
        
        /// <summary>
        /// Непрочитанные уведомления: Lauf:Notifications:User:{userId}:Unread
        /// </summary>
        public static string UnreadByUser(int userId) => $"{Prefix}Notifications:User:{userId}:Unread";
        
        /// <summary>
        /// Количество непрочитанных: Lauf:Notifications:User:{userId}:UnreadCount
        /// </summary>
        public static string UnreadCount(int userId) => $"{Prefix}Notifications:User:{userId}:UnreadCount";
    }
    
    /// <summary>
    /// Кэш достижений
    /// </summary>
    public static class Achievements
    {
        /// <summary>
        /// Достижения пользователя: Lauf:Achievements:User:{userId}
        /// </summary>
        public static string ByUser(int userId) => $"{Prefix}Achievements:User:{userId}";
        
        /// <summary>
        /// Все доступные достижения: Lauf:Achievements:All
        /// </summary>
        public const string All = Prefix + "Achievements:All";
    }
    
    /// <summary>
    /// Кэш системных настроек
    /// </summary>
    public static class System
    {
        /// <summary>
        /// Системные настройки: Lauf:System:Settings
        /// </summary>
        public const string Settings = Prefix + "System:Settings";
        
        /// <summary>
        /// Рабочие часы: Lauf:System:WorkingHours
        /// </summary>
        public const string WorkingHours = Prefix + "System:WorkingHours";
        
        /// <summary>
        /// Праздничные дни: Lauf:System:Holidays
        /// </summary>
        public const string Holidays = Prefix + "System:Holidays";
    }
    
    /// <summary>
    /// Кэш для лимитирования запросов (Rate Limiting)
    /// </summary>
    public static class RateLimit
    {
        /// <summary>
        /// Лимит запросов для пользователя: Lauf:RateLimit:User:{userId}
        /// </summary>
        public static string ForUser(int userId) => $"{Prefix}RateLimit:User:{userId}";
        
        /// <summary>
        /// Лимит запросов по IP: Lauf:RateLimit:IP:{ipAddress}
        /// </summary>
        public static string ForIp(string ipAddress) => $"{Prefix}RateLimit:IP:{ipAddress}";
    }
    
    /// <summary>
    /// Времена жизни кэша в секундах
    /// </summary>
    public static class TTL
    {
        /// <summary>
        /// 5 минут
        /// </summary>
        public const int Short = 300;
        
        /// <summary>
        /// 30 минут
        /// </summary>
        public const int Medium = 1800;
        
        /// <summary>
        /// 2 часа
        /// </summary>
        public const int Long = 7200;
        
        /// <summary>
        /// 24 часа
        /// </summary>
        public const int Day = 86400;
        
        /// <summary>
        /// 7 дней
        /// </summary>
        public const int Week = 604800;
    }
}