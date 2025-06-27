using MediatR;
using Microsoft.Extensions.Logging;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Application.DTOs.Flows;
using System.Text.Json;

namespace Lauf.Application.Queries.Flows;

/// <summary>
/// Обработчик запроса поиска потоков
/// </summary>
public class SearchFlowsQueryHandler : IRequestHandler<SearchFlowsQuery, SearchFlowsQueryResult>
{
    private readonly IFlowRepository _flowRepository;
    private readonly ILogger<SearchFlowsQueryHandler> _logger;

    public SearchFlowsQueryHandler(
        IFlowRepository flowRepository,
        ILogger<SearchFlowsQueryHandler> logger)
    {
        _flowRepository = flowRepository;
        _logger = logger;
    }

    public async Task<SearchFlowsQueryResult> Handle(SearchFlowsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Поиск потоков: {SearchTerm}", request.SearchTerm);

            // Получаем все потоки
            var allFlows = await _flowRepository.GetAllAsync(cancellationToken);
            var filteredFlows = allFlows.AsEnumerable();

            // Фильтрация по активности (новая архитектура)
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

            // Конвертируем в DTO (новая архитектура)
            var flowDtos = pagedFlows.Select(flow => new FlowDto
            {
                Id = flow.Id,
                Name = flow.Name, // Title теперь Name
                Description = flow.Description,
                CreatedById = flow.CreatedBy,
                CreatedAt = flow.CreatedAt
            }).ToList();

            _logger.LogInformation("Найдено {Count} потоков по запросу '{SearchTerm}'", 
                totalCount, request.SearchTerm);

            return new SearchFlowsQueryResult
            {
                Flows = flowDtos,
                TotalCount = totalCount,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при поиске потоков");
            
            return new SearchFlowsQueryResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}