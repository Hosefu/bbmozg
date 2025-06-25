using MediatR;
using Microsoft.Extensions.Logging;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Application.DTOs.Flows;
using Lauf.Domain.Enums;

namespace Lauf.Application.Queries.FlowAssignments;

/// <summary>
/// Обработчик запроса получения активных назначений пользователя
/// </summary>
public class GetActiveAssignmentsQueryHandler : IRequestHandler<GetActiveAssignmentsQuery, GetActiveAssignmentsQueryResult>
{
    private readonly IFlowAssignmentRepository _flowAssignmentRepository;
    private readonly ILogger<GetActiveAssignmentsQueryHandler> _logger;

    public GetActiveAssignmentsQueryHandler(
        IFlowAssignmentRepository flowAssignmentRepository,
        ILogger<GetActiveAssignmentsQueryHandler> logger)
    {
        _flowAssignmentRepository = flowAssignmentRepository;
        _logger = logger;
    }

    public async Task<GetActiveAssignmentsQueryResult> Handle(GetActiveAssignmentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Получение активных назначений для пользователя {UserId}", request.UserId);

            // Получаем назначения пользователя
            var assignments = await _flowAssignmentRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            
            // Фильтруем только активные назначения (назначенные и в процессе)
            var activeAssignments = assignments.Where(a => 
                a.Status == AssignmentStatus.Assigned || 
                a.Status == AssignmentStatus.InProgress).ToList();

            // Конвертируем в DTO
            var assignmentDtos = activeAssignments.Select(assignment => new FlowAssignmentDto
            {
                Id = assignment.Id,
                UserId = assignment.UserId,
                FlowId = assignment.FlowId,
                Status = assignment.Status,
                AssignedById = assignment.AssignedById,
                BuddyId = assignment.BuddyId,
                DueDate = assignment.DueDate,
                CompletedAt = assignment.CompletedAt,
                CreatedAt = assignment.CreatedAt,
                UpdatedAt = assignment.UpdatedAt
            }).ToList();

            _logger.LogInformation("Найдено {Count} активных назначений для пользователя {UserId}", 
                assignmentDtos.Count, request.UserId);

            return new GetActiveAssignmentsQueryResult
            {
                Assignments = assignmentDtos,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении активных назначений пользователя {UserId}", request.UserId);
            
            return new GetActiveAssignmentsQueryResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}