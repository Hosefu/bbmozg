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
        
        // В новой архитектуре прогресс рассчитывается через FlowAssignmentProgress

        // Обновляем прогресс на основе статуса назначения
        var allAssignments = await _context.FlowAssignments
            .Where(a => a.UserId == assignment.UserId)
            .ToListAsync(cancellationToken);

        var assignedCount = allAssignments.Count;
        var completedCount = allAssignments.Count(a => a.Status == Domain.Enums.AssignmentStatus.Completed);
        var activeCount = allAssignments.Count(a => a.Status == Domain.Enums.AssignmentStatus.InProgress);
        var overdueCount = allAssignments.Count(a => a.Deadline < DateTime.UtcNow && a.Status != Domain.Enums.AssignmentStatus.Completed);

        progress.RecalculateProgress(assignedCount, completedCount, activeCount, overdueCount);

        return progress;
    }

    public async Task<IEnumerable<UserProgress>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // Получаем все назначения пользователя для вычисления общего прогресса
        var assignments = await _context.FlowAssignments
            .Where(a => a.UserId == userId)
            .ToListAsync(cancellationToken);

        if (!assignments.Any())
            return Enumerable.Empty<UserProgress>();

        var progressList = new List<UserProgress>();
        
        // Создаем прогресс для каждого назначения
        foreach (var assignment in assignments)
        {
            var progress = new UserProgress(userId);
            
            var allUserAssignments = assignments;
            var assignedCount = allUserAssignments.Count;
            var completedCount = allUserAssignments.Count(a => a.Status == Domain.Enums.AssignmentStatus.Completed);
            var activeCount = allUserAssignments.Count(a => a.Status == Domain.Enums.AssignmentStatus.InProgress);
            var overdueCount = allUserAssignments.Count(a => a.Deadline < DateTime.UtcNow && a.Status != Domain.Enums.AssignmentStatus.Completed);

            progress.RecalculateProgress(assignedCount, completedCount, activeCount, overdueCount);
            progressList.Add(progress);
        }

        return progressList;
    }

    public async Task<IEnumerable<UserProgress>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // Получаем только активные назначения пользователя
        var activeAssignments = await _context.FlowAssignments
            .Where(a => a.UserId == userId && a.Status == Domain.Enums.AssignmentStatus.InProgress)
            .ToListAsync(cancellationToken);

        if (!activeAssignments.Any())
            return Enumerable.Empty<UserProgress>();

        var progressList = new List<UserProgress>();
        
        foreach (var assignment in activeAssignments)
        {
            var progress = new UserProgress(userId);
            progress.RecalculateProgress(1, 0, 1, 0); // Активное назначение
            progressList.Add(progress);
        }

        return progressList;
    }

    public async Task AddUserProgressAsync(UserProgress userProgress, CancellationToken cancellationToken = default)
    {
        // UserProgress не сохраняется физически в БД, а вычисляется на лету
        await Task.CompletedTask;
    }

    public async Task UpdateUserProgressAsync(UserProgress userProgress, CancellationToken cancellationToken = default)
    {
        // UserProgress не сохраняется физически в БД, а вычисляется на лету
        await Task.CompletedTask;
    }

    public async Task<UserProgress> AddAsync(UserProgress userProgress, CancellationToken cancellationToken = default)
    {
        // UserProgress не сохраняется физически в БД, а вычисляется на лету
        await Task.CompletedTask;
        return userProgress;
    }

    public async Task<UserProgress> UpdateAsync(UserProgress userProgress, CancellationToken cancellationToken = default)
    {
        // UserProgress не сохраняется физически в БД, а вычисляется на лету
        await Task.CompletedTask;
        return userProgress;
    }

    public async Task DeleteAsync(UserProgress userProgress, CancellationToken cancellationToken = default)
    {
        // UserProgress не сохраняется физически в БД
        await Task.CompletedTask;
    }

    public async Task<List<UserProgress>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // Получаем всех пользователей с назначениями
        var userIds = await _context.FlowAssignments
            .Select(a => a.UserId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var progressList = new List<UserProgress>();

        foreach (var userId in userIds)
        {
            var userProgress = await GetByUserIdAsync(userId, cancellationToken);
            if (userProgress != null && userProgress.Any())
            {
                progressList.AddRange(userProgress);
            }
        }

        return progressList;
    }
}