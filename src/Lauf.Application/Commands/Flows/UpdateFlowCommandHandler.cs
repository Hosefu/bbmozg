using Lauf.Domain.Interfaces;
using Lauf.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Lauf.Application.DTOs.Flows;

namespace Lauf.Application.Commands.Flows;

/// <summary>
/// Обработчик команды обновления потока обучения
/// </summary>
public class UpdateFlowCommandHandler : IRequestHandler<UpdateFlowCommand, UpdateFlowCommandResult>
{
    private readonly IFlowRepository _flowRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateFlowCommandHandler> _logger;

    public UpdateFlowCommandHandler(
        IFlowRepository flowRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateFlowCommandHandler> logger)
    {
        _flowRepository = flowRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<UpdateFlowCommandResult> Handle(UpdateFlowCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Начинается обновление потока {FlowId}", request.FlowId);

            // Получаем поток
            var flow = await _flowRepository.GetByIdAsync(request.FlowId, cancellationToken);
            if (flow == null)
            {
                return new UpdateFlowCommandResult
                {
                    Success = false,
                    ErrorMessage = $"Поток с ID {request.FlowId} не найден"
                };
            }

            // Обновляем свойства (новая архитектура - только базовые координаторные свойства)
            if (!string.IsNullOrWhiteSpace(request.Title))
            {
                flow.Name = request.Title;
            }

            if (!string.IsNullOrWhiteSpace(request.Description))
            {
                flow.Description = request.Description;
            }

            // Сохраняем изменения
            await _flowRepository.UpdateAsync(flow, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Поток {FlowId} успешно обновлен", request.FlowId);

            return new UpdateFlowCommandResult
            {
                Success = true,
                Flow = new FlowDto
                {
                    Id = flow.Id,
                    Name = flow.Name,
                    Description = flow.Description,
                    CreatedById = flow.CreatedBy,
                    CreatedAt = flow.CreatedAt
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении потока {FlowId}", request.FlowId);
            
            return new UpdateFlowCommandResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}