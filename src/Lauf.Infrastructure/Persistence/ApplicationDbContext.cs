using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Lauf.Domain.Entities.Users;
using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Entities.Progress;
using Lauf.Domain.Entities.Notifications;
using Lauf.Domain.Entities.Components;
using Lauf.Domain.ValueObjects;
using Lauf.Infrastructure.Persistence.Configurations;
using Lauf.Infrastructure.Persistence.Interceptors;
using Lauf.Infrastructure.Persistence.Seeds;

namespace Lauf.Infrastructure.Persistence;

/// <summary>
/// Контекст базы данных приложения
/// </summary>
public class ApplicationDbContext : DbContext
{
    private readonly AuditInterceptor _auditInterceptor;
    private readonly DomainEventInterceptor _domainEventInterceptor;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        AuditInterceptor auditInterceptor,
        DomainEventInterceptor domainEventInterceptor) : base(options)
    {
        _auditInterceptor = auditInterceptor;
        _domainEventInterceptor = domainEventInterceptor;
    }

    /// <summary>
    /// Конструктор для миграций без перехватчиков
    /// </summary>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        _auditInterceptor = null!;
        _domainEventInterceptor = null!;
    }

    // Users
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<Achievement> Achievements { get; set; } = null!;
    public DbSet<UserAchievement> UserAchievements { get; set; } = null!;

    // Flows
    public DbSet<Flow> Flows { get; set; } = null!;
    public DbSet<FlowContent> FlowContents { get; set; } = null!;
    public DbSet<FlowStep> FlowSteps { get; set; } = null!;
    public DbSet<FlowSettings> FlowSettings { get; set; } = null!;
    public DbSet<FlowAssignment> FlowAssignments { get; set; } = null!;
    public DbSet<FlowAssignmentProgress> FlowAssignmentProgress { get; set; } = null!;


    // Progress
    public DbSet<UserProgress> UserProgress { get; set; } = null!;

    // Notifications
    public DbSet<Notification> Notifications { get; set; } = null!;

    // Components (TPT)
    public DbSet<ComponentBase> Components { get; set; } = null!;
    public DbSet<ArticleComponent> ArticleComponents { get; set; } = null!;
    public DbSet<QuizComponent> QuizComponents { get; set; } = null!;
    public DbSet<TaskComponent> TaskComponents { get; set; } = null!;
    public DbSet<QuizQuestion> QuizQuestions { get; set; } = null!;
    public DbSet<QuestionOption> QuestionOptions { get; set; } = null!;

    /// <summary>
    /// Настройка модели базы данных
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Игнорируем value objects как отдельные entities - они используются как owned entities
        modelBuilder.Ignore<ProgressPercentage>();

        // Применяем все конфигурации из текущей сборки, кроме дублирующихся
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        
        // Исключаем FlowContentConfiguration из автоматического применения
        // так как связи настроены в SimpleFlowConfiguration
    }

    /// <summary>
    /// Настройка перехватчиков
    /// </summary>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_auditInterceptor != null)
        {
            optionsBuilder.AddInterceptors(_auditInterceptor);
        }

        if (_domainEventInterceptor != null)
        {
            optionsBuilder.AddInterceptors(_domainEventInterceptor);
        }

        base.OnConfiguring(optionsBuilder);
    }

    /// <summary>
    /// Начало транзакции
    /// </summary>
    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return await Database.BeginTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// Подтверждение транзакции
    /// </summary>
    public async Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            await transaction.DisposeAsync();
        }
    }

    /// <summary>
    /// Инициализация seed данных
    /// </summary>
    public async Task SeedDataAsync()
    {
        try
        {
            // Seeding достижений
            await AchievementSeeder.SeedAsync(this);
            
            // Seeding демонстрационного потока дизайн-студии
            await DesignStudioFlowSeeder.SeedAsync(this);
        }
        catch (Exception)
        {
            // Логируем ошибку, но не прерываем работу приложения
            // В production здесь должно быть более детальное логирование
            throw;
        }
    }
}