using Lauf.Domain.Entities.Versions;
using Lauf.Domain.Enums;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Domain.Services;
using Lauf.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lauf.Application.Commands.FlowVersions;

/// <summary>
/// Обработчик команды создания новой версии потока
/// </summary>
public class CreateFlowVersionCommandHandler : IRequestHandler<CreateFlowVersionCommand, CreateFlowVersionResponse>
{
    private readonly IFlowVersionRepository _flowVersionRepository;
    private readonly IVersioningService _versioningService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateFlowVersionCommandHandler> _logger;

    public CreateFlowVersionCommandHandler(
        IFlowVersionRepository flowVersionRepository,
        IVersioningService versioningService,
        IUnitOfWork unitOfWork,
        ILogger<CreateFlowVersionCommandHandler> logger)
    {
        _flowVersionRepository = flowVersionRepository ?? throw new ArgumentNullException(nameof(flowVersionRepository));
        _versioningService = versioningService ?? throw new ArgumentNullException(nameof(versioningService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<CreateFlowVersionResponse> Handle(CreateFlowVersionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Создание новой версии потока для OriginalFlowId: {OriginalFlowId}", request.OriginalFlowId);

        try
        {
            // Получаем следующий номер версии
            var maxVersion = await _flowVersionRepository.GetMaxVersionAsync(request.OriginalFlowId, cancellationToken);
            var newVersion = maxVersion + 1;

            // Создаем новую версию потока
            var flowVersion = new FlowVersion(
                request.OriginalFlowId,
                newVersion,
                request.Title,
                request.Description,
                request.Tags,
                FlowStatus.Draft, // Новая версия всегда начинается как черновик
                request.Priority,
                request.IsRequired,
                request.CreatedById,
                request.ActivateImmediately // Активируем сразу, если требуется
            );

            // Добавляем новую версию
            await _flowVersionRepository.AddAsync(flowVersion, cancellationToken);

            // Если нужно активировать сразу, деактивируем все остальные версии
            if (request.ActivateImmediately)
            {
                await _flowVersionRepository.DeactivateAllVersionsAsync(request.OriginalFlowId, cancellationToken);
                flowVersion.Activate();
            }

            // Сохраняем изменения
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Создана новая версия потока {Version} (ID: {FlowVersionId})", 
                newVersion, flowVersion.Id);

            return new CreateFlowVersionResponse
            {
                FlowVersionId = flowVersion.Id,
                Version = flowVersion.Version,
                IsActive = flowVersion.IsActive,
                CreatedAt = flowVersion.CreatedAt,
                Message = $"Создана версия {newVersion} потока"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании новой версии потока для OriginalFlowId: {OriginalFlowId}", request.OriginalFlowId);
            throw;
        }
    }
}