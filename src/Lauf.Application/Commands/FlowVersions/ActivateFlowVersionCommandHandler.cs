using Lauf.Domain.Interfaces.Repositories;
using Lauf.Domain.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lauf.Application.Commands.FlowVersions;

/// <summary>
/// Обработчик команды активации версии потока
/// </summary>
public class ActivateFlowVersionCommandHandler : IRequestHandler<ActivateFlowVersionCommand, ActivateFlowVersionResponse>
{
    private readonly IFlowVersionRepository _flowVersionRepository;
    private readonly IFlowAssignmentRepository _flowAssignmentRepository;
    private readonly IVersioningService _versioningService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ActivateFlowVersionCommandHandler> _logger;

    public ActivateFlowVersionCommandHandler(
        IFlowVersionRepository flowVersionRepository,
        IFlowAssignmentRepository flowAssignmentRepository,
        IVersioningService versioningService,
        IUnitOfWork unitOfWork,
        ILogger<ActivateFlowVersionCommandHandler> logger)
    {
        _flowVersionRepository = flowVersionRepository ?? throw new ArgumentNullException(nameof(flowVersionRepository));
        _flowAssignmentRepository = flowAssignmentRepository ?? throw new ArgumentNullException(nameof(flowAssignmentRepository));
        _versioningService = versioningService ?? throw new ArgumentNullException(nameof(versioningService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ActivateFlowVersionResponse> Handle(ActivateFlowVersionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Активация версии потока {FlowVersionId}", request.FlowVersionId);

        try
        {
            // Получаем версию потока для активации
            var flowVersion = await _flowVersionRepository.GetByIdAsync(request.FlowVersionId, cancellationToken);
            if (flowVersion == null)
            {
                throw new InvalidOperationException($"Версия потока с ID {request.FlowVersionId} не найдена");
            }

            // Проверяем, что версия не уже активна
            if (flowVersion.IsActive)
            {
                _logger.LogWarning("Версия потока {FlowVersionId} уже активна", request.FlowVersionId);
                throw new InvalidOperationException("Версия потока уже активна");
            }

            // Получаем текущую активную версию
            var currentActiveVersion = await _versioningService.GetActiveFlowVersionAsync(flowVersion.OriginalId, cancellationToken);

            // Подсчитываем количество связанных назначений, которые будут затронуты
            var affectedAssignmentsCount = 0;
            if (currentActiveVersion != null)
            {
                var assignments = await _flowAssignmentRepository.GetByFlowIdAsync(currentActiveVersion.OriginalId, cancellationToken);
                affectedAssignmentsCount = assignments.Where(a => a.FlowVersionId == currentActiveVersion.Id).Count();
            }

            // Проверяем предупреждения
            var warnings = new List<string>();
            if (!request.ForceActivation && affectedAssignmentsCount > 0)
            {
                warnings.Add($"Активация повлияет на {affectedAssignmentsCount} активных назначений");
            }

            // Если есть предупреждения и не установлен флаг принудительной активации
            if (warnings.Any() && !request.ForceActivation)
            {
                return new ActivateFlowVersionResponse
                {
                    FlowVersionId = request.FlowVersionId,
                    Version = flowVersion.Version,
                    OriginalFlowId = flowVersion.OriginalId,
                    PreviousActiveVersionId = currentActiveVersion?.Id,
                    ActivatedAt = DateTime.UtcNow,
                    AffectedAssignmentsCount = affectedAssignmentsCount,
                    Message = "Активация требует подтверждения",
                    Warnings = warnings.ToArray()
                };
            }

            // Выполняем активацию через сервис версионирования
            await _versioningService.ActivateFlowVersionAsync(request.FlowVersionId, cancellationToken);

            _logger.LogInformation("Версия потока {FlowVersionId} (версия {Version}) активирована", 
                request.FlowVersionId, flowVersion.Version);

            return new ActivateFlowVersionResponse
            {
                FlowVersionId = request.FlowVersionId,
                Version = flowVersion.Version,
                OriginalFlowId = flowVersion.OriginalId,
                PreviousActiveVersionId = currentActiveVersion?.Id,
                ActivatedAt = DateTime.UtcNow,
                AffectedAssignmentsCount = affectedAssignmentsCount,
                Message = $"Версия {flowVersion.Version} потока активирована",
                Warnings = warnings.ToArray()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при активации версии потока {FlowVersionId}", request.FlowVersionId);
            throw;
        }
    }
}