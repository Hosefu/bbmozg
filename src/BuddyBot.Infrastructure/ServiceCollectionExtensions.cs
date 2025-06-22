using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuddyBot.Infrastructure;

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
        // Пока базовая реализация - полная настройка будет в следующих этапах
        // Здесь будут регистрироваться:
        // - Entity Framework DbContext
        // - Repositories
        // - Redis кэширование
        // - Telegram Bot сервисы
        // - Файловое хранилище
        // - Email сервисы
        // - Background jobs (Hangfire)
        // - Authentication/Authorization сервисы
        
        return services;
    }
}