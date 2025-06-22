namespace BuddyBot.Shared.Constants;

/// <summary>
/// Константы типов уведомлений в системе
/// </summary>
public static class NotificationTypes
{
    /// <summary>
    /// Информационное уведомление (общая информация)
    /// </summary>
    public const string Info = "Info";
    
    /// <summary>
    /// Предупреждение (требует внимания, но не критично)
    /// </summary>
    public const string Warning = "Warning";
    
    /// <summary>
    /// Ошибка (требует немедленного внимания)
    /// </summary>
    public const string Error = "Error";
    
    /// <summary>
    /// Успешное выполнение действия
    /// </summary>
    public const string Success = "Success";
    
    /// <summary>
    /// Напоминание о задаче или дедлайне
    /// </summary>
    public const string Reminder = "Reminder";
    
    /// <summary>
    /// Новое назначение потока
    /// </summary>
    public const string NewAssignment = "NewAssignment";
    
    /// <summary>
    /// Приближающийся дедлайн
    /// </summary>
    public const string DeadlineApproaching = "DeadlineApproaching";
    
    /// <summary>
    /// Просроченный дедлайн
    /// </summary>
    public const string DeadlineOverdue = "DeadlineOverdue";
    
    /// <summary>
    /// Завершение компонента
    /// </summary>
    public const string ComponentCompleted = "ComponentCompleted";
    
    /// <summary>
    /// Завершение шага
    /// </summary>
    public const string StepCompleted = "StepCompleted";
    
    /// <summary>
    /// Завершение потока
    /// </summary>
    public const string FlowCompleted = "FlowCompleted";
    
    /// <summary>
    /// Разблокировка нового шага
    /// </summary>
    public const string StepUnlocked = "StepUnlocked";
    
    /// <summary>
    /// Получение достижения
    /// </summary>
    public const string AchievementEarned = "AchievementEarned";
    
    /// <summary>
    /// Сообщение от Buddy (наставника)
    /// </summary>
    public const string BuddyMessage = "BuddyMessage";
    
    /// <summary>
    /// Системное уведомление
    /// </summary>
    public const string System = "System";
    
    /// <summary>
    /// Все типы уведомлений для проверок
    /// </summary>
    public static readonly string[] AllTypes = 
    {
        Info,
        Warning,
        Error,
        Success,
        Reminder,
        NewAssignment,
        DeadlineApproaching,
        DeadlineOverdue,
        ComponentCompleted,
        StepCompleted,
        FlowCompleted,
        StepUnlocked,
        AchievementEarned,
        BuddyMessage,
        System
    };
    
    /// <summary>
    /// Критические типы уведомлений (требуют немедленного внимания)
    /// </summary>
    public static readonly string[] CriticalTypes = 
    {
        Error,
        DeadlineOverdue
    };
    
    /// <summary>
    /// Позитивные типы уведомлений
    /// </summary>
    public static readonly string[] PositiveTypes = 
    {
        Success,
        ComponentCompleted,
        StepCompleted,
        FlowCompleted,
        StepUnlocked,
        AchievementEarned
    };
}