using MediatR;
using AutoMapper;
using Lauf.Application.DTOs.Flows;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Domain.Enums;

namespace Lauf.Application.Queries.Flows;

/// <summary>
/// Запрос для получения детальной информации о потоке
/// </summary>
public record GetFlowDetailsQuery : IRequest<FlowDetailsDto?>
{
    /// <summary>
    /// Идентификатор потока
    /// </summary>
    public Guid FlowId { get; init; }

    /// <summary>
    /// Идентификатор пользователя для контекста
    /// </summary>
    public Guid? UserId { get; init; }

    public GetFlowDetailsQuery(Guid flowId, Guid? userId = null)
    {
        FlowId = flowId;
        UserId = userId;
    }
}

/// <summary>
/// DTO для детальной информации о потоке
/// </summary>
public class FlowDetailsDto : FlowDto
{
    /// <summary>
    /// Полная информация о шагах
    /// </summary>
    public new ICollection<FlowStepDetailsDto> Steps { get; set; } = new List<FlowStepDetailsDto>();

    /// <summary>
    /// Статистика потока
    /// </summary>
    public FlowStatisticsDto Statistics { get; set; } = new();

    /// <summary>
    /// Прогресс пользователя (если UserId указан)
    /// </summary>
    public UserFlowProgressDto? UserProgress { get; set; }
}

/// <summary>
/// DTO для детальной информации о шаге потока
/// </summary>
public class FlowStepDetailsDto : FlowStepDto
{
    /// <summary>
    /// Полная информация о компонентах
    /// </summary>
    public new ICollection<FlowStepComponentDetailsDto> Components { get; set; } = new List<FlowStepComponentDetailsDto>();

    /// <summary>
    /// Доступен ли шаг для пользователя
    /// </summary>
    public bool IsAccessible { get; set; }

    /// <summary>
    /// Завершен ли шаг
    /// </summary>
    public bool IsCompleted { get; set; }
}

/// <summary>
/// DTO для детальной информации о компоненте шага
/// </summary>
public class FlowStepComponentDetailsDto
{
    /// <summary>
    /// Идентификатор компонента
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название компонента
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание компонента
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Тип компонента
    /// </summary>
    public ComponentType ComponentType { get; set; }

    /// <summary>
    /// Контент компонента
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Настройки компонента
    /// </summary>
    public Dictionary<string, object> Settings { get; set; } = new();

    /// <summary>
    /// Обязателен ли компонент
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Порядок сортировки
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Прогресс выполнения компонента
    /// </summary>
    public ComponentProgressDto? Progress { get; set; }
}

/// <summary>
/// DTO для статистики потока
/// </summary>
public class FlowStatisticsDto
{
    /// <summary>
    /// Общее количество назначений
    /// </summary>
    public int TotalAssignments { get; set; }

    /// <summary>
    /// Количество активных назначений
    /// </summary>
    public int ActiveAssignments { get; set; }

    /// <summary>
    /// Количество завершенных назначений
    /// </summary>
    public int CompletedAssignments { get; set; }

    /// <summary>
    /// Средний прогресс выполнения
    /// </summary>
    public decimal AverageProgress { get; set; }

    /// <summary>
    /// Среднее время прохождения
    /// </summary>
    public TimeSpan? AverageCompletionTime { get; set; }
}

/// <summary>
/// DTO для прогресса пользователя по потоку
/// </summary>
public class UserFlowProgressDto
{
    /// <summary>
    /// Общий прогресс (0-100)
    /// </summary>
    public decimal OverallProgress { get; set; }

    /// <summary>
    /// Текущий шаг
    /// </summary>
    public int CurrentStep { get; set; }

    /// <summary>
    /// Завершенные шаги
    /// </summary>
    public int CompletedSteps { get; set; }

    /// <summary>
    /// Общее количество шагов
    /// </summary>
    public int TotalSteps { get; set; }

    /// <summary>
    /// Дата начала
    /// </summary>
    public DateTime StartedAt { get; set; }

    /// <summary>
    /// Последняя активность
    /// </summary>
    public DateTime? LastActivityAt { get; set; }

    /// <summary>
    /// Статус назначения
    /// </summary>
    public AssignmentStatus Status { get; set; }
}

/// <summary>
/// DTO для прогресса компонента
/// </summary>
public class ComponentProgressDto
{
    /// <summary>
    /// Завершен ли компонент
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// Прогресс выполнения (0-100)
    /// </summary>
    public decimal Progress { get; set; }

    /// <summary>
    /// Данные прогресса (JSON)
    /// </summary>
    public Dictionary<string, object> Data { get; set; } = new();

    /// <summary>
    /// Время завершения
    /// </summary>
    public DateTime? CompletedAt { get; set; }
}

/// <summary>
/// Обработчик запроса получения детальной информации о потоке
/// </summary>
public class GetFlowDetailsQueryHandler : IRequestHandler<GetFlowDetailsQuery, FlowDetailsDto?>
{
    private readonly IFlowRepository _flowRepository;
    private readonly IFlowAssignmentRepository _assignmentRepository;
    private readonly IUserProgressRepository _progressRepository;
    private readonly IMapper _mapper;

    public GetFlowDetailsQueryHandler(
        IFlowRepository flowRepository,
        IFlowAssignmentRepository assignmentRepository,
        IUserProgressRepository progressRepository,
        IMapper mapper)
    {
        _flowRepository = flowRepository ?? throw new ArgumentNullException(nameof(flowRepository));
        _assignmentRepository = assignmentRepository ?? throw new ArgumentNullException(nameof(assignmentRepository));
        _progressRepository = progressRepository ?? throw new ArgumentNullException(nameof(progressRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Обработка запроса получения детальной информации о потоке
    /// </summary>
    public async Task<FlowDetailsDto?> Handle(GetFlowDetailsQuery request, CancellationToken cancellationToken)
    {
        var flow = await _flowRepository.GetByIdWithStepsAsync(request.FlowId, cancellationToken);
        if (flow == null)
        {
            return null;
        }

        var flowDetails = _mapper.Map<FlowDetailsDto>(flow);

        // Получаем статистику потока
        flowDetails.Statistics = await GetFlowStatisticsAsync(request.FlowId, cancellationToken);

        // Если указан пользователь, получаем его прогресс
        if (request.UserId.HasValue)
        {
            flowDetails.UserProgress = await GetUserProgressAsync(request.FlowId, request.UserId.Value, cancellationToken);
            
            // Дополняем информацию о доступности шагов и компонентов
            await EnrichWithUserContextAsync(flowDetails, request.UserId.Value, cancellationToken);
        }

        return flowDetails;
    }

    /// <summary>
    /// Получение статистики потока
    /// </summary>
    private async Task<FlowStatisticsDto> GetFlowStatisticsAsync(Guid flowId, CancellationToken cancellationToken)
    {
        var assignments = await _assignmentRepository.GetByFlowIdAsync(flowId, cancellationToken);
        
        var statistics = new FlowStatisticsDto
        {
            TotalAssignments = assignments.Count(),
            ActiveAssignments = assignments.Count(a => a.Status == AssignmentStatus.Assigned || a.Status == AssignmentStatus.InProgress),
            CompletedAssignments = assignments.Count(a => a.Status == AssignmentStatus.Completed)
        };

        // Рассчитываем средний прогресс
        if (assignments.Any())
        {
            var progressList = new List<decimal>();
            foreach (var assignment in assignments)
            {
                var progress = await _progressRepository.GetByAssignmentIdAsync(assignment.Id, cancellationToken);
                progressList.Add(progress?.OverallProgress?.Value ?? 0);
            }
            statistics.AverageProgress = progressList.Any() ? progressList.Average() : 0;
        }

        return statistics;
    }

    /// <summary>
    /// Получение прогресса пользователя по потоку
    /// </summary>
    private async Task<UserFlowProgressDto?> GetUserProgressAsync(Guid flowId, Guid userId, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentRepository.GetByUserAndFlowAsync(userId, flowId, cancellationToken);
        if (assignment == null)
        {
            return null;
        }

        var progress = await _progressRepository.GetByAssignmentIdAsync(assignment.Id, cancellationToken);
        
        var totalSteps = assignment.Flow?.ActiveContent?.Steps.Count ?? 0;
        var overallProgress = progress?.OverallProgress?.Value ?? 0;
        var completedSteps = totalSteps > 0 ? (int)(overallProgress * totalSteps / 100) : 0;
        var currentStep = completedSteps < totalSteps ? completedSteps + 1 : totalSteps;
        
        return new UserFlowProgressDto
        {
            OverallProgress = overallProgress,
            CurrentStep = currentStep,
            CompletedSteps = completedSteps,
            TotalSteps = totalSteps,
            StartedAt = assignment.AssignedAt,
            LastActivityAt = progress?.UpdatedAt,
            Status = assignment.Status
        };
    }

    /// <summary>
    /// Обогащение данных контекстом пользователя
    /// </summary>
    private async Task EnrichWithUserContextAsync(FlowDetailsDto flowDetails, Guid userId, CancellationToken cancellationToken)
    {
        // Получаем прогресс пользователя
        var assignment = await _assignmentRepository.GetByUserAndFlowAsync(userId, flowDetails.Id, cancellationToken);
        var userProgress = assignment != null ? 
            await _progressRepository.GetByAssignmentIdAsync(assignment.Id, cancellationToken) : null;

        var isSequential = flowDetails.Settings?.RequireSequentialCompletionComponents ?? true;
        var completedSteps = new HashSet<int>();

        // Определяем какие шаги завершены (простая логика)
        if (userProgress?.OverallProgress != null)
        {
            var progressPercent = userProgress.OverallProgress.Value;
            var totalSteps = flowDetails.Steps.Count;
            if (totalSteps > 0)
            {
                var completedStepCount = (int)(progressPercent * totalSteps / 100);
                for (int i = 1; i <= completedStepCount; i++)
                {
                    completedSteps.Add(i);
                }
            }
        }

        foreach (var step in flowDetails.Steps.OrderBy(s => s.Order))
        {
            var stepNumber = Array.IndexOf(flowDetails.Steps.OrderBy(s => s.Order).ToArray(), step) + 1;
            step.IsCompleted = completedSteps.Contains(stepNumber);
            
            if (isSequential)
            {
                // Последовательное прохождение: доступен только следующий незавершенный шаг
                if (stepNumber == 1)
                {
                    step.IsAccessible = true;
                }
                else
                {
                    var previousStepCompleted = completedSteps.Contains(stepNumber - 1);
                    step.IsAccessible = previousStepCompleted;
                }
            }
            else
            {
                // Свободное прохождение: все шаги доступны
                step.IsAccessible = true;
            }
            
            // Обновляем прогресс компонентов
            foreach (var component in step.Components)
            {
                var componentProgress = step.IsCompleted ? 100 : 0;
                component.Progress = new ComponentProgressDto
                {
                    IsCompleted = step.IsCompleted,
                    Progress = componentProgress,
                    CompletedAt = step.IsCompleted ? DateTime.UtcNow : null
                };
            }
        }
    }
}