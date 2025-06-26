using Microsoft.Extensions.Logging;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Domain.Enums;

namespace Lauf.Application.BackgroundJobs;

/// <summary>
/// Фоновая задача для проверки дедлайнов
/// </summary>
public class DeadlineCheckJob
{
    private readonly ILogger<DeadlineCheckJob> _logger;
    private readonly IFlowAssignmentRepository _assignmentRepository;
    private readonly IUserRepository _userRepository;

    public DeadlineCheckJob(
        ILogger<DeadlineCheckJob> logger,
        IFlowAssignmentRepository assignmentRepository,
        IUserRepository userRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _assignmentRepository = assignmentRepository ?? throw new ArgumentNullException(nameof(assignmentRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    /// <summary>
    /// Выполнение проверки дедлайнов
    /// </summary>
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Начало выполнения задачи проверки дедлайнов");

        try
        {
            // Проверяем просроченные назначения
            await CheckOverdueAssignmentsAsync(cancellationToken);

            // Проверяем приближающиеся дедлайны
            await CheckApproachingDeadlinesAsync(cancellationToken);

            _logger.LogInformation("Задача проверки дедлайнов успешно завершена");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при выполнении задачи проверки дедлайнов");
            throw;
        }
    }

    /// <summary>
    /// Проверка просроченных назначений
    /// </summary>
    private async Task CheckOverdueAssignmentsAsync(CancellationToken cancellationToken)
    {
        var overdueAssignments = await _assignmentRepository.GetOverdueAsync(cancellationToken);
        var overdueList = overdueAssignments.ToList();

        _logger.LogInformation(
            "Найдено {Count} просроченных назначений",
            overdueList.Count);

        foreach (var assignment in overdueList)
        {
            try
            {
                await ProcessOverdueAssignmentAsync(assignment, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Ошибка при обработке просроченного назначения {AssignmentId}",
                    assignment.Id);
            }
        }
    }

    /// <summary>
    /// Проверка приближающихся дедлайнов
    /// </summary>
    private async Task CheckApproachingDeadlinesAsync(CancellationToken cancellationToken)
    {
        // Проверяем дедлайны, которые наступят в течение 3 дней
        var approachingAssignments = await _assignmentRepository.GetWithApproachingDeadlineAsync(3, cancellationToken);
        var approachingList = approachingAssignments.ToList();

        _logger.LogInformation(
            "Найдено {Count} назначений с приближающимся дедлайном",
            approachingList.Count);

        foreach (var assignment in approachingList)
        {
            try
            {
                await ProcessApproachingDeadlineAsync(assignment, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Ошибка при обработке приближающегося дедлайна для назначения {AssignmentId}",
                    assignment.Id);
            }
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
            "Обработка просроченного назначения {AssignmentId}, дедлайн: {DueDate}",
            assignment.Id,
            assignment.DueDate);

        // Уведомляем пользователя
        await NotifyUserAboutOverdueAsync(assignment, cancellationToken);

        // Уведомляем бадди
        if (assignment.BuddyId.HasValue)
        {
            await NotifyBuddyAboutOverdueAsync(assignment, cancellationToken);
        }

        // Уведомляем администратора/HR
        await NotifyAdminAboutOverdueAsync(assignment, cancellationToken);

        // Автоматическое управление просрочками будет реализовано позже
    }

    /// <summary>
    /// Обработка приближающегося дедлайна
    /// </summary>
    private async Task ProcessApproachingDeadlineAsync(
        Lauf.Domain.Entities.Flows.FlowAssignment assignment,
        CancellationToken cancellationToken)
    {
        if (!assignment.DueDate.HasValue)
        {
            return;
        }

        var daysUntilDeadline = (assignment.DueDate.Value - DateTime.UtcNow).TotalDays;

        _logger.LogInformation(
            "Обработка приближающегося дедлайна для назначения {AssignmentId}, осталось дней: {DaysLeft}",
            assignment.Id,
            Math.Round(daysUntilDeadline, 1));

        // Определяем тип уведомления в зависимости от оставшегося времени
        if (daysUntilDeadline <= 1)
        {
            await NotifyUrgentDeadlineAsync(assignment, cancellationToken);
        }
        else if (daysUntilDeadline <= 3)
        {
            await NotifyApproachingDeadlineAsync(assignment, cancellationToken);
        }
    }

    /// <summary>
    /// Уведомление пользователя о просрочке
    /// </summary>
    private async Task NotifyUserAboutOverdueAsync(
        Lauf.Domain.Entities.Flows.FlowAssignment assignment,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(assignment.UserId, cancellationToken);
        if (user == null)
        {
            return;
        }

        _logger.LogInformation(
                            "Отправка уведомления о просрочке пользователю {UserId}",
                user.Id);

        // Отправка уведомлений будет реализована через NotificationService
        await Task.CompletedTask;
    }

    /// <summary>
    /// Уведомление бадди о просрочке подопечного
    /// </summary>
    private async Task NotifyBuddyAboutOverdueAsync(
        Lauf.Domain.Entities.Flows.FlowAssignment assignment,
        CancellationToken cancellationToken)
    {
        if (!assignment.BuddyId.HasValue)
        {
            return;
        }

        var buddy = await _userRepository.GetByIdAsync(assignment.BuddyId.Value, cancellationToken);
        if (buddy == null)
        {
            return;
        }

        _logger.LogInformation(
                            "Отправка уведомления бадди {UserId} о просрочке подопечного",
                buddy.Id);

        // Отправка уведомлений будет реализована через NotificationService
        await Task.CompletedTask;
    }

    /// <summary>
    /// Уведомление администратора о просрочке
    /// </summary>
    private async Task NotifyAdminAboutOverdueAsync(
        Lauf.Domain.Entities.Flows.FlowAssignment assignment,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Отправка уведомления администратору о просрочке назначения {AssignmentId}",
            assignment.Id);

        // Отправка уведомлений будет реализована через NotificationService администратору
        await Task.CompletedTask;
    }

    /// <summary>
    /// Уведомление о срочном дедлайне
    /// </summary>
    private async Task NotifyUrgentDeadlineAsync(
        Lauf.Domain.Entities.Flows.FlowAssignment assignment,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Отправка срочного уведомления о дедлайне для назначения {AssignmentId}",
            assignment.Id);

        // Отправка срочных уведомлений будет реализована через NotificationService
        await Task.CompletedTask;
    }

    /// <summary>
    /// Уведомление о приближающемся дедлайне
    /// </summary>
    private async Task NotifyApproachingDeadlineAsync(
        Lauf.Domain.Entities.Flows.FlowAssignment assignment,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Отправка уведомления о приближающемся дедлайне для назначения {AssignmentId}",
            assignment.Id);

        // Отправка уведомлений будет реализована через NotificationService
        await Task.CompletedTask;
    }
}