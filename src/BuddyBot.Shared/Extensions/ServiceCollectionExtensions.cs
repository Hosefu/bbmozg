using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace BuddyBot.Shared.Extensions;

/// <summary>
/// Методы расширения для IServiceCollection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Регистрирует все общие сервисы Shared библиотеки
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="configuration">Конфигурация приложения</param>
    /// <returns>Коллекция сервисов для цепочки вызовов</returns>
    public static IServiceCollection AddSharedServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Здесь будут регистрироваться общие сервисы и утилиты
        // После создания хелперов мы добавим их регистрацию
        
        return services;
    }
    
    /// <summary>
    /// Регистрирует сервис с проверкой на null
    /// </summary>
    /// <typeparam name="TInterface">Интерфейс сервиса</typeparam>
    /// <typeparam name="TImplementation">Реализация сервиса</typeparam>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="lifetime">Время жизни сервиса</param>
    /// <returns>Коллекция сервисов для цепочки вызовов</returns>
    public static IServiceCollection AddServiceSafe<TInterface, TImplementation>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        return lifetime switch
        {
            ServiceLifetime.Singleton => services.AddSingleton<TInterface, TImplementation>(),
            ServiceLifetime.Transient => services.AddTransient<TInterface, TImplementation>(),
            _ => services.AddScoped<TInterface, TImplementation>()
        };
    }
    
    /// <summary>
    /// Регистрирует сервис только если он еще не зарегистрирован
    /// </summary>
    /// <typeparam name="TInterface">Интерфейс сервиса</typeparam>
    /// <typeparam name="TImplementation">Реализация сервиса</typeparam>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="lifetime">Время жизни сервиса</param>
    /// <returns>Коллекция сервисов для цепочки вызовов</returns>
    public static IServiceCollection TryAddService<TInterface, TImplementation>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        if (services.Any(s => s.ServiceType == typeof(TInterface)))
            return services;
            
        return services.AddServiceSafe<TInterface, TImplementation>(lifetime);
    }
    
    /// <summary>
    /// Регистрирует все реализации интерфейса из указанной сборки
    /// </summary>
    /// <typeparam name="TInterface">Базовый интерфейс</typeparam>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="assembly">Сборка для поиска реализаций</param>
    /// <param name="lifetime">Время жизни сервисов</param>
    /// <returns>Коллекция сервисов для цепочки вызовов</returns>
    public static IServiceCollection AddAllImplementations<TInterface>(
        this IServiceCollection services,
        System.Reflection.Assembly assembly,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TInterface : class
    {
        var interfaceType = typeof(TInterface);
        var implementations = assembly.GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract && interfaceType.IsAssignableFrom(type))
            .ToList();
            
        foreach (var implementation in implementations)
        {
            switch (lifetime)
            {
                case ServiceLifetime.Singleton:
                    services.AddSingleton(interfaceType, implementation);
                    break;
                case ServiceLifetime.Transient:
                    services.AddTransient(interfaceType, implementation);
                    break;
                default:
                    services.AddScoped(interfaceType, implementation);
                    break;
            }
        }
        
        return services;
    }
    
    /// <summary>
    /// Регистрирует декоратор для сервиса
    /// </summary>
    /// <typeparam name="TInterface">Интерфейс сервиса</typeparam>
    /// <typeparam name="TDecorator">Декоратор</typeparam>
    /// <param name="services">Коллекция сервисов</param>
    /// <returns>Коллекция сервисов для цепочки вызовов</returns>
    public static IServiceCollection AddDecorator<TInterface, TDecorator>(this IServiceCollection services)
        where TInterface : class
        where TDecorator : class, TInterface
    {
        var originalDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(TInterface));
        if (originalDescriptor == null)
            throw new InvalidOperationException($"Сервис {typeof(TInterface).Name} не найден для декорирования");
            
        services.Remove(originalDescriptor);
        
        if (originalDescriptor.ImplementationInstance != null)
        {
            services.AddSingleton<TInterface>(provider =>
            {
                var decoratorInstance = ActivatorUtilities.CreateInstance<TDecorator>(provider, originalDescriptor.ImplementationInstance);
                return decoratorInstance;
            });
        }
        else if (originalDescriptor.ImplementationFactory != null)
        {
            services.Add(ServiceDescriptor.Describe(typeof(TInterface), provider =>
            {
                var originalInstance = originalDescriptor.ImplementationFactory(provider);
                return ActivatorUtilities.CreateInstance<TDecorator>(provider, originalInstance);
            }, originalDescriptor.Lifetime));
        }
        else if (originalDescriptor.ImplementationType != null)
        {
            services.Add(ServiceDescriptor.Describe(originalDescriptor.ImplementationType, originalDescriptor.ImplementationType, originalDescriptor.Lifetime));
            services.Add(ServiceDescriptor.Describe(typeof(TInterface), provider =>
            {
                var originalInstance = provider.GetRequiredService(originalDescriptor.ImplementationType);
                return ActivatorUtilities.CreateInstance<TDecorator>(provider, originalInstance);
            }, originalDescriptor.Lifetime));
        }
        
        return services;
    }
    
    /// <summary>
    /// Получает значение конфигурации с проверкой на null
    /// </summary>
    /// <param name="configuration">Конфигурация</param>
    /// <param name="key">Ключ настройки</param>
    /// <param name="defaultValue">Значение по умолчанию</param>
    /// <returns>Значение настройки</returns>
    public static string GetRequiredValue(this IConfiguration configuration, string key, string? defaultValue = null)
    {
        var value = configuration[key];
        if (string.IsNullOrWhiteSpace(value))
        {
            if (defaultValue != null)
                return defaultValue;
            throw new InvalidOperationException($"Не найдена обязательная настройка: {key}");
        }
        return value;
    }
    
    /// <summary>
    /// Получает строку подключения с проверкой на null
    /// </summary>
    /// <param name="configuration">Конфигурация</param>
    /// <param name="name">Имя строки подключения</param>
    /// <returns>Строка подключения</returns>
    public static string GetRequiredConnectionString(this IConfiguration configuration, string name)
    {
        var connectionString = configuration.GetConnectionString(name);
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException($"Не найдена строка подключения: {name}");
        return connectionString;
    }
    
    /// <summary>
    /// Проверяет, зарегистрирован ли сервис
    /// </summary>
    /// <typeparam name="T">Тип сервиса</typeparam>
    /// <param name="services">Коллекция сервисов</param>
    /// <returns>true, если сервис зарегистрирован</returns>
    public static bool IsRegistered<T>(this IServiceCollection services)
    {
        return services.Any(s => s.ServiceType == typeof(T));
    }
    
    /// <summary>
    /// Проверяет, зарегистрирован ли сервис по типу
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="serviceType">Тип сервиса</param>
    /// <returns>true, если сервис зарегистрирован</returns>
    public static bool IsRegistered(this IServiceCollection services, Type serviceType)
    {
        return services.Any(s => s.ServiceType == serviceType);
    }
    
    /// <summary>
    /// Удаляет все регистрации указанного типа
    /// </summary>
    /// <typeparam name="T">Тип сервиса</typeparam>
    /// <param name="services">Коллекция сервисов</param>
    /// <returns>Коллекция сервисов для цепочки вызовов</returns>
    public static IServiceCollection RemoveAll<T>(this IServiceCollection services)
    {
        var descriptors = services.Where(s => s.ServiceType == typeof(T)).ToList();
        foreach (var descriptor in descriptors)
        {
            services.Remove(descriptor);
        }
        return services;
    }
    
    /// <summary>
    /// Заменяет регистрацию сервиса
    /// </summary>
    /// <typeparam name="TInterface">Интерфейс сервиса</typeparam>
    /// <typeparam name="TImplementation">Новая реализация</typeparam>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="lifetime">Время жизни сервиса</param>
    /// <returns>Коллекция сервисов для цепочки вызовов</returns>
    public static IServiceCollection Replace<TInterface, TImplementation>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        services.RemoveAll<TInterface>();
        return services.AddServiceSafe<TInterface, TImplementation>(lifetime);
    }
}