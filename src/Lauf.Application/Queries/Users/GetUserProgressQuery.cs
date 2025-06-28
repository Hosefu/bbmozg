using MediatR;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Domain.Enums;

namespace Lauf.Application.Queries.Users;

/// <summary>
/// Запрос для получения прогресса пользователя
/// </summary>
public record GetUserProgressQuery : IRequest<UserProgressDto?>
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Идентификатор назначения (опционально)
    /// </summary>
    public Guid? AssignmentId { get; init; }

    public GetUserProgressQuery(Guid userId, Guid? assignmentId = null)
    {
        UserId = userId;
        AssignmentId = assignmentId;
    }
}

/// <summary>
/// DTO для прогресса пользователя
/// </summary>
public class UserProgressDto
{
    /// <summary>
    /// Общий прогресс пользователя
    /// </summary>
    public decimal OverallProgress { get; set; }

    /// <summary>
    /// Количество активных назначений
    /// </summary>
    public int ActiveAssignments { get; set; }

    /// <summary>
    /// Количество завершенных назначений
    /// </summary>
    public int CompletedAssignments { get; set; }

    /// <summary>
    /// Количество просроченных назначений
    /// </summary>
    public int OverdueAssignments { get; set; }

    /// <summary>
    /// Прогресс по конкретным назначениям
    /// </summary>
    public ICollection<AssignmentProgressDto> AssignmentProgress { get; set; } = new List<AssignmentProgressDto>();
}

/// <summary>
/// DTO для прогресса назначения
/// </summary>
public class AssignmentProgressDto
{
    /// <summary>
    /// Идентификатор назначения
    /// </summary>
    public Guid AssignmentId { get; set; }

    /// <summary>
    /// Название потока
    /// </summary>
    public string FlowTitle { get; set; } = string.Empty;

    /// <summary>
    /// Прогресс выполнения
    /// </summary>
    public decimal Progress { get; set; }

    /// <summary>
    /// Статус назначения
    /// </summary>
    public AssignmentStatus Status { get; set; }

    /// <summary>
    /// Дедлайн
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Просрочен ли
    /// </summary>
    public bool IsOverdue { get; set; }
}

/// <summary>
/// Обработчик запроса получения прогресса пользователя
/// </summary>
public class GetUserProgressQueryHandler : IRequestHandler<GetUserProgressQuery, UserProgressDto?>
{
    private readonly IUserProgressRepository _progressRepository;
    private readonly IFlowAssignmentRepository _assignmentRepository;

    public GetUserProgressQueryHandler(
        IUserProgressRepository progressRepository,
        IFlowAssignmentRepository assignmentRepository)
    {
        _progressRepository = progressRepository ?? throw new ArgumentNullException(nameof(progressRepository));
        _assignmentRepository = assignmentRepository ?? throw new ArgumentNullException(nameof(assignmentRepository));
    }

    /// <summary>
    /// Обработка запроса получения прогресса пользователя
    /// </summary>
    public async Task<UserProgressDto?> Handle(GetUserProgressQuery request, CancellationToken cancellationToken)
    {
        var assignments = await _assignmentRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        
        if (!assignments.Any())
        {
            return null;
        }

        var userProgress = new UserProgressDto();
        var assignmentProgressList = new List<AssignmentProgressDto>();

        foreach (var assignment in assignments)
        {
            var progress = await _progressRepository.GetByAssignmentIdAsync(assignment.Id, cancellationToken);
            
            var assignmentProgress = new AssignmentProgressDto
            {
                AssignmentId = assignment.Id,
                FlowTitle = assignment.Flow?.Name ?? "Неизвестный поток",
                Progress = progress?.OverallProgress?.Value ?? 0,
                Status = assignment.Status,
                DueDate = assignment.Deadline,
                IsOverdue = assignment.Deadline < DateTime.UtcNow && assignment.Status != AssignmentStatus.Completed
            };

            assignmentProgressList.Add(assignmentProgress);
        }

        userProgress.AssignmentProgress = assignmentProgressList;
        userProgress.ActiveAssignments = assignmentProgressList.Count(a => a.Status == AssignmentStatus.Assigned || a.Status == AssignmentStatus.InProgress);
        userProgress.CompletedAssignments = assignmentProgressList.Count(a => a.Status == AssignmentStatus.Completed);
        userProgress.OverdueAssignments = assignmentProgressList.Count(a => a.IsOverdue);
        userProgress.OverallProgress = assignmentProgressList.Any() ? assignmentProgressList.Average(a => a.Progress) : 0;

        return userProgress;
    }
}