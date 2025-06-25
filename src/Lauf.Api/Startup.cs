using Microsoft.EntityFrameworkCore;
using Lauf.Shared.Extensions;
using Lauf.Application.Commands.FlowAssignment;
using Lauf.Application.Services.Interfaces;
using Lauf.Domain.Interfaces;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Infrastructure.Persistence;
using Lauf.Infrastructure.Persistence.Repositories;
using Lauf.Infrastructure.Persistence.Interceptors;
using Lauf.Infrastructure.Services;
using Lauf.Infrastructure.ExternalServices.FileStorage;
using Lauf.Infrastructure.ExternalServices.Cache;
using Lauf.Api.Services;
using MediatR;

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

        // Настройка Entity Framework
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));
        });

        // Регистрация перехватчиков
        services.AddScoped<AuditInterceptor>();
        services.AddScoped<DomainEventInterceptor>();

        // Регистрация MediatR
        services.AddMediatR(typeof(AssignFlowCommand).Assembly);

        // Регистрация репозиториев (этап 8)
        services.AddScoped<IUserRepository, SimpleUserRepository>();
        services.AddScoped<IFlowRepository, FlowRepository>();
        services.AddScoped<IFlowAssignmentRepository, FlowAssignmentRepository>();

        // Регистрация Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Регистрация инфраструктурных сервисов (этап 8)
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IDateTimeService, DateTimeService>();

        // Регистрация внешних сервисов (этап 8)
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddMemoryCache();
        services.AddScoped<ICacheService, InMemoryCacheService>();

        // GraphQL настройка (этап 9)
        services.AddGraphQLServer()
            .AddQueryType<Lauf.Api.GraphQL.Resolvers.Query>()
            .AddMutationType<Lauf.Api.GraphQL.Resolvers.Mutation>()
            .AddType<Lauf.Api.GraphQL.Types.UserType>()
            .AddType<Lauf.Api.GraphQL.Types.FlowType>()
            .AddType<Lauf.Api.GraphQL.Types.FlowDetailsType>()
            .AddType<Lauf.Api.GraphQL.Types.FlowStepType>()
            .AddType<Lauf.Api.GraphQL.Types.FlowStepDetailsType>()
            .AddType<Lauf.Api.GraphQL.Types.FlowStepComponentType>()
            .AddType<Lauf.Api.GraphQL.Types.FlowStepComponentDetailsType>()
            .AddType<Lauf.Api.GraphQL.Types.FlowAssignmentType>()
            .AddType<Lauf.Api.GraphQL.Types.FlowSettingsType>()
            .AddType<Lauf.Api.GraphQL.Types.FlowStatisticsType>()
            .AddType<Lauf.Api.GraphQL.Types.UserFlowProgressType>()
            .AddType<Lauf.Api.GraphQL.Types.ComponentProgressType>()
            .AddType<Lauf.Api.GraphQL.Types.AssignFlowResultType>()
            .AddProjections()
            .AddFiltering()
            .AddSorting();

        // SignalR настройка (этап 9)
        services.AddSignalR();
        services.AddScoped<Lauf.Api.Services.SignalRNotificationService>();
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
            
            // GraphQL endpoint (этап 9)
            endpoints.MapGraphQL("/graphql");
            
            // SignalR hubs (этап 9)
            endpoints.MapHub<Lauf.Api.Hubs.NotificationHub>("/hubs/notifications");
            endpoints.MapHub<Lauf.Api.Hubs.ProgressHub>("/hubs/progress");
        });
    }
}