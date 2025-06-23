using BuddyBot.Domain.Enums;

namespace BuddyBot.Domain.ValueObjects;

/// <summary>
/// Value Object для расчета дедлайнов с учетом рабочих дней
/// </summary>
public record DeadlineCalculation
{
    /// <summary>
    /// Количество рабочих дней
    /// </summary>
    public int WorkingDays { get; }

    /// <summary>
    /// Дата начала
    /// </summary>
    public DateTime StartDate { get; }

    /// <summary>
    /// Дата дедлайна
    /// </summary>
    public DateTime DeadlineDate { get; }

    /// <summary>
    /// Рабочие дни недели
    /// </summary>
    public List<Enums.DayOfWeek> WorkingDaysOfWeek { get; }

    /// <summary>
    /// Праздничные дни
    /// </summary>
    public List<DateTime> Holidays { get; }

    /// <summary>
    /// Конструктор для расчета дедлайна
    /// </summary>
    /// <param name="workingDays">Количество рабочих дней</param>
    /// <param name="startDate">Дата начала</param>
    /// <param name="workingDaysOfWeek">Рабочие дни недели</param>
    /// <param name="holidays">Праздничные дни</param>
    public DeadlineCalculation(
        int workingDays,
        DateTime startDate,
        List<Enums.DayOfWeek>? workingDaysOfWeek = null,
        List<DateTime>? holidays = null)
    {
        if (workingDays <= 0)
        {
            throw new ArgumentException("Количество рабочих дней должно быть положительным", nameof(workingDays));
        }

        WorkingDays = workingDays;
        StartDate = startDate.Date;
        WorkingDaysOfWeek = workingDaysOfWeek ?? GetDefaultWorkingDays();
        Holidays = holidays ?? new List<DateTime>();

        DeadlineDate = CalculateDeadline();
    }

    /// <summary>
    /// Расчет даты дедлайна с учетом рабочих дней и праздников
    /// </summary>
    private DateTime CalculateDeadline()
    {
        var currentDate = StartDate;
        var workingDaysLeft = WorkingDays;

        while (workingDaysLeft > 0)
        {
            currentDate = currentDate.AddDays(1);

            // Проверяем, является ли день рабочим
            var dayOfWeek = (Enums.DayOfWeek)((int)currentDate.DayOfWeek == 0 ? 7 : (int)currentDate.DayOfWeek);
            var isWorkingDay = WorkingDaysOfWeek.Contains(dayOfWeek);
            var isHoliday = Holidays.Any(h => h.Date == currentDate.Date);

            if (isWorkingDay && !isHoliday)
            {
                workingDaysLeft--;
            }
        }

        return currentDate;
    }

    /// <summary>
    /// Получить стандартные рабочие дни (пн-пт)
    /// </summary>
    private static List<Enums.DayOfWeek> GetDefaultWorkingDays()
    {
        return new List<Enums.DayOfWeek>
        {
            Enums.DayOfWeek.Monday,
            Enums.DayOfWeek.Tuesday,
            Enums.DayOfWeek.Wednesday,
            Enums.DayOfWeek.Thursday,
            Enums.DayOfWeek.Friday
        };
    }

    /// <summary>
    /// Проверить, просрочен ли дедлайн
    /// </summary>
    public bool IsOverdue(DateTime currentDate) => currentDate.Date > DeadlineDate.Date;

    /// <summary>
    /// Получить количество дней до дедлайна
    /// </summary>
    public int DaysUntilDeadline(DateTime currentDate) => (DeadlineDate.Date - currentDate.Date).Days;

    /// <summary>
    /// Проверить, приближается ли дедлайн (осталось меньше указанного количества дней)
    /// </summary>
    public bool IsApproaching(DateTime currentDate, int warningDays = 3)
    {
        var daysLeft = DaysUntilDeadline(currentDate);
        return daysLeft > 0 && daysLeft <= warningDays;
    }
}