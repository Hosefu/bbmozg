using MediatR;
using Microsoft.Extensions.Logging;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Application.DTOs.Flows;

namespace Lauf.Application.Queries.FlowAssignments;

/// <summary>
/// Обработчик запроса получения назначения потока по ID
/// </summary>
public class GetFlowAssignmentByIdQueryHandler : IRequestHandler<GetFlowAssignmentByIdQuery, GetFlowAssignmentByIdQueryResult>
{
    private readonly IFlowAssignmentRepository _flowAssignmentRepository;
    private readonly ILogger<GetFlowAssignmentByIdQueryHandler> _logger;

    public GetFlowAssignmentByIdQueryHandler(
        IFlowAssignmentRepository flowAssignmentRepository,
        ILogger<GetFlowAssignmentByIdQueryHandler> logger)
    {
        _flowAssignmentRepository = flowAssignmentRepository;
        _logger = logger;
    }

    public async Task<GetFlowAssignmentByIdQueryResult> Handle(GetFlowAssignmentByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Получение назначения по ID {AssignmentId}", request.AssignmentId);

            // Получаем назначение по ID
            var assignment = await _flowAssignmentRepository.GetByIdAsync(request.AssignmentId, cancellationToken);
            
            if (assignment == null)
            {
                _logger.LogWarning("Назначение с ID {AssignmentId} не найдено", request.AssignmentId);
                
                return new GetFlowAssignmentByIdQueryResult
                {
                    Assignment = null,
                    Success = true
                };
            }

            // Конвертируем в DTO
            var assignmentDto = new FlowAssignmentDto
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
            };

            _logger.LogInformation("Назначение {AssignmentId} успешно получено", request.AssignmentId);

            return new GetFlowAssignmentByIdQueryResult
            {
                Assignment = assignmentDto,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении назначения {AssignmentId}", request.AssignmentId);
            
            return new GetFlowAssignmentByIdQueryResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}