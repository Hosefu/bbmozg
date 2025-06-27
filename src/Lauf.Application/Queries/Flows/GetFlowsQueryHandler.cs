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
    private readonly IFlowVersionRepository _flowVersionRepository;
    private readonly IVersioningService _versioningService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetFlowsQueryHandler> _logger;

    public GetFlowsQueryHandler(
        IFlowRepository flowRepository,
        IFlowVersionRepository flowVersionRepository,
        IVersioningService versioningService,
        IMapper mapper,
        ILogger<GetFlowsQueryHandler> logger)
    {
        _flowRepository = flowRepository ?? throw new ArgumentNullException(nameof(flowRepository));
        _flowVersionRepository = flowVersionRepository ?? throw new ArgumentNullException(nameof(flowVersionRepository));
        _versioningService = versioningService ?? throw new ArgumentNullException(nameof(versioningService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<GetFlowsQueryResult> Handle(GetFlowsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Получение списка потоков: Skip={Skip}, Take={Take}", 
                request.Skip, request.Take);

            // Получаем все оригинальные потоки
            var originalFlows = await _flowRepository.GetAllAsync(cancellationToken);
            
            // Получаем активные версии для каждого потока
            var activeFlowVersions = new List<Domain.Entities.Versions.FlowVersion>();
            foreach (var originalFlow in originalFlows)
            {
                var activeVersion = await _versioningService.GetActiveFlowVersionAsync(originalFlow.Id, cancellationToken);
                if (activeVersion != null)
                {
                    // Загружаем полную версию с деталями если нужно
                    if (request.IncludeSteps)
                    {
                        activeVersion = await _flowVersionRepository.GetByIdWithDetailsAsync(activeVersion.Id, cancellationToken);
                    }
                    
                    if (activeVersion != null)
                    {
                        activeFlowVersions.Add(activeVersion);
                    }
                }
            }
            
            // Применяем фильтры к активным версиям
            var filteredFlowVersions = activeFlowVersions.AsEnumerable();
            
            if (request.Status.HasValue)
            {
                filteredFlowVersions = filteredFlowVersions.Where(fv => fv.Status == request.Status.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLowerInvariant();
                filteredFlowVersions = filteredFlowVersions.Where(fv => 
                    fv.Title.ToLowerInvariant().Contains(searchTerm) ||
                    fv.Description.ToLowerInvariant().Contains(searchTerm));
            }

            var totalCount = filteredFlowVersions.Count();
            var pagedFlowVersions = filteredFlowVersions
                .Skip(request.Skip)
                .Take(request.Take)
                .ToList();

            // Конвертируем FlowVersion в FlowDto для прозрачной работы API
            var flowDtos = _mapper.Map<List<FlowDto>>(pagedFlowVersions);

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