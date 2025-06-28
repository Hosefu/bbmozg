using MediatR;
using Microsoft.Extensions.Logging;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Application.DTOs.Flows;
using Lauf.Domain.Enums;

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
            AssignmentStatus.Paused => ProgressStatus.InProgress, // Паузу считаем как "в процессе"
            AssignmentStatus.Overdue => ProgressStatus.InProgress, // Просроченное тоже как "в процессе"
            _ => ProgressStatus.NotStarted
        };
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

            // Конвертируем в DTO (новая архитектура)
            var assignmentDto = new FlowAssignmentDto
            {
                Id = assignment.Id,
                UserId = assignment.UserId,
                FlowId = assignment.FlowId,
                Status = ConvertAssignmentStatusToProgressStatus(assignment.Status),
                AssignedBy = assignment.AssignedBy,
                Buddy = assignment.Buddy,
                Deadline = assignment.Deadline,
                CompletedAt = assignment.CompletedAt,
                AssignedAt = assignment.AssignedAt,
                Notes = null // Будет добавлено при необходимости
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