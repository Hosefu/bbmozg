using Lauf.Application.Services.Interfaces;

namespace Lauf.Infrastructure.Services;

/// <summary>
/// Сервис для работы с датой и временем
/// </summary>
public class DateTimeService : IDateTimeService
{
    /// <summary>
    /// Получить текущее время UTC
    /// </summary>
    public DateTime UtcNow => DateTime.UtcNow;

    /// <summary>
    /// Получить текущее локальное время
    /// </summary>
    public DateTime Now => DateTime.Now;

    /// <summary>
    /// Получить сегодняшнюю дату
    /// </summary>
    public DateTime Today => DateTime.Today;

    /// <summary>
    /// Конвертировать UTC в локальное время
    /// </summary>
    public DateTime ToLocalTime(DateTime utcDateTime)
    {
        return utcDateTime.ToLocalTime();
    }

    /// <summary>
    /// Конвертировать локальное время в UTC
    /// </summary>
    public DateTime ToUtcTime(DateTime localDateTime)
    {
        return localDateTime.ToUniversalTime();
    }

    /// <summary>
    /// Получить начало дня для указанной даты
    /// </summary>
    public DateTime StartOfDay(DateTime date)
    {
        return date.Date;
    }

    /// <summary>
    /// Получить конец дня для указанной даты
    /// </summary>
    public DateTime EndOfDay(DateTime date)
    {
        return date.Date.AddDays(1).AddTicks(-1);
    }

    /// <summary>
    /// Получить начало недели для указанной даты
    /// </summary>
    public DateTime StartOfWeek(DateTime date, DayOfWeek startOfWeek = DayOfWeek.Monday)
    {
        var diff = (7 + (date.DayOfWeek - startOfWeek)) % 7;
        return date.AddDays(-1 * diff).Date;
    }

    /// <summary>
    /// Получить конец недели для указанной даты
    /// </summary>
    public DateTime EndOfWeek(DateTime date, DayOfWeek startOfWeek = DayOfWeek.Monday)
    {
        return StartOfWeek(date, startOfWeek).AddDays(7).AddTicks(-1);
    }

    /// <summary>
    /// Получить начало месяца для указанной даты
    /// </summary>
    public DateTime StartOfMonth(DateTime date)
    {
        return new DateTime(date.Year, date.Month, 1);
    }

    /// <summary>
    /// Получить конец месяца для указанной даты
    /// </summary>
    public DateTime EndOfMonth(DateTime date)
    {
        return StartOfMonth(date).AddMonths(1).AddTicks(-1);
    }

    /// <summary>
    /// Получить рабочие дни между двумя датами
    /// </summary>
    public int GetWorkingDays(DateTime startDate, DateTime endDate)
    {
        var totalDays = 0;
        var current = startDate.Date;

        while (current <= endDate.Date)
        {
            if (current.DayOfWeek != DayOfWeek.Saturday && current.DayOfWeek != DayOfWeek.Sunday)
            {
                totalDays++;
            }
            current = current.AddDays(1);
        }

        return totalDays;
    }

    /// <summary>
    /// Добавить рабочие дни к дате
    /// </summary>
    public DateTime AddWorkingDays(DateTime date, int workingDays)
    {
        var current = date.Date;
        var direction = workingDays > 0 ? 1 : -1;
        var remainingDays = Math.Abs(workingDays);

        while (remainingDays > 0)
        {
            current = current.AddDays(direction);
            
            if (current.DayOfWeek != DayOfWeek.Saturday && current.DayOfWeek != DayOfWeek.Sunday)
            {
                remainingDays--;
            }
        }

        return current;
    }

    /// <summary>
    /// Проверить, является ли дата рабочим днем
    /// </summary>
    public bool IsWorkingDay(DateTime date)
    {
        return date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;
    }

    /// <summary>
    /// Получить временную зону по умолчанию
    /// </summary>
    public TimeZoneInfo GetDefaultTimeZone()
    {
        return TimeZoneInfo.Local;
    }

    /// <summary>
    /// Конвертировать время в указанную временную зону
    /// </summary>
    public DateTime ConvertToTimeZone(DateTime dateTime, TimeZoneInfo timeZone)
    {
        return TimeZoneInfo.ConvertTime(dateTime, timeZone);
    }

    /// <summary>
    /// Форматировать дату для отображения
    /// </summary>
    public string FormatForDisplay(DateTime dateTime, string format = "dd.MM.yyyy HH:mm")
    {
        return dateTime.ToString(format);
    }

    /// <summary>
    /// Получить относительное время (например, "2 часа назад")
    /// </summary>
    public string GetRelativeTime(DateTime dateTime)
    {
        var timeSpan = DateTime.UtcNow - dateTime;

        if (timeSpan.TotalMinutes < 1)
            return "только что";
        if (timeSpan.TotalMinutes < 60)
            return $"{(int)timeSpan.TotalMinutes} мин назад";
        if (timeSpan.TotalHours < 24)
            return $"{(int)timeSpan.TotalHours} ч назад";
        if (timeSpan.TotalDays < 30)
            return $"{(int)timeSpan.TotalDays} дн назад";
        if (timeSpan.TotalDays < 365)
            return $"{(int)(timeSpan.TotalDays / 30)} мес назад";

        return $"{(int)(timeSpan.TotalDays / 365)} г назад";
    }
}