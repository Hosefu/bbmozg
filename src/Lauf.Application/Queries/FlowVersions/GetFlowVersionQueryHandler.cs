using Lauf.Application.DTOs.Flows;
using Lauf.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lauf.Application.Queries.FlowVersions;

/// <summary>
/// Обработчик запроса для получения версии потока по ID
/// </summary>
public class GetFlowVersionQueryHandler : IRequestHandler<GetFlowVersionQuery, FlowVersionDto?>
{
    private readonly IFlowVersionRepository _flowVersionRepository;
    private readonly ILogger<GetFlowVersionQueryHandler> _logger;

    public GetFlowVersionQueryHandler(
        IFlowVersionRepository flowVersionRepository,
        ILogger<GetFlowVersionQueryHandler> logger)
    {
        _flowVersionRepository = flowVersionRepository ?? throw new ArgumentNullException(nameof(flowVersionRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<FlowVersionDto?> Handle(GetFlowVersionQuery request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Получение версии потока по ID: {FlowVersionId}", request.FlowVersionId);

        try
        {
            var flowVersion = request.IncludeDetails
                ? await _flowVersionRepository.GetByIdWithDetailsAsync(request.FlowVersionId, cancellationToken)
                : await _flowVersionRepository.GetByIdAsync(request.FlowVersionId, cancellationToken);

            if (flowVersion == null)
            {
                _logger.LogWarning("Версия потока с ID {FlowVersionId} не найдена", request.FlowVersionId);
                return null;
            }

            // TODO: Добавить AutoMapper маппинг в FlowVersionDto
            return MapToDto(flowVersion, request.IncludeDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении версии потока {FlowVersionId}", request.FlowVersionId);
            throw;
        }
    }

    private FlowVersionDto MapToDto(Domain.Entities.Versions.FlowVersion flowVersion, bool includeDetails)
    {
        // Временная реализация маппинга
        // TODO: Заменить на AutoMapper после создания профилей
        return new FlowVersionDto
        {
            Id = flowVersion.Id,
            OriginalId = flowVersion.OriginalId,
            Version = flowVersion.Version,
            Title = flowVersion.Title,
            Description = flowVersion.Description,
            Tags = flowVersion.Tags,
            Status = flowVersion.Status,
            Priority = flowVersion.Priority,
            IsRequired = flowVersion.IsRequired,
            IsActive = flowVersion.IsActive,
            CreatedById = flowVersion.CreatedById,
            CreatedAt = flowVersion.CreatedAt,
            UpdatedAt = flowVersion.UpdatedAt
        };
    }
}

/// <summary>
/// Обработчик запроса для получения активной версии потока
/// </summary>
public class GetActiveFlowVersionQueryHandler : IRequestHandler<GetActiveFlowVersionQuery, FlowVersionDto?>
{
    private readonly IFlowVersionRepository _flowVersionRepository;
    private readonly ILogger<GetActiveFlowVersionQueryHandler> _logger;

    public GetActiveFlowVersionQueryHandler(
        IFlowVersionRepository flowVersionRepository,
        ILogger<GetActiveFlowVersionQueryHandler> logger)
    {
        _flowVersionRepository = flowVersionRepository ?? throw new ArgumentNullException(nameof(flowVersionRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<FlowVersionDto?> Handle(GetActiveFlowVersionQuery request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Получение активной версии потока для OriginalFlowId: {OriginalFlowId}", request.OriginalFlowId);

        try
        {
            var activeVersion = await _flowVersionRepository.GetActiveVersionAsync(request.OriginalFlowId, cancellationToken);

            if (activeVersion == null)
            {
                _logger.LogWarning("Активная версия потока для OriginalFlowId {OriginalFlowId} не найдена", request.OriginalFlowId);
                return null;
            }

            if (request.IncludeDetails)
            {
                activeVersion = await _flowVersionRepository.GetByIdWithDetailsAsync(activeVersion.Id, cancellationToken);
            }

            return MapToDto(activeVersion!, request.IncludeDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении активной версии потока {OriginalFlowId}", request.OriginalFlowId);
            throw;
        }
    }

    private FlowVersionDto MapToDto(Domain.Entities.Versions.FlowVersion flowVersion, bool includeDetails)
    {
        return new FlowVersionDto
        {
            Id = flowVersion.Id,
            OriginalId = flowVersion.OriginalId,
            Version = flowVersion.Version,
            Title = flowVersion.Title,
            Description = flowVersion.Description,
            Tags = flowVersion.Tags,
            Status = flowVersion.Status,
            Priority = flowVersion.Priority,
            IsRequired = flowVersion.IsRequired,
            IsActive = flowVersion.IsActive,
            CreatedById = flowVersion.CreatedById,
            CreatedAt = flowVersion.CreatedAt,
            UpdatedAt = flowVersion.UpdatedAt
        };
    }
}

/// <summary>
/// Обработчик запроса для получения конкретной версии потока
/// </summary>
public class GetSpecificFlowVersionQueryHandler : IRequestHandler<GetSpecificFlowVersionQuery, FlowVersionDto?>
{
    private readonly IFlowVersionRepository _flowVersionRepository;
    private readonly ILogger<GetSpecificFlowVersionQueryHandler> _logger;

    public GetSpecificFlowVersionQueryHandler(
        IFlowVersionRepository flowVersionRepository,
        ILogger<GetSpecificFlowVersionQueryHandler> logger)
    {
        _flowVersionRepository = flowVersionRepository ?? throw new ArgumentNullException(nameof(flowVersionRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<FlowVersionDto?> Handle(GetSpecificFlowVersionQuery request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Получение версии {Version} потока для OriginalFlowId: {OriginalFlowId}", 
            request.Version, request.OriginalFlowId);

        try
        {
            var flowVersion = await _flowVersionRepository.GetVersionAsync(
                request.OriginalFlowId, 
                request.Version, 
                cancellationToken);

            if (flowVersion == null)
            {
                _logger.LogWarning("Версия {Version} потока для OriginalFlowId {OriginalFlowId} не найдена", 
                    request.Version, request.OriginalFlowId);
                return null;
            }

            if (request.IncludeDetails)
            {
                flowVersion = await _flowVersionRepository.GetByIdWithDetailsAsync(flowVersion.Id, cancellationToken);
            }

            return MapToDto(flowVersion!, request.IncludeDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении версии {Version} потока {OriginalFlowId}", 
                request.Version, request.OriginalFlowId);
            throw;
        }
    }

    private FlowVersionDto MapToDto(Domain.Entities.Versions.FlowVersion flowVersion, bool includeDetails)
    {
        return new FlowVersionDto
        {
            Id = flowVersion.Id,
            OriginalId = flowVersion.OriginalId,
            Version = flowVersion.Version,
            Title = flowVersion.Title,
            Description = flowVersion.Description,
            Tags = flowVersion.Tags,
            Status = flowVersion.Status,
            Priority = flowVersion.Priority,
            IsRequired = flowVersion.IsRequired,
            IsActive = flowVersion.IsActive,
            CreatedById = flowVersion.CreatedById,
            CreatedAt = flowVersion.CreatedAt,
            UpdatedAt = flowVersion.UpdatedAt
        };
    }
}

/// <summary>
/// Обработчик запроса для получения всех версий потока
/// </summary>
public class GetAllFlowVersionsQueryHandler : IRequestHandler<GetAllFlowVersionsQuery, IList<FlowVersionDto>>
{
    private readonly IFlowVersionRepository _flowVersionRepository;
    private readonly ILogger<GetAllFlowVersionsQueryHandler> _logger;

    public GetAllFlowVersionsQueryHandler(
        IFlowVersionRepository flowVersionRepository,
        ILogger<GetAllFlowVersionsQueryHandler> logger)
    {
        _flowVersionRepository = flowVersionRepository ?? throw new ArgumentNullException(nameof(flowVersionRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IList<FlowVersionDto>> Handle(GetAllFlowVersionsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Получение всех версий потока для OriginalFlowId: {OriginalFlowId}", request.OriginalFlowId);

        try
        {
            var flowVersions = await _flowVersionRepository.GetAllVersionsAsync(request.OriginalFlowId, cancellationToken);

            var result = new List<FlowVersionDto>();
            var count = 0;

            foreach (var flowVersion in flowVersions)
            {
                if (request.Limit.HasValue && count >= request.Limit.Value)
                    break;

                var dto = MapToDto(flowVersion, request.IncludeDetails);
                result.Add(dto);
                count++;
            }

            _logger.LogDebug("Получено {Count} версий потока для OriginalFlowId: {OriginalFlowId}", 
                result.Count, request.OriginalFlowId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении всех версий потока {OriginalFlowId}", request.OriginalFlowId);
            throw;
        }
    }

    private FlowVersionDto MapToDto(Domain.Entities.Versions.FlowVersion flowVersion, bool includeDetails)
    {
        return new FlowVersionDto
        {
            Id = flowVersion.Id,
            OriginalId = flowVersion.OriginalId,
            Version = flowVersion.Version,
            Title = flowVersion.Title,
            Description = flowVersion.Description,
            Tags = flowVersion.Tags,
            Status = flowVersion.Status,
            Priority = flowVersion.Priority,
            IsRequired = flowVersion.IsRequired,
            IsActive = flowVersion.IsActive,
            CreatedById = flowVersion.CreatedById,
            CreatedAt = flowVersion.CreatedAt,
            UpdatedAt = flowVersion.UpdatedAt
        };
    }
}