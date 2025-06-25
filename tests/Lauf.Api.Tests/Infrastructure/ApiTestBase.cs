using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Lauf.Infrastructure.Persistence;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Lauf.Api.Tests.Infrastructure;

/// <summary>
/// Базовый класс для API интеграционных тестов
/// </summary>
public abstract class ApiTestBase : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    protected readonly WebApplicationFactory<Program> Factory;
    protected readonly HttpClient Client;
    protected readonly ApplicationDbContext Context;

    protected ApiTestBase(WebApplicationFactory<Program> factory)
    {
        Factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            
            builder.ConfigureServices(services =>
            {
                // Удаляем существующую регистрацию DbContext
                services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
                services.RemoveAll(typeof(ApplicationDbContext));

                // Добавляем in-memory базу данных для тестов
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
                });

                // Убираждаемся, что база создана
                var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.EnsureCreated();
            });
        });

        Client = Factory.CreateClient();
        
        // Получаем контекст базы данных для тестов
        using var scope = Factory.Services.CreateScope();
        Context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    /// <summary>
    /// Создает HTTP контент из объекта
    /// </summary>
    protected static StringContent CreateJsonContent(object obj)
    {
        var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    /// <summary>
    /// Десериализует HTTP ответ в объект
    /// </summary>
    protected static async Task<T?> DeserializeResponse<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    /// <summary>
    /// Очищает данные тестовой базы
    /// </summary>
    protected async Task ClearDatabase()
    {
        Context.FlowAssignments.RemoveRange(Context.FlowAssignments);
        Context.FlowSteps.RemoveRange(Context.FlowSteps);
        Context.Flows.RemoveRange(Context.Flows);
        Context.Users.RemoveRange(Context.Users);
        await Context.SaveChangesAsync();
    }

    public virtual void Dispose()
    {
        Client?.Dispose();
        Context?.Dispose();
    }
}