using Lauf.Domain.Entities.Versions;
using Lauf.Domain.Enums;
using Lauf.Domain.ValueObjects;

namespace Lauf.Domain.Entities.Progress;

/// <summary>
/// Прогресс пользователя по конкретному компоненту
/// </summary>
public class ComponentProgress
{
    /// <summary>
    /// Идентификатор записи прогресса
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Идентификатор прогресса шага
    /// </summary>
    public Guid StepProgressId { get; private set; }

    /// <summary>
    /// Прогресс шага
    /// </summary>
    public StepProgress StepProgress { get; private set; } = null!;

    /// <summary>
    /// Идентификатор версии компонента
    /// </summary>
    public Guid ComponentVersionId { get; private set; }

    /// <summary>
    /// Версия компонента
    /// </summary>
    public ComponentVersion ComponentVersion { get; private set; } = null!;

    /// <summary>
    /// Порядковый номер компонента в шаге
    /// </summary>
    public int Order { get; private set; }

    /// <summary>
    /// Является ли компонент обязательным
    /// </summary>
    public bool IsRequired { get; private set; }

    /// <summary>
    /// Статус выполнения компонента
    /// </summary>
    public ProgressStatus Status { get; private set; }

    /// <summary>
    /// Завершен ли компонент
    /// </summary>
    public bool IsCompleted { get; private set; }

    /// <summary>
    /// Количество попыток выполнения
    /// </summary>
    public int AttemptsCount { get; private set; }

    /// <summary>
    /// Лучший результат (для квизов и заданий)
    /// </summary>
    public int? BestScore { get; private set; }

    /// <summary>
    /// Последний результат
    /// </summary>
    public int? LastScore { get; private set; }

    /// <summary>
    /// Время, потраченное на компонент в минутах
    /// </summary>
    public int TimeSpentMinutes { get; private set; }

    /// <summary>
    /// Данные прогресса компонента (JSON)
    /// </summary>
    public ComponentProgressData ProgressData { get; private set; }

    /// <summary>
    /// Дата начала выполнения
    /// </summary>
    public DateTime? StartedAt { get; private set; }

    /// <summary>
    /// Дата завершения
    /// </summary>
    public DateTime? CompletedAt { get; private set; }

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime LastUpdatedAt { get; private set; }

    /// <summary>
    /// Приватный конструктор для EF Core
    /// </summary>
    private ComponentProgress() { }

    /// <summary>
    /// Конструктор для создания записи прогресса компонента
    /// </summary>
    /// <param name="stepProgressId">ID прогресса шага</param>
    /// <param name="componentVersionId">ID версии компонента</param>
    /// <param name="order">Порядковый номер</param>
    /// <param name="isRequired">Является ли обязательным</param>
    public ComponentProgress(
        Guid stepProgressId,
        Guid componentVersionId,
        int order,
        bool isRequired)
    {
        Id = Guid.NewGuid();
        StepProgressId = stepProgressId;
        ComponentVersionId = componentVersionId;
        Order = order;
        IsRequired = isRequired;
        Status = ProgressStatus.NotStarted;
        IsCompleted = false;
        AttemptsCount = 0;
        TimeSpentMinutes = 0;
        ProgressData = ComponentProgressData.Empty;
        LastUpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Начать выполнение компонента
    /// </summary>
    public void Start()
    {
        if (Status == ProgressStatus.NotStarted)
        {
            Status = ProgressStatus.InProgress;
            StartedAt = DateTime.UtcNow;
            LastUpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Завершить компонент
    /// </summary>
    /// <param name="score">Результат (для квизов и заданий)</param>
    public void Complete(int? score = null)
    {
        Status = ProgressStatus.Completed;
        IsCompleted = true;
        CompletedAt = DateTime.UtcNow;
        LastUpdatedAt = DateTime.UtcNow;

        if (score.HasValue)
        {
            LastScore = score.Value;
            if (!BestScore.HasValue || score.Value > BestScore.Value)
            {
                BestScore = score.Value;
            }
        }
    }

    /// <summary>
    /// Зарегистрировать попытку выполнения
    /// </summary>
    /// <param name="score">Результат попытки</param>
    /// <param name="progressData">Данные прогресса</param>
    public void RegisterAttempt(int? score = null, ComponentProgressData? progressData = null)
    {
        AttemptsCount++;
        LastScore = score;
        
        if (score.HasValue && (!BestScore.HasValue || score.Value > BestScore.Value))
        {
            BestScore = score.Value;
        }

        if (progressData != null)
        {
            ProgressData = progressData;
        }

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
    /// Обновить данные прогресса
    /// </summary>
    /// <param name="progressData">Новые данные прогресса</param>
    public void UpdateProgressData(ComponentProgressData progressData)
    {
        ProgressData = progressData ?? ComponentProgressData.Empty;
        LastUpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Приостановить выполнение компонента
    /// </summary>
    public void Pause()
    {
        if (Status == ProgressStatus.InProgress)
        {
            Status = ProgressStatus.Paused;
            LastUpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Возобновить выполнение компонента
    /// </summary>
    public void Resume()
    {
        if (Status == ProgressStatus.Paused)
        {
            Status = ProgressStatus.InProgress;
            LastUpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Сбросить прогресс компонента
    /// </summary>
    public void Reset()
    {
        Status = ProgressStatus.NotStarted;
        IsCompleted = false;
        AttemptsCount = 0;
        BestScore = null;
        LastScore = null;
        TimeSpentMinutes = 0;
        ProgressData = ComponentProgressData.Empty;
        StartedAt = null;
        CompletedAt = null;
        LastUpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Проверить, можно ли выполнить еще одну попытку
    /// </summary>
    /// <param name="maxAttempts">Максимальное количество попыток</param>
    public bool CanAttempt(int? maxAttempts)
    {
        if (!maxAttempts.HasValue || maxAttempts.Value <= 0)
        {
            return true; // Неограниченное количество попыток
        }

        return AttemptsCount < maxAttempts.Value;
    }

    /// <summary>
    /// Проверить, достигнут ли минимальный проходной балл
    /// </summary>
    /// <param name="minimumScore">Минимальный проходной балл</param>
    public bool HasPassingScore(int? minimumScore)
    {
        if (!minimumScore.HasValue || minimumScore.Value <= 0)
        {
            return true; // Нет требований к минимальному баллу
        }

        return BestScore.HasValue && BestScore.Value >= minimumScore.Value;
    }

    /// <summary>
    /// Получить типизированные данные прогресса
    /// </summary>
    /// <typeparam name="T">Тип данных прогресса</typeparam>
    public T? GetProgressData<T>() where T : class
    {
        return ProgressData.GetData<T>();
    }
}