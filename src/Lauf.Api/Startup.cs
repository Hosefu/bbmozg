using Lauf.Shared.Extensions;

namespace Lauf.Api;

/// <summary>
/// Класс конфигурации приложения. Настраивает сервисы и middleware pipeline
/// для ASP.NET Core приложения согласно принципам Clean Architecture.
/// </summary>
public class Startup
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Инициализирует новый экземпляр класса Startup
    /// </summary>
    /// <param name="configuration">Конфигурация приложения</param>
    public Startup(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Метод конфигурации сервисов. Вызывается runtime для добавления сервисов в DI контейнер.
    /// Настраивает все необходимые компоненты: GraphQL, аутентификацию, базу данных и т.д.
    /// </summary>
    /// <param name="services">Коллекция сервисов для регистрации</param>
    public void ConfigureServices(IServiceCollection services)
    {
        // Базовые MVC сервисы для контроллеров
        services.AddControllers();
        
        // Swagger для документации API (временно, пока не настроен GraphQL)
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        // CORS политики для работы с фронтендом
        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigins", policy =>
            {
                policy.WithOrigins(_configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>())
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });

        // Health checks для мониторинга состояния приложения
        services.AddHealthChecks();

        // TODO: Настройка GraphQL (HotChocolate) - будет добавлено в следующих этапах
        // TODO: Настройка аутентификации и авторизации - будет добавлено в следующих этапах  
        // TODO: Настройка SignalR для real-time коммуникации - будет добавлено в следующих этапах
        // TODO: Настройка Redis для кэширования - будет добавлено в следующих этапах
        // TODO: Настройка Hangfire для фоновых задач - будет добавлено в следующих этапах
    }

    /// <summary>
    /// Метод конфигурации middleware pipeline. Вызывается runtime для настройки HTTP request pipeline.
    /// Определяет порядок выполнения middleware компонентов.
    /// </summary>
    /// <param name="app">Строитель приложения для настройки middleware</param>
    /// <param name="env">Информация об окружении выполнения</param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Middleware для обработки исключений в development окружении
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseDeveloperExceptionPage();
        }
        else
        {
            // Middleware для production окружения
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        // Принудительное использование HTTPS
        app.UseHttpsRedirection();

        // Обслуживание статических файлов
        app.UseStaticFiles();

        // Маршрутизация
        app.UseRouting();

        // CORS middleware
        app.UseCors("AllowSpecificOrigins");

        // Аутентификация и авторизация (пока базовая настройка)
        app.UseAuthentication();
        app.UseAuthorization();

        // Health checks endpoint
        app.UseHealthChecks("/health");

        // Настройка endpoints
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            
            // TODO: Добавить GraphQL endpoint - будет настроено в следующих этапах
            // TODO: Добавить SignalR hubs - будет настроено в следующих этапах
        });
    }
}