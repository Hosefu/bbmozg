using Microsoft.Extensions.DependencyInjection;

namespace BuddyBot.Application;

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
        // Пока базовая реализация - полная настройка будет в следующих этапах
        // Здесь будут регистрироваться:
        // - MediatR для CQRS
        // - AutoMapper для маппинга
        // - FluentValidation для валидации
        // - Pipeline behaviors
        // - Event handlers
        
        return services;
    }
}