using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;
using MediatR;
using Lauf.Application.Behaviors;
using Lauf.Application.Services;
using Lauf.Domain.Interfaces.Services;

namespace Lauf.Application;

/// <summary>
/// Методы расширения для регистрации сервисов Application слоя в DI контейнере.
/// Настраивает MediatR, AutoMapper, FluentValidation и другие компоненты слоя приложения.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Регистрирует все сервисы Application слоя, включая CQRS паттерн через MediatR,
    /// маппинг объектов через AutoMapper и валидацию через FluentValidation.
    /// </summary>
    /// <param name="services">Коллекция сервисов для регистрации</param>
    /// <returns>Коллекцию сервисов для цепочки вызовов</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR для CQRS паттерна
        services.AddMediatR(assembly);

        // AutoMapper для маппинга между слоями
        services.AddAutoMapper(assembly);

        // FluentValidation для валидации команд и запросов
        services.AddValidatorsFromAssembly(assembly);

        // Pipeline behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // Application сервисы
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<Services.AchievementCalculationService>();

        // Background Jobs
        services.AddScoped<BackgroundJobs.DailyReminderJob>();
        services.AddScoped<BackgroundJobs.DeadlineCheckJob>();

        return services;
    }
}