using MediatR;
using Microsoft.Extensions.Logging;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Application.DTOs.Flows;
using Lauf.Domain.Enums;

namespace Lauf.Application.Queries.FlowAssignments;

/// <summary>
/// Обработчик запроса получения назначений потоков по пользователю
/// </summary>
public class GetFlowAssignmentsByUserQueryHandler : IRequestHandler<GetFlowAssignmentsByUserQuery, GetFlowAssignmentsByUserQueryResult>
{
    private readonly IFlowAssignmentRepository _flowAssignmentRepository;
    private readonly ILogger<GetFlowAssignmentsByUserQueryHandler> _logger;

    public GetFlowAssignmentsByUserQueryHandler(
        IFlowAssignmentRepository flowAssignmentRepository,
        ILogger<GetFlowAssignmentsByUserQueryHandler> logger)
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

    public async Task<GetFlowAssignmentsByUserQueryResult> Handle(GetFlowAssignmentsByUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Получение назначений для пользователя {UserId}", request.UserId);

            // Получаем назначения пользователя
            var assignments = await _flowAssignmentRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            
            var totalCount = assignments.Count();
            var pagedAssignments = assignments
                .Skip(request.Skip)
                .Take(request.Take)
                .ToList();

            // Конвертируем в DTO
            var assignmentDtos = pagedAssignments.Select(assignment => new FlowAssignmentDto
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

            _logger.LogInformation("Найдено {TotalCount} назначений для пользователя {UserId}", 
                totalCount, request.UserId);

            return new GetFlowAssignmentsByUserQueryResult
            {
                Assignments = assignmentDtos,
                TotalCount = totalCount,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении назначений пользователя {UserId}", request.UserId);
            
            return new GetFlowAssignmentsByUserQueryResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}