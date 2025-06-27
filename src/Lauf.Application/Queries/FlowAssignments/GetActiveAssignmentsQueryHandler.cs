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

    public async Task<GetActiveAssignmentsQueryResult> Handle(GetActiveAssignmentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Получение активных назначений для пользователя {UserId}", request.UserId);

            // Получаем активные назначения пользователя (новая архитектура)
            var activeAssignments = await _assignmentRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            var filteredAssignments = activeAssignments.Where(a => a.Status == Domain.Enums.ProgressStatus.InProgress).ToList();

            // Преобразуем в DTO (новая архитектура)
            var assignmentDtos = filteredAssignments.Select(assignment => new FlowAssignmentDto
            {
                Id = assignment.Id,
                UserId = assignment.UserId,
                FlowId = assignment.FlowId,
                Status = assignment.Status,
                AssignedBy = assignment.AssignedBy,
                Buddy = assignment.Buddy?.Id,
                Deadline = assignment.Deadline, // DueDate теперь Deadline
                CompletedAt = assignment.CompletedAt,
                AssignedAt = assignment.AssignedAt // CreatedAt теперь AssignedAt
            }).ToList();

            _logger.LogInformation("Найдено {Count} активных назначений для пользователя {UserId}", 
                assignmentDtos.Count, request.UserId);

            return new GetActiveAssignmentsQueryResult
            {
                Assignments = assignmentDtos,
                TotalCount = assignmentDtos.Count,
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