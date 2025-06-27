using Lauf.Domain.Interfaces.Repositories;
using Lauf.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Lauf.Application.BackgroundJobs;

/// <summary>
/// Фоновая задача для проверки дедлайнов назначений
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

            // Получаем все активные назначения (новая архитектура)
            var activeAssignments = await _assignmentRepository.GetActiveAsync(cancellationToken);

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

        // Уведомляем пользователя
        await NotifyUserAboutOverdueAsync(assignment, cancellationToken);

        // Уведомляем бадди если есть
        if (assignment.Buddy != null)
        {
            await NotifyBuddyAboutOverdueAsync(assignment, cancellationToken);
        }

        // Уведомляем администратора/HR
        await NotifyAdminAboutOverdueAsync(assignment, cancellationToken);
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

        // Уведомляем пользователя о приближающемся дедлайне
        await NotifyUserAboutApproachingDeadlineAsync(assignment, cancellationToken);

        // Уведомляем бадди если есть
        if (assignment.Buddy != null)
        {
            await NotifyBuddyAboutApproachingDeadlineAsync(
                assignment.UserId,
                assignment.FlowId,
                assignment.Buddy.Id,
                assignment.Deadline,
                cancellationToken);
        }
    }

    /// <summary>
    /// Уведомление пользователя о просрочке
    /// </summary>
    private async Task NotifyUserAboutOverdueAsync(
        Lauf.Domain.Entities.Flows.FlowAssignment assignment,
        CancellationToken cancellationToken)
    {
        try
        {
            // Создаем уведомление о просрочке
            await _notificationService.NotifyDeadlineOverdueAsync(
                assignment.UserId,
                assignment.FlowId,
                assignment.Deadline,
                assignment.Id,
                cancellationToken);

            _logger.LogInformation(
                "Уведомление о просрочке отправлено пользователю {UserId} для назначения {AssignmentId}",
                assignment.UserId, assignment.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Ошибка отправки уведомления о просрочке пользователю {UserId}",
                assignment.UserId);
        }
    }

    /// <summary>
    /// Уведомление пользователя о приближающемся дедлайне
    /// </summary>
    private async Task NotifyUserAboutApproachingDeadlineAsync(
        Lauf.Domain.Entities.Flows.FlowAssignment assignment,
        CancellationToken cancellationToken)
    {
        try
        {
            await _notificationService.NotifyDeadlineApproachingAsync(
                assignment.UserId,
                assignment.FlowId,
                assignment.Deadline,
                assignment.Id,
                cancellationToken);

            _logger.LogInformation(
                "Уведомление о приближающемся дедлайне отправлено пользователю {UserId}",
                assignment.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Ошибка отправки уведомления о приближающемся дедлайне пользователю {UserId}",
                assignment.UserId);
        }
    }

    /// <summary>
    /// Уведомление бадди о просрочке подопечного
    /// </summary>
    private async Task NotifyBuddyAboutOverdueAsync(
        Lauf.Domain.Entities.Flows.FlowAssignment assignment,
        CancellationToken cancellationToken)
    {
        try
        {
            await _notificationService.NotifyBuddyAboutOverdueAsync(
                assignment.Buddy!.Id,
                assignment.UserId,
                assignment.FlowId,
                assignment.Deadline,
                assignment.Id,
                cancellationToken);

            _logger.LogInformation(
                "Уведомление о просрочке подопечного отправлено бадди {BuddyId}",
                assignment.Buddy.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Ошибка отправки уведомления бадди {BuddyId} о просрочке",
                assignment.Buddy?.Id);
        }
    }

    /// <summary>
    /// Уведомление бадди о приближающемся дедлайне подопечного
    /// </summary>
    private async Task NotifyBuddyAboutApproachingDeadlineAsync(
        Guid userId,
        Guid flowId,
        Guid buddyId,
        DateTime deadline,
        CancellationToken cancellationToken)
    {
        try
        {
            await _notificationService.NotifyBuddyAboutApproachingDeadlineAsync(
                buddyId, userId, flowId, deadline, cancellationToken);

            _logger.LogInformation(
                "Уведомление о приближающемся дедлайне подопечного отправлено бадди {BuddyId}",
                buddyId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Ошибка отправки уведомления бадди {BuddyId} о приближающемся дедлайне",
                buddyId);
        }
    }

    /// <summary>
    /// Уведомление администратора о просрочке
    /// </summary>
    private async Task NotifyAdminAboutOverdueAsync(
        Lauf.Domain.Entities.Flows.FlowAssignment assignment,
        CancellationToken cancellationToken)
    {
        try
        {
            // Логика уведомления администраторов будет реализована позже
            _logger.LogInformation(
                "Требуется уведомить администратора о просрочке назначения {AssignmentId}",
                assignment.Id);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Ошибка отправки уведомления администратору о просрочке назначения {AssignmentId}",
                assignment.Id);
        }
    }
}