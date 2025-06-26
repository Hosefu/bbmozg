using MediatR;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Domain.Enums;

namespace Lauf.Application.Queries.Progress;

/// <summary>
/// Запрос для получения прогресса по назначению
/// </summary>
public record GetProgressByAssignmentQuery : IRequest<AssignmentProgressDetailsDto?>
{
    /// <summary>
    /// Идентификатор назначения
    /// </summary>
    public Guid AssignmentId { get; init; }

    public GetProgressByAssignmentQuery(Guid assignmentId)
    {
        AssignmentId = assignmentId;
    }
}

/// <summary>
/// DTO для детального прогресса назначения
/// </summary>
public class AssignmentProgressDetailsDto
{
    /// <summary>
    /// Идентификатор назначения
    /// </summary>
    public Guid AssignmentId { get; set; }

    /// <summary>
    /// Информация о потоке
    /// </summary>
    public FlowInfoDto Flow { get; set; } = new();

    /// <summary>
    /// Информация о пользователе
    /// </summary>
    public UserInfoDto User { get; set; } = new();

    /// <summary>
    /// Общий прогресс (0-100)
    /// </summary>
    public decimal OverallProgress { get; set; }

    /// <summary>
    /// Статус назначения
    /// </summary>
    public AssignmentStatus Status { get; set; }

    /// <summary>
    /// Дата начала
    /// </summary>
    public DateTime StartedAt { get; set; }

    /// <summary>
    /// Дедлайн
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Просрочен ли
    /// </summary>
    public bool IsOverdue { get; set; }

    /// <summary>
    /// Дней осталось до дедлайна
    /// </summary>
    public int? DaysUntilDeadline { get; set; }

    /// <summary>
    /// Последняя активность
    /// </summary>
    public DateTime? LastActivityAt { get; set; }

    /// <summary>
    /// Прогресс по шагам
    /// </summary>
    public ICollection<StepProgressDto> StepProgress { get; set; } = new List<StepProgressDto>();

    /// <summary>
    /// Статистика времени
    /// </summary>
    public TimeStatsDto TimeStats { get; set; } = new();
}

/// <summary>
/// DTO для информации о потоке
/// </summary>
public class FlowInfoDto
{
    /// <summary>
    /// Идентификатор потока
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название потока
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание потока
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Общее количество шагов
    /// </summary>
    public int TotalSteps { get; set; }

    /// <summary>
    /// Примерное время прохождения (в часах)
    /// </summary>
    public int? EstimatedHours { get; set; }
}

/// <summary>
/// DTO для информации о пользователе
/// </summary>
public class UserInfoDto
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Фамилия пользователя
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Telegram ID пользователя
    /// </summary>
    public long TelegramUserId { get; set; }
}

/// <summary>
/// DTO для прогресса шага
/// </summary>
public class StepProgressDto
{
    /// <summary>
    /// Номер шага
    /// </summary>
    public int StepNumber { get; set; }

    /// <summary>
    /// Название шага
    /// </summary>
    public string StepTitle { get; set; } = string.Empty;

    /// <summary>
    /// Прогресс шага (0-100)
    /// </summary>
    public decimal Progress { get; set; }

    /// <summary>
    /// Статус шага
    /// </summary>
    public StepStatus Status { get; set; }

    /// <summary>
    /// Дата начала шага
    /// </summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// Дата завершения шага
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Время, потраченное на шаг
    /// </summary>
    public TimeSpan? TimeSpent { get; set; }

    /// <summary>
    /// Прогресс по компонентам шага
    /// </summary>
    public ICollection<ComponentProgressSummaryDto> ComponentProgress { get; set; } = new List<ComponentProgressSummaryDto>();
}

/// <summary>
/// DTO для краткого прогресса компонента
/// </summary>
public class ComponentProgressSummaryDto
{
    /// <summary>
    /// Название компонента
    /// </summary>
    public string ComponentTitle { get; set; } = string.Empty;

    /// <summary>
    /// Тип компонента
    /// </summary>
    public ComponentType ComponentType { get; set; }

    /// <summary>
    /// Завершен ли компонент
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// Прогресс компонента (0-100)
    /// </summary>
    public decimal Progress { get; set; }

    /// <summary>
    /// Дата завершения
    /// </summary>
    public DateTime? CompletedAt { get; set; }
}

/// <summary>
/// DTO для статистики времени
/// </summary>
public class TimeStatsDto
{
    /// <summary>
    /// Общее время, потраченное на поток
    /// </summary>
    public TimeSpan TotalTimeSpent { get; set; }

    /// <summary>
    /// Дней с момента начала
    /// </summary>
    public int DaysSinceStart { get; set; }

    /// <summary>
    /// Среднее время в день
    /// </summary>
    public TimeSpan AverageTimePerDay { get; set; }

    /// <summary>
    /// Примерное время до завершения
    /// </summary>
    public TimeSpan? EstimatedTimeToCompletion { get; set; }
}

/// <summary>
/// Обработчик запроса получения прогресса по назначению
/// </summary>
public class GetProgressByAssignmentQueryHandler : IRequestHandler<GetProgressByAssignmentQuery, AssignmentProgressDetailsDto?>
{
    private readonly IFlowAssignmentRepository _assignmentRepository;
    private readonly IUserProgressRepository _progressRepository;
    private readonly IUserRepository _userRepository;

    public GetProgressByAssignmentQueryHandler(
        IFlowAssignmentRepository assignmentRepository,
        IUserProgressRepository progressRepository,
        IUserRepository userRepository)
    {
        _assignmentRepository = assignmentRepository ?? throw new ArgumentNullException(nameof(assignmentRepository));
        _progressRepository = progressRepository ?? throw new ArgumentNullException(nameof(progressRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    /// <summary>
    /// Обработка запроса получения прогресса по назначению
    /// </summary>
    public async Task<AssignmentProgressDetailsDto?> Handle(GetProgressByAssignmentQuery request, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentRepository.GetWithDetailsAsync(request.AssignmentId, cancellationToken);
        if (assignment == null)
        {
            return null;
        }

        var progress = await _progressRepository.GetByAssignmentIdAsync(request.AssignmentId, cancellationToken);
        var user = await _userRepository.GetByIdAsync(assignment.UserId, cancellationToken);

        var result = new AssignmentProgressDetailsDto
        {
            AssignmentId = request.AssignmentId,
            Flow = MapFlowInfo(assignment.Flow),
            User = MapUserInfo(user),
            OverallProgress = progress?.OverallProgress?.Value ?? 0,
            Status = assignment.Status,
            StartedAt = assignment.CreatedAt,
            DueDate = assignment.DueDate,
            IsOverdue = assignment.DueDate.HasValue && assignment.DueDate.Value < DateTime.UtcNow && assignment.Status != AssignmentStatus.Completed,
            LastActivityAt = progress?.UpdatedAt,
            TimeStats = CalculateTimeStats(assignment, progress)
        };

        if (result.DueDate.HasValue)
        {
            result.DaysUntilDeadline = (int)(result.DueDate.Value - DateTime.UtcNow).TotalDays;
        }

        // Прогресс по шагам будет загружаться отдельным запросом
        result.StepProgress = new List<StepProgressDto>();

        return result;
    }

    /// <summary>
    /// Маппинг информации о потоке
    /// </summary>
    private FlowInfoDto MapFlowInfo(Lauf.Domain.Entities.Flows.Flow? flow)
    {
        if (flow == null)
        {
            return new FlowInfoDto();
        }

        return new FlowInfoDto
        {
            Id = flow.Id,
            Title = flow.Title,
            Description = flow.Description,
            TotalSteps = flow.Steps?.Count ?? 0,
            EstimatedHours = null // Расчетное время удалено из модели
        };
    }

    /// <summary>
    /// Маппинг информации о пользователе
    /// </summary>
    private UserInfoDto MapUserInfo(Lauf.Domain.Entities.Users.User? user)
    {
        if (user == null)
        {
            return new UserInfoDto();
        }

        return new UserInfoDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            TelegramUserId = user.TelegramUserId?.Value ?? 0
        };
    }

    /// <summary>
    /// Расчет статистики времени
    /// </summary>
    private TimeStatsDto CalculateTimeStats(
        Lauf.Domain.Entities.Flows.FlowAssignment assignment,
        Lauf.Domain.Entities.Progress.UserProgress? progress)
    {
        var daysSinceStart = (int)(DateTime.UtcNow - assignment.CreatedAt).TotalDays;
        daysSinceStart = Math.Max(1, daysSinceStart); // Минимум 1 день

        return new TimeStatsDto
        {
            TotalTimeSpent = TimeSpan.Zero, // Время работы будет отслеживаться через систему аналитики
            DaysSinceStart = daysSinceStart,
            AverageTimePerDay = TimeSpan.Zero, // Среднее время за день будет рассчитываться позже
            EstimatedTimeToCompletion = null // Оценка времени завершения будет рассчитываться по ML моделям
        };
    }
}