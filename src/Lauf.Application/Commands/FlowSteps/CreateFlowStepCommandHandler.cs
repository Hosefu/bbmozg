using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Application.DTOs.Flows;

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

            // Проверяем существование потока
            var flow = await _flowRepository.GetByIdAsync(request.FlowId, cancellationToken);
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

            // Определяем порядковый номер
            var order = request.Order ?? (flow.Steps.Count + 1);

            // Если указан конкретный порядок, проверяем его корректность
            if (request.Order.HasValue)
            {
                if (request.Order.Value < 1 || request.Order.Value > flow.Steps.Count + 1)
                {
                    return CreateFlowStepCommandResult.Failure("Некорректный порядковый номер шага");
                }

                // Если вставляем шаг не в конец, нужно сдвинуть существующие шаги
                var stepsToUpdate = flow.Steps.Where(s => s.Order >= order).ToList();
                foreach (var step in stepsToUpdate)
                {
                    step.Order++;
                }
            }

            // Создаем новый шаг
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

            // Добавляем шаг к потоку
            flow.AddStep(flowStep);

            // Сохраняем изменения
            await _flowRepository.UpdateAsync(flow, cancellationToken);

            _logger.LogInformation("Шаг {StepId} успешно создан для потока {FlowId}", flowStep.Id, request.FlowId);

            // Преобразуем в DTO
            var stepDto = _mapper.Map<FlowStepDto>(flowStep);

            return CreateFlowStepCommandResult.Success(flowStep.Id, stepDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании шага для потока {FlowId}", request.FlowId);
            return CreateFlowStepCommandResult.Failure("Произошла ошибка при создании шага");
        }
    }
}