using Lauf.Domain.Entities.Users;
using Lauf.Domain.Enums;

namespace Lauf.Domain.Entities.Flows;

/// <summary>
/// Назначение потока пользователю
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
    /// Идентификатор потока-координатора
    /// </summary>
    public Guid FlowId { get; set; }

    /// <summary>
    /// Идентификатор конкретной версии контента
    /// </summary>
    public Guid FlowContentId { get; set; }

    /// <summary>
    /// Идентификатор назначившего (админ или бадди)
    /// </summary>
    public Guid AssignedBy { get; set; }

    /// <summary>
    /// Статус назначения
    /// </summary>
    public AssignmentStatus Status { get; set; } = AssignmentStatus.Assigned;

    /// <summary>
    /// Дата назначения
    /// </summary>
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Пользователь, которому назначен поток
    /// </summary>
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// Поток-координатор
    /// </summary>
    public virtual Flow Flow { get; set; } = null!;

    /// <summary>
    /// Конкретная версия контента
    /// </summary>
    public virtual FlowContent FlowContent { get; set; } = null!;

    /// <summary>
    /// Пользователь, который назначил
    /// </summary>
    public virtual User AssignedByUser { get; set; } = null!;

    /// <summary>
    /// Бадди (может быть несколько) - новая архитектура
    /// </summary>
    public virtual ICollection<User> Buddies { get; set; } = new List<User>();

    /// <summary>
    /// Вычисляемый дедлайн назначения (на основе FlowSettings)
    /// </summary>
    public DateTime Deadline 
    { 
        get 
        {
            // Простая логика: 7 дней по умолчанию на весь поток
            var daysPerStep = Flow?.Settings?.DaysPerStep ?? 7;
            var totalSteps = FlowContent?.Steps?.Count ?? 1;
            return AssignedAt.AddDays(daysPerStep * totalSteps);
        } 
    }

    /// <summary>
    /// Дата завершения назначения (если завершено)
    /// </summary>
    public DateTime? CompletedAt => Progress?.CompletedAt;

    /// <summary>
    /// Первый бадди (для обратной совместимости)
    /// </summary>
    public Guid? Buddy => Buddies?.FirstOrDefault()?.Id;

    /// <summary>
    /// Прогресс назначения
    /// </summary>
    public virtual FlowAssignmentProgress Progress { get; set; } = null!;

    /// <summary>
    /// Конструктор для создания нового назначения потока
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="flowId">Идентификатор потока</param>
    /// <param name="flowContentId">Идентификатор версии контента</param>
    /// <param name="assignedBy">Идентификатор назначившего</param>
    public FlowAssignment(Guid userId, Guid flowId, Guid flowContentId, Guid assignedBy)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        FlowId = flowId;
        FlowContentId = flowContentId;
        AssignedBy = assignedBy;
        Status = AssignmentStatus.Assigned;
        AssignedAt = DateTime.UtcNow;
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
        return Status == AssignmentStatus.Assigned;
    }

    /// <summary>
    /// Запускает прохождение потока
    /// </summary>
    public void Start()
    {
        if (!CanStart())
            throw new InvalidOperationException("Назначение не может быть запущено в текущем состоянии");

        Status = AssignmentStatus.InProgress;
        Progress.Start();
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
        Progress.Pause(reason);
    }

    /// <summary>
    /// Возобновляет прохождение с паузы
    /// </summary>
    public void Resume()
    {
        if (Status != AssignmentStatus.Paused)
            throw new InvalidOperationException("Только приостановленные назначения могут быть возобновлены");

        Status = AssignmentStatus.InProgress;
        Progress.Resume();
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
        Progress.Complete(finalScore);
    }

    /// <summary>
    /// Отменяет назначение
    /// </summary>
    public void Cancel()
    {
        if (Status == AssignmentStatus.Completed)
            throw new InvalidOperationException("Завершенные назначения не могут быть отменены");

        Status = AssignmentStatus.Cancelled;
    }

    /// <summary>
    /// Добавляет бадди
    /// </summary>
    /// <param name="buddy">Пользователь-бадди</param>
    public void AddBuddy(User buddy)
    {
        if (!Buddies.Contains(buddy))
        {
            Buddies.Add(buddy);
        }
    }

    /// <summary>
    /// Удаляет бадди
    /// </summary>
    /// <param name="buddy">Пользователь-бадди</param>
    public void RemoveBuddy(User buddy)
    {
        Buddies.Remove(buddy);
    }
}