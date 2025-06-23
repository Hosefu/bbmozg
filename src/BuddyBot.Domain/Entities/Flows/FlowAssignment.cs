using BuddyBot.Domain.Entities.Users;
using BuddyBot.Domain.Enums;

namespace BuddyBot.Domain.Entities.Flows;

/// <summary>
/// Назначение потока пользователю - создает связь между пользователем и потоком с конкретными условиями
/// </summary>
public class FlowAssignment
{
    /// <summary>
    /// Уникальный идентификатор назначения
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Пользователь, которому назначен поток
    /// </summary>
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// Идентификатор потока
    /// </summary>
    public Guid FlowId { get; set; }

    /// <summary>
    /// Поток, который назначен пользователю
    /// </summary>
    public virtual Flow Flow { get; set; } = null!;

    /// <summary>
    /// Идентификатор снапшота потока (будет создан в 4 этапе)
    /// Снапшот - это неизменяемая копия потока на момент назначения
    /// </summary>
    public Guid? FlowSnapshotId { get; set; }

    /// <summary>
    /// Идентификатор бадди (наставника)
    /// </summary>
    public Guid? BuddyId { get; set; }

    /// <summary>
    /// Бадди (наставник), курирующий прохождение
    /// </summary>
    public virtual User? Buddy { get; set; }

    /// <summary>
    /// Идентификатор администратора, который сделал назначение
    /// </summary>
    public Guid AssignedById { get; set; }

    /// <summary>
    /// Администратор, который сделал назначение
    /// </summary>
    public virtual User AssignedBy { get; set; } = null!;

    /// <summary>
    /// Статус назначения
    /// </summary>
    public AssignmentStatus Status { get; set; } = AssignmentStatus.Assigned;

    /// <summary>
    /// Приоритет выполнения
    /// </summary>
    public int Priority { get; set; } = 0;

    /// <summary>
    /// Дата назначения
    /// </summary>
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Дата начала прохождения
    /// </summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// Дедлайн завершения
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Дата завершения
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Дата последней активности
    /// </summary>
    public DateTime? LastActivityAt { get; set; }

    /// <summary>
    /// Дата постановки на паузу
    /// </summary>
    public DateTime? PausedAt { get; set; }

    /// <summary>
    /// Причина постановки на паузу
    /// </summary>
    public string? PauseReason { get; set; }

    /// <summary>
    /// Общий прогресс в процентах (0-100)
    /// </summary>
    public int ProgressPercent { get; set; } = 0;

    /// <summary>
    /// Количество завершенных шагов
    /// </summary>
    public int CompletedSteps { get; set; } = 0;

    /// <summary>
    /// Общее количество шагов в потоке
    /// </summary>
    public int TotalSteps { get; set; } = 0;

    /// <summary>
    /// Количество попыток прохождения
    /// </summary>
    public int AttemptCount { get; set; } = 1;

    /// <summary>
    /// Финальная оценка (если применимо)
    /// </summary>
    public int? FinalScore { get; set; }

    /// <summary>
    /// Заметки администратора
    /// </summary>
    public string AdminNotes { get; set; } = string.Empty;

    /// <summary>
    /// Обратная связь от пользователя
    /// </summary>
    public string UserFeedback { get; set; } = string.Empty;

    /// <summary>
    /// Оценка потока пользователем (1-5)
    /// </summary>
    public int? UserRating { get; set; }

    /// <summary>
    /// Дата создания записи
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Конструктор для создания нового назначения потока
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="flowId">Идентификатор потока</param>
    /// <param name="snapshotId">Идентификатор снапшота</param>
    /// <param name="dueDate">Дедлайн</param>
    /// <param name="buddyId">Идентификатор buddy</param>
    /// <param name="assignedById">Идентификатор назначившего</param>
    /// <param name="notes">Заметки</param>
    /// <param name="priority">Приоритет</param>
    public FlowAssignment(Guid userId, Guid flowId, Guid snapshotId, DateTime? dueDate, Guid? buddyId, Guid assignedById, string? notes, int priority)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        FlowId = flowId;
        FlowSnapshotId = snapshotId;
        BuddyId = buddyId;
        DueDate = dueDate;
        AssignedById = assignedById;
        AdminNotes = notes ?? string.Empty;
        Priority = priority;
        Status = AssignmentStatus.Assigned;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Конструктор для EF Core
    /// </summary>
    protected FlowAssignment() { }

    /// <summary>
    /// Проверяет, может ли пользователь начать прохождение
    /// </summary>
    /// <returns>true, если может начать</returns>
    public bool CanStart()
    {
        return Status == AssignmentStatus.Assigned && 
               !StartedAt.HasValue;
    }

    /// <summary>
    /// Запускает прохождение потока
    /// </summary>
    public void Start()
    {
        if (!CanStart())
            throw new InvalidOperationException("Назначение не может быть запущено в текущем состоянии");

        Status = AssignmentStatus.InProgress;
        StartedAt = DateTime.UtcNow;
        LastActivityAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Ставит прохождение на паузу
    /// </summary>
    /// <param name="reason">Причина постановки на паузу</param>
    public void Pause(string reason)
    {
        if (Status != AssignmentStatus.InProgress)
            throw new InvalidOperationException("Только активные назначения могут быть поставлены на паузу");

        Status = AssignmentStatus.Paused;
        PausedAt = DateTime.UtcNow;
        PauseReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Возобновляет прохождение с паузы
    /// </summary>
    public void Resume()
    {
        if (Status != AssignmentStatus.Paused)
            throw new InvalidOperationException("Только приостановленные назначения могут быть возобновлены");

        Status = AssignmentStatus.InProgress;
        PausedAt = null;
        PauseReason = null;
        LastActivityAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Завершает прохождение потока
    /// </summary>
    /// <param name="finalScore">Финальная оценка</param>
    public void Complete(int? finalScore = null)
    {
        if (Status != AssignmentStatus.InProgress)
            throw new InvalidOperationException("Только активные назначения могут быть завершены");

        Status = AssignmentStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        FinalScore = finalScore;
        ProgressPercent = 100;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Отменяет назначение
    /// </summary>
    /// <param name="reason">Причина отмены</param>
    public void Cancel(string reason)
    {
        if (Status == AssignmentStatus.Completed)
            throw new InvalidOperationException("Завершенные назначения не могут быть отменены");

        Status = AssignmentStatus.Cancelled;
        AdminNotes = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Продлевает дедлайн
    /// </summary>
    /// <param name="newDueDate">Новый дедлайн</param>
    public void ExtendDeadline(DateTime newDueDate)
    {
        if (newDueDate <= DateTime.UtcNow)
            throw new ArgumentException("Новый дедлайн должен быть в будущем");

        DueDate = newDueDate;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Обновляет прогресс прохождения
    /// </summary>
    /// <param name="completedSteps">Количество завершенных шагов</param>
    /// <param name="totalSteps">Общее количество шагов</param>
    public void UpdateProgress(int completedSteps, int totalSteps)
    {
        CompletedSteps = completedSteps;
        TotalSteps = totalSteps;
        ProgressPercent = totalSteps > 0 ? (int)Math.Round((double)completedSteps / totalSteps * 100) : 0;
        LastActivityAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Назначает бадди
    /// </summary>
    /// <param name="buddyId">Идентификатор бадди</param>
    public void AssignBuddy(Guid buddyId)
    {
        BuddyId = buddyId;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Проверяет, просрочено ли назначение
    /// </summary>
    /// <returns>true, если просрочено</returns>
    public bool IsOverdue()
    {
        return DueDate.HasValue && 
               DateTime.UtcNow > DueDate.Value && 
               Status == AssignmentStatus.InProgress;
    }

    /// <summary>
    /// Проверяет, приближается ли дедлайн
    /// </summary>
    /// <param name="daysThreshold">Количество дней до дедлайна</param>
    /// <returns>true, если дедлайн приближается</returns>
    public bool IsDeadlineApproaching(int daysThreshold)
    {
        return DueDate.HasValue && 
               Status == AssignmentStatus.InProgress &&
               (DueDate.Value - DateTime.UtcNow).Days <= daysThreshold;
    }

    /// <summary>
    /// Добавляет обратную связь от пользователя
    /// </summary>
    /// <param name="feedback">Текст обратной связи</param>
    /// <param name="rating">Оценка от 1 до 5</param>
    public void AddUserFeedback(string feedback, int? rating = null)
    {
        UserFeedback = feedback;
        if (rating.HasValue && rating >= 1 && rating <= 5)
        {
            UserRating = rating;
        }
        UpdatedAt = DateTime.UtcNow;
    }
}