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
            var flows = await _flowRepository.GetAllAsync(cancellationToken);
            
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

            if (!string.IsNullOrWhiteSpace(request.Category))
            {
                filteredFlows = filteredFlows.Where(f => 
                    f.Category != null && f.Category.Equals(request.Category, StringComparison.OrdinalIgnoreCase));
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