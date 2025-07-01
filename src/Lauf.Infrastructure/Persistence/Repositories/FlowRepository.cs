using Microsoft.EntityFrameworkCore;
using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Entities.Components;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Domain.Enums;
using Lauf.Infrastructure.Persistence;

namespace Lauf.Infrastructure.Persistence.Repositories;

/// <summary>
/// Репозиторий для работы с потоками
/// </summary>
public class FlowRepository : IFlowRepository
{
    private readonly ApplicationDbContext _context;

    public FlowRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Flow?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var flow = await _context.Flows
            .Include(x => x.Settings)
            .Include(x => x.ActiveContent)
                .ThenInclude(ac => ac.Steps.OrderBy(s => s.Order))
                    .ThenInclude(s => s.Components.OrderBy(c => c.Order))
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        // Загружаем вопросы квизов отдельно
        if (flow?.ActiveContent != null)
        {
            await LoadQuizQuestionsAsync(flow.ActiveContent.Steps, cancellationToken);
        }

        return flow;
    }

    public async Task<Flow?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<Flow?> GetByIdWithStepsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var flow = await _context.Flows
            .Include(x => x.ActiveContent)
                .ThenInclude(ac => ac.Steps.OrderBy(s => s.Order))
                    .ThenInclude(s => s.Components.OrderBy(c => c.Order))
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        // Загружаем вопросы квизов отдельно
        if (flow?.ActiveContent != null)
        {
            await LoadQuizQuestionsAsync(flow.ActiveContent.Steps, cancellationToken);
        }

        return flow;
    }

    public async Task<Flow?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Flows
            .Include(x => x.Settings)
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }

    public async Task<IReadOnlyList<Flow>> GetByStatusAsync(FlowStatus status, int skip = 0, int take = 50, CancellationToken cancellationToken = default)
    {
        return await _context.Flows
            .Include(x => x.Settings)
            .Where(x => x.IsActive == (status == FlowStatus.Published))
            .OrderBy(x => x.Name)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Flow>> GetPublishedAsync(int skip = 0, int take = 50, CancellationToken cancellationToken = default)
    {
        return await _context.Flows
            .Include(x => x.Settings)
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Flow>> GetByCategoryAsync(string category, int skip = 0, int take = 50, CancellationToken cancellationToken = default)
    {
        // Пока категории нет в модели, возвращаем все опубликованные потоки
        return await GetPublishedAsync(skip, take, cancellationToken);
    }

    public async Task<IReadOnlyList<Flow>> GetRequiredFlowsAsync(CancellationToken cancellationToken = default)
    {
        // Пока логики обязательных потоков нет, возвращаем все опубликованные
        return await GetPublishedAsync(cancellationToken: cancellationToken);
    }

    public async Task<int> CountByStatusAsync(FlowStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Flows
            .CountAsync(x => x.IsActive == (status == FlowStatus.Published), cancellationToken);
    }

    public async Task<Flow> AddAsync(Flow flow, CancellationToken cancellationToken = default)
    {
        _context.Flows.Add(flow);
        await _context.SaveChangesAsync(cancellationToken);
        return flow;
    }

    public async Task<Flow> UpdateAsync(Flow flow, CancellationToken cancellationToken = default)
    {
        // Обновляем дату изменения
        flow.UpdatedAt = DateTime.UtcNow;
        
        // Проверяем состояние сущности в контексте
        var entry = _context.Entry(flow);
        if (entry.State == EntityState.Detached)
        {
            // Если сущность не отслеживается, добавляем её в контекст
            _context.Flows.Update(flow);
        }
        
        // Сохраняем изменения
        await _context.SaveChangesAsync(cancellationToken);
        return flow;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var flow = await GetByIdAsync(id, cancellationToken);
        if (flow != null)
        {
            await DeleteAsync(flow, cancellationToken);
        }
    }

    public async Task DeleteAsync(Flow flow, CancellationToken cancellationToken = default)
    {
        _context.Flows.Remove(flow);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Flows.AnyAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Flow>> SearchAsync(string searchTerm, int skip = 0, int take = 50, CancellationToken cancellationToken = default)
    {
        return await _context.Flows
            .Include(x => x.Settings)
            .Where(x => x.Name.Contains(searchTerm) || x.Description.Contains(searchTerm))
            .OrderBy(x => x.Name)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Flows.Where(x => x.Name == name);
        
        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Flow>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Flows
            .Include(f => f.ActiveContent)
                .ThenInclude(ac => ac.Steps)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Flow>> GetAllWithStepsAsync(CancellationToken cancellationToken = default)
    {
        var flows = await _context.Flows
            .Include(f => f.Settings)
            .Include(f => f.ActiveContent)
                .ThenInclude(ac => ac.Steps.OrderBy(s => s.Order))
                    .ThenInclude(s => s.Components.OrderBy(c => c.Order))
            .ToListAsync(cancellationToken);

        // Загружаем вопросы квизов отдельно для всех потоков
        foreach (var flow in flows.Where(f => f.ActiveContent != null))
        {
            await LoadQuizQuestionsAsync(flow.ActiveContent!.Steps, cancellationToken);
        }

        return flows;
    }

    public async Task<Flow?> GetFlowByStepIdAsync(Guid stepId, CancellationToken cancellationToken = default)
    {
        return await _context.Flows
            .Include(f => f.ActiveContent)
                .ThenInclude(ac => ac.Steps.OrderBy(s => s.Order))
                    .ThenInclude(s => s.Components.OrderBy(c => c.Order))
            .Where(f => f.ActiveContent.Steps.Any(s => s.Id == stepId))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<FlowStep> AddStepAsync(Guid flowId, FlowStep step, CancellationToken cancellationToken = default)
    {
        // Начинаем транзакцию
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // Загружаем поток с контентом и шагами
            var flow = await _context.Flows
                .Include(f => f.ActiveContent)
                    .ThenInclude(ac => ac.Steps)
                .FirstOrDefaultAsync(f => f.Id == flowId, cancellationToken);

            if (flow == null)
                throw new InvalidOperationException($"Поток с ID {flowId} не найден");

            if (flow.ActiveContent == null)
                throw new InvalidOperationException($"У потока {flowId} нет активного контента");

            // Добавляем шаг к контенту
            step.FlowContentId = flow.ActiveContent.Id;
            flow.UpdatedAt = DateTime.UtcNow;

            // Добавляем шаг в контекст
            _context.FlowSteps.Add(step);

            // Сохраняем изменения
            await _context.SaveChangesAsync(cancellationToken);
            
            // Коммитим транзакцию
            await transaction.CommitAsync(cancellationToken);

            return step;
        }
        catch
        {
            // Откатываем транзакцию в случае ошибки
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<FlowStep?> GetStepByIdAsync(Guid stepId, CancellationToken cancellationToken = default)
    {
        var step = await _context.FlowSteps
            .Include(s => s.Components.OrderBy(c => c.Order))
            .FirstOrDefaultAsync(s => s.Id == stepId, cancellationToken);

        // Загружаем вопросы квизов отдельно
        if (step != null)
        {
            await LoadQuizQuestionsAsync(new[] { step }, cancellationToken);
        }

        return step;
    }

    public async Task<FlowStep?> GetStepByComponentIdAsync(Guid componentId, CancellationToken cancellationToken = default)
    {
        var step = await _context.FlowSteps
            .Include(s => s.Components.OrderBy(c => c.Order))
            .FirstOrDefaultAsync(s => s.Components.Any(c => c.Id == componentId), cancellationToken);

        // Загружаем вопросы квизов отдельно
        if (step != null)
        {
            await LoadQuizQuestionsAsync(new[] { step }, cancellationToken);
        }

        return step;
    }

    /// <summary>
    /// Загружает вопросы и варианты ответов для всех квиз компонентов в шагах
    /// </summary>
    private async Task LoadQuizQuestionsAsync(IEnumerable<FlowStep> steps, CancellationToken cancellationToken)
    {
        var quizComponentIds = steps
            .SelectMany(s => s.Components)
            .OfType<QuizComponent>()
            .Select(q => q.Id)
            .ToList();

        if (quizComponentIds.Any())
        {
            var questions = await _context.Set<QuizQuestion>()
                .Include(q => q.Options.OrderBy(o => o.Order))
                .Where(q => quizComponentIds.Contains(q.QuizComponentId))
                .OrderBy(q => q.Order)
                .ToListAsync(cancellationToken);

            // Группируем вопросы по квиз компонентам
            var questionsByQuiz = questions.GroupBy(q => q.QuizComponentId).ToDictionary(g => g.Key, g => g.ToList());

            // Присваиваем вопросы соответствующим квиз компонентам
            foreach (var step in steps)
            {
                foreach (var quizComponent in step.Components.OfType<QuizComponent>())
                {
                    if (questionsByQuiz.TryGetValue(quizComponent.Id, out var quizQuestions))
                    {
                        // Очищаем существующие вопросы и добавляем загруженные
                        quizComponent.Questions.Clear();
                        foreach (var question in quizQuestions)
                        {
                            quizComponent.Questions.Add(question);
                        }
                    }
                }
            }
        }
    }
}