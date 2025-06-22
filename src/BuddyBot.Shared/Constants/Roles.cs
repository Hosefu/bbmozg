namespace BuddyBot.Shared.Constants;

/// <summary>
/// Константы ролей пользователей в системе
/// </summary>
public static class Roles
{
    /// <summary>
    /// Администратор системы - полный доступ ко всем функциям
    /// </summary>
    public const string Admin = "Admin";
    
    /// <summary>
    /// HR-специалист - управление потоками и назначениями
    /// </summary>
    public const string HRSpecialist = "HRSpecialist";
    
    /// <summary>
    /// Buddy (наставник) - курирование процесса прохождения
    /// </summary>
    public const string Buddy = "Buddy";
    
    /// <summary>
    /// Обычный сотрудник - прохождение потоков обучения
    /// </summary>
    public const string Employee = "Employee";
    
    /// <summary>
    /// Все роли в системе для проверок
    /// </summary>
    public static readonly string[] AllRoles = 
    {
        Admin,
        HRSpecialist,
        Buddy,
        Employee
    };
    
    /// <summary>
    /// Роли с административными правами
    /// </summary>
    public static readonly string[] AdminRoles = 
    {
        Admin,
        HRSpecialist
    };
    
    /// <summary>
    /// Роли с правами наставничества
    /// </summary>
    public static readonly string[] MentorRoles = 
    {
        Admin,
        HRSpecialist,
        Buddy
    };
}