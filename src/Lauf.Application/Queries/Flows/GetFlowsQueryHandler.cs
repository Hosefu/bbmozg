using MediatR;
using Microsoft.Extensions.Logging;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Domain.Services;
using Lauf.Application.DTOs.Flows;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        _flowRepository = flowRepository ?? throw new ArgumentNullException(nameof(flowRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<GetFlowsQueryResult> Handle(GetFlowsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Получение списка потоков: Skip={Skip}, Take={Take}", 
                request.Skip, request.Take);

            // Получаем потоки (новая архитектура - напрямую из Flow)
            var flows = request.IncludeSteps 
                ? await _flowRepository.GetAllWithStepsAsync(cancellationToken)
                : await _flowRepository.GetAllAsync(cancellationToken);
            
            // Применяем фильтры к потокам
            var filteredFlows = flows.AsEnumerable();
            
            // Фильтруем только активные потоки (в новой архитектуре статус хранится в Flow.IsActive)
            filteredFlows = filteredFlows.Where(f => f.IsActive);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLowerInvariant();
                filteredFlows = filteredFlows.Where(f => 
                    f.Name.ToLowerInvariant().Contains(searchTerm) ||
                    f.Description.ToLowerInvariant().Contains(searchTerm));
            }

            var totalCount = filteredFlows.Count();
            var pagedFlows = filteredFlows
                .Skip(request.Skip)
                .Take(request.Take)
                .ToList();

            // Конвертируем Flow в FlowDto (новая архитектура)
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