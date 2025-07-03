using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Entities.Components;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Application.DTOs.Flows;
using Lauf.Shared.Helpers;
using Lauf.Domain.Enums;
using System.Text.Json;

namespace Lauf.Application.Commands.FlowComponents;

/// <summary>
/// Обработчик команды создания компонента шага потока
/// </summary>
public class CreateFlowComponentCommandHandler : IRequestHandler<CreateFlowComponentCommand, CreateFlowComponentCommandResult>
{
    private readonly IFlowRepository _flowRepository;
    private readonly IComponentRepository _componentRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateFlowComponentCommandHandler> _logger;

    public CreateFlowComponentCommandHandler(
        IFlowRepository flowRepository,
        IComponentRepository componentRepository,
        IMapper mapper,
        ILogger<CreateFlowComponentCommandHandler> logger)
    {
        _flowRepository = flowRepository;
        _componentRepository = componentRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CreateFlowComponentCommandResult> Handle(CreateFlowComponentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Создание компонента типа {ComponentType} для шага {StepId}", request.Type, request.FlowStepId);

            // Находим шаг потока через поток
            var flow = await _flowRepository.GetFlowByStepIdAsync(request.FlowStepId, cancellationToken);
            if (flow == null)
            {
                _logger.LogWarning("Поток с шагом {StepId} не найден", request.FlowStepId);
                return CreateFlowComponentCommandResult.Failure("Шаг потока не найден");
            }

            var flowStep = flow.ActiveContent.Steps.FirstOrDefault(s => s.Id == request.FlowStepId);
            if (flowStep == null)
            {
                _logger.LogWarning("Шаг {StepId} не найден в потоке {FlowId}", request.FlowStepId, flow.Id);
                return CreateFlowComponentCommandResult.Failure("Шаг потока не найден");
            }

            // Проверяем, что поток активен (новая архитектура)
            if (!flow.IsActive)
            {
                _logger.LogWarning("Попытка добавления компонента к шагу неактивного потока {FlowId}", flow.Id);
                return CreateFlowComponentCommandResult.Failure("Нельзя добавлять компоненты к шагам неактивного потока");
            }

            // Определяем LexoRank порядок - всегда добавляем в конец
            var order = GenerateNextComponentOrder(flowStep.Components);

            // Создаем компонент в зависимости от типа
            ComponentBase component = await CreateComponentByTypeAsync(request, order, cancellationToken);

            // Добавляем компонент к шагу
            flowStep.AddComponent(component);
            await _flowRepository.UpdateAsync(flow, cancellationToken);

            _logger.LogInformation("Компонент {ComponentId} типа {ComponentType} успешно создан для шага {StepId}", 
                component.Id, request.Type, request.FlowStepId);

            // Преобразуем в DTO
            var componentDto = _mapper.Map<FlowStepComponentDto>(component);

            return CreateFlowComponentCommandResult.Success(component.Id, componentDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании компонента для шага {StepId}", request.FlowStepId);
            return CreateFlowComponentCommandResult.Failure("Произошла ошибка при создании компонента");
        }
    }

    /// <summary>
    /// Создает компонент в зависимости от типа
    /// </summary>
    private async Task<ComponentBase> CreateComponentByTypeAsync(CreateFlowComponentCommand request, string order, CancellationToken cancellationToken)
    {
        return request.Type switch
        {
            ComponentType.Article => await CreateArticleComponentAsync(request, order, cancellationToken),
            ComponentType.Quiz => await CreateQuizComponentAsync(request, order, cancellationToken),
            ComponentType.Task => await CreateTaskComponentAsync(request, order, cancellationToken),
            _ => throw new ArgumentException($"Неподдерживаемый тип компонента: {request.Type}")
        };
    }

    /// <summary>
    /// Создает компонент статьи
    /// </summary>
    private async Task<ComponentBase> CreateArticleComponentAsync(CreateFlowComponentCommand request, string order, CancellationToken cancellationToken)
    {
        // Парсим содержимое для статьи
        var contentData = ParseContentForArticle(request.Content);
        
        var articleComponent = new ArticleComponent(
            flowStepId: request.FlowStepId,
            title: request.Title,
            description: request.Description,
            content: contentData.Content,
            order: order,
            isRequired: request.IsRequired); // readingTimeMinutes убран из новой архитектуры

        return await _componentRepository.AddArticleComponentAsync(articleComponent, cancellationToken);
    }

    /// <summary>
    /// Создает компонент квиза
    /// </summary>
    private async Task<ComponentBase> CreateQuizComponentAsync(CreateFlowComponentCommand request, string order, CancellationToken cancellationToken)
    {
        // Парсим содержимое для квиза
        var contentData = ParseContentForQuiz(request.Content);
        
        var quizComponent = new QuizComponent(
            flowStepId: request.FlowStepId,
            title: request.Title,
            description: request.Description,
            content: contentData.QuestionText, // questionText теперь content
            order: order,
            isRequired: request.IsRequired);

        // Создаем вопрос квиза
        var quizQuestion = new Domain.Entities.Components.QuizQuestion(
            quizComponent.Id,
            contentData.QuestionText,
            LexoRankHelper.Middle());
        quizComponent.Questions.Add(quizQuestion);

        // Добавляем варианты ответов
        foreach (var option in contentData.Options)
        {
            var questionOption = new QuestionOption(
                quizQuestionId: quizQuestion.Id,
                text: option.Text,
                isCorrect: option.IsCorrect,
                order: LexoRankHelper.Middle());
            
            quizQuestion.Options.Add(questionOption);
        }

        return await _componentRepository.AddQuizComponentAsync(quizComponent, cancellationToken);
    }

    /// <summary>
    /// Создает компонент задания
    /// </summary>
    private async Task<ComponentBase> CreateTaskComponentAsync(CreateFlowComponentCommand request, string order, CancellationToken cancellationToken)
    {
        // Парсим содержимое для задания
        var contentData = ParseContentForTask(request.Content);
        
        var taskComponent = new TaskComponent(
            flowStepId: request.FlowStepId,
            title: request.Title,
            description: request.Description,
            content: contentData.Instruction, // instruction теперь content
            codeWord: contentData.CodeWord,
            order: order,
            isRequired: request.IsRequired);

        return await _componentRepository.AddTaskComponentAsync(taskComponent, cancellationToken);
    }

    /// <summary>
    /// Парсит содержимое для статьи
    /// </summary>
    private static (string Content, int ReadingTimeMinutes) ParseContentForArticle(string contentJson)
    {
        try
        {
            var contentData = JsonSerializer.Deserialize<JsonElement>(contentJson);
            
            var content = contentData.TryGetProperty("content", out var contentProp) ? 
                contentProp.GetString() ?? string.Empty : string.Empty;
            
            var readingTime = contentData.TryGetProperty("readingTimeMinutes", out var timeProp) ? 
                timeProp.GetInt32() : 15;

            return (content, readingTime);
        }
        catch
        {
            return (string.Empty, 15);
        }
    }

    /// <summary>
    /// Парсит содержимое для квиза
    /// </summary>
    private static (string QuestionText, List<(string Text, bool IsCorrect)> Options, int EstimatedDurationMinutes) ParseContentForQuiz(string contentJson)
    {
        try
        {
            var contentData = JsonSerializer.Deserialize<JsonElement>(contentJson);
            
            var questionText = contentData.TryGetProperty("questionText", out var questionProp) ? 
                questionProp.GetString() ?? string.Empty : string.Empty;
            
            var estimatedTime = contentData.TryGetProperty("estimatedDurationMinutes", out var timeProp) ? 
                timeProp.GetInt32() : 5;

            var options = new List<(string Text, bool IsCorrect)>();
            if (contentData.TryGetProperty("options", out var optionsProp) && optionsProp.ValueKind == JsonValueKind.Array)
            {
                foreach (var option in optionsProp.EnumerateArray())
                {
                    var text = option.TryGetProperty("text", out var textProp) ? textProp.GetString() ?? string.Empty : string.Empty;
                    var isCorrect = option.TryGetProperty("isCorrect", out var correctProp) && correctProp.GetBoolean();
                    options.Add((text, isCorrect));
                }
            }

            return (questionText, options, estimatedTime);
        }
        catch
        {
            return (string.Empty, new List<(string, bool)>(), 5);
        }
    }

    /// <summary>
    /// Парсит содержимое для задания
    /// </summary>
    private static (string Instruction, string CodeWord, string Hint, int EstimatedDurationMinutes) ParseContentForTask(string contentJson)
    {
        try
        {
            var contentData = JsonSerializer.Deserialize<JsonElement>(contentJson);
            
            var instruction = contentData.TryGetProperty("instruction", out var instProp) ? 
                instProp.GetString() ?? string.Empty : string.Empty;
            
            var codeWord = contentData.TryGetProperty("codeWord", out var codeProp) ? 
                codeProp.GetString() ?? string.Empty : string.Empty;
            
            var hint = contentData.TryGetProperty("hint", out var hintProp) ? 
                hintProp.GetString() ?? string.Empty : string.Empty;
            
            var estimatedTime = contentData.TryGetProperty("estimatedDurationMinutes", out var timeProp) ? 
                timeProp.GetInt32() : 30;

            return (instruction, codeWord, hint, estimatedTime);
        }
        catch
        {
            return (string.Empty, string.Empty, string.Empty, 30);
        }
    }

    /// <summary>
    /// Генерирует следующий LexoRank для компонента
    /// </summary>
    private static string GenerateNextComponentOrder(ICollection<ComponentBase> existingComponents)
    {
        if (!existingComponents.Any())
            return LexoRankHelper.Middle();
            
        var lastComponent = existingComponents.OrderBy(c => c.Order).LastOrDefault();
        return lastComponent != null ? 
            LexoRankHelper.Next(lastComponent.Order) : 
            LexoRankHelper.Middle();
    }
}