using MediatR;
using Microsoft.Extensions.Logging;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Application.DTOs.Flows;
using AutoMapper;
using Lauf.Domain.Enums;

namespace Lauf.Application.Queries.FlowAssignments;

/// <summary>
/// Обработчик запроса получения списка назначений потоков
/// </summary>
public class GetFlowAssignmentsQueryHandler : IRequestHandler<GetFlowAssignmentsQuery, GetFlowAssignmentsQueryResult>
{
    private readonly IFlowAssignmentRepository _flowAssignmentRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetFlowAssignmentsQueryHandler> _logger;

    public GetFlowAssignmentsQueryHandler(
        IFlowAssignmentRepository flowAssignmentRepository,
        IMapper mapper,
        ILogger<GetFlowAssignmentsQueryHandler> logger)
    {
        _flowAssignmentRepository = flowAssignmentRepository;
        _mapper = mapper;
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

    public async Task<GetFlowAssignmentsQueryResult> Handle(GetFlowAssignmentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Получение списка назначений: Skip={Skip}, Take={Take}", 
                request.Skip, request.Take);

            // Получаем назначения из репозитория
            var assignments = await _flowAssignmentRepository.GetAllAsync(cancellationToken);
            
            // Применяем фильтры
            var filteredAssignments = assignments.AsEnumerable();
            
            if (request.Status.HasValue)
            {
                filteredAssignments = filteredAssignments.Where(a => a.Status == request.Status.Value);
            }

            if (request.UserId.HasValue)
            {
                filteredAssignments = filteredAssignments.Where(a => a.UserId == request.UserId.Value);
            }

            if (request.FlowId.HasValue)
            {
                filteredAssignments = filteredAssignments.Where(a => a.FlowId == request.FlowId.Value);
            }

            var totalCount = filteredAssignments.Count();
            var pagedAssignments = filteredAssignments
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

            _logger.LogInformation("Найдено {TotalCount} назначений, возвращено {Count}", 
                totalCount, assignmentDtos.Count);

            return new GetFlowAssignmentsQueryResult
            {
                Assignments = assignmentDtos,
                TotalCount = totalCount,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении списка назначений");
            
            return new GetFlowAssignmentsQueryResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}