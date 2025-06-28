using MediatR;
using Microsoft.Extensions.Logging;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Application.DTOs.Flows;

namespace Lauf.Application.Queries.FlowAssignments;

/// <summary>
/// Обработчик запроса получения активных назначений пользователя
/// </summary>
public class GetActiveAssignmentsQueryHandler : IRequestHandler<GetActiveAssignmentsQuery, GetActiveAssignmentsQueryResult>
{
    private readonly IFlowAssignmentRepository _assignmentRepository;
    private readonly ILogger<GetActiveAssignmentsQueryHandler> _logger;

    public GetActiveAssignmentsQueryHandler(
        IFlowAssignmentRepository assignmentRepository,
        ILogger<GetActiveAssignmentsQueryHandler> logger)
    {
        _assignmentRepository = assignmentRepository;
        _logger = logger;
    }

    /// <summary>
    /// Конвертирует AssignmentStatus в ProgressStatus для DTO
    /// </summary>
    private static Domain.Enums.ProgressStatus ConvertToProgressStatus(Domain.Enums.AssignmentStatus status)
    {
        return status switch
        {
            Domain.Enums.AssignmentStatus.Assigned => Domain.Enums.ProgressStatus.NotStarted,
            Domain.Enums.AssignmentStatus.InProgress => Domain.Enums.ProgressStatus.InProgress,
            Domain.Enums.AssignmentStatus.Completed => Domain.Enums.ProgressStatus.Completed,
            Domain.Enums.AssignmentStatus.Cancelled => Domain.Enums.ProgressStatus.Cancelled,
            Domain.Enums.AssignmentStatus.Paused => Domain.Enums.ProgressStatus.InProgress,
            Domain.Enums.AssignmentStatus.Overdue => Domain.Enums.ProgressStatus.InProgress,
            _ => Domain.Enums.ProgressStatus.NotStarted
        };
    }

    public async Task<GetActiveAssignmentsQueryResult> Handle(GetActiveAssignmentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Получение активных назначений для пользователя {UserId}", request.UserId);

            // Получаем активные назначения пользователя (новая архитектура)
            var activeAssignments = await _assignmentRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            var filteredAssignments = activeAssignments.Where(a => 
                a.Status == Domain.Enums.AssignmentStatus.Assigned || 
                a.Status == Domain.Enums.AssignmentStatus.InProgress).ToList();

            // Преобразуем в DTO (новая архитектура)
            var assignmentDtos = filteredAssignments.Select(assignment => new FlowAssignmentDto
            {
                Id = assignment.Id,
                UserId = assignment.UserId,
                FlowId = assignment.FlowId,
                Status = ConvertToProgressStatus(assignment.Status),
                AssignedBy = assignment.AssignedBy,
                Buddy = assignment.Buddy,
                Deadline = assignment.Deadline,
                CompletedAt = assignment.CompletedAt,
                AssignedAt = assignment.AssignedAt
            }).ToList();

            _logger.LogInformation("Найдено {Count} активных назначений для пользователя {UserId}", 
                assignmentDtos.Count, request.UserId);

            return new GetActiveAssignmentsQueryResult
            {
                Assignments = assignmentDtos,
                // TotalCount убран если нет в GetActiveAssignmentsQueryResult
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении активных назначений для пользователя {UserId}", request.UserId);
            
            return new GetActiveAssignmentsQueryResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}