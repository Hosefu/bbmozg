using Lauf.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lauf.Application.Commands.FlowVersions;

/// <summary>
/// Обработчик команды обновления версии потока
/// </summary>
public class UpdateFlowVersionCommandHandler : IRequestHandler<UpdateFlowVersionCommand, UpdateFlowVersionResponse>
{
    private readonly IFlowVersionRepository _flowVersionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateFlowVersionCommandHandler> _logger;

    public UpdateFlowVersionCommandHandler(
        IFlowVersionRepository flowVersionRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateFlowVersionCommandHandler> logger)
    {
        _flowVersionRepository = flowVersionRepository ?? throw new ArgumentNullException(nameof(flowVersionRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<UpdateFlowVersionResponse> Handle(UpdateFlowVersionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Обновление версии потока {FlowVersionId}", request.FlowVersionId);

        try
        {
            // Получаем версию потока
            var flowVersion = await _flowVersionRepository.GetByIdAsync(request.FlowVersionId, cancellationToken);
            if (flowVersion == null)
            {
                throw new InvalidOperationException($"Версия потока с ID {request.FlowVersionId} не найдена");
            }

            // Проверяем, что версия может быть изменена
            if (flowVersion.IsActive)
            {
                _logger.LogWarning("Попытка изменить активную версию потока {FlowVersionId}", request.FlowVersionId);
                throw new InvalidOperationException("Нельзя изменять активную версию потока. Создайте новую версию.");
            }

            // Обновляем поля используя метод UpdateMetadata
            flowVersion.UpdateMetadata(
                request.Title ?? flowVersion.Title,
                request.Description ?? flowVersion.Description,
                request.Tags ?? flowVersion.Tags,
                request.Priority ?? flowVersion.Priority,
                request.IsRequired ?? flowVersion.IsRequired);

            // Обновляем версию в репозитории
            await _flowVersionRepository.UpdateAsync(flowVersion, cancellationToken);

            // Сохраняем изменения
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Версия потока {FlowVersionId} обновлена", request.FlowVersionId);

            return new UpdateFlowVersionResponse
            {
                FlowVersionId = flowVersion.Id,
                Version = flowVersion.Version,
                UpdatedAt = flowVersion.UpdatedAt,
                Message = $"Версия {flowVersion.Version} потока обновлена"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении версии потока {FlowVersionId}", request.FlowVersionId);
            throw;
        }
    }
}