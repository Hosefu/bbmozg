using MediatR;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Domain.Enums;

namespace Lauf.Application.Queries.Flows;

/// <summary>
/// Запрос для получения статистики потока
/// </summary>
public record GetFlowStatsQuery : IRequest<FlowStatsDto>
{
    /// <summary>
    /// Идентификатор потока
    /// </summary>
    public Guid FlowId { get; init; }

    public GetFlowStatsQuery(Guid flowId)
    {
        FlowId = flowId;
    }
}

/// <summary>
/// DTO для статистики потока
/// </summary>
public class FlowStatsDto
{
    /// <summary>
    /// Идентификатор потока
    /// </summary>
    public Guid FlowId { get; set; }

    /// <summary>
    /// Название потока
    /// </summary>
    public string FlowTitle { get; set; } = string.Empty;

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
    /// Количество просроченных назначений
    /// </summary>
    public int OverdueAssignments { get; set; }

    /// <summary>
    /// Средний прогресс выполнения (%)
    /// </summary>
    public decimal AverageProgress { get; set; }

    /// <summary>
    /// Процент завершения
    /// </summary>
    public decimal CompletionRate { get; set; }

    /// <summary>
    /// Среднее время прохождения (в днях)
    /// </summary>
    public double? AverageCompletionTimeDays { get; set; }

    /// <summary>
    /// Самые проблемные шаги (с низким процентом прохождения)
    /// </summary>
    public ICollection<StepStatsDto> ProblematicSteps { get; set; } = new List<StepStatsDto>();
}

/// <summary>
/// DTO для статистики шага
/// </summary>
public class StepStatsDto
{
    /// <summary>
    /// Номер шага
    /// </summary>
    public int StepNumber { get; set; }

    /// <summary>
    /// Название шага
    /// </summary>
    public string StepTitle { get; set; } = string.Empty;

    /// <summary>
    /// Процент завершения шага
    /// </summary>
    public decimal CompletionRate { get; set; }

    /// <summary>
    /// Среднее время прохождения шага (в часах)
    /// </summary>
    public double? AverageTimeHours { get; set; }
}

/// <summary>
/// Обработчик запроса получения статистики потока
/// </summary>
public class GetFlowStatsQueryHandler : IRequestHandler<GetFlowStatsQuery, FlowStatsDto>
{
    private readonly IFlowRepository _flowRepository;
    private readonly IFlowAssignmentRepository _assignmentRepository;
    private readonly IUserProgressRepository _progressRepository;

    public GetFlowStatsQueryHandler(
        IFlowRepository flowRepository,
        IFlowAssignmentRepository assignmentRepository,
        IUserProgressRepository progressRepository)
    {
        _flowRepository = flowRepository ?? throw new ArgumentNullException(nameof(flowRepository));
        _assignmentRepository = assignmentRepository ?? throw new ArgumentNullException(nameof(assignmentRepository));
        _progressRepository = progressRepository ?? throw new ArgumentNullException(nameof(progressRepository));
    }

    /// <summary>
    /// Обработка запроса получения статистики потока
    /// </summary>
    public async Task<FlowStatsDto> Handle(GetFlowStatsQuery request, CancellationToken cancellationToken)
    {
        var flow = await _flowRepository.GetByIdAsync(request.FlowId, cancellationToken);
        if (flow == null)
        {
            throw new ArgumentException($"Поток с ID {request.FlowId} не найден");
        }

        var assignments = await _assignmentRepository.GetByFlowIdAsync(request.FlowId, cancellationToken);
        var assignmentsList = assignments.ToList();

        var stats = new FlowStatsDto
        {
            FlowId = request.FlowId,
            FlowTitle = flow.Name,
            TotalAssignments = assignmentsList.Count,
            ActiveAssignments = assignmentsList.Count(a => 
                a.Status == AssignmentStatus.Assigned || 
                a.Status == AssignmentStatus.InProgress),
            CompletedAssignments = assignmentsList.Count(a => a.Status == AssignmentStatus.Completed),
            OverdueAssignments = assignmentsList.Count(a => 
                a.Deadline < DateTime.UtcNow && 
                a.Status != AssignmentStatus.Completed)
        };

        // Рассчитываем процент завершения
        if (stats.TotalAssignments > 0)
        {
            stats.CompletionRate = (decimal)stats.CompletedAssignments / stats.TotalAssignments * 100;
        }

        // Рассчитываем средний прогресс
        if (assignmentsList.Any())
        {
            var progressList = new List<decimal>();
            var completionTimes = new List<TimeSpan>();

            foreach (var assignment in assignmentsList)
            {
                var progress = await _progressRepository.GetByAssignmentIdAsync(assignment.Id, cancellationToken);
                progressList.Add(progress?.OverallProgress?.Value ?? 0);

                // Рассчитываем время завершения для завершенных назначений
                if (assignment.Status == AssignmentStatus.Completed)
                {
                    var completionTime = assignment.CompletedAt.GetValueOrDefault() - assignment.AssignedAt;
                    completionTimes.Add(completionTime);
                }
            }

            stats.AverageProgress = progressList.Any() ? progressList.Average() : 0;
            stats.AverageCompletionTimeDays = completionTimes.Any() ? 
                completionTimes.Average(t => t.TotalDays) : null;
        }

        // Рассчитываем статистику по шагам
        stats.ProblematicSteps = await CalculateStepStatistics(request.FlowId, assignmentsList, cancellationToken);

        return stats;
    }

    /// <summary>
    /// Рассчитывает статистику по шагам потока
    /// </summary>
    /// <param name="flowId">Идентификатор потока</param>
    /// <param name="assignments">Назначения потока</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Статистика по шагам</returns>
    private async Task<ICollection<StepStatsDto>> CalculateStepStatistics(
        Guid flowId, 
        List<Domain.Entities.Flows.FlowAssignment> assignments, 
        CancellationToken cancellationToken)
    {
        var flowWithSteps = await _flowRepository.GetByIdWithStepsAsync(flowId, cancellationToken);
        if (flowWithSteps?.ActiveContent?.Steps == null || !flowWithSteps.ActiveContent.Steps.Any())
        {
            return new List<StepStatsDto>();
        }

        var stepStats = new List<StepStatsDto>();

        foreach (var step in flowWithSteps.ActiveContent.Steps.OrderBy(s => s.Order))
        {
            var stepProgressList = new List<bool>();
            var stepTimeList = new List<double>();

            foreach (var assignment in assignments)
            {
                var progress = await _progressRepository.GetByAssignmentIdAsync(assignment.Id, cancellationToken);
                if (progress != null)
                {
                    // Простая логика: считаем шаг завершенным, если прогресс > 80%
                    var isStepCompleted = progress.OverallProgress?.Value > 80;
                    stepProgressList.Add(isStepCompleted);

                    // Оценка времени на основе оценки длительности
                    if (isStepCompleted)
                    {
                        // Поскольку EstimatedDurationMinutes больше нет, используем примерное время 1 час на шаг
                        stepTimeList.Add(1.0);
                    }
                }
            }

            var completionRate = stepProgressList.Any() ? 
                (decimal)(stepProgressList.Count(x => x) * 100) / stepProgressList.Count : 0;

            var avgTime = stepTimeList.Any() ? stepTimeList.Average() : (double?)null;

            stepStats.Add(new StepStatsDto
            {
                StepNumber = Array.IndexOf(flowWithSteps.ActiveContent.Steps.OrderBy(s => s.Order).ToArray(), step) + 1, // Конвертируем LexoRank в номер
                StepTitle = step.Name,
                CompletionRate = completionRate,
                AverageTimeHours = avgTime
            });
        }

        // Возвращаем только проблемные шаги (с низким процентом прохождения)
        return stepStats.Where(s => s.CompletionRate < 70).ToList();
    }
}