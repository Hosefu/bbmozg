using Microsoft.EntityFrameworkCore;
using Lauf.Domain.Entities.Components;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Infrastructure.Persistence;

namespace Lauf.Infrastructure.Persistence.Repositories;

/// <summary>
/// Репозиторий для работы с компонентами
/// </summary>
public class ComponentRepository : IComponentRepository
{
    private readonly ApplicationDbContext _context;

    public ComponentRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ArticleComponent?> GetArticleComponentByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ArticleComponents
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<QuizComponent?> GetQuizComponentByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.QuizComponents
            .Include(x => x.Questions)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<TaskComponent?> GetTaskComponentByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.TaskComponents
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<ArticleComponent> AddArticleComponentAsync(ArticleComponent component, CancellationToken cancellationToken = default)
    {
        _context.ArticleComponents.Add(component);
        await _context.SaveChangesAsync(cancellationToken);
        return component;
    }

    public async Task<QuizComponent> AddQuizComponentAsync(QuizComponent component, CancellationToken cancellationToken = default)
    {
        _context.QuizComponents.Add(component);
        await _context.SaveChangesAsync(cancellationToken);
        return component;
    }

    public async Task<TaskComponent> AddTaskComponentAsync(TaskComponent component, CancellationToken cancellationToken = default)
    {
        _context.TaskComponents.Add(component);
        await _context.SaveChangesAsync(cancellationToken);
        return component;
    }


    public async Task<ArticleComponent> UpdateArticleComponentAsync(ArticleComponent component, CancellationToken cancellationToken = default)
    {
        _context.ArticleComponents.Update(component);
        await _context.SaveChangesAsync(cancellationToken);
        return component;
    }

    public async Task<QuizComponent> UpdateQuizComponentAsync(QuizComponent component, CancellationToken cancellationToken = default)
    {
        _context.QuizComponents.Update(component);
        await _context.SaveChangesAsync(cancellationToken);
        return component;
    }

    public async Task<TaskComponent> UpdateTaskComponentAsync(TaskComponent component, CancellationToken cancellationToken = default)
    {
        _context.TaskComponents.Update(component);
        await _context.SaveChangesAsync(cancellationToken);
        return component;
    }

    public async Task DeleteArticleComponentAsync(ArticleComponent component, CancellationToken cancellationToken = default)
    {
        _context.ArticleComponents.Remove(component);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteQuizComponentAsync(QuizComponent component, CancellationToken cancellationToken = default)
    {
        _context.QuizComponents.Remove(component);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteTaskComponentAsync(TaskComponent component, CancellationToken cancellationToken = default)
    {
        _context.TaskComponents.Remove(component);
        await _context.SaveChangesAsync(cancellationToken);
    }
} 