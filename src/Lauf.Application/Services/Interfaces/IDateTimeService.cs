namespace Lauf.Application.Services.Interfaces;

/// <summary>
/// Интерфейс сервиса для работы с датой и временем
/// </summary>
public interface IDateTimeService
{
    /// <summary>
    /// Получить текущее время UTC
    /// </summary>
    DateTime UtcNow { get; }

    /// <summary>
    /// Получить текущее локальное время
    /// </summary>
    DateTime Now { get; }

    /// <summary>
    /// Получить сегодняшнюю дату
    /// </summary>
    DateTime Today { get; }

    /// <summary>
    /// Конвертировать UTC в локальное время
    /// </summary>
    DateTime ToLocalTime(DateTime utcDateTime);

    /// <summary>
    /// Конвертировать локальное время в UTC
    /// </summary>
    DateTime ToUtcTime(DateTime localDateTime);

    /// <summary>
    /// Получить начало дня для указанной даты
    /// </summary>
    DateTime StartOfDay(DateTime date);

    /// <summary>
    /// Получить конец дня для указанной даты
    /// </summary>
    DateTime EndOfDay(DateTime date);

    /// <summary>
    /// Получить начало недели для указанной даты
    /// </summary>
    DateTime StartOfWeek(DateTime date, DayOfWeek startOfWeek = DayOfWeek.Monday);

    /// <summary>
    /// Получить конец недели для указанной даты
    /// </summary>
    DateTime EndOfWeek(DateTime date, DayOfWeek startOfWeek = DayOfWeek.Monday);

    /// <summary>
    /// Получить начало месяца для указанной даты
    /// </summary>
    DateTime StartOfMonth(DateTime date);

    /// <summary>
    /// Получить конец месяца для указанной даты
    /// </summary>
    DateTime EndOfMonth(DateTime date);

    /// <summary>
    /// Получить рабочие дни между двумя датами
    /// </summary>
    int GetWorkingDays(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Добавить рабочие дни к дате
    /// </summary>
    DateTime AddWorkingDays(DateTime date, int workingDays);

    /// <summary>
    /// Проверить, является ли дата рабочим днем
    /// </summary>
    bool IsWorkingDay(DateTime date);

    /// <summary>
    /// Получить временную зону по умолчанию
    /// </summary>
    TimeZoneInfo GetDefaultTimeZone();

    /// <summary>
    /// Конвертировать время в указанную временную зону
    /// </summary>
    DateTime ConvertToTimeZone(DateTime dateTime, TimeZoneInfo timeZone);

    /// <summary>
    /// Форматировать дату для отображения
    /// </summary>
    string FormatForDisplay(DateTime dateTime, string format = "dd.MM.yyyy HH:mm");

    /// <summary>
    /// Получить относительное время (например, "2 часа назад")
    /// </summary>
    string GetRelativeTime(DateTime dateTime);
}