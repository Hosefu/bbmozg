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
    /// Количество дней на один шаг потока
    /// </summary>
    public int DaysPerStep { get; set; } = 7;

    /// <summary>
    /// Требовать последовательное прохождение компонентов
    /// </summary>
    public bool RequireSequentialCompletionComponents { get; set; } = false;

    /// <summary>
    /// Разрешить самостоятельный перезапуск
    /// </summary>
    public bool AllowSelfRestart { get; set; } = false;

    /// <summary>
    /// Разрешить ставить на паузу самостоятельно
    /// </summary>
    public bool AllowSelfPause { get; set; } = true;

    /// <summary>
    /// Отправлять уведомление о начале
    /// </summary>
    public bool SendStartNotification { get; set; } = true;

    /// <summary>
    /// Отправлять напоминания о прогрессе
    /// </summary>
    public bool SendProgressReminders { get; set; } = true;

    /// <summary>
    /// Отправлять уведомление о завершении
    /// </summary>
    public bool SendCompletionNotification { get; set; } = true;

    /// <summary>
    /// Интервал между напоминаниями
    /// </summary>
    public TimeSpan ReminderInterval { get; set; } = TimeSpan.FromDays(1);

    /// <summary>
    /// Дата создания настроек
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Рассчитывает дедлайн на основе количества шагов
    /// </summary>
    /// <param name="assignmentDate">Дата назначения</param>
    /// <param name="totalSteps">Общее количество шагов</param>
    /// <returns>Дедлайн</returns>
    public DateTime CalculateDeadline(DateTime assignmentDate, int totalSteps)
    {
        var totalDays = DaysPerStep * totalSteps;
        return assignmentDate.AddDays(totalDays);
    }
}