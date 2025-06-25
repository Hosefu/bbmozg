using MediatR;
using Microsoft.Extensions.Logging;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Application.DTOs.Flows;

namespace Lauf.Application.Queries.FlowAssignments;

/// <summary>
/// Обработчик запроса получения назначений потока по ID потока
/// </summary>
public class GetFlowAssignmentsByFlowQueryHandler : IRequestHandler<GetFlowAssignmentsByFlowQuery, GetFlowAssignmentsByFlowQueryResult>
{
    private readonly IFlowAssignmentRepository _flowAssignmentRepository;
    private readonly ILogger<GetFlowAssignmentsByFlowQueryHandler> _logger;

    public GetFlowAssignmentsByFlowQueryHandler(
        IFlowAssignmentRepository flowAssignmentRepository,
        ILogger<GetFlowAssignmentsByFlowQueryHandler> logger)
    {
        _flowAssignmentRepository = flowAssignmentRepository;
        _logger = logger;
    }

    public async Task<GetFlowAssignmentsByFlowQueryResult> Handle(GetFlowAssignmentsByFlowQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Получение назначений для потока {FlowId}", request.FlowId);

            // Получаем назначения потока
            var assignments = await _flowAssignmentRepository.GetByFlowIdAsync(request.FlowId, cancellationToken);
            
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
                Status = assignment.Status,
                AssignedById = assignment.AssignedById,
                BuddyId = assignment.BuddyId,
                DueDate = assignment.DueDate,
                CompletedAt = assignment.CompletedAt,
                CreatedAt = assignment.CreatedAt,
                UpdatedAt = assignment.UpdatedAt
            }).ToList();

            _logger.LogInformation("Найдено {TotalCount} назначений для потока {FlowId}", 
                totalCount, request.FlowId);

            return new GetFlowAssignmentsByFlowQueryResult
            {
                Assignments = assignmentDtos,
                TotalCount = totalCount,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении назначений потока {FlowId}", request.FlowId);
            
            return new GetFlowAssignmentsByFlowQueryResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}