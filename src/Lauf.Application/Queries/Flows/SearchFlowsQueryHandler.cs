using MediatR;
using Microsoft.Extensions.Logging;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Application.DTOs.Flows;

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
            _logger.LogInformation("Поиск потоков по запросу '{SearchTerm}'", request.SearchTerm);

            // Получаем все потоки
            var flows = await _flowRepository.GetAllAsync(cancellationToken);
            
            // Применяем поиск
            var filteredFlows = flows.AsEnumerable();
            
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLowerInvariant();
                filteredFlows = filteredFlows.Where(f => 
                    f.Title.ToLowerInvariant().Contains(searchTerm) ||
                    f.Description.ToLowerInvariant().Contains(searchTerm) ||
                    (f.Category != null && f.Category.ToLowerInvariant().Contains(searchTerm)) ||
                    (f.Tags != null && ParseTagsFromJson(f.Tags).Any(tag => tag.ToLowerInvariant().Contains(searchTerm))));
            }

            var totalCount = filteredFlows.Count();
            var pagedFlows = filteredFlows
                .Skip(request.Skip)
                .Take(request.Take)
                .ToList();

            // Конвертируем в DTO
            var flowDtos = pagedFlows.Select(flow => new FlowDto
            {
                Id = flow.Id,
                Title = flow.Title,
                Description = flow.Description,
                Status = flow.Status,
                Category = flow.Category,
                Tags = ParseTagsFromJson(flow.Tags),
                Priority = flow.Priority,
                IsRequired = flow.IsRequired,
                CreatedAt = flow.CreatedAt,
                UpdatedAt = flow.UpdatedAt,
                CreatedById = flow.CreatedById
            }).ToList();

            _logger.LogInformation("По запросу '{SearchTerm}' найдено {TotalCount} потоков, возвращено {Count}", 
                request.SearchTerm, totalCount, flowDtos.Count);

            return new SearchFlowsQueryResult
            {
                Flows = flowDtos,
                TotalCount = totalCount,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при поиске потоков по запросу '{SearchTerm}'", request.SearchTerm);
            
            return new SearchFlowsQueryResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    private static List<string> ParseTagsFromJson(string? tagsJson)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tagsJson))
                return new List<string>();
                
            return System.Text.Json.JsonSerializer.Deserialize<List<string>>(tagsJson) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }
}