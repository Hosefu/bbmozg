namespace Lauf.Domain.Entities.Flows;

/// <summary>
/// Настройки потока обучения
/// </summary>
public class FlowSettings
{
    /// <summary>
    /// Уникальный идентификатор настроек
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор потока
    /// </summary>
    public Guid FlowId { get; set; }

    /// <summary>
    /// Поток, к которому относятся настройки
    /// </summary>
    public virtual Flow Flow { get; set; } = null!;

    /// <summary>
    /// Количество дней на прохождение (если null - без ограничений)
    /// </summary>
    public int? DaysToComplete { get; set; }

    /// <summary>
    /// Учитывать ли выходные дни при расчете дедлайна
    /// </summary>
    public bool ExcludeWeekends { get; set; } = true;

    /// <summary>
    /// Учитывать ли праздничные дни при расчете дедлайна
    /// </summary>
    public bool ExcludeHolidays { get; set; } = true;

    /// <summary>
    /// Требуется ли назначение бадди
    /// </summary>
    public bool RequiresBuddy { get; set; } = true;

    /// <summary>
    /// Автоматически назначать бадди
    /// </summary>
    public bool AutoAssignBuddy { get; set; } = false;

    /// <summary>
    /// Разрешить самостоятельное прохождение без бадди
    /// </summary>
    public bool AllowSelfPaced { get; set; } = false;

    /// <summary>
    /// Можно ли ставить поток на паузу
    /// </summary>
    public bool AllowPause { get; set; } = true;



    /// <summary>
    /// Отправлять уведомления о приближении дедлайна
    /// </summary>
    public bool SendDeadlineReminders { get; set; } = true;

    /// <summary>
    /// За сколько дней до дедлайна отправлять первое напоминание
    /// </summary>
    public int FirstReminderDaysBefore { get; set; } = 3;

    /// <summary>
    /// За сколько дней до дедлайна отправлять финальное напоминание
    /// </summary>
    public int FinalReminderDaysBefore { get; set; } = 1;

    /// <summary>
    /// Отправлять ежедневные уведомления о прогрессе
    /// </summary>
    public bool SendDailyProgress { get; set; } = false;

    /// <summary>
    /// Отправлять уведомления при завершении шагов
    /// </summary>
    public bool SendStepCompletionNotifications { get; set; } = true;

    /// <summary>
    /// Дополнительные настройки в формате JSON
    /// </summary>
    public string CustomSettings { get; set; } = "{}";

    /// <summary>
    /// Дата создания настроек
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Рассчитывает дедлайн для назначения на основе настроек
    /// </summary>
    /// <param name="assignmentDate">Дата назначения</param>
    /// <returns>Дедлайн или null, если нет ограничений по времени</returns>
    public DateTime? CalculateDeadline(DateTime assignmentDate)
    {
        if (!DaysToComplete.HasValue)
            return null;

        var deadline = assignmentDate.AddDays(DaysToComplete.Value);

        if (ExcludeWeekends)
        {
            deadline = AdjustForWeekends(deadline);
        }

        if (ExcludeHolidays)
        {
            deadline = AdjustForHolidays(deadline);
        }

        return deadline;
    }

    /// <summary>
    /// Корректирует дату с учетом выходных дней
    /// </summary>
    /// <param name="date">Исходная дата</param>
    /// <returns>Скорректированная дата</returns>
    private DateTime AdjustForWeekends(DateTime date)
    {
        while (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
        {
            date = date.AddDays(1);
        }
        return date;
    }

    /// <summary>
    /// Корректирует дату с учетом праздничных дней
    /// </summary>
    /// <param name="date">Исходная дата</param>
    /// <returns>Скорректированная дата</returns>
    private DateTime AdjustForHolidays(DateTime date)
    {
        // Основные российские праздники
        var holidays = GetRussianHolidays(date.Year);
        
        while (holidays.Contains(date.Date))
        {
            date = date.AddDays(1);
        }
        
        return date;
    }

    /// <summary>
    /// Получает список российских праздников для указанного года
    /// </summary>
    /// <param name="year">Год</param>
    /// <returns>Список праздников</returns>
    private HashSet<DateTime> GetRussianHolidays(int year)
    {
        return new HashSet<DateTime>
        {
            new DateTime(year, 1, 1),  // Новый год
            new DateTime(year, 1, 2),  // Новогодние каникулы
            new DateTime(year, 1, 3),  // Новогодние каникулы
            new DateTime(year, 1, 4),  // Новогодние каникулы
            new DateTime(year, 1, 5),  // Новогодние каникулы
            new DateTime(year, 1, 6),  // Новогодние каникулы
            new DateTime(year, 1, 7),  // Рождество
            new DateTime(year, 1, 8),  // Новогодние каникулы
            new DateTime(year, 2, 23), // День защитника Отечества
            new DateTime(year, 3, 8),  // Международный женский день
            new DateTime(year, 5, 1),  // Праздник Весны и Труда
            new DateTime(year, 5, 9),  // День Победы
            new DateTime(year, 6, 12), // День России
            new DateTime(year, 11, 4)  // День народного единства
        };
    }

    /// <summary>
    /// Проверяет, нужно ли отправлять напоминание о дедлайне
    /// </summary>
    /// <param name="deadline">Дедлайн</param>
    /// <param name="currentDate">Текущая дата</param>
    /// <returns>true, если нужно отправить напоминание</returns>
    public bool ShouldSendDeadlineReminder(DateTime deadline, DateTime currentDate)
    {
        if (!SendDeadlineReminders)
            return false;

        var daysToDeadline = (deadline.Date - currentDate.Date).Days;

        return daysToDeadline == FirstReminderDaysBefore || 
               daysToDeadline == FinalReminderDaysBefore;
    }

    /// <summary>
    /// Проверяет, просрочен ли дедлайн
    /// </summary>
    /// <param name="deadline">Дедлайн</param>
    /// <param name="currentDate">Текущая дата</param>
    /// <returns>true, если дедлайн просрочен</returns>
    public bool IsOverdue(DateTime deadline, DateTime currentDate)
    {
        return currentDate.Date > deadline.Date;
    }
}