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

            // Обновляем свойства
            if (!string.IsNullOrWhiteSpace(request.Title))
            {
                // Прямое обновление свойства
                var titleProperty = typeof(Domain.Entities.Flows.Flow).GetProperty("Title");
                titleProperty?.SetValue(flow, request.Title);
            }

            if (!string.IsNullOrWhiteSpace(request.Description))
            {
                // Прямое обновление свойства
                var descProperty = typeof(Domain.Entities.Flows.Flow).GetProperty("Description");
                descProperty?.SetValue(flow, request.Description);
            }

            if (request.Status.HasValue)
            {
                // Для обновления статуса можно добавить метод в доменную модель
                // flow.UpdateStatus(request.Status.Value);
                // Пока обновляем напрямую
                var statusProperty = typeof(Domain.Entities.Flows.Flow).GetProperty("Status");
                statusProperty?.SetValue(flow, request.Status.Value);
            }


            if (!string.IsNullOrWhiteSpace(request.Tags))
            {
                // Преобразуем в JSON строку для хранения в базе
                var tagsList = request.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim()).ToList();
                flow.Tags = System.Text.Json.JsonSerializer.Serialize(tagsList);
            }

            if (request.Priority.HasValue)
            {
                flow.Priority = request.Priority.Value;
            }

            if (request.IsRequired.HasValue)
            {
                flow.IsRequired = request.IsRequired.Value;
            }

            // Сохраняем изменения
            await _flowRepository.UpdateAsync(flow, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Поток {FlowId} успешно обновлен", request.FlowId);

            // Создаем DTO для ответа
            var flowDto = new FlowDto
            {
                Id = flow.Id,
                Title = flow.Title,
                Description = flow.Description,
                Status = flow.Status,
                Tags = ParseTags(flow.Tags),
                Priority = flow.Priority,
                IsRequired = flow.IsRequired,
                CreatedAt = flow.CreatedAt,
                UpdatedAt = flow.UpdatedAt,
                CreatedById = flow.CreatedById,
                TotalSteps = flow.TotalSteps
            };

            return new UpdateFlowCommandResult
            {
                Flow = flowDto,
                Success = true
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

    private static List<string> ParseTags(string? tagsJson)
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