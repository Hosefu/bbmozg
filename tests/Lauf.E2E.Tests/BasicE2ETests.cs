using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using Xunit;
using Xunit.Abstractions;
using Lauf.Infrastructure.Persistence;
using Lauf.Api;

namespace Lauf.E2E.Tests;

/// <summary>
/// Базовые E2E тесты API системы Lauf
/// Проверяют основную функциональность через HTTP/GraphQL запросы
/// </summary>
public class BasicE2ETests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public BasicE2ETests(WebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                // Заменяем реальную БД на InMemory для тестов
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
                });

                // Минимальное логирование
                services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning));
            });
        });

        _client = _factory.CreateClient();
        _output = output;
    }

    [Fact]
    public async Task HealthCheck_ShouldReturnHealthy()
    {
        _output.WriteLine("=== Проверка Health Check ===");
        
        var response = await _client.GetAsync("/health");
        var content = await response.Content.ReadAsStringAsync();
        
        response.IsSuccessStatusCode.Should().BeTrue();
        _output.WriteLine($"Status: {response.StatusCode}");
        _output.WriteLine($"Response: {content}");
    }

    [Fact]
    public async Task GraphQL_Introspection_ShouldWork()
    {
        _output.WriteLine("=== Проверка GraphQL Introspection ===");
        
        var query = new
        {
            query = @"
                query {
                    __schema {
                        types {
                            name
                        }
                    }
                }"
        };

        var json = JsonConvert.SerializeObject(query);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _output.WriteLine($"Отправка запроса: {json}");
        
        var response = await _client.PostAsync("/graphql", content);
        var responseContent = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Status: {response.StatusCode}");
        _output.WriteLine($"Response: {responseContent}");

        response.IsSuccessStatusCode.Should().BeTrue();
        responseContent.Should().Contain("__schema");
    }

    [Fact]
    public async Task CreateUser_ShouldSucceed()
    {
        _output.WriteLine("=== Тест создания пользователя ===");

        var mutation = new
        {
            query = @"
                mutation CreateUser($input: CreateUserInput!) {
                    createUser(input: $input) {
                        id
                        email
                        fullName
                        position
                    }
                }",
            variables = new
            {
                input = new
                {
                    telegramId = 123456,
                    email = "test@example.com",
                    fullName = "Тестовый Пользователь",
                    position = "Тестировщик"
                }
            }
        };

        var json = JsonConvert.SerializeObject(mutation);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _output.WriteLine($"Создание пользователя: {mutation.variables.input.fullName}");

        var response = await _client.PostAsync("/graphql", content);
        var responseContent = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Status: {response.StatusCode}");
        _output.WriteLine($"Response: {responseContent}");

        if (response.IsSuccessStatusCode)
        {
            responseContent.Should().Contain("test@example.com");
            responseContent.Should().Contain("Тестовый Пользователь");
            _output.WriteLine("✅ Пользователь успешно создан");
        }
        else
        {
            _output.WriteLine($"❌ Ошибка создания пользователя: {response.StatusCode}");
            _output.WriteLine($"Содержимое ответа: {responseContent}");
        }

        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task CreateFlow_ShouldSucceed()
    {
        _output.WriteLine("=== Тест создания потока ===");

        var mutation = new
        {
            query = @"
                mutation CreateFlow($input: CreateFlowInput!) {
                    createFlow(input: $input) {
                        id
                        title
                        status
                    }
                }",
            variables = new
            {
                input = new
                {
                    title = "Тестовый поток обучения",
                    description = "Описание тестового потока для проверки API",
                    isSequential = true,
                    allowRetry = true,
                    timeLimit = 30
                }
            }
        };

        var json = JsonConvert.SerializeObject(mutation);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _output.WriteLine($"Создание потока: {mutation.variables.input.title}");

        var response = await _client.PostAsync("/graphql", content);
        var responseContent = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Status: {response.StatusCode}");
        _output.WriteLine($"Response: {responseContent}");

        if (response.IsSuccessStatusCode)
        {
            responseContent.Should().Contain("Тестовый поток обучения");
            _output.WriteLine("✅ Поток успешно создан");
        }
        else
        {
            _output.WriteLine($"❌ Ошибка создания потока: {response.StatusCode}");
            _output.WriteLine($"Содержимое ответа: {responseContent}");
        }

        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task GetUsers_ShouldReturnList()
    {
        _output.WriteLine("=== Тест получения списка пользователей ===");

        var query = new
        {
            query = @"
                query GetUsers($skip: Int, $take: Int) {
                    users(skip: $skip, take: $take) {
                        id
                        email
                        fullName
                        isActive
                    }
                }",
            variables = new
            {
                skip = 0,
                take = 10
            }
        };

        var json = JsonConvert.SerializeObject(query);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _output.WriteLine("Получение списка пользователей...");

        var response = await _client.PostAsync("/graphql", content);
        var responseContent = await response.Content.ReadAsStringAsync();

        _output.WriteLine($"Status: {response.StatusCode}");
        _output.WriteLine($"Response: {responseContent}");

        response.IsSuccessStatusCode.Should().BeTrue();
        responseContent.Should().Contain("users");
        
        _output.WriteLine("✅ Список пользователей получен");
    }

    public void Dispose()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }
}