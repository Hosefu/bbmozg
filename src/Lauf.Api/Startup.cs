using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Hangfire;
using Hangfire.MemoryStorage;
using Lauf.Shared.Extensions;
using Lauf.Application;
using Lauf.Infrastructure;
using Lauf.Infrastructure.BackgroundJobs;
using Lauf.Infrastructure.Persistence;
using Lauf.Api.Services;

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

        // JWT Authentication для мини-апп
        var jwtSettings = _configuration.GetSection("JWT");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? "default-secret-key-for-development-only-32chars");
        
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization();

        // Регистрация слоев приложения
        services.AddApplicationServices();
        services.AddInfrastructureServices(_configuration);

        // Дополнительные API сервисы
        services.AddHttpContextAccessor();

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
            .AddType<Lauf.Api.GraphQL.Types.Components.ComponentUnionType>()
            .AddType<Lauf.Api.GraphQL.Types.Components.ArticleComponentType>()
            .AddType<Lauf.Api.GraphQL.Types.Components.QuizComponentType>()
            .AddType<Lauf.Api.GraphQL.Types.Components.TaskComponentType>()
            .AddType<Lauf.Api.GraphQL.Types.Components.QuestionOptionType>()
            .AddProjections()
            .AddFiltering()
            .AddSorting();

        // SignalR настройка (этап 9)
        services.AddSignalR();
        services.AddScoped<Lauf.Api.Services.SignalRNotificationService>();
        
        // Hangfire configuration для background jobs
        services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseMemoryStorage());

        services.AddHangfireServer();
        services.AddScoped<HangfireJobScheduler>();
        
        // API-специфичные Telegram сервисы
        services.AddScoped<TelegramAuthValidator>(provider =>
        {
            var botToken = _configuration.GetSection("TelegramBot")["Token"] ?? "default-token";
            return new TelegramAuthValidator(botToken);
        });
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
            
            // GraphQL интерфейс для разработки включается автоматически в HotChocolate 13.x
        }
        else
        {
            // Middleware для production окружения
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        // HTTPS редирект отключен для development
        // if (!env.IsDevelopment())
        // {
        //     app.UseHttpsRedirection();
        // }

        // Обслуживание статических файлов
        app.UseStaticFiles();

        // Логирование запросов (должно быть перед маршрутизацией)
        app.UseMiddleware<Lauf.Api.Middleware.RequestLoggingMiddleware>();

        // Маршрутизация
        app.UseRouting();

        // CORS middleware
        app.UseCors("AllowSpecificOrigins");

        // Аутентификация и авторизация (пока базовая настройка)
        app.UseAuthentication();
        app.UseAuthorization();

        // Health checks endpoint
        app.UseHealthChecks("/health");

        // Hangfire Dashboard (только для development)
        if (env.IsDevelopment())
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationFilter() }
            });
        }

        // Настройка endpoints
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            
            // GraphQL endpoint (этап 9)
            endpoints.MapGraphQL("/graphql");
            
            // SignalR hubs (этап 9)
            endpoints.MapHub<Lauf.Api.SignalR.NotificationHub>("/hubs/notifications");
            endpoints.MapHub<Lauf.Api.SignalR.ProgressHub>("/hubs/progress");
            
            // Документация API
            endpoints.MapFallbackToFile("/docs", "/docs/index.html");
            endpoints.MapFallbackToFile("/voyager", "/voyager/index.html");
            endpoints.MapFallbackToFile("/playground", "/playground/index.html");
            
            // Redirect корня на документацию в development
            if (env.IsDevelopment())
            {
                endpoints.MapGet("/", context =>
                {
                    context.Response.Redirect("/docs");
                    return Task.CompletedTask;
                });
            }
        });
        
        // Инициализация seed данных и Hangfire jobs в фоне
        Task.Run(async () => 
        {
            await InitializeSeedDataAsync(app.ApplicationServices);
            
            // Небольшая задержка перед настройкой Hangfire jobs
            await Task.Delay(1000);
            ConfigureHangfireJobs(app.ApplicationServices);
        });
    }

    /// <summary>
    /// Настройка регулярных задач Hangfire
    /// </summary>
    /// <param name="serviceProvider">Провайдер сервисов</param>
    private static void ConfigureHangfireJobs(IServiceProvider serviceProvider)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var jobScheduler = scope.ServiceProvider.GetRequiredService<HangfireJobScheduler>();
            jobScheduler.ConfigureRecurringJobs();
        }
        catch (Exception ex)
        {
            // Логируем ошибку, но не прерываем запуск приложения
            var logger = serviceProvider.GetService<ILogger<Startup>>();
                         logger?.LogError(ex, "Ошибка настройки Hangfire jobs");
        }
    }

    /// <summary>
    /// Инициализация seed данных
    /// </summary>
    /// <param name="serviceProvider">Провайдер сервисов</param>
    private static async Task InitializeSeedDataAsync(IServiceProvider serviceProvider)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            // Применяем миграции если необходимо
            await context.Database.MigrateAsync();
            
            // Инициализируем seed данные
            await context.SeedDataAsync();
        }
        catch (Exception ex)
        {
            var logger = serviceProvider.GetService<ILogger<Startup>>();
            logger?.LogError(ex, "Ошибка инициализации seed данных");
        }
    }
}