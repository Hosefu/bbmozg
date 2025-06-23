using System.Globalization;

namespace Lauf.Shared.Extensions;

/// <summary>
/// Методы расширения для работы с датой и временем
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Проверяет, является ли дата рабочим днем (не выходной)
    /// </summary>
    /// <param name="date">Проверяемая дата</param>
    /// <returns>true, если дата является рабочим днем</returns>
    public static bool IsWorkingDay(this DateTime date)
    {
        return date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;
    }
    
    /// <summary>
    /// Проверяет, является ли дата выходным днем
    /// </summary>
    /// <param name="date">Проверяемая дата</param>
    /// <returns>true, если дата является выходным днем</returns>
    public static bool IsWeekend(this DateTime date)
    {
        return !date.IsWorkingDay();
    }
    
    /// <summary>
    /// Возвращает начало дня (00:00:00)
    /// </summary>
    /// <param name="date">Исходная дата</param>
    /// <returns>Начало дня</returns>
    public static DateTime StartOfDay(this DateTime date)
    {
        return date.Date;
    }
    
    /// <summary>
    /// Возвращает конец дня (23:59:59.999)
    /// </summary>
    /// <param name="date">Исходная дата</param>
    /// <returns>Конец дня</returns>
    public static DateTime EndOfDay(this DateTime date)
    {
        return date.Date.AddDays(1).AddTicks(-1);
    }
    
    /// <summary>
    /// Возвращает начало недели (понедельник)
    /// </summary>
    /// <param name="date">Исходная дата</param>
    /// <returns>Начало недели</returns>
    public static DateTime StartOfWeek(this DateTime date)
    {
        var daysFromMonday = ((int)date.DayOfWeek - 1 + 7) % 7;
        return date.AddDays(-daysFromMonday).StartOfDay();
    }
    
    /// <summary>
    /// Возвращает конец недели (воскресенье)
    /// </summary>
    /// <param name="date">Исходная дата</param>
    /// <returns>Конец недели</returns>
    public static DateTime EndOfWeek(this DateTime date)
    {
        return date.StartOfWeek().AddDays(6).EndOfDay();
    }
    
    /// <summary>
    /// Возвращает начало месяца
    /// </summary>
    /// <param name="date">Исходная дата</param>
    /// <returns>Начало месяца</returns>
    public static DateTime StartOfMonth(this DateTime date)
    {
        return new DateTime(date.Year, date.Month, 1);
    }
    
    /// <summary>
    /// Возвращает конец месяца
    /// </summary>
    /// <param name="date">Исходная дата</param>
    /// <returns>Конец месяца</returns>
    public static DateTime EndOfMonth(this DateTime date)
    {
        return date.StartOfMonth().AddMonths(1).AddTicks(-1);
    }
    
    /// <summary>
    /// Возвращает начало года
    /// </summary>
    /// <param name="date">Исходная дата</param>
    /// <returns>Начало года</returns>
    public static DateTime StartOfYear(this DateTime date)
    {
        return new DateTime(date.Year, 1, 1);
    }
    
    /// <summary>
    /// Возвращает конец года
    /// </summary>
    /// <param name="date">Исходная дата</param>
    /// <returns>Конец года</returns>
    public static DateTime EndOfYear(this DateTime date)
    {
        return new DateTime(date.Year, 12, 31, 23, 59, 59, 999);
    }
    
    /// <summary>
    /// Добавляет указанное количество рабочих дней к дате
    /// </summary>
    /// <param name="date">Исходная дата</param>
    /// <param name="workingDays">Количество рабочих дней для добавления</param>
    /// <returns>Дата с добавленными рабочими днями</returns>
    public static DateTime AddWorkingDays(this DateTime date, int workingDays)
    {
        var direction = workingDays < 0 ? -1 : 1;
        var current = date;
        var remainingDays = Math.Abs(workingDays);
        
        while (remainingDays > 0)
        {
            current = current.AddDays(direction);
            if (current.IsWorkingDay())
            {
                remainingDays--;
            }
        }
        
        return current;
    }
    
    /// <summary>
    /// Вычисляет количество рабочих дней между двумя датами
    /// </summary>
    /// <param name="startDate">Начальная дата</param>
    /// <param name="endDate">Конечная дата</param>
    /// <returns>Количество рабочих дней</returns>
    public static int WorkingDaysBetween(this DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
            return -startDate.WorkingDaysBetween(endDate);
            
        var workingDays = 0;
        var current = startDate;
        
        while (current < endDate)
        {
            if (current.IsWorkingDay())
                workingDays++;
            current = current.AddDays(1);
        }
        
        return workingDays;
    }
    
    /// <summary>
    /// Возвращает возраст в годах на указанную дату
    /// </summary>
    /// <param name="birthDate">Дата рождения</param>
    /// <param name="asOfDate">Дата, на которую рассчитывается возраст (по умолчанию - текущая)</param>
    /// <returns>Возраст в годах</returns>
    public static int AgeInYears(this DateTime birthDate, DateTime? asOfDate = null)
    {
        var referenceDate = asOfDate ?? DateTime.Today;
        var age = referenceDate.Year - birthDate.Year;
        
        if (referenceDate.Month < birthDate.Month || 
            (referenceDate.Month == birthDate.Month && referenceDate.Day < birthDate.Day))
        {
            age--;
        }
        
        return age;
    }
    
    /// <summary>
    /// Проверяет, находится ли дата в указанном диапазоне
    /// </summary>
    /// <param name="date">Проверяемая дата</param>
    /// <param name="startDate">Начало диапазона</param>
    /// <param name="endDate">Конец диапазона</param>
    /// <param name="inclusive">Включать ли границы диапазона</param>
    /// <returns>true, если дата находится в диапазоне</returns>
    public static bool IsBetween(this DateTime date, DateTime startDate, DateTime endDate, bool inclusive = true)
    {
        return inclusive 
            ? date >= startDate && date <= endDate
            : date > startDate && date < endDate;
    }
    
    /// <summary>
    /// Форматирует дату в относительном формате (например, "2 дня назад")
    /// </summary>
    /// <param name="date">Форматируемая дата</param>
    /// <param name="baseDate">Базовая дата для сравнения (по умолчанию - текущая)</param>
    /// <returns>Строка с относительным временем</returns>
    public static string ToRelativeString(this DateTime date, DateTime? baseDate = null)
    {
        var now = baseDate ?? DateTime.Now;
        var timeSpan = now - date;
        
        if (timeSpan.TotalDays < -365)
            return $"через {Math.Abs(timeSpan.Days / 365)} {GetYearWord(Math.Abs(timeSpan.Days / 365))}";
        if (timeSpan.TotalDays < -30)
            return $"через {Math.Abs(timeSpan.Days / 30)} {GetMonthWord(Math.Abs(timeSpan.Days / 30))}";
        if (timeSpan.TotalDays < -1)
            return $"через {Math.Abs(timeSpan.Days)} {GetDayWord(Math.Abs(timeSpan.Days))}";
        if (timeSpan.TotalHours < -1)
            return $"через {Math.Abs((int)timeSpan.TotalHours)} {GetHourWord((int)Math.Abs(timeSpan.TotalHours))}";
        if (timeSpan.TotalMinutes < -1)
            return $"через {Math.Abs((int)timeSpan.TotalMinutes)} {GetMinuteWord((int)Math.Abs(timeSpan.TotalMinutes))}";
        if (timeSpan.TotalMinutes < 1)
            return "только что";
        if (timeSpan.TotalMinutes < 60)
            return $"{(int)timeSpan.TotalMinutes} {GetMinuteWord((int)timeSpan.TotalMinutes)} назад";
        if (timeSpan.TotalHours < 24)
            return $"{(int)timeSpan.TotalHours} {GetHourWord((int)timeSpan.TotalHours)} назад";
        if (timeSpan.TotalDays < 30)
            return $"{(int)timeSpan.TotalDays} {GetDayWord((int)timeSpan.TotalDays)} назад";
        if (timeSpan.TotalDays < 365)
            return $"{(int)timeSpan.TotalDays / 30} {GetMonthWord((int)timeSpan.TotalDays / 30)} назад";
        
        return $"{(int)timeSpan.TotalDays / 365} {GetYearWord((int)timeSpan.TotalDays / 365)} назад";
    }
    
    /// <summary>
    /// Преобразует дату в Unix timestamp
    /// </summary>
    /// <param name="date">Дата для преобразования</param>
    /// <returns>Unix timestamp</returns>
    public static long ToUnixTimestamp(this DateTime date)
    {
        return ((DateTimeOffset)date).ToUnixTimeSeconds();
    }
    
    /// <summary>
    /// Преобразует Unix timestamp в DateTime
    /// </summary>
    /// <param name="timestamp">Unix timestamp</param>
    /// <returns>DateTime</returns>
    public static DateTime FromUnixTimestamp(long timestamp)
    {
        return DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
    }
    
    /// <summary>
    /// Возвращает правильную форму слова "год" в зависимости от числа
    /// </summary>
    private static string GetYearWord(int count)
    {
        var remainder = count % 10;
        var tens = count % 100;
        
        if (tens >= 11 && tens <= 14)
            return "лет";
        
        return remainder switch
        {
            1 => "год",
            2 or 3 or 4 => "года",
            _ => "лет"
        };
    }
    
    /// <summary>
    /// Возвращает правильную форму слова "месяц" в зависимости от числа
    /// </summary>
    private static string GetMonthWord(int count)
    {
        var remainder = count % 10;
        var tens = count % 100;
        
        if (tens >= 11 && tens <= 14)
            return "месяцев";
        
        return remainder switch
        {
            1 => "месяц",
            2 or 3 or 4 => "месяца",
            _ => "месяцев"
        };
    }
    
    /// <summary>
    /// Возвращает правильную форму слова "день" в зависимости от числа
    /// </summary>
    private static string GetDayWord(int count)
    {
        var remainder = count % 10;
        var tens = count % 100;
        
        if (tens >= 11 && tens <= 14)
            return "дней";
        
        return remainder switch
        {
            1 => "день",
            2 or 3 or 4 => "дня",
            _ => "дней"
        };
    }
    
    /// <summary>
    /// Возвращает правильную форму слова "час" в зависимости от числа
    /// </summary>
    private static string GetHourWord(int count)
    {
        var remainder = count % 10;
        var tens = count % 100;
        
        if (tens >= 11 && tens <= 14)
            return "часов";
        
        return remainder switch
        {
            1 => "час",
            2 or 3 or 4 => "часа",
            _ => "часов"
        };
    }
    
    /// <summary>
    /// Возвращает правильную форму слова "минута" в зависимости от числа
    /// </summary>
    private static string GetMinuteWord(int count)
    {
        var remainder = count % 10;
        var tens = count % 100;
        
        if (tens >= 11 && tens <= 14)
            return "минут";
        
        return remainder switch
        {
            1 => "минуту",
            2 or 3 or 4 => "минуты",
            _ => "минут"
        };
    }
}