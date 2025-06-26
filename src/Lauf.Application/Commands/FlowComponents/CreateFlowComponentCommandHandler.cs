using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Application.DTOs.Flows;

namespace Lauf.Application.Commands.FlowComponents;

/// <summary>
/// Обработчик команды создания компонента шага потока
/// </summary>
public class CreateFlowComponentCommandHandler : IRequestHandler<CreateFlowComponentCommand, CreateFlowComponentCommandResult>
{
    private readonly IFlowRepository _flowRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateFlowComponentCommandHandler> _logger;

    public CreateFlowComponentCommandHandler(
        IFlowRepository flowRepository,
        IMapper mapper,
        ILogger<CreateFlowComponentCommandHandler> logger)
    {
        _flowRepository = flowRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CreateFlowComponentCommandResult> Handle(CreateFlowComponentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Создание компонента для шага {StepId}", request.FlowStepId);

            // Находим шаг потока через поток
            var flow = await _flowRepository.GetFlowByStepIdAsync(request.FlowStepId, cancellationToken);
            if (flow == null)
            {
                _logger.LogWarning("Поток с шагом {StepId} не найден", request.FlowStepId);
                return CreateFlowComponentCommandResult.Failure("Шаг потока не найден");
            }

            var flowStep = flow.Steps.FirstOrDefault(s => s.Id == request.FlowStepId);
            if (flowStep == null)
            {
                _logger.LogWarning("Шаг {StepId} не найден в потоке {FlowId}", request.FlowStepId, flow.Id);
                return CreateFlowComponentCommandResult.Failure("Шаг потока не найден");
            }

            // Проверяем, что поток в статусе черновика
            if (flow.Status != Domain.Enums.FlowStatus.Draft)
            {
                _logger.LogWarning("Попытка добавления компонента к шагу опубликованного потока {FlowId}", flow.Id);
                return CreateFlowComponentCommandResult.Failure("Нельзя добавлять компоненты к шагам опубликованного потока");
            }

            // Определяем порядковый номер
            var order = request.Order ?? (flowStep.Components.Count + 1);

            // Если указан конкретный порядок, проверяем его корректность
            if (request.Order.HasValue)
            {
                if (request.Order.Value < 1 || request.Order.Value > flowStep.Components.Count + 1)
                {
                    return CreateFlowComponentCommandResult.Failure("Некорректный порядковый номер компонента");
                }

                // Если вставляем компонент не в конец, нужно сдвинуть существующие компоненты
                var componentsToUpdate = flowStep.Components.Where(c => c.Order >= order).ToList();
                foreach (var component in componentsToUpdate)
                {
                    component.Order++;
                }
            }

            // Создаем новый компонент
            var flowComponent = new FlowStepComponent(
                flowStepId: request.FlowStepId,
                componentType: request.Type,
                title: request.Title,
                description: request.Description,
                order: order,
                isRequired: request.IsRequired)
            {
                Settings = request.Content
            };

            // Добавляем компонент к шагу (но НЕ используем flowStep.AddComponent чтобы избежать дублирования порядка)
            flowComponent.FlowStepId = flowStep.Id;
            flowStep.Components.Add(flowComponent);

            // Сохраняем изменения
            await _flowRepository.UpdateAsync(flow, cancellationToken);

            _logger.LogInformation("Компонент {ComponentId} успешно создан для шага {StepId}", flowComponent.Id, request.FlowStepId);

            // Преобразуем в DTO
            var componentDto = _mapper.Map<FlowStepComponentDto>(flowComponent);

            return CreateFlowComponentCommandResult.Success(flowComponent.Id, componentDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании компонента для шага {StepId}", request.FlowStepId);
            return CreateFlowComponentCommandResult.Failure("Произошла ошибка при создании компонента");
        }
    }
}