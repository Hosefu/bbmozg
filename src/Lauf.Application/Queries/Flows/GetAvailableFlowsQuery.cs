using MediatR;
using AutoMapper;
using Lauf.Application.DTOs.Flows;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Domain.Enums;

namespace Lauf.Application.Queries.Flows;

/// <summary>
/// Запрос для получения доступных потоков
/// </summary>
public record GetAvailableFlowsQuery : IRequest<IEnumerable<FlowDto>>
{
    /// <summary>
    /// Идентификатор пользователя для контекста
    /// </summary>
    public Guid? UserId { get; init; }

    /// <summary>
    /// Категория потоков
    /// </summary>
    public string? Category { get; init; }

    /// <summary>
    /// Поисковый запрос
    /// </summary>
    public string? SearchText { get; init; }

    /// <summary>
    /// Только обязательные потоки
    /// </summary>
    public bool OnlyRequired { get; init; } = false;

    /// <summary>
    /// Количество записей для пропуска
    /// </summary>
    public int Skip { get; init; } = 0;

    /// <summary>
    /// Количество записей для получения
    /// </summary>
    public int Take { get; init; } = 50;

    public GetAvailableFlowsQuery(
        Guid? userId = null,
        string? category = null,
        string? searchText = null,
        bool onlyRequired = false,
        int skip = 0,
        int take = 50)
    {
        UserId = userId;
        Category = category;
        SearchText = searchText;
        OnlyRequired = onlyRequired;
        Skip = skip;
        Take = take;
    }
}

/// <summary>
/// Обработчик запроса получения доступных потоков
/// </summary>
public class GetAvailableFlowsQueryHandler : IRequestHandler<GetAvailableFlowsQuery, IEnumerable<FlowDto>>
{
    private readonly IFlowRepository _flowRepository;
    private readonly IFlowAssignmentRepository _assignmentRepository;
    private readonly IMapper _mapper;

    public GetAvailableFlowsQueryHandler(
        IFlowRepository flowRepository,
        IFlowAssignmentRepository assignmentRepository,
        IMapper mapper)
    {
        _flowRepository = flowRepository ?? throw new ArgumentNullException(nameof(flowRepository));
        _assignmentRepository = assignmentRepository ?? throw new ArgumentNullException(nameof(assignmentRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Обработка запроса получения доступных потоков
    /// </summary>
    public async Task<IEnumerable<FlowDto>> Handle(GetAvailableFlowsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Lauf.Domain.Entities.Flows.Flow> flows;

        // Применяем фильтры
        if (request.OnlyRequired)
        {
            flows = await _flowRepository.GetRequiredFlowsAsync(cancellationToken);
        }
        else if (!string.IsNullOrEmpty(request.SearchText))
        {
            flows = await _flowRepository.SearchAsync(request.SearchText, request.Skip, request.Take, cancellationToken);
        }
        else if (!string.IsNullOrEmpty(request.Category))
        {
            flows = await _flowRepository.GetByCategoryAsync(request.Category, request.Skip, request.Take, cancellationToken);
        }
        else
        {
            flows = await _flowRepository.GetPublishedAsync(request.Skip, request.Take, cancellationToken);
        }

        // Фильтруем уже назначенные потоки для пользователя
        if (request.UserId.HasValue)
        {
            var userAssignments = await _assignmentRepository.GetByUserIdAsync(request.UserId.Value, cancellationToken);
            var assignedFlowIds = userAssignments.Select(a => a.FlowId).ToHashSet();
            flows = flows.Where(f => !assignedFlowIds.Contains(f.Id));
        }

        return _mapper.Map<IEnumerable<FlowDto>>(flows);
    }
}