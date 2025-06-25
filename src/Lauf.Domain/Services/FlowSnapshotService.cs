using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Entities.Snapshots;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Domain.Exceptions;

namespace Lauf.Domain.Services;

/// <summary>
/// Сервис для работы со снапшотами потоков
/// </summary>
public class FlowSnapshotService : IFlowSnapshotService
{
    private readonly IFlowRepository _flowRepository;
    private readonly IFlowSnapshotRepository _snapshotRepository;

    /// <summary>
    /// Конструктор сервиса снапшотов потоков
    /// </summary>
    /// <param name="flowRepository">Репозиторий потоков</param>
    /// <param name="snapshotRepository">Репозиторий снапшотов</param>
    public FlowSnapshotService(
        IFlowRepository flowRepository,
        IFlowSnapshotRepository snapshotRepository)
    {
        _flowRepository = flowRepository ?? throw new ArgumentNullException(nameof(flowRepository));
        _snapshotRepository = snapshotRepository ?? throw new ArgumentNullException(nameof(snapshotRepository));
    }

    /// <summary>
    /// Создать полный снапшот потока с все его шагами и компонентами
    /// </summary>
    public async Task<FlowSnapshot> CreateFlowSnapshotAsync(Flow flow, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(flow);

        // Получаем следующую версию снапшота для данного потока
        var existingSnapshots = await _snapshotRepository.GetByOriginalFlowIdAsync(flow.Id, cancellationToken);
        var nextVersion = existingSnapshots.Count > 0 ? existingSnapshots.Max(s => s.Version) + 1 : 1;

        // Создаем основной снапшот потока
        var flowSnapshot = new FlowSnapshot(
            flow.Id,
            flow.Title,
            flow.Description,
            flow.Status,
            8, // Временная заглушка для EstimatedHours
            5, // Временная заглушка для WorkingDaysToComplete
            true, // Временная заглушка для IsRequired
            "[]", // Временная заглушка для Tags
            nextVersion);

        // Создаем снапшоты шагов
        foreach (var step in flow.Steps.OrderBy(s => s.Order))
        {
            var stepSnapshot = new FlowStepSnapshot(
                step.Id,
                flowSnapshot.Id,
                step.Title,
                step.Description,
                step.Order,
                false, // Временная заглушка для RequiresSequentialCompletion
                60); // Временная заглушка для EstimatedMinutes

            // Создаем снапшоты компонентов
            foreach (var component in step.Components.OrderBy(c => c.Order))
            {
                var componentSnapshot = new ComponentSnapshot(
                    component.Id,
                    stepSnapshot.Id,
                    component.ComponentType, // Используем существующее свойство
                    component.Title,
                    component.Description,
                    component.Order,
                    component.IsRequired,
                    30, // Временная заглушка для EstimatedMinutes
                    "{}", // Временная заглушка для SerializeContent
                    "{}", // Временная заглушка для Settings
                    null, // Временная заглушка для MaxAttempts
                    null); // Временная заглушка для MinimumScore

                stepSnapshot.AddComponentSnapshot(componentSnapshot);
            }

            flowSnapshot.AddStepSnapshot(stepSnapshot);
        }

        // Сохраняем снапшот
        await _snapshotRepository.AddAsync(flowSnapshot, cancellationToken);

        return flowSnapshot;
    }

    /// <summary>
    /// Создать снапшот конкретного потока с определенной версией
    /// </summary>
    public async Task<FlowSnapshot> CreateFlowSnapshotAsync(Guid flowId, int version, CancellationToken cancellationToken = default)
    {
        var flow = await _flowRepository.GetWithDetailsAsync(flowId, cancellationToken);
        if (flow == null)
        {
            throw new FlowNotFoundException(flowId);
        }

        return await CreateFlowSnapshotAsync(flow, cancellationToken);
    }

    /// <summary>
    /// Получить снапшот потока по ID
    /// </summary>
    public async Task<FlowSnapshot?> GetFlowSnapshotAsync(Guid snapshotId, CancellationToken cancellationToken = default)
    {
        return await _snapshotRepository.GetByIdWithDetailsAsync(snapshotId, cancellationToken);
    }

    /// <summary>
    /// Получить снапшот потока по назначению
    /// </summary>
    public async Task<FlowSnapshot?> GetFlowSnapshotByAssignmentAsync(Guid assignmentId, CancellationToken cancellationToken = default)
    {
        return await _snapshotRepository.GetByAssignmentIdAsync(assignmentId, cancellationToken);
    }

    /// <summary>
    /// Получить существующий снапшот потока или создать новый
    /// </summary>
    public async Task<FlowSnapshot> GetOrCreateFlowSnapshotAsync(Guid flowId, CancellationToken cancellationToken = default)
    {
        // Сначала пытаемся найти существующий снапшот
        var existingSnapshots = await _snapshotRepository.GetByOriginalFlowIdAsync(flowId, cancellationToken);
        if (existingSnapshots.Any())
        {
            // Возвращаем последний снапшот
            var latestSnapshot = existingSnapshots.OrderByDescending(s => s.Version).First();
            return await _snapshotRepository.GetByIdWithDetailsAsync(latestSnapshot.Id, cancellationToken) 
                ?? throw new InvalidOperationException("Снапшот найден, но не удалось загрузить детали");
        }

        // Если снапшота нет, создаем новый
        var flow = await _flowRepository.GetWithDetailsAsync(flowId, cancellationToken);
        if (flow == null)
        {
            throw new FlowNotFoundException(flowId);
        }

        return await CreateFlowSnapshotAsync(flow, cancellationToken);
    }

    /// <summary>
    /// Проверить целостность снапшота
    /// </summary>
    public async Task<bool> ValidateSnapshotIntegrityAsync(Guid snapshotId, CancellationToken cancellationToken = default)
    {
        var snapshot = await _snapshotRepository.GetByIdWithDetailsAsync(snapshotId, cancellationToken);
        if (snapshot == null)
        {
            return false;
        }

        // Проверяем базовую структуру
        if (string.IsNullOrEmpty(snapshot.Title) || snapshot.Steps.Count == 0)
        {
            return false;
        }

        // Проверяем порядок шагов
        var stepOrders = snapshot.Steps.Select(s => s.Order).OrderBy(o => o).ToList();
        for (int i = 0; i < stepOrders.Count; i++)
        {
            if (stepOrders[i] != i)
            {
                return false; // Нарушен порядок шагов
            }
        }

        // Проверяем каждый шаг
        foreach (var step in snapshot.Steps)
        {
            if (string.IsNullOrEmpty(step.Title) || step.Components.Count == 0)
            {
                return false;
            }

            // Проверяем порядок компонентов в шаге
            var componentOrders = step.Components.Select(c => c.Order).OrderBy(o => o).ToList();
            for (int i = 0; i < componentOrders.Count; i++)
            {
                if (componentOrders[i] != i)
                {
                    return false; // Нарушен порядок компонентов
                }
            }

            // Проверяем каждый компонент
            foreach (var component in step.Components)
            {
                if (string.IsNullOrEmpty(component.Title) || string.IsNullOrEmpty(component.Content))
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Получить различия между оригинальным потоком и снапшотом
    /// </summary>
    public async Task<List<string>> GetSnapshotDifferencesAsync(Guid flowId, Guid snapshotId, CancellationToken cancellationToken = default)
    {
        var differences = new List<string>();

        var flow = await _flowRepository.GetWithDetailsAsync(flowId, cancellationToken);
        var snapshot = await _snapshotRepository.GetByIdWithDetailsAsync(snapshotId, cancellationToken);

        if (flow == null)
        {
            differences.Add("Оригинальный поток не найден");
            return differences;
        }

        if (snapshot == null)
        {
            differences.Add("Снапшот не найден");
            return differences;
        }

        // Сравниваем основную информацию
        if (flow.Title != snapshot.Title)
        {
            differences.Add($"Изменено название: '{snapshot.Title}' -> '{flow.Title}'");
        }

        if (flow.Description != snapshot.Description)
        {
            differences.Add($"Изменено описание потока");
        }

        // EstimatedHours сравнение закомментировано до реализации в Flow
        // if (flow.EstimatedHours != snapshot.EstimatedHours)
        // {
        //     differences.Add($"Изменено расчетное время: {snapshot.EstimatedHours}ч -> {flow.EstimatedHours}ч");
        // }

        // Сравниваем количество шагов
        if (flow.Steps.Count != snapshot.Steps.Count)
        {
            differences.Add($"Изменено количество шагов: {snapshot.Steps.Count} -> {flow.Steps.Count}");
        }

        // Сравниваем шаги
        var flowStepsDict = flow.Steps.ToDictionary(s => s.Id);
        foreach (var stepSnapshot in snapshot.Steps)
        {
            if (flowStepsDict.TryGetValue(stepSnapshot.OriginalStepId, out var flowStep))
            {
                if (flowStep.Title != stepSnapshot.Title)
                {
                    differences.Add($"Изменено название шага '{stepSnapshot.Title}' -> '{flowStep.Title}'");
                }

                if (flowStep.Components.Count != stepSnapshot.Components.Count)
                {
                    differences.Add($"Изменено количество компонентов в шаге '{stepSnapshot.Title}': {stepSnapshot.Components.Count} -> {flowStep.Components.Count}");
                }
            }
            else
            {
                differences.Add($"Шаг '{stepSnapshot.Title}' был удален из оригинального потока");
            }
        }

        // Проверяем новые шаги
        var snapshotStepIds = snapshot.Steps.Select(s => s.OriginalStepId).ToHashSet();
        foreach (var flowStep in flow.Steps)
        {
            if (!snapshotStepIds.Contains(flowStep.Id))
            {
                differences.Add($"Добавлен новый шаг '{flowStep.Title}'");
            }
        }

        return differences;
    }

    /// <summary>
    /// Удалить устаревшие снапшоты
    /// </summary>
    public async Task<int> CleanupOldSnapshotsAsync(int olderThanDays = 365, int keepMinimum = 1, CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-olderThanDays);
        var oldSnapshots = await _snapshotRepository.GetOldSnapshotsAsync(cutoffDate, cancellationToken);

        // Группируем по оригинальному потоку
        var snapshotsByFlow = oldSnapshots.GroupBy(s => s.OriginalFlowId);
        var deletedCount = 0;

        foreach (var group in snapshotsByFlow)
        {
            var flowSnapshots = group.OrderByDescending(s => s.CreatedAt).ToList();
            
            // Оставляем минимальное количество снапшотов
            if (flowSnapshots.Count > keepMinimum)
            {
                var snapshotsToDelete = flowSnapshots.Skip(keepMinimum);
                foreach (var snapshot in snapshotsToDelete)
                {
                    // Проверяем, что снапшот не используется активными назначениями
                    var hasActiveAssignments = await _snapshotRepository.HasActiveAssignmentsAsync(snapshot.Id, cancellationToken);
                    if (!hasActiveAssignments)
                    {
                        await _snapshotRepository.DeleteAsync(snapshot.Id, cancellationToken);
                        deletedCount++;
                    }
                }
            }
        }

        return deletedCount;
    }
}