using Microsoft.Extensions.Logging;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Domain.Interfaces.Services;
using Lauf.Domain.Enums;

namespace Lauf.Application.BackgroundJobs;

/// <summary>
/// Фоновая задача для отправки ежедневных напоминаний
/// </summary>
public class DailyReminderJob
{
    private readonly ILogger<DailyReminderJob> _logger;
    private readonly IFlowAssignmentRepository _assignmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly INotificationService _notificationService;

    public DailyReminderJob(
        ILogger<DailyReminderJob> logger,
        IFlowAssignmentRepository assignmentRepository,
        IUserRepository userRepository,
        INotificationService notificationService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _assignmentRepository = assignmentRepository ?? throw new ArgumentNullException(nameof(assignmentRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
    }

    /// <summary>
    /// Выполнение ежедневной отправки напоминаний
    /// </summary>
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Начало выполнения задачи ежедневных напоминаний");

        try
        {
            // Получаем активные назначения
            var activeAssignments = await _assignmentRepository.GetByStatusAsync(
                AssignmentStatus.Assigned, 0, 1000, cancellationToken);

            var inProgressAssignments = await _assignmentRepository.GetByStatusAsync(
                AssignmentStatus.InProgress, 0, 1000, cancellationToken);

            var allActiveAssignments = activeAssignments.Concat(inProgressAssignments).ToList();

            _logger.LogInformation(
                "Найдено {Count} активных назначений для отправки напоминаний",
                allActiveAssignments.Count);

            foreach (var assignment in allActiveAssignments)
            {
                try
                {
                    await ProcessAssignmentReminderAsync(assignment, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Ошибка при обработке напоминания для назначения {AssignmentId}",
                        assignment.Id);
                }
            }

            _logger.LogInformation("Задача ежедневных напоминаний успешно завершена");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при выполнении задачи ежедневных напоминаний");
            throw;
        }
    }

    /// <summary>
    /// Обработка напоминания для конкретного назначения
    /// </summary>
    private async Task ProcessAssignmentReminderAsync(
        Lauf.Domain.Entities.Flows.FlowAssignment assignment,
        CancellationToken cancellationToken)
    {
        // Проверяем, нужно ли отправлять напоминание
        if (!ShouldSendReminder(assignment))
        {
            return;
        }

        var user = await _userRepository.GetByIdAsync(assignment.UserId, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning(
                "Пользователь не найден для назначения {AssignmentId}",
                assignment.Id);
            return;
        }

        // Определяем тип напоминания
        var reminderType = GetReminderType(assignment);

        // Отправляем напоминание
        await SendReminderAsync(user, assignment, reminderType, cancellationToken);

        _logger.LogInformation(
            "Напоминание отправлено пользователю {UserId} для назначения {AssignmentId}",
            user.Id,
            assignment.Id);
    }

    /// <summary>
    /// Проверка, нужно ли отправлять напоминание
    /// </summary>
    private bool ShouldSendReminder(Lauf.Domain.Entities.Flows.FlowAssignment assignment)
    {
        // Проверяем время последней активности
        var daysSinceLastActivity = (DateTime.UtcNow - assignment.UpdatedAt).TotalDays;

        // Отправляем напоминание если:
        // 1. Нет активности больше 3 дней для активных назначений
        // 2. Нет активности больше 1 дня для назначений в процессе
        var reminderThreshold = assignment.Status == AssignmentStatus.Assigned ? 3 : 1;

        return daysSinceLastActivity >= reminderThreshold;
    }

    /// <summary>
    /// Определение типа напоминания
    /// </summary>
    private ReminderType GetReminderType(Lauf.Domain.Entities.Flows.FlowAssignment assignment)
    {
        if (assignment.DueDate.HasValue)
        {
            var daysUntilDeadline = (assignment.DueDate.Value - DateTime.UtcNow).TotalDays;

            if (daysUntilDeadline <= 1)
            {
                return ReminderType.UrgentDeadline;
            }
            else if (daysUntilDeadline <= 3)
            {
                return ReminderType.ApproachingDeadline;
            }
        }

        return assignment.Status == AssignmentStatus.Assigned 
            ? ReminderType.NotStarted 
            : ReminderType.InProgress;
    }

    /// <summary>
    /// Отправка напоминания
    /// </summary>
    private async Task SendReminderAsync(
        Lauf.Domain.Entities.Users.User user,
        Lauf.Domain.Entities.Flows.FlowAssignment assignment,
        ReminderType reminderType,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Отправка напоминания типа {ReminderType} пользователю {Email}",
                reminderType,
                user.Email);

            // Отправляем напоминание через NotificationService в зависимости от типа
            switch (reminderType)
            {
                case ReminderType.NotStarted:
                    await _notificationService.NotifyReminderNotStartedAsync(
                        user.Id,
                        assignment.Flow.Title,
                        assignment.DueDate ?? DateTime.UtcNow.AddDays(7),
                        assignment.Id,
                        cancellationToken);
                    break;

                case ReminderType.InProgress:
                    await _notificationService.NotifyReminderInProgressAsync(
                        user.Id,
                        assignment.Flow.Title,
                        assignment.ProgressPercent,
                        assignment.DueDate ?? DateTime.UtcNow.AddDays(7),
                        assignment.Id,
                        cancellationToken);
                    break;

                case ReminderType.ApproachingDeadline:
                    await _notificationService.NotifyApproachingDeadlineAsync(
                        user.Id,
                        assignment.Flow.Title,
                        assignment.DueDate!.Value,
                        (int)(assignment.DueDate!.Value - DateTime.UtcNow).TotalDays,
                        assignment.Id,
                        cancellationToken);
                    break;

                case ReminderType.UrgentDeadline:
                    await _notificationService.NotifyUrgentDeadlineAsync(
                        user.Id,
                        assignment.Flow.Title,
                        assignment.DueDate!.Value,
                        assignment.Id,
                        cancellationToken);
                    break;
            }

            _logger.LogInformation(
                "Напоминание типа {ReminderType} успешно отправлено пользователю {UserId}",
                reminderType,
                user.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Ошибка отправки напоминания типа {ReminderType} пользователю {UserId}",
                reminderType,
                user.Id);
            // Не пробрасываем исключение, чтобы не прерывать обработку других напоминаний
        }
    }
}

/// <summary>
/// Типы напоминаний
/// </summary>
public enum ReminderType
{
    /// <summary>
    /// Поток не начат
    /// </summary>
    NotStarted,

    /// <summary>
    /// Поток в процессе выполнения
    /// </summary>
    InProgress,

    /// <summary>
    /// Приближающийся дедлайн
    /// </summary>
    ApproachingDeadline,

    /// <summary>
    /// Срочный дедлайн
    /// </summary>
    UrgentDeadline
}