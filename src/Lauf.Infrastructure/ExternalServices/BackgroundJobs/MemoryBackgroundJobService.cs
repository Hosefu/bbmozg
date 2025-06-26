using System.Collections.Concurrent;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Lauf.Domain.Interfaces.ExternalServices;

namespace Lauf.Infrastructure.ExternalServices.BackgroundJobs;

/// <summary>
/// In-memory реализация сервиса фоновых задач для разработки и тестирования
/// </summary>
public class MemoryBackgroundJobService : IBackgroundJobService
{
    private readonly ILogger<MemoryBackgroundJobService> _logger;
    private readonly ConcurrentDictionary<string, JobInfo> _jobs = new();
    private readonly ConcurrentDictionary<string, RecurringJobInfo> _recurringJobs = new();
    private static long _jobCounter = 0;

    public MemoryBackgroundJobService(ILogger<MemoryBackgroundJobService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Запланировать выполнение задачи в фоне
    /// </summary>
    public string Enqueue(Expression<Action> methodCall)
    {
        var jobId = GenerateJobId();
        var methodName = ExtractMethodName(methodCall);
        var jobInfo = new JobInfo
        {
            Id = jobId,
            MethodCall = methodCall.ToString(),
            State = "Enqueued",
            CreatedAt = DateTime.UtcNow,
            EnqueueAt = DateTime.UtcNow
        };

        _jobs[jobId] = jobInfo;
        _logger.LogInformation(
            "⚙️ [JOB-{JobId}] ENQUEUE {MethodName} | Queue size: {QueueSize}",
            jobId, methodName, _jobs.Count);
        
        // Симулируем немедленное выполнение для разработки
        _ = Task.Run(async () => await ExecuteJob(jobInfo));
        
        return jobId;
    }

    /// <summary>
    /// Запланировать выполнение задачи в фоне
    /// </summary>
    public string Enqueue(Expression<Func<Task>> methodCall)
    {
        var jobId = GenerateJobId();
        var jobInfo = new JobInfo
        {
            Id = jobId,
            MethodCall = methodCall.ToString(),
            State = "Enqueued",
            CreatedAt = DateTime.UtcNow,
            EnqueueAt = DateTime.UtcNow
        };

        _jobs[jobId] = jobInfo;
        _logger.LogInformation("Асинхронная задача {JobId} добавлена в очередь: {Method}", jobId, methodCall.ToString());
        
        // Симулируем немедленное выполнение для разработки
        _ = Task.Run(async () => await ExecuteJob(jobInfo));
        
        return jobId;
    }

    /// <summary>
    /// Запланировать выполнение задачи с задержкой
    /// </summary>
    public string Schedule(Expression<Action> methodCall, TimeSpan delay)
    {
        return Schedule(methodCall, DateTimeOffset.UtcNow.Add(delay));
    }

    /// <summary>
    /// Запланировать выполнение задачи с задержкой
    /// </summary>
    public string Schedule(Expression<Func<Task>> methodCall, TimeSpan delay)
    {
        return Schedule(methodCall, DateTimeOffset.UtcNow.Add(delay));
    }

    /// <summary>
    /// Запланировать выполнение задачи на определенное время
    /// </summary>
    public string Schedule(Expression<Action> methodCall, DateTimeOffset enqueueAt)
    {
        var jobId = GenerateJobId();
        var jobInfo = new JobInfo
        {
            Id = jobId,
            MethodCall = methodCall.ToString(),
            State = "Scheduled",
            CreatedAt = DateTime.UtcNow,
            EnqueueAt = enqueueAt.DateTime
        };

        _jobs[jobId] = jobInfo;
        _logger.LogInformation("Задача {JobId} запланирована на {EnqueueAt}: {Method}", jobId, enqueueAt, methodCall.ToString());
        
        // Симулируем отложенное выполнение
        _ = Task.Delay(enqueueAt - DateTimeOffset.UtcNow).ContinueWith(async _ => await ExecuteJob(jobInfo));
        
        return jobId;
    }

    /// <summary>
    /// Запланировать выполнение задачи на определенное время
    /// </summary>
    public string Schedule(Expression<Func<Task>> methodCall, DateTimeOffset enqueueAt)
    {
        var jobId = GenerateJobId();
        var jobInfo = new JobInfo
        {
            Id = jobId,
            MethodCall = methodCall.ToString(),
            State = "Scheduled",
            CreatedAt = DateTime.UtcNow,
            EnqueueAt = enqueueAt.DateTime
        };

        _jobs[jobId] = jobInfo;
        _logger.LogInformation("Асинхронная задача {JobId} запланирована на {EnqueueAt}: {Method}", jobId, enqueueAt, methodCall.ToString());
        
        // Симулируем отложенное выполнение
        _ = Task.Delay(enqueueAt - DateTimeOffset.UtcNow).ContinueWith(async _ => await ExecuteJob(jobInfo));
        
        return jobId;
    }

    /// <summary>
    /// Создать повторяющуюся задачу
    /// </summary>
    public void AddOrUpdateRecurringJob(string recurringJobId, Expression<Action> methodCall, string cronExpression)
    {
        var jobInfo = new RecurringJobInfo
        {
            Id = recurringJobId,
            MethodCall = methodCall.ToString(),
            CronExpression = cronExpression,
            CreatedAt = DateTime.UtcNow
        };

        _recurringJobs[recurringJobId] = jobInfo;
        _logger.LogInformation("Повторяющаяся задача {RecurringJobId} создана/обновлена: {Method} с расписанием {Cron}", 
            recurringJobId, methodCall.ToString(), cronExpression);
    }

    /// <summary>
    /// Создать повторяющуюся задачу
    /// </summary>
    public void AddOrUpdateRecurringJob(string recurringJobId, Expression<Func<Task>> methodCall, string cronExpression)
    {
        var jobInfo = new RecurringJobInfo
        {
            Id = recurringJobId,
            MethodCall = methodCall.ToString(),
            CronExpression = cronExpression,
            CreatedAt = DateTime.UtcNow
        };

        _recurringJobs[recurringJobId] = jobInfo;
        _logger.LogInformation("Повторяющаяся асинхронная задача {RecurringJobId} создана/обновлена: {Method} с расписанием {Cron}", 
            recurringJobId, methodCall.ToString(), cronExpression);
    }

    /// <summary>
    /// Удалить повторяющуюся задачу
    /// </summary>
    public void RemoveRecurringJob(string recurringJobId)
    {
        if (_recurringJobs.TryRemove(recurringJobId, out _))
        {
            _logger.LogInformation("Повторяющаяся задача {RecurringJobId} удалена", recurringJobId);
        }
        else
        {
            _logger.LogWarning("Попытка удалить несуществующую повторяющуюся задачу {RecurringJobId}", recurringJobId);
        }
    }

    /// <summary>
    /// Отменить задачу
    /// </summary>
    public bool Delete(string jobId)
    {
        if (_jobs.TryGetValue(jobId, out var job))
        {
            job.State = "Deleted";
            _logger.LogInformation("Задача {JobId} отменена", jobId);
            return true;
        }
        
        _logger.LogWarning("Попытка отменить несуществующую задачу {JobId}", jobId);
        return false;
    }

    /// <summary>
    /// Получить состояние задачи
    /// </summary>
    public string? GetJobState(string jobId)
    {
        return _jobs.TryGetValue(jobId, out var job) ? job.State : null;
    }

    /// <summary>
    /// Сгенерировать уникальный ID задачи
    /// </summary>
    private static string GenerateJobId()
    {
        return $"job_{Interlocked.Increment(ref _jobCounter)}_{DateTime.UtcNow:yyyyMMddHHmmss}";
    }

    /// <summary>
    /// Извлечь имя метода из Expression
    /// </summary>
    private string ExtractMethodName(Expression expression)
    {
        try
        {
            if (expression is LambdaExpression lambda)
            {
                if (lambda.Body is MethodCallExpression methodCall)
                {
                    return $"{methodCall.Method.DeclaringType?.Name}.{methodCall.Method.Name}";
                }
            }
            
            // Fallback для других типов выражений
            var expressionString = expression.ToString();
            if (expressionString.Contains("=>"))
            {
                var methodPart = expressionString.Split("=>")[1].Trim();
                return methodPart.Split('(')[0].Trim();
            }
            
            return "UnknownMethod";
        }
        catch
        {
            return "UnknownMethod";
        }
    }

    /// <summary>
    /// Симулировать выполнение задачи
    /// </summary>
    private async Task ExecuteJob(JobInfo jobInfo)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        try
        {
            if (jobInfo.State == "Deleted")
            {
                _logger.LogInformation("⚙️ [JOB-{JobId}] CANCELLED - job was deleted", jobInfo.Id);
                return;
            }

            jobInfo.State = "Processing";
            var methodName = ExtractMethodName(System.Linq.Expressions.Expression.Constant(jobInfo.MethodCall));
            
            _logger.LogInformation(
                "⚙️ [JOB-{JobId}] START {MethodName} | Queued for: {QueuedTime}ms",
                jobInfo.Id, methodName, (DateTime.UtcNow - jobInfo.CreatedAt).TotalMilliseconds);
            
            // Симулируем выполнение задачи
            await Task.Delay(100); // Короткая задержка для симуляции работы
            
            stopwatch.Stop();
            jobInfo.State = "Succeeded";
            jobInfo.CompletedAt = DateTime.UtcNow;
            
            _logger.LogInformation(
                "✅ [JOB-{JobId}] SUCCESS {MethodName} за {ElapsedMs}ms",
                jobInfo.Id, methodName, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            jobInfo.State = "Failed";
            jobInfo.CompletedAt = DateTime.UtcNow;
            
            _logger.LogError(ex, 
                "🚨 [JOB-{JobId}] FAILED за {ElapsedMs}ms | Error: {ErrorMessage}",
                jobInfo.Id, stopwatch.ElapsedMilliseconds, ex.Message);
        }
    }

    /// <summary>
    /// Информация о задаче
    /// </summary>
    private class JobInfo
    {
        public string Id { get; set; } = null!;
        public string MethodCall { get; set; } = null!;
        public string State { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime EnqueueAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    /// <summary>
    /// Информация о повторяющейся задаче
    /// </summary>
    private class RecurringJobInfo
    {
        public string Id { get; set; } = null!;
        public string MethodCall { get; set; } = null!;
        public string CronExpression { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}