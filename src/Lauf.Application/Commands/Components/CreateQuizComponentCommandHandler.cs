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
/// Обработчик команды создания компонента квиза
/// </summary>
public class CreateQuizComponentCommandHandler : IRequestHandler<CreateQuizComponentCommand, CreateQuizComponentResult>
{
    private readonly IComponentRepository _componentRepository;
    private readonly IFlowRepository _flowRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateQuizComponentCommandHandler> _logger;

    public CreateQuizComponentCommandHandler(
        IComponentRepository componentRepository,
        IFlowRepository flowRepository,
        IMapper mapper,
        ILogger<CreateQuizComponentCommandHandler> logger)
    {
        _componentRepository = componentRepository;
        _flowRepository = flowRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CreateQuizComponentResult> Handle(CreateQuizComponentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Создание компонента квиза для шага {StepId}", request.FlowStepId);

            // Валидация входных данных
            if (string.IsNullOrWhiteSpace(request.Title))
                return CreateQuizComponentResult.Failure("Название квиза обязательно");

            if (string.IsNullOrWhiteSpace(request.QuestionText))
                return CreateQuizComponentResult.Failure("Текст вопроса обязателен");

            if (request.Options == null || request.Options.Count != 5)
                return CreateQuizComponentResult.Failure("Квиз должен содержать ровно 5 вариантов ответа");

            // Проверяем существование шага
            var flowStep = await _flowRepository.GetStepByIdAsync(request.FlowStepId, cancellationToken);
            if (flowStep == null)
                return CreateQuizComponentResult.Failure("Шаг потока не найден");

            // Генерируем порядок для нового компонента
            var order = GenerateNextOrder(flowStep.Components);

            // Создание компонента квиза с привязкой к шагу (новая архитектура)
            var quizComponent = new QuizComponent(
                flowStepId: request.FlowStepId,
                title: request.Title,
                description: request.Description,
                content: request.QuestionText,
                order: order,
                isRequired: request.IsRequired);

            // Создаем вопрос квиза
            var quizQuestion = new QuizQuestion(
                quizComponent.Id,
                request.QuestionText,
                LexoRankHelper.Middle());

            // Создание вариантов ответов с LexoRank
            var options = new List<QuestionOption>();
            for (int i = 0; i < request.Options.Count; i++)
            {
                var optionInput = request.Options[i];
                var option = new QuestionOption(
                    quizQuestionId: quizQuestion.Id,
                    text: optionInput.Text,
                    isCorrect: optionInput.IsCorrect,
                    score: optionInput.Points,
                    order: i == 0 ? LexoRankHelper.Middle() : LexoRankHelper.Next(options[i-1].Order));
                options.Add(option);
            }

            quizQuestion.Options = options;
            quizComponent.Questions.Add(quizQuestion);

            // Сохраняем компонент в базе
            var savedComponent = await _componentRepository.AddQuizComponentAsync(quizComponent, cancellationToken);

            // Добавляем компонент к шагу
            flowStep.AddComponent(savedComponent);
            
            // Получаем поток для обновления
            var flow = await _flowRepository.GetFlowByStepIdAsync(request.FlowStepId, cancellationToken);
            if (flow != null)
            {
                await _flowRepository.UpdateAsync(flow, cancellationToken);
            }

            _logger.LogInformation("Компонент квиза {ComponentId} успешно создан для шага {StepId}", 
                savedComponent.Id, request.FlowStepId);

            // Преобразовать в DTO
            var componentDto = _mapper.Map<Application.DTOs.Components.QuizComponentDto>(savedComponent);

            return CreateQuizComponentResult.Success(savedComponent.Id, componentDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании компонента квиза для шага {StepId}", request.FlowStepId);
            return CreateQuizComponentResult.Failure($"Ошибка при создании компонента квиза: {ex.Message}");
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