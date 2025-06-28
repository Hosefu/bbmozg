using Lauf.Domain.Interfaces.Repositories;
using Lauf.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Lauf.Application.BackgroundJobs;

/// <summary>
/// Фоновая задача для проверки дедлайнов назначений (новая архитектура)
/// </summary>
public class DeadlineCheckJob
{
    private readonly IFlowAssignmentRepository _assignmentRepository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<DeadlineCheckJob> _logger;

    public DeadlineCheckJob(
        IFlowAssignmentRepository assignmentRepository,
        INotificationService notificationService,
        ILogger<DeadlineCheckJob> logger)
    {
        _assignmentRepository = assignmentRepository;
        _notificationService = notificationService;
        _logger = logger;
    }

    /// <summary>
    /// Выполнение проверки дедлайнов
    /// </summary>
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Начинается проверка дедлайнов назначений");

            // Получаем все назначения и фильтруем активные (новая архитектура)
            var allAssignments = await _assignmentRepository.GetAllAsync(cancellationToken);
            var activeAssignments = allAssignments.Where(a => 
                a.Status == Domain.Enums.AssignmentStatus.Assigned || 
                a.Status == Domain.Enums.AssignmentStatus.InProgress).ToList();

            var now = DateTime.UtcNow;
            var overdueCount = 0;
            var approachingCount = 0;

            foreach (var assignment in activeAssignments)
            {
                // Проверяем просроченные задания
                if (assignment.Deadline < now)
                {
                    await ProcessOverdueAssignmentAsync(assignment, cancellationToken);
                    overdueCount++;
                }
                // Проверяем приближающиеся дедлайны (за 1 день)
                else if (assignment.Deadline <= now.AddDays(1))
                {
                    await ProcessApproachingDeadlineAsync(assignment, cancellationToken);
                    approachingCount++;
                }
            }

            _logger.LogInformation(
                "Проверка дедлайнов завершена. Всего назначений: {TotalAssignments}, Просрочено: {OverdueCount}, Приближаются: {ApproachingCount}",
                activeAssignments.Count, overdueCount, approachingCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при проверке дедлайнов назначений");
            throw;
        }
    }

    /// <summary>
    /// Обработка просроченного назначения
    /// </summary>
    private async Task ProcessOverdueAssignmentAsync(
        Lauf.Domain.Entities.Flows.FlowAssignment assignment,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Обработка просроченного назначения {AssignmentId}, дедлайн: {Deadline}",
            assignment.Id,
            assignment.Deadline);

        // Уведомляем пользователя через специализированный метод
        await _notificationService.NotifyUrgentDeadlineAsync(
            assignment.UserId,
            assignment.Flow.Name,
            assignment.Deadline,
            assignment.Id,
            cancellationToken);

        // Уведомляем бадди если есть через системное уведомление
        if (assignment.Buddy.HasValue)
        {
            await _notificationService.CreateCustomNotificationAsync(
                assignment.Buddy.Value,
                Domain.Enums.NotificationType.SystemNotification,
                "Подопечный просрочил дедлайн",
                $"Подопечный просрочил дедлайн по назначению потока '{assignment.Flow.Name}'",
                Domain.Enums.NotificationPriority.High,
                cancellationToken: cancellationToken);
        }
    }

    /// <summary>
    /// Обработка приближающегося дедлайна
    /// </summary>
    private async Task ProcessApproachingDeadlineAsync(
        Lauf.Domain.Entities.Flows.FlowAssignment assignment,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Обработка приближающегося дедлайна {AssignmentId}, дедлайн: {Deadline}",
            assignment.Id,
            assignment.Deadline);

        // Уведомляем пользователя о приближающемся дедлайне через специализированный метод
        var daysLeft = (int)(assignment.Deadline - DateTime.UtcNow).TotalDays;
        await _notificationService.NotifyApproachingDeadlineAsync(
            assignment.UserId,
            assignment.Flow.Name,
            assignment.Deadline,
            daysLeft,
            assignment.Id,
            cancellationToken);

        // Уведомляем бадди если есть через системное уведомление
        if (assignment.Buddy.HasValue)
        {
            await _notificationService.CreateCustomNotificationAsync(
                assignment.Buddy.Value,
                Domain.Enums.NotificationType.SystemNotification,
                "У подопечного приближается дедлайн",
                $"У подопечного приближается дедлайн по назначению потока '{assignment.Flow.Name}' ({daysLeft} дн.)",
                Domain.Enums.NotificationPriority.Medium,
                cancellationToken: cancellationToken);
        }
    }
}