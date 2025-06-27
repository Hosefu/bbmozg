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
/// Обработчик команды создания компонента статьи
/// </summary>
public class CreateArticleComponentCommandHandler : IRequestHandler<CreateArticleComponentCommand, CreateArticleComponentResult>
{
    private readonly IComponentRepository _componentRepository;
    private readonly IFlowRepository _flowRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateArticleComponentCommandHandler> _logger;

    public CreateArticleComponentCommandHandler(
        IComponentRepository componentRepository,
        IFlowRepository flowRepository,
        IMapper mapper,
        ILogger<CreateArticleComponentCommandHandler> logger)
    {
        _componentRepository = componentRepository;
        _flowRepository = flowRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CreateArticleComponentResult> Handle(CreateArticleComponentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Создание компонента статьи для шага {StepId}", request.FlowStepId);

            // Валидация входных данных
            if (string.IsNullOrWhiteSpace(request.Title))
                return CreateArticleComponentResult.Failure("Название статьи обязательно");

            if (string.IsNullOrWhiteSpace(request.Content))
                return CreateArticleComponentResult.Failure("Содержимое статьи обязательно");

            if (request.ReadingTimeMinutes <= 0)
                return CreateArticleComponentResult.Failure("Время чтения должно быть больше 0");

            // Проверяем существование шага
            var flowStep = await _flowRepository.GetStepByIdAsync(request.FlowStepId, cancellationToken);
            if (flowStep == null)
                return CreateArticleComponentResult.Failure("Шаг потока не найден");

            // Генерируем порядок для нового компонента
            var order = GenerateNextOrder(flowStep.Components);

            // Создание компонента статьи с привязкой к шагу
            var articleComponent = new ArticleComponent(
                flowStepId: request.FlowStepId,
                title: request.Title,
                description: request.Description,
                content: request.Content,
                order: order,
                isRequired: request.IsRequired,
                readingTimeMinutes: request.ReadingTimeMinutes);

            // Сохраняем компонент в базе
            var savedComponent = await _componentRepository.AddArticleComponentAsync(articleComponent, cancellationToken);

            // Добавляем компонент к шагу
            flowStep.AddComponent(savedComponent);
            
            // Получаем поток для обновления
            var flow = await _flowRepository.GetFlowByStepIdAsync(request.FlowStepId, cancellationToken);
            if (flow != null)
            {
                await _flowRepository.UpdateAsync(flow, cancellationToken);
            }

            _logger.LogInformation("Компонент статьи {ComponentId} успешно создан для шага {StepId}", 
                savedComponent.Id, request.FlowStepId);

            // Преобразовать в DTO
            var componentDto = _mapper.Map<Application.DTOs.Components.ArticleComponentDto>(savedComponent);

            return CreateArticleComponentResult.Success(savedComponent.Id, componentDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании компонента статьи для шага {StepId}", request.FlowStepId);
            return CreateArticleComponentResult.Failure($"Ошибка при создании компонента статьи: {ex.Message}");
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