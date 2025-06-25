using MediatR;
using AutoMapper;
using Lauf.Application.DTOs.Flows;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Domain.Enums;

namespace Lauf.Application.Queries.Users;

/// <summary>
/// Запрос для получения потоков пользователя
/// </summary>
public record GetUserFlowsQuery : IRequest<IEnumerable<FlowDto>>
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Фильтр по статусу назначения
    /// </summary>
    public AssignmentStatus? Status { get; init; }

    /// <summary>
    /// Включать ли завершенные потоки
    /// </summary>
    public bool IncludeCompleted { get; init; } = false;

    public GetUserFlowsQuery(Guid userId, AssignmentStatus? status = null, bool includeCompleted = false)
    {
        UserId = userId;
        Status = status;
        IncludeCompleted = includeCompleted;
    }
}

/// <summary>
/// Обработчик запроса получения потоков пользователя
/// </summary>
public class GetUserFlowsQueryHandler : IRequestHandler<GetUserFlowsQuery, IEnumerable<FlowDto>>
{
    private readonly IFlowAssignmentRepository _assignmentRepository;
    private readonly IMapper _mapper;

    public GetUserFlowsQueryHandler(IFlowAssignmentRepository assignmentRepository, IMapper mapper)
    {
        _assignmentRepository = assignmentRepository ?? throw new ArgumentNullException(nameof(assignmentRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Обработка запроса получения потоков пользователя
    /// </summary>
    public async Task<IEnumerable<FlowDto>> Handle(GetUserFlowsQuery request, CancellationToken cancellationToken)
    {
        var assignments = await _assignmentRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        // Фильтрация по статусу
        if (request.Status.HasValue)
        {
            assignments = assignments.Where(a => a.Status == request.Status.Value);
        }

        // Фильтрация завершенных
        if (!request.IncludeCompleted)
        {
            assignments = assignments.Where(a => a.Status != AssignmentStatus.Completed);
        }

        var flows = assignments.Select(a => a.Flow).Where(f => f != null);
        return _mapper.Map<IEnumerable<FlowDto>>(flows);
    }
}