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

    public async Task<GetOverdueAssignmentsQueryResult> Handle(GetOverdueAssignmentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Получение просроченных назначений");

            // Получаем все назначения
            var assignments = await _flowAssignmentRepository.GetAllAsync(cancellationToken);
            
            var now = DateTime.UtcNow;
            
            // Фильтруем просроченные назначения (с установленным дедлайном, но не завершенные)
            var overdueAssignments = assignments.Where(a => 
                a.DueDate.HasValue && 
                a.DueDate.Value < now &&
                a.Status != AssignmentStatus.Completed &&
                a.Status != AssignmentStatus.Cancelled).ToList();

            // Конвертируем в DTO
            var assignmentDtos = overdueAssignments.Select(assignment => new FlowAssignmentDto
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