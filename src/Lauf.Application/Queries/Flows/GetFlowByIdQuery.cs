using MediatR;
using AutoMapper;
using Lauf.Application.DTOs.Flows;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Domain.Entities.Flows;

namespace Lauf.Application.Queries.Flows;

/// <summary>
/// Запрос для получения потока по идентификатору
/// </summary>
public record GetFlowByIdQuery : IRequest<FlowDto?>
{
    /// <summary>
    /// Идентификатор потока
    /// </summary>
    public Guid FlowId { get; init; }

    /// <summary>
    /// Включать ли шаги потока
    /// </summary>
    public bool IncludeSteps { get; init; } = false;

    public GetFlowByIdQuery(Guid flowId, bool includeSteps = false)
    {
        FlowId = flowId;
        IncludeSteps = includeSteps;
    }
}

/// <summary>
/// Обработчик запроса получения потока по идентификатору
/// </summary>
public class GetFlowByIdQueryHandler : IRequestHandler<GetFlowByIdQuery, FlowDto?>
{
    private readonly IFlowRepository _flowRepository;
    private readonly IMapper _mapper;

    public GetFlowByIdQueryHandler(IFlowRepository flowRepository, IMapper mapper)
    {
        _flowRepository = flowRepository ?? throw new ArgumentNullException(nameof(flowRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Обработка запроса получения потока
    /// </summary>
    public async Task<FlowDto?> Handle(GetFlowByIdQuery request, CancellationToken cancellationToken)
    {
        Flow? flow;
        
        if (request.IncludeSteps)
        {
            flow = await _flowRepository.GetByIdWithStepsAsync(request.FlowId, cancellationToken);
        }
        else
        {
            flow = await _flowRepository.GetByIdAsync(request.FlowId, cancellationToken);
        }

        return flow != null ? _mapper.Map<FlowDto>(flow) : null;
    }
}