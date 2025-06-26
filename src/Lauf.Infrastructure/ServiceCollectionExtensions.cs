using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Lauf.Infrastructure.Persistence;
using Lauf.Infrastructure.Persistence.Repositories;
using Lauf.Infrastructure.Persistence.Interceptors;
using Lauf.Infrastructure.Services;
using Lauf.Infrastructure.ExternalServices.Cache;
using Lauf.Infrastructure.ExternalServices.FileStorage;
using Lauf.Infrastructure.ExternalServices.BackgroundJobs;
using Lauf.Domain.Interfaces;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Application.Services.Interfaces;
using Lauf.Domain.Interfaces.ExternalServices;
using Lauf.Domain.Services;

namespace Lauf.Infrastructure;

/// <summary>
/// Методы расширения для регистрации сервисов Infrastructure слоя в DI контейнере.
/// Настраивает базу данных, внешние сервисы, кэширование и другие инфраструктурные компоненты.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Регистрирует все сервисы Infrastructure слоя, включая Entity Framework,
    /// Redis, внешние API, файловое хранилище и другие инфраструктурные зависимости.
    /// </summary>
    /// <param name="services">Коллекция сервисов для регистрации</param>
    /// <param name="configuration">Конфигурация приложения для получения строк подключения</param>
    /// <returns>Коллекцию сервисов для цепочки вызовов</returns>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Entity Framework DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
        });

        // Interceptors
        services.AddScoped<AuditInterceptor>();
        services.AddScoped<DomainEventInterceptor>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositories
        services.AddScoped<IUserRepository, SimpleUserRepository>();
        services.AddScoped<IFlowRepository, FlowRepository>();
        services.AddScoped<IFlowAssignmentRepository, FlowAssignmentRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IAchievementRepository, AchievementRepository>();
        services.AddScoped<IUserAchievementRepository, UserAchievementRepository>();
        services.AddScoped<IUserProgressRepository, UserProgressRepository>();

        // Domain services
        services.AddScoped<FlowSnapshotService>();
        services.AddScoped<ProgressCalculationService>();

        // Infrastructure services
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IDateTimeService, DateTimeService>();

        // External services
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddMemoryCache();
        services.AddScoped<ICacheService, InMemoryCacheService>();
        services.AddScoped<IBackgroundJobService, MemoryBackgroundJobService>();

        // Telegram services
        services.AddHttpClient<TelegramNotificationService>();
        services.AddScoped<TelegramNotificationService>();
        services.AddScoped<ExternalServices.TelegramBot.TelegramWebhookHandler>();

        return services;
    }
}