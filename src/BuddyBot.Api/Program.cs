using BuddyBot.Api;
using BuddyBot.Application;
using BuddyBot.Infrastructure;
using BuddyBot.Shared.Extensions;
using Serilog;

namespace BuddyBot.Api;

/// <summary>
/// Главный класс приложения - точка входа для ASP.NET Core приложения.
/// Настраивает все необходимые сервисы согласно архитектуре Clean Architecture.
/// </summary>
public class Program
{
    /// <summary>
    /// Точка входа в приложение. Настраивает сервисы, middleware pipeline и запускает приложение.
    /// </summary>
    /// <param name="args">Аргументы командной строки</param>
    /// <returns>Код завершения приложения</returns>
    public static async Task<int> Main(string[] args)
    {
        // Настройка базового логирования для bootstrap процесса
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        try
        {
            Log.Information("Запуск BuddyBot API приложения");

            var builder = WebApplication.CreateBuilder(args);

            // Настройка Serilog как основного логгера
            builder.Host.UseSerilog((context, configuration) =>
                configuration.ReadFrom.Configuration(context.Configuration));

            // Регистрация сервисов из разных слоев
            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructureServices(builder.Configuration);
            
            // Настройка API слоя
            var startup = new Startup(builder.Configuration);
            startup.ConfigureServices(builder.Services);

            var app = builder.Build();

            // Настройка middleware pipeline
            startup.Configure(app, app.Environment);

            Log.Information("BuddyBot API приложение готово к запуску");
            
            await app.RunAsync();
            
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Критическая ошибка при запуске BuddyBot API приложения");
            return 1;
        }
        finally
        {
            // Корректное завершение логирования
            await Log.CloseAndFlushAsync();
        }
    }
}