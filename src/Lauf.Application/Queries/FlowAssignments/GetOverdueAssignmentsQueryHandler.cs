using MediatR;
using Microsoft.Extensions.Logging;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Application.DTOs.Flows;
using Lauf.Domain.Enums;

namespace Lauf.Application.Queries.FlowAssignments;

/// <summary>
/// Обработчик запроса получения просроченных назначений
/// </summary>
public class GetOverdueAssignmentsQueryHandler : IRequestHandler<GetOverdueAssignmentsQuery, GetOverdueAssignmentsQueryResult>
{
    private readonly IFlowAssignmentRepository _flowAssignmentRepository;
    private readonly ILogger<GetOverdueAssignmentsQueryHandler> _logger;

    public GetOverdueAssignmentsQueryHandler(
        IFlowAssignmentRepository flowAssignmentRepository,
        ILogger<GetOverdueAssignmentsQueryHandler> logger)
    {
        _flowAssignmentRepository = flowAssignmentRepository;
        _logger = logger;
    }

    /// <summary>
    /// Конвертирует AssignmentStatus в ProgressStatus
    /// </summary>
    private static ProgressStatus ConvertAssignmentStatusToProgressStatus(AssignmentStatus status)
    {
        return status switch
        {
            AssignmentStatus.Assigned => ProgressStatus.NotStarted,
            AssignmentStatus.InProgress => ProgressStatus.InProgress,
            AssignmentStatus.Completed => ProgressStatus.Completed,
            AssignmentStatus.Cancelled => ProgressStatus.Cancelled,
            AssignmentStatus.Paused => ProgressStatus.InProgress,
            AssignmentStatus.Overdue => ProgressStatus.InProgress,
            _ => ProgressStatus.NotStarted
        };
    }

    public async Task<GetOverdueAssignmentsQueryResult> Handle(GetOverdueAssignmentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Получение просроченных назначений");

            // Получаем все назначения
            var assignments = await _flowAssignmentRepository.GetAllAsync(cancellationToken);
            
            var now = DateTime.UtcNow;
            
            // Фильтруем просроченные назначения (с вычисляемым дедлайном, но не завершенные)
            var overdueAssignments = assignments.Where(a => 
            {
                var deadline = a.Progress?.StartedAt?.AddDays(30) ?? a.AssignedAt.AddDays(30);
                return deadline < now &&
                       a.Status != AssignmentStatus.Completed &&
                       a.Status != AssignmentStatus.Cancelled;
            }).ToList();

            // Конвертируем в DTO
            var assignmentDtos = overdueAssignments.Select(assignment => new FlowAssignmentDto
            {
                Id = assignment.Id,
                UserId = assignment.UserId,
                FlowId = assignment.FlowId,
                Status = ConvertAssignmentStatusToProgressStatus(assignment.Status),
                AssignedBy = assignment.AssignedBy,
                Buddy = assignment.Buddies?.FirstOrDefault()?.Id,
                Deadline = assignment.Progress?.StartedAt?.AddDays(30) ?? assignment.AssignedAt.AddDays(30),
                CompletedAt = assignment.Progress?.CompletedAt,
                AssignedAt = assignment.AssignedAt,
                Notes = null
            }).ToList();

            _logger.LogInformation("Найдено {Count} просроченных назначений", assignmentDtos.Count);

            return new GetOverdueAssignmentsQueryResult
            {
                Assignments = assignmentDtos,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении просроченных назначений");
            
            return new GetOverdueAssignmentsQueryResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}