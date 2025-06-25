using Hangfire;
using Microsoft.Extensions.Logging;
using Lauf.Application.BackgroundJobs;

namespace Lauf.Infrastructure.BackgroundJobs;

/// <summary>
/// Планировщик задач на основе Hangfire
/// </summary>
public class HangfireJobScheduler
{
    private readonly ILogger<HangfireJobScheduler> _logger;
    private readonly IRecurringJobManager _recurringJobManager;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public HangfireJobScheduler(
        ILogger<HangfireJobScheduler> logger,
        IRecurringJobManager recurringJobManager,
        IBackgroundJobClient backgroundJobClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _recurringJobManager = recurringJobManager ?? throw new ArgumentNullException(nameof(recurringJobManager));
        _backgroundJobClient = backgroundJobClient ?? throw new ArgumentNullException(nameof(backgroundJobClient));
    }

    /// <summary>
    /// Настроить регулярные задачи
    /// </summary>
    public void ConfigureRecurringJobs()
    {
        _logger.LogInformation("Настройка регулярных задач Hangfire");

        try
        {
            // Ежедневные напоминания - каждый день в 9:00 утра
            _recurringJobManager.AddOrUpdate<DailyReminderJob>(
                "daily-reminders",
                job => job.ExecuteAsync(CancellationToken.None),
                "0 9 * * *"); // Cron: каждый день в 9:00

            // Проверка дедлайнов - каждые 4 часа
            _recurringJobManager.AddOrUpdate<DeadlineCheckJob>(
                "deadline-checks",
                job => job.ExecuteAsync(CancellationToken.None),
                "0 */4 * * *"); // Cron: каждые 4 часа

            _logger.LogInformation("Регулярные задачи успешно настроены");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка настройки регулярных задач");
            throw;
        }
    }

    /// <summary>
    /// Запланировать одноразовую задачу
    /// </summary>
    /// <param name="jobName">Название задачи</param>
    /// <param name="delay">Задержка перед выполнением</param>
    /// <param name="action">Действие для выполнения</param>
    public void ScheduleOneTimeJob(string jobName, TimeSpan delay, Action action)
    {
        _backgroundJobClient.Schedule(() => action(), delay);
        _logger.LogInformation("Запланирована одноразовая задача '{JobName}' с задержкой {Delay}", jobName, delay);
    }

    /// <summary>
    /// Запланировать задачу на конкретное время
    /// </summary>
    /// <param name="jobName">Название задачи</param>
    /// <param name="scheduledTime">Время выполнения</param>
    /// <param name="action">Действие для выполнения</param>
    public void ScheduleJobAt(string jobName, DateTime scheduledTime, Action action)
    {
        _backgroundJobClient.Schedule(() => action(), scheduledTime);
        _logger.LogInformation("Запланирована задача '{JobName}' на время {ScheduledTime}", jobName, scheduledTime);
    }

    /// <summary>
    /// Выполнить задачу немедленно в фоне
    /// </summary>
    /// <param name="jobName">Название задачи</param>
    /// <param name="action">Действие для выполнения</param>
    public void EnqueueJob(string jobName, Action action)
    {
        _backgroundJobClient.Enqueue(() => action());
        _logger.LogInformation("Задача '{JobName}' добавлена в очередь для немедленного выполнения", jobName);
    }

    /// <summary>
    /// Удалить регулярную задачу
    /// </summary>
    /// <param name="jobId">Идентификатор задачи</param>
    public void RemoveRecurringJob(string jobId)
    {
        _recurringJobManager.RemoveIfExists(jobId);
        _logger.LogInformation("Регулярная задача '{JobId}' удалена", jobId);
    }

    /// <summary>
    /// Триггер немедленного выполнения регулярной задачи
    /// </summary>
    /// <param name="jobId">Идентификатор задачи</param>
    public void TriggerRecurringJob(string jobId)
    {
        _recurringJobManager.Trigger(jobId);
        _logger.LogInformation("Запущено немедленное выполнение регулярной задачи '{JobId}'", jobId);
    }
} 