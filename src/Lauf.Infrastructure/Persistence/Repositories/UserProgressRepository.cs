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
}

/// <summary>
/// Простая реализация UserProgress для совместимости
/// </summary>
public class SimpleUserProgress : UserProgress
{
    public Guid Id { get; set; }
    public Guid AssignmentId { get; set; }
    public Guid UserId { get; set; }
    public ProgressPercentage OverallProgress { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public SimpleUserProgress() : base(Guid.Empty)
    {
        Id = Guid.NewGuid();
        OverallProgress = new ProgressPercentage(0);
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}