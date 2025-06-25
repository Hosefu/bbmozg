using Lauf.Domain.Entities.Progress;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Domain.ValueObjects;
using Lauf.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Lauf.Infrastructure.Persistence.Repositories;

/// <summary>
/// Репозиторий для работы с прогрессом пользователей
/// </summary>
public class UserProgressRepository : IUserProgressRepository
{
    private readonly ApplicationDbContext _context;

    public UserProgressRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserProgress?> GetByAssignmentIdAsync(Guid assignmentId, CancellationToken cancellationToken = default)
    {
        // Получаем назначение для вычисления прогресса
        var assignment = await _context.FlowAssignments
            .Include(a => a.User)
            .Include(a => a.Flow)
            .FirstOrDefaultAsync(a => a.Id == assignmentId, cancellationToken);

        if (assignment == null)
            return null;

        // Создаем объект прогресса на основе назначения
        var progress = new UserProgress(assignment.UserId);
        
        // Вычисляем базовый прогресс
        decimal progressValue = 0;
        
        switch (assignment.Status)
        {
            case Domain.Enums.AssignmentStatus.Assigned:
                progressValue = 0;
                break;
            case Domain.Enums.AssignmentStatus.InProgress:
                progressValue = 50; // 50% за начало работы
                break;
            case Domain.Enums.AssignmentStatus.Completed:
                progressValue = 100;
                break;
            default:
                progressValue = 0;
                break;
        }

        // Устанавливаем значения через рефлексию или создаем новый объект
        var userProgress = new UserProgress(assignment.UserId);
        
        // Поскольку в сущности нет публичных сеттеров, создаем простую реализацию
        return new SimpleUserProgress
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignmentId,
            UserId = assignment.UserId,
            OverallProgress = new ProgressPercentage(progressValue),
            CreatedAt = assignment.CreatedAt,
            UpdatedAt = assignment.UpdatedAt
        };
    }

    public async Task<IEnumerable<UserProgress>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // Получаем все назначения пользователя
        var assignments = await _context.FlowAssignments
            .Where(a => a.UserId == userId)
            .ToListAsync(cancellationToken);

        var progressList = new List<UserProgress>();

        foreach (var assignment in assignments)
        {
            var progress = await GetByAssignmentIdAsync(assignment.Id, cancellationToken);
            if (progress != null)
            {
                progressList.Add(progress);
            }
        }

        return progressList;
    }

    public async Task<IEnumerable<UserProgress>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // Получаем активные назначения пользователя
        var assignments = await _context.FlowAssignments
            .Where(a => a.UserId == userId && 
                       (a.Status == Domain.Enums.AssignmentStatus.Assigned || 
                        a.Status == Domain.Enums.AssignmentStatus.InProgress))
            .ToListAsync(cancellationToken);

        var progressList = new List<UserProgress>();

        foreach (var assignment in assignments)
        {
            var progress = await GetByAssignmentIdAsync(assignment.Id, cancellationToken);
            if (progress != null)
            {
                progressList.Add(progress);
            }
        }

        return progressList;
    }

    // Методы для работы с FlowProgress
    public async Task<FlowProgress?> GetFlowProgressByAssignmentIdAsync(Guid assignmentId, CancellationToken cancellationToken = default)
    {
        var assignment = await _context.FlowAssignments
            .Include(a => a.Flow)
            .FirstOrDefaultAsync(a => a.Id == assignmentId, cancellationToken);

        if (assignment == null)
            return null;

        var flowProgress = new FlowProgress(
            assignmentId,
            assignment.FlowId,
            assignment.UserId,
            0, // completedStepsCount
            assignment.TotalSteps); // totalStepsCount

        return flowProgress;
    }

    public async Task AddFlowProgressAsync(FlowProgress flowProgress, CancellationToken cancellationToken = default)
    {
        // В текущей реализации FlowProgress не сохраняется отдельно
        // Прогресс вычисляется на основе FlowAssignment
        await Task.CompletedTask;
    }

    public async Task UpdateFlowProgressAsync(FlowProgress flowProgress, CancellationToken cancellationToken = default)
    {
        // В текущей реализации FlowProgress не сохраняется отдельно
        // Прогресс вычисляется на основе FlowAssignment
        await Task.CompletedTask;
    }

    // Методы для работы с ComponentProgress
    public async Task<ComponentProgress?> GetComponentProgressAsync(Guid assignmentId, Guid componentId, CancellationToken cancellationToken = default)
    {
        // Базовая реализация - возвращаем новый прогресс компонента
        var componentProgress = new ComponentProgress(
            assignmentId,
            componentId,
            0, // order
            false); // isRequired

        return componentProgress;
    }

    public async Task UpdateComponentProgressAsync(ComponentProgress componentProgress, CancellationToken cancellationToken = default)
    {
        // В текущей реализации ComponentProgress не сохраняется отдельно
        await Task.CompletedTask;
    }

    public async Task UpdateStepProgressAsync(StepProgress stepProgress, CancellationToken cancellationToken = default)
    {
        // В текущей реализации StepProgress не сохраняется отдельно
        await Task.CompletedTask;
    }

    // Методы для работы с UserProgress
    public async Task<UserProgress?> GetUserProgressAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var assignments = await _context.FlowAssignments
            .Where(a => a.UserId == userId)
            .ToListAsync(cancellationToken);

        if (!assignments.Any())
            return null;

        // Создаем агрегированный прогресс пользователя
        var totalProgress = assignments.Average(a => a.ProgressPercent);
        
        return new SimpleUserProgress
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            OverallProgress = new ProgressPercentage((decimal)totalProgress),
            CreatedAt = assignments.Min(a => a.CreatedAt),
            UpdatedAt = assignments.Max(a => a.UpdatedAt)
        };
    }

    public async Task AddUserProgressAsync(UserProgress userProgress, CancellationToken cancellationToken = default)
    {
        // В текущей реализации UserProgress не сохраняется отдельно
        await Task.CompletedTask;
    }

    public async Task UpdateUserProgressAsync(UserProgress userProgress, CancellationToken cancellationToken = default)
    {
        // В текущей реализации UserProgress не сохраняется отдельно
        await Task.CompletedTask;
    }
}

/// <summary>
/// Простая реализация UserProgress для совместимости
/// </summary>
public class SimpleUserProgress : UserProgress
{
    public new Guid Id { get; set; }
    public Guid AssignmentId { get; set; }
    public new Guid UserId { get; set; }
    public new ProgressPercentage OverallProgress { get; set; }
    public DateTime CreatedAt { get; set; }
    public new DateTime UpdatedAt { get; set; }

    public SimpleUserProgress() : base(Guid.Empty)
    {
        Id = Guid.NewGuid();
        OverallProgress = new ProgressPercentage(0);
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}