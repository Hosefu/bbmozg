using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Lauf.Shared.Helpers;

/// <summary>
/// Вспомогательный класс для валидации данных
/// </summary>
public static class ValidationHelper
{
    /// <summary>
    /// Регулярное выражение для проверки email
    /// </summary>
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
    /// <summary>
    /// Регулярное выражение для проверки телефона (российский формат)
    /// </summary>
    private static readonly Regex PhoneRegex = new(
        @"^(\+7|7|8)?[\s\-]?\(?[489][0-9]{2}\)?[\s\-]?[0-9]{3}[\s\-]?[0-9]{2}[\s\-]?[0-9]{2}$",
        RegexOptions.Compiled);
        
    /// <summary>
    /// Регулярное выражение для проверки URL
    /// </summary>
    private static readonly Regex UrlRegex = new(
        @"^https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    /// <summary>
    /// Валидирует объект с использованием Data Annotations
    /// </summary>
    /// <param name="obj">Объект для валидации</param>
    /// <returns>Список ошибок валидации</returns>
    public static List<ValidationResult> ValidateObject(object obj)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(obj);
        Validator.TryValidateObject(obj, context, results, true);
        return results;
    }
    
    /// <summary>
    /// Проверяет, валиден ли объект
    /// </summary>
    /// <param name="obj">Объект для проверки</param>
    /// <returns>true, если объект валиден</returns>
    public static bool IsValid(object obj)
    {
        return ValidateObject(obj).Count == 0;
    }
    
    /// <summary>
    /// Валидирует объект и возвращает словарь ошибок по свойствам
    /// </summary>
    /// <param name="obj">Объект для валидации</param>
    /// <returns>Словарь ошибок (свойство -> список ошибок)</returns>
    public static Dictionary<string, List<string>> ValidateObjectToDictionary(object obj)
    {
        var errors = new Dictionary<string, List<string>>();
        var results = ValidateObject(obj);
        
        foreach (var result in results)
        {
            foreach (var memberName in result.MemberNames)
            {
                if (!errors.ContainsKey(memberName))
                    errors[memberName] = new List<string>();
                    
                if (!string.IsNullOrEmpty(result.ErrorMessage))
                    errors[memberName].Add(result.ErrorMessage);
            }
        }
        
        return errors;
    }
    
    /// <summary>
    /// Проверяет, является ли строка валидным email адресом
    /// </summary>
    /// <param name="email">Email для проверки</param>
    /// <returns>true, если email валиден</returns>
    public static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;
            
        return EmailRegex.IsMatch(email);
    }
    
    /// <summary>
    /// Проверяет, является ли строка валидным номером телефона
    /// </summary>
    /// <param name="phone">Номер телефона для проверки</param>
    /// <returns>true, если номер валиден</returns>
    public static bool IsValidPhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;
            
        return PhoneRegex.IsMatch(phone);
    }
    
    /// <summary>
    /// Проверяет, является ли строка валидным URL
    /// </summary>
    /// <param name="url">URL для проверки</param>
    /// <returns>true, если URL валиден</returns>
    public static bool IsValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;
            
        return UrlRegex.IsMatch(url);
    }
    
    /// <summary>
    /// Нормализует номер телефона к единому формату
    /// </summary>
    /// <param name="phone">Исходный номер телефона</param>
    /// <returns>Нормализованный номер или null если невалиден</returns>
    public static string? NormalizePhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return null;
            
        // Удаляем все символы кроме цифр и +
        var cleaned = Regex.Replace(phone, @"[^\d+]", "");
        
        // Приводим к формату +7XXXXXXXXXX
        if (cleaned.StartsWith("8") && cleaned.Length == 11)
            cleaned = "+7" + cleaned.Substring(1);
        else if (cleaned.StartsWith("7") && cleaned.Length == 11)
            cleaned = "+" + cleaned;
        else if (cleaned.StartsWith("+7") && cleaned.Length == 12)
            return cleaned;
        else if (cleaned.Length == 10)
            cleaned = "+7" + cleaned;
        else
            return null;
            
        return IsValidPhone(cleaned) ? cleaned : null;
    }
    
    /// <summary>
    /// Проверяет, является ли строка валидным именем пользователя
    /// </summary>
    /// <param name="username">Имя пользователя</param>
    /// <param name="minLength">Минимальная длина</param>
    /// <param name="maxLength">Максимальная длина</param>
    /// <returns>true, если имя пользователя валидно</returns>
    public static bool IsValidUsername(string? username, int minLength = 3, int maxLength = 50)
    {
        if (string.IsNullOrWhiteSpace(username))
            return false;
            
        if (username.Length < minLength || username.Length > maxLength)
            return false;
            
        // Только буквы, цифры, подчеркивания и дефисы
        return Regex.IsMatch(username, @"^[a-zA-Z0-9_-]+$");
    }
    
    /// <summary>
    /// Проверяет, является ли число в указанном диапазоне
    /// </summary>
    /// <param name="value">Проверяемое значение</param>
    /// <param name="min">Минимальное значение</param>
    /// <param name="max">Максимальное значение</param>
    /// <param name="inclusive">Включать ли границы диапазона</param>
    /// <returns>true, если значение в диапазоне</returns>
    public static bool IsInRange(decimal value, decimal min, decimal max, bool inclusive = true)
    {
        return inclusive 
            ? value >= min && value <= max
            : value > min && value < max;
    }
    
    /// <summary>
    /// Проверяет, является ли строка валидным GUID
    /// </summary>
    /// <param name="guid">Строка GUID</param>
    /// <returns>true, если GUID валиден</returns>
    public static bool IsValidGuid(string? guid)
    {
        return !string.IsNullOrWhiteSpace(guid) && Guid.TryParse(guid, out _);
    }
    
    /// <summary>
    /// Проверяет, является ли дата в валидном диапазоне
    /// </summary>
    /// <param name="date">Проверяемая дата</param>
    /// <param name="minDate">Минимальная дата</param>
    /// <param name="maxDate">Максимальная дата</param>
    /// <returns>true, если дата в диапазоне</returns>
    public static bool IsValidDateRange(DateTime date, DateTime? minDate = null, DateTime? maxDate = null)
    {
        if (minDate.HasValue && date < minDate.Value)
            return false;
            
        if (maxDate.HasValue && date > maxDate.Value)
            return false;
            
        return true;
    }
    
    /// <summary>
    /// Проверяет возраст по дате рождения
    /// </summary>
    /// <param name="birthDate">Дата рождения</param>
    /// <param name="minAge">Минимальный возраст</param>
    /// <param name="maxAge">Максимальный возраст</param>
    /// <returns>true, если возраст в допустимом диапазоне</returns>
    public static bool IsValidAge(DateTime birthDate, int minAge = 0, int maxAge = 120)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;
        
        if (birthDate.Date > today.AddYears(-age))
            age--;
            
        return age >= minAge && age <= maxAge;
    }
    
    /// <summary>
    /// Проверяет, не содержит ли строка запрещенные символы
    /// </summary>
    /// <param name="input">Проверяемая строка</param>
    /// <param name="forbiddenChars">Запрещенные символы</param>
    /// <returns>true, если строка не содержит запрещенных символов</returns>
    public static bool ContainsNoForbiddenChars(string? input, params char[] forbiddenChars)
    {
        if (string.IsNullOrEmpty(input) || forbiddenChars.Length == 0)
            return true;
            
        return !input.Any(forbiddenChars.Contains);
    }
    
    /// <summary>
    /// Проверяет, является ли строка безопасной для использования в SQL запросах
    /// </summary>
    /// <param name="input">Проверяемая строка</param>
    /// <returns>true, если строка безопасна</returns>
    public static bool IsSqlSafe(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return true;
            
        // Проверяем на наличие потенциально опасных SQL ключевых слов
        var dangerousPatterns = new[]
        {
            @"\bSELECT\b", @"\bINSERT\b", @"\bUPDATE\b", @"\bDELETE\b",
            @"\bDROP\b", @"\bCREATE\b", @"\bALTER\b", @"\bTRUNCATE\b",
            @"\bEXEC\b", @"\bEXECUTE\b", @"\bSCRIPT\b", @"\bUNION\b",
            @"--", @"/\*", @"\*/", @"'", @""""
        };
        
        return !dangerousPatterns.Any(pattern => 
            Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase));
    }
    
    /// <summary>
    /// Создает результат валидации
    /// </summary>
    /// <param name="isValid">Результат валидации</param>
    /// <param name="errorMessage">Сообщение об ошибке</param>
    /// <param name="memberName">Имя свойства</param>
    /// <returns>Результат валидации</returns>
    public static ValidationResult CreateValidationResult(bool isValid, string errorMessage, string? memberName = null)
    {
        return isValid 
            ? ValidationResult.Success! 
            : new ValidationResult(errorMessage, memberName != null ? new[] { memberName } : null);
    }
    
    /// <summary>
    /// Объединяет несколько результатов валидации
    /// </summary>
    /// <param name="results">Результаты валидации</param>
    /// <returns>Объединенный список результатов</returns>
    public static List<ValidationResult> CombineValidationResults(params IEnumerable<ValidationResult>[] results)
    {
        var combined = new List<ValidationResult>();
        
        foreach (var resultSet in results)
        {
            combined.AddRange(resultSet.Where(r => r != ValidationResult.Success));
        }
        
        return combined;
    }
    
    /// <summary>
    /// Результат валидации с дополнительной информацией
    /// </summary>
    public class DetailedValidationResult
    {
        /// <summary>
        /// Является ли результат валидным
        /// </summary>
        public bool IsValid { get; set; }
        
        /// <summary>
        /// Ошибки валидации
        /// </summary>
        public List<ValidationResult> Errors { get; set; } = new();
        
        /// <summary>
        /// Предупреждения
        /// </summary>
        public List<string> Warnings { get; set; } = new();
        
        /// <summary>
        /// Дополнительная информация
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new();
    }
    
    /// <summary>
    /// Выполняет детальную валидацию объекта
    /// </summary>
    /// <param name="obj">Объект для валидации</param>
    /// <returns>Детальный результат валидации</returns>
    public static DetailedValidationResult ValidateDetailed(object obj)
    {
        var result = new DetailedValidationResult();
        result.Errors = ValidateObject(obj);
        result.IsValid = result.Errors.Count == 0;
        
        return result;
    }
}