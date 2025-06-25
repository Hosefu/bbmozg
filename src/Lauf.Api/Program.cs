using Lauf.Api;
using Lauf.Application;
using Lauf.Infrastructure;
using Lauf.Shared.Extensions;
using Serilog;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace Lauf.Api;

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
            Log.Information("Запуск Lauf API приложения");

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

            Log.Information("Lauf API приложение готово к запуску");
            
            // Регистрируем слушателя для получения информации о запущенных адресах
            app.Lifetime.ApplicationStarted.Register(() =>
            {
                var addresses = app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>();
                Log.Information("Сервер запущен на адресах: {Addresses}", string.Join(", ", addresses?.Addresses ?? new[] { "не определены" }));
                Log.Information("GraphQL Playground доступен по адресу: {PlaygroundUrl}", $"{addresses?.Addresses?.FirstOrDefault()}/playground");
                Log.Information("GraphQL API доступен по адресу: {GraphQLUrl}", $"{addresses?.Addresses?.FirstOrDefault()}/graphql");
                Log.Information("Документация API доступна по адресу: {DocsUrl}", $"{addresses?.Addresses?.FirstOrDefault()}/docs");
            });
            
            await app.RunAsync();
            
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Критическая ошибка при запуске Lauf API приложения");
            return 1;
        }
        finally
        {
            // Корректное завершение логирования
            await Log.CloseAndFlushAsync();
        }
    }
}