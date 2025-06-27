using Lauf.Domain.Entities.Versions;
using Lauf.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lauf.Domain.Entities.Progress;

/// <summary>
/// Прогресс пользователя по конкретному шагу потока
/// </summary>
public class StepProgress
{
    /// <summary>
    /// Идентификатор записи прогресса
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Идентификатор прогресса потока
    /// </summary>
    public Guid FlowProgressId { get; private set; }

    /// <summary>
    /// Прогресс потока
    /// </summary>
    public FlowProgress FlowProgress { get; private set; } = null!;

    /// <summary>
    /// Идентификатор версии шага
    /// </summary>
    public Guid StepVersionId { get; private set; }

    /// <summary>
    /// Версия шага
    /// </summary>
    public FlowStepVersion StepVersion { get; private set; } = null!;

    /// <summary>
    /// Порядковый номер шага
    /// </summary>
    public int Order { get; private set; }

    /// <summary>
    /// Процент прогресса по шагу
    /// </summary>
    public ProgressPercentage Progress { get; private set; }

    /// <summary>
    /// Количество завершенных компонентов
    /// </summary>
    public int CompletedComponentsCount { get; private set; }

    /// <summary>
    /// Общее количество компонентов в шаге
    /// </summary>
    public int TotalComponentsCount { get; private set; }

    /// <summary>
    /// Время, потраченное на шаг в минутах
    /// </summary>
    public int TimeSpentMinutes { get; private set; }

    /// <summary>
    /// Дата начала шага
    /// </summary>
    public DateTime? StartedAt { get; private set; }

    /// <summary>
    /// Дата завершения шага
    /// </summary>
    public DateTime? CompletedAt { get; private set; }

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime LastUpdatedAt { get; private set; }

    /// <summary>
    /// Разблокирован ли шаг для выполнения
    /// </summary>
    public bool IsUnlocked { get; private set; }

    /// <summary>
    /// Прогресс по компонентам шага
    /// </summary>
    public List<ComponentProgress> ComponentProgresses { get; private set; } = new();

    /// <summary>
    /// Приватный конструктор для EF Core
    /// </summary>
    private StepProgress() { }

    /// <summary>
    /// Конструктор для создания записи прогресса по шагу
    /// </summary>
    /// <param name="flowProgressId">ID прогресса потока</param>
    /// <param name="stepVersionId">ID версии шага</param>
    /// <param name="order">Порядковый номер шага</param>
    /// <param name="totalComponentsCount">Общее количество компонентов</param>
    /// <param name="isUnlocked">Разблокирован ли шаг изначально</param>
    public StepProgress(
        Guid flowProgressId,
        Guid stepVersionId,
        int order,
        int totalComponentsCount,
        bool isUnlocked = false)
    {
        Id = Guid.NewGuid();
        FlowProgressId = flowProgressId;
        StepVersionId = stepVersionId;
        Order = order;
        Progress = new ProgressPercentage(0);
        CompletedComponentsCount = 0;
        TotalComponentsCount = totalComponentsCount > 0 ? totalComponentsCount : throw new ArgumentException("Количество компонентов должно быть положительным");
        TimeSpentMinutes = 0;
        IsUnlocked = isUnlocked;
        LastUpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Пересчитать прогресс на основе прогресса по компонентам
    /// </summary>
    public void RecalculateProgress()
    {
        if (ComponentProgresses.Count == 0)
        {
            Progress = new ProgressPercentage(0);
            return;
        }

        // Подсчитываем завершенные компоненты
        CompletedComponentsCount = ComponentProgresses.Count(cp => cp.IsCompleted);
        TimeSpentMinutes = ComponentProgresses.Sum(cp => cp.TimeSpentMinutes);

        // Рассчитываем прогресс как процент завершенных компонентов
        Progress = ProgressPercentage.FromRatio(CompletedComponentsCount, TotalComponentsCount);

        // Проверяем завершение шага
        if (Progress.Value >= 100 && !CompletedAt.HasValue)
        {
            CompletedAt = DateTime.UtcNow;
        }

        LastUpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Начать выполнение шага
    /// </summary>
    public void Start()
    {
        if (!IsUnlocked)
        {
            throw new InvalidOperationException("Шаг не разблокирован для выполнения");
        }

        StartedAt ??= DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Разблокировать шаг
    /// </summary>
    public void Unlock()
    {
        IsUnlocked = true;
        LastUpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Заблокировать шаг
    /// </summary>
    public void Lock()
    {
        IsUnlocked = false;
        LastUpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Добавить время выполнения
    /// </summary>
    /// <param name="minutes">Количество минут</param>
    public void AddTimeSpent(int minutes)
    {
        if (minutes > 0)
        {
            TimeSpentMinutes += minutes;
            LastUpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Проверить, завершен ли шаг
    /// </summary>
    public bool IsCompleted() => CompletedAt.HasValue || Progress.Value >= 100;

    /// <summary>
    /// Проверить, начат ли шаг
    /// </summary>
    public bool IsStarted() => StartedAt.HasValue;

    /// <summary>
    /// Получить следующий доступный компонент
    /// </summary>
    public Guid? GetNextAvailableComponentId()
    {
        var incompleteComponent = ComponentProgresses
            .Where(cp => !cp.IsCompleted)
            .OrderBy(cp => cp.Order)
            .FirstOrDefault();

        return incompleteComponent?.ComponentVersionId;
    }

    /// <summary>
    /// Получить обязательные компоненты, которые еще не завершены
    /// </summary>
    public List<ComponentProgress> GetIncompleteRequiredComponents()
    {
        return ComponentProgresses
            .Where(cp => cp.IsRequired && !cp.IsCompleted)
            .OrderBy(cp => cp.Order)
            .ToList();
    }

    /// <summary>
    /// Проверить, можно ли завершить шаг (все обязательные компоненты выполнены)
    /// </summary>
    public bool CanComplete()
    {
        return GetIncompleteRequiredComponents().Count == 0;
    }
}