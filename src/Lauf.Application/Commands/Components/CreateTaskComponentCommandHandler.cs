using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Lauf.Domain.Entities.Components;
using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Domain.Enums;
using Lauf.Shared.Helpers;

namespace Lauf.Application.Commands.Components;

/// <summary>
/// Обработчик команды создания компонента задания
/// </summary>
public class CreateTaskComponentCommandHandler : IRequestHandler<CreateTaskComponentCommand, CreateTaskComponentResult>
{
    private readonly IComponentRepository _componentRepository;
    private readonly IFlowRepository _flowRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateTaskComponentCommandHandler> _logger;

    public CreateTaskComponentCommandHandler(
        IComponentRepository componentRepository,
        IFlowRepository flowRepository,
        IMapper mapper,
        ILogger<CreateTaskComponentCommandHandler> logger)
    {
        _componentRepository = componentRepository;
        _flowRepository = flowRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CreateTaskComponentResult> Handle(CreateTaskComponentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Создание компонента задания для шага {StepId}", request.FlowStepId);

            // Валидация входных данных
            if (string.IsNullOrWhiteSpace(request.Title))
                return CreateTaskComponentResult.Failure("Название задания обязательно");

            if (string.IsNullOrWhiteSpace(request.Instruction))
                return CreateTaskComponentResult.Failure("Инструкция обязательна");

            if (string.IsNullOrWhiteSpace(request.CodeWord))
                return CreateTaskComponentResult.Failure("Кодовое слово обязательно");

            // Проверяем существование шага
            var flowStep = await _flowRepository.GetStepByIdAsync(request.FlowStepId, cancellationToken);
            if (flowStep == null)
                return CreateTaskComponentResult.Failure("Шаг потока не найден");

            // Генерируем порядок для нового компонента
            var order = GenerateNextOrder(flowStep.Components);

            // Создание компонента задания с привязкой к шагу
            var taskComponent = new TaskComponent(
                flowStepId: request.FlowStepId,
                title: request.Title,
                description: request.Description,
                instruction: request.Instruction,
                codeWord: request.CodeWord,
                hint: request.Hint,
                order: order,
                isRequired: request.IsRequired);

            // Сохраняем компонент в базе
            var savedComponent = await _componentRepository.AddTaskComponentAsync(taskComponent, cancellationToken);

            // Добавляем компонент к шагу
            flowStep.AddComponent(savedComponent);
            
            // Получаем поток для обновления
            var flow = await _flowRepository.GetFlowByStepIdAsync(request.FlowStepId, cancellationToken);
            if (flow != null)
            {
                await _flowRepository.UpdateAsync(flow, cancellationToken);
            }

            _logger.LogInformation("Компонент задания {ComponentId} успешно создан для шага {StepId}", 
                savedComponent.Id, request.FlowStepId);

            // Преобразовать в DTO
            var componentDto = _mapper.Map<Application.DTOs.Components.TaskComponentDto>(savedComponent);

            return CreateTaskComponentResult.Success(savedComponent.Id, componentDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании компонента задания для шага {StepId}", request.FlowStepId);
            return CreateTaskComponentResult.Failure($"Ошибка при создании компонента задания: {ex.Message}");
        }
    }

    /// <summary>
    /// Генерирует следующий LexoRank для компонента
    /// </summary>
    private static string GenerateNextOrder(ICollection<ComponentBase> existingComponents)
    {
        if (!existingComponents.Any())
            return LexoRankHelper.Middle();
            
        var lastComponent = existingComponents.OrderBy(c => c.Order).LastOrDefault();
        return lastComponent != null ? 
            LexoRankHelper.Next(lastComponent.Order) : 
            LexoRankHelper.Middle();
    }
} 