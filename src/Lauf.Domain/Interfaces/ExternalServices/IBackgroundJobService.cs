using System.Linq.Expressions;

namespace Lauf.Domain.Interfaces.ExternalServices;

/// <summary>
/// Интерфейс для управления фоновыми задачами
/// </summary>
public interface IBackgroundJobService
{
    /// <summary>
    /// Запланировать выполнение задачи в фоне
    /// </summary>
    /// <param name="methodCall">Вызов метода</param>
    /// <returns>ID задачи</returns>
    string Enqueue(Expression<Action> methodCall);

    /// <summary>
    /// Запланировать выполнение задачи в фоне
    /// </summary>
    /// <param name="methodCall">Вызов асинхронного метода</param>
    /// <returns>ID задачи</returns>
    string Enqueue(Expression<Func<Task>> methodCall);

    /// <summary>
    /// Запланировать выполнение задачи с задержкой
    /// </summary>
    /// <param name="methodCall">Вызов метода</param>
    /// <param name="delay">Задержка выполнения</param>
    /// <returns>ID задачи</returns>
    string Schedule(Expression<Action> methodCall, TimeSpan delay);

    /// <summary>
    /// Запланировать выполнение задачи с задержкой
    /// </summary>
    /// <param name="methodCall">Вызов асинхронного метода</param>
    /// <param name="delay">Задержка выполнения</param>
    /// <returns>ID задачи</returns>
    string Schedule(Expression<Func<Task>> methodCall, TimeSpan delay);

    /// <summary>
    /// Запланировать выполнение задачи на определенное время
    /// </summary>
    /// <param name="methodCall">Вызов метода</param>
    /// <param name="enqueueAt">Время выполнения</param>
    /// <returns>ID задачи</returns>
    string Schedule(Expression<Action> methodCall, DateTimeOffset enqueueAt);

    /// <summary>
    /// Запланировать выполнение задачи на определенное время
    /// </summary>
    /// <param name="methodCall">Вызов асинхронного метода</param>
    /// <param name="enqueueAt">Время выполнения</param>
    /// <returns>ID задачи</returns>
    string Schedule(Expression<Func<Task>> methodCall, DateTimeOffset enqueueAt);

    /// <summary>
    /// Создать повторяющуюся задачу
    /// </summary>
    /// <param name="recurringJobId">ID повторяющейся задачи</param>
    /// <param name="methodCall">Вызов метода</param>
    /// <param name="cronExpression">CRON выражение для расписания</param>
    void AddOrUpdateRecurringJob(string recurringJobId, Expression<Action> methodCall, string cronExpression);

    /// <summary>
    /// Создать повторяющуюся задачу
    /// </summary>
    /// <param name="recurringJobId">ID повторяющейся задачи</param>
    /// <param name="methodCall">Вызов асинхронного метода</param>
    /// <param name="cronExpression">CRON выражение для расписания</param>
    void AddOrUpdateRecurringJob(string recurringJobId, Expression<Func<Task>> methodCall, string cronExpression);

    /// <summary>
    /// Удалить повторяющуюся задачу
    /// </summary>
    /// <param name="recurringJobId">ID повторяющейся задачи</param>
    void RemoveRecurringJob(string recurringJobId);

    /// <summary>
    /// Отменить задачу
    /// </summary>
    /// <param name="jobId">ID задачи</param>
    /// <returns>True если задача была отменена</returns>
    bool Delete(string jobId);

    /// <summary>
    /// Получить состояние задачи
    /// </summary>
    /// <param name="jobId">ID задачи</param>
    /// <returns>Состояние задачи</returns>
    string? GetJobState(string jobId);
}