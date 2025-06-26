using MediatR;
using Microsoft.Extensions.Logging;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Application.DTOs.Flows;
using AutoMapper;

namespace Lauf.Application.Queries.Flows;

/// <summary>
/// Обработчик запроса получения списка потоков
/// </summary>
public class GetFlowsQueryHandler : IRequestHandler<GetFlowsQuery, GetFlowsQueryResult>
{
    private readonly IFlowRepository _flowRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetFlowsQueryHandler> _logger;

    public GetFlowsQueryHandler(
        IFlowRepository flowRepository,
        IMapper mapper,
        ILogger<GetFlowsQueryHandler> logger)
    {
        _flowRepository = flowRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetFlowsQueryResult> Handle(GetFlowsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Получение списка потоков: Skip={Skip}, Take={Take}", 
                request.Skip, request.Take);

            // Получаем потоки из репозитория
            var flows = request.IncludeSteps 
                ? await _flowRepository.GetAllWithStepsAsync(cancellationToken)
                : await _flowRepository.GetAllAsync(cancellationToken);
            
            // Применяем фильтры
            var filteredFlows = flows.AsEnumerable();
            
            if (request.Status.HasValue)
            {
                filteredFlows = filteredFlows.Where(f => f.Status == request.Status.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLowerInvariant();
                filteredFlows = filteredFlows.Where(f => 
                    f.Title.ToLowerInvariant().Contains(searchTerm) ||
                    f.Description.ToLowerInvariant().Contains(searchTerm));
            }

            // Фильтрация по категории удалена, так как поле Category больше не существует

            var totalCount = filteredFlows.Count();
            var pagedFlows = filteredFlows
                .Skip(request.Skip)
                .Take(request.Take)
                .ToList();

            // Конвертируем в DTO с помощью AutoMapper
            var flowDtos = _mapper.Map<List<FlowDto>>(pagedFlows);

            _logger.LogInformation("Найдено {TotalCount} потоков, возвращено {Count}", 
                totalCount, flowDtos.Count);

            return new GetFlowsQueryResult
            {
                Flows = flowDtos,
                TotalCount = totalCount,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении списка потоков");
            
            return new GetFlowsQueryResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }


}