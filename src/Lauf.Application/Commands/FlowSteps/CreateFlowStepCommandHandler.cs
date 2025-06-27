using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Application.DTOs.Flows;
using Lauf.Shared.Helpers;

namespace Lauf.Application.Commands.FlowSteps;

/// <summary>
/// Обработчик команды создания шага потока
/// </summary>
public class CreateFlowStepCommandHandler : IRequestHandler<CreateFlowStepCommand, CreateFlowStepCommandResult>
{
    private readonly IFlowRepository _flowRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateFlowStepCommandHandler> _logger;

    public CreateFlowStepCommandHandler(
        IFlowRepository flowRepository,
        IMapper mapper,
        ILogger<CreateFlowStepCommandHandler> logger)
    {
        _flowRepository = flowRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CreateFlowStepCommandResult> Handle(CreateFlowStepCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Создание шага для потока {FlowId}", request.FlowId);

            // Проверяем существование потока с загрузкой шагов
            var flow = await _flowRepository.GetByIdWithStepsAsync(request.FlowId, cancellationToken);
            if (flow == null)
            {
                _logger.LogWarning("Поток с ID {FlowId} не найден", request.FlowId);
                return CreateFlowStepCommandResult.Failure("Поток не найден");
            }

            // Проверяем, что поток в статусе черновика
            if (flow.Status != Domain.Enums.FlowStatus.Draft)
            {
                _logger.LogWarning("Попытка добавления шага к опубликованному потоку {FlowId}", request.FlowId);
                return CreateFlowStepCommandResult.Failure("Нельзя добавлять шаги к опубликованному потоку");
            }

            // Проверяем корректность порядкового номера если указан
            if (request.Order.HasValue)
            {
                if (request.Order.Value < 1 || request.Order.Value > flow.Steps.Count + 1)
                {
                    return CreateFlowStepCommandResult.Failure("Некорректный порядковый номер шага");
                }
            }

            // Создаем новый шаг
            var order = request.Order?.ToString() ?? GenerateNextStepOrder(flow.Steps);
            
            var flowStep = new FlowStep(
                flowId: request.FlowId,
                title: request.Title,
                description: request.Description,
                order: order,
                isRequired: request.IsRequired)
            {
                Instructions = request.Instructions,
                Notes = request.Notes
            };

            // Используем специальный метод репозитория для добавления шага
            var addedStep = await _flowRepository.AddStepAsync(request.FlowId, flowStep, cancellationToken);

            _logger.LogInformation("Шаг {StepId} успешно создан для потока {FlowId}", addedStep.Id, request.FlowId);

            // Преобразуем в DTO
            var stepDto = _mapper.Map<FlowStepDto>(addedStep);

            return CreateFlowStepCommandResult.Success(addedStep.Id, stepDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании шага для потока {FlowId}", request.FlowId);
            return CreateFlowStepCommandResult.Failure("Произошла ошибка при создании шага");
        }
    }

    /// <summary>
    /// Генерирует следующий LexoRank для шага
    /// </summary>
    private static string GenerateNextStepOrder(ICollection<FlowStep> existingSteps)
    {
        if (!existingSteps.Any())
            return LexoRankHelper.Middle();
            
        var lastStep = existingSteps.OrderBy(s => s.Order).LastOrDefault();
        return lastStep != null ? 
            LexoRankHelper.Next(lastStep.Order) : 
            LexoRankHelper.Middle();
    }
}