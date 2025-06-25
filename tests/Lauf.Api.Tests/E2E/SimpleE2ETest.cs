using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Lauf.Api.Tests.E2E;

/// <summary>
/// Простой E2E тест для проверки работоспособности API
/// </summary>
public class SimpleE2ETest : BaseE2ETest
{
    public SimpleE2ETest(WebApplicationFactory<Program> factory, ITestOutputHelper output) 
        : base(factory, output)
    {
    }

    [Fact]
    public async Task HealthCheck_ShouldReturnOk()
    {
        try
        {
            _output.WriteLine("=== Проверка состояния API ===");
            
            // Простая проверка health check endpoint
            var response = await _client.GetAsync("/health");
            
            response.IsSuccessStatusCode.Should().BeTrue();
            _output.WriteLine($"Health check статус: {response.StatusCode}");
            
            var content = await response.Content.ReadAsStringAsync();
            _output.WriteLine($"Health check ответ: {content}");
        }
        finally
        {
            GenerateHtmlReport("HealthCheck");
        }
    }

    [Fact]
    public async Task GraphQL_Endpoint_ShouldBeAccessible()
    {
        try
        {
            _output.WriteLine("=== Проверка доступности GraphQL ===");
            
            // Простой GraphQL запрос для проверки доступности
            const string introspectionQuery = @"
                query {
                    __schema {
                        types {
                            name
                        }
                    }
                }";

            var result = await ExecuteGraphQLAsync<IntrospectionResult>(introspectionQuery);
            
            result.Should().NotBeNull();
            result.Schema.Should().NotBeNull();
            result.Schema.Types.Should().NotBeEmpty();
            
            _output.WriteLine($"GraphQL схема содержит {result.Schema.Types.Count} типов");
        }
        finally
        {
            GenerateHtmlReport("GraphQLIntrospection");
        }
    }

    [Fact]
    public async Task CreateUser_ShouldWork()
    {
        try
        {
            _output.WriteLine("=== Проверка создания пользователя ===");
            
            const string createUserMutation = @"
                mutation CreateUser($input: CreateUserInput!) {
                    createUser(input: $input) {
                        id
                        email
                        fullName
                    }
                }";

            var input = new
            {
                telegramId = 123456L,
                email = "test@example.com",
                fullName = "Тестовый Пользователь",
                position = "Тестовая Позиция"
            };

            var result = await ExecuteGraphQLAsync<CreateUserTestResult>(createUserMutation, new { input });
            
            result.Should().NotBeNull();
            result.CreateUser.Should().NotBeNull();
            result.CreateUser.Email.Should().Be("test@example.com");
            result.CreateUser.FullName.Should().Be("Тестовый Пользователь");
            
            _output.WriteLine($"Создан пользователь: {result.CreateUser.Id}");
        }
        catch (Exception ex)
        {
            _output.WriteLine($"Ошибка создания пользователя: {ex.Message}");
            throw;
        }
        finally
        {
            GenerateHtmlReport("CreateUser");
        }
    }
}

#region Response Models

public class IntrospectionResult
{
    public SchemaInfo Schema { get; set; } = new();
}

public class SchemaInfo  
{
    public List<TypeInfo> Types { get; set; } = new();
}

public class TypeInfo
{
    public string Name { get; set; } = "";
}

public class CreateUserTestResult
{
    public UserTestInfo CreateUser { get; set; } = new();
}

public class UserTestInfo
{
    public Guid Id { get; set; }
    public string Email { get; set; } = "";
    public string FullName { get; set; } = "";
}

#endregion