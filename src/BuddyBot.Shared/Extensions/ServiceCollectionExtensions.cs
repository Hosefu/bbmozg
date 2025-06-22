using Microsoft.Extensions.DependencyInjection;

namespace BuddyBot.Shared.Extensions;

/// <summary>
/// Методы расширения для IServiceCollection, предоставляющие удобные способы 
/// регистрации общих сервисов в DI контейнере.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Регистрирует общие shared сервисы в DI контейнере.
    /// Включает вспомогательные классы и утилиты, используемые во всех слоях приложения.
    /// </summary>
    /// <param name="services">Коллекция сервисов для регистрации</param>
    /// <returns>Коллекцию сервисов для цепочки вызовов</returns>
    public static IServiceCollection AddSharedServices(this IServiceCollection services)
    {
        // Пока пустая реализация - будет заполнена в следующих этапах
        // Здесь будут регистрироваться общие утилиты, помощники и константы
        
        return services;
    }
}