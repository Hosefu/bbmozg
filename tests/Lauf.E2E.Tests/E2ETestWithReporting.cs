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
/// E2E тесты с генерацией HTML отчетов
/// </summary>
public class E2ETestWithReporting : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;
    private readonly List<ApiCall> _apiCalls = new();

    public E2ETestWithReporting(WebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
                });

                services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning));
            });
        });

        _client = _factory.CreateClient();
        _output = output;
    }

    [Fact]
    public async Task CompleteWorkflow_ShouldGenerateDetailedReport()
    {
        try
        {
            _output.WriteLine("🚀 === ПОЛНЫЙ E2E ТЕСТ С ДЕТАЛЬНЫМ ОТЧЕТОМ ===");
            
            // === ЭТАП 1: Проверка Health Check ===
            _output.WriteLine("=== ЭТАП 1: Health Check ===");
            await ExecuteHttpRequest("GET", "/health", null, "Health Check");

            // === ЭТАП 2: GraphQL Introspection ===
            _output.WriteLine("=== ЭТАП 2: GraphQL Introspection ===");
            var introspectionQuery = new
            {
                query = @"query { __schema { types { name } } }"
            };
            await ExecuteGraphQLRequest(introspectionQuery, "GraphQL Introspection");

            // === ЭТАП 3: Попытка создания пользователя ===
            _output.WriteLine("=== ЭТАП 3: Создание пользователя ===");
            var createUserMutation = new
            {
                query = @"
                    mutation CreateUser($input: CreateUserInput!) {
                        createUser(input: $input) {
                            id
                            email
                            fullName
                        }
                    }",
                variables = new
                {
                    input = new
                    {
                        telegramId = 999999,
                        email = "e2e.test@example.com",
                        fullName = "E2E Тестовый Пользователь",
                        position = "QA Engineer"
                    }
                }
            };
            await ExecuteGraphQLRequest(createUserMutation, "Create User");

            // === ЭТАП 4: Попытка создания потока ===
            _output.WriteLine("=== ЭТАП 4: Создание потока ===");
            var createFlowMutation = new
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
                        title = "E2E Тестовый Поток",
                        description = "Полное тестирование создания потока через API",
                        isSequential = true,
                        allowRetry = true,
                        timeLimit = 30
                    }
                }
            };
            await ExecuteGraphQLRequest(createFlowMutation, "Create Flow");

            // === ЭТАП 5: Получение списка пользователей ===
            _output.WriteLine("=== ЭТАП 5: Получение пользователей ===");
            var getUsersQuery = new
            {
                query = @"
                    query GetUsers($skip: Int, $take: Int) {
                        users(skip: $skip, take: $take) {
                            id
                            email
                            fullName
                        }
                    }",
                variables = new { skip = 0, take = 10 }
            };
            await ExecuteGraphQLRequest(getUsersQuery, "Get Users");

            _output.WriteLine($"✅ Тест завершен! Выполнено {_apiCalls.Count} API вызовов");
        }
        finally
        {
            GenerateHtmlReport("CompleteE2EWorkflow");
        }
    }

    private async Task ExecuteHttpRequest(string method, string url, object? body, string description)
    {
        var apiCall = new ApiCall
        {
            Timestamp = DateTime.UtcNow,
            Method = method,
            Url = url,
            Description = description
        };

        try
        {
            HttpResponseMessage response;
            if (method == "GET")
            {
                response = await _client.GetAsync(url);
            }
            else
            {
                var json = body != null ? JsonConvert.SerializeObject(body) : "";
                apiCall.RequestBody = json;
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                response = await _client.PostAsync(url, content);
            }

            apiCall.StatusCode = (int)response.StatusCode;
            apiCall.ResponseBody = await response.Content.ReadAsStringAsync();
            apiCall.Success = response.IsSuccessStatusCode;

            _output.WriteLine($"[{apiCall.Timestamp:HH:mm:ss}] {description}: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            apiCall.StatusCode = 0;
            apiCall.ResponseBody = $"Exception: {ex.Message}";
            apiCall.Success = false;
            _output.WriteLine($"[{apiCall.Timestamp:HH:mm:ss}] {description}: ERROR - {ex.Message}");
        }

        _apiCalls.Add(apiCall);
    }

    private async Task ExecuteGraphQLRequest(object query, string description)
    {
        await ExecuteHttpRequest("POST", "/graphql", query, description);
    }

    private void GenerateHtmlReport(string testName)
    {
        var reportDir = Path.Combine(Directory.GetCurrentDirectory(), "TestReports");
        Directory.CreateDirectory(reportDir);

        var reportPath = Path.Combine(reportDir, $"{testName}_{DateTime.Now:yyyyMMdd_HHmmss}.html");
        var html = GenerateHtmlContent(testName);
        
        File.WriteAllText(reportPath, html);
        
        _output.WriteLine("");
        _output.WriteLine($"📊 HTML отчет сохранен: {reportPath}");
        _output.WriteLine($"🌐 Откройте файл в браузере для просмотра детального отчета");
    }

    private string GenerateHtmlContent(string testName)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang='ru'>");
        sb.AppendLine("<head>");
        sb.AppendLine("<meta charset='UTF-8'>");
        sb.AppendLine("<meta name='viewport' content='width=device-width, initial-scale=1.0'>");
        sb.AppendLine($"<title>E2E Test Report: {testName}</title>");
        sb.AppendLine("<style>");
        sb.AppendLine(@"
            body { 
                font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; 
                margin: 0; 
                padding: 20px; 
                background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                color: #333;
            }
            .container {
                max-width: 1200px;
                margin: 0 auto;
                background: white;
                border-radius: 10px;
                box-shadow: 0 10px 30px rgba(0,0,0,0.3);
                overflow: hidden;
            }
            .header {
                background: linear-gradient(135deg, #1e3c72 0%, #2a5298 100%);
                color: white;
                padding: 30px;
                text-align: center;
            }
            .header h1 { margin: 0; font-size: 2.5em; }
            .header p { margin: 10px 0 0 0; opacity: 0.9; }
            .summary {
                padding: 20px 30px;
                background: #f8f9fa;
                border-bottom: 1px solid #dee2e6;
            }
            .summary-stats {
                display: flex;
                justify-content: space-around;
                text-align: center;
            }
            .stat {
                padding: 15px;
            }
            .stat-number {
                font-size: 2em;
                font-weight: bold;
                display: block;
            }
            .stat-success { color: #28a745; }
            .stat-error { color: #dc3545; }
            .stat-total { color: #007bff; }
            .api-calls {
                padding: 20px 30px;
            }
            .api-call {
                border: 1px solid #dee2e6;
                margin: 15px 0;
                border-radius: 8px;
                overflow: hidden;
                transition: box-shadow 0.3s ease;
            }
            .api-call:hover {
                box-shadow: 0 4px 15px rgba(0,0,0,0.1);
            }
            .api-call.success { border-left: 5px solid #28a745; }
            .api-call.error { border-left: 5px solid #dc3545; }
            .call-header {
                background: #f8f9fa;
                padding: 15px 20px;
                display: flex;
                justify-content: space-between;
                align-items: center;
            }
            .call-info {
                display: flex;
                align-items: center;
                gap: 15px;
            }
            .method {
                font-weight: bold;
                padding: 4px 8px;
                border-radius: 4px;
                color: white;
                font-size: 0.8em;
            }
            .method.GET { background: #28a745; }
            .method.POST { background: #007bff; }
            .url { 
                font-family: 'Courier New', monospace; 
                background: #e9ecef; 
                padding: 4px 8px; 
                border-radius: 4px;
                font-size: 0.9em;
            }
            .status-code {
                font-weight: bold;
                padding: 4px 8px;
                border-radius: 4px;
                color: white;
            }
            .status-code.success { background: #28a745; }
            .status-code.error { background: #dc3545; }
            .timestamp {
                color: #6c757d;
                font-size: 0.85em;
            }
            .call-body {
                padding: 20px;
            }
            .section {
                margin: 15px 0;
            }
            .section-title {
                font-weight: bold;
                margin-bottom: 8px;
                color: #495057;
                font-size: 0.9em;
                text-transform: uppercase;
                letter-spacing: 0.5px;
            }
            .json {
                background: #2d3748;
                color: #e2e8f0;
                padding: 15px;
                border-radius: 6px;
                font-family: 'Courier New', monospace;
                font-size: 0.85em;
                white-space: pre-wrap;
                overflow-x: auto;
                max-height: 300px;
                overflow-y: auto;
            }
            .description {
                background: linear-gradient(90deg, #4facfe 0%, #00f2fe 100%);
                color: white;
                padding: 8px 12px;
                border-radius: 20px;
                font-size: 0.8em;
                font-weight: 500;
            }
        ");
        sb.AppendLine("</style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");

        sb.AppendLine("<div class='container'>");
        sb.AppendLine("<div class='header'>");
        sb.AppendLine($"<h1>🧪 E2E Test Report</h1>");
        sb.AppendLine($"<p>{testName} - {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");
        sb.AppendLine("</div>");

        // Статистика
        var successCount = _apiCalls.Count(c => c.Success);
        var errorCount = _apiCalls.Count(c => !c.Success);
        
        sb.AppendLine("<div class='summary'>");
        sb.AppendLine("<div class='summary-stats'>");
        sb.AppendLine($"<div class='stat'><span class='stat-number stat-total'>{_apiCalls.Count}</span>Всего вызовов</div>");
        sb.AppendLine($"<div class='stat'><span class='stat-number stat-success'>{successCount}</span>Успешно</div>");
        sb.AppendLine($"<div class='stat'><span class='stat-number stat-error'>{errorCount}</span>Ошибки</div>");
        sb.AppendLine("</div>");
        sb.AppendLine("</div>");

        sb.AppendLine("<div class='api-calls'>");
        sb.AppendLine("<h2>📋 Детали API вызовов</h2>");

        foreach (var call in _apiCalls)
        {
            var cssClass = call.Success ? "success" : "error";
            var statusClass = call.Success ? "success" : "error";

            sb.AppendLine($"<div class='api-call {cssClass}'>");
            sb.AppendLine("<div class='call-header'>");
            sb.AppendLine("<div class='call-info'>");
            sb.AppendLine($"<span class='method {call.Method}'>{call.Method}</span>");
            sb.AppendLine($"<span class='url'>{call.Url}</span>");
            if (!string.IsNullOrEmpty(call.Description))
            {
                sb.AppendLine($"<span class='description'>{call.Description}</span>");
            }
            sb.AppendLine("</div>");
            sb.AppendLine("<div>");
            sb.AppendLine($"<span class='status-code {statusClass}'>{call.StatusCode}</span>");
            sb.AppendLine($"<span class='timestamp'>{call.Timestamp:HH:mm:ss.fff}</span>");
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");

            sb.AppendLine("<div class='call-body'>");
            
            if (!string.IsNullOrEmpty(call.RequestBody))
            {
                sb.AppendLine("<div class='section'>");
                sb.AppendLine("<div class='section-title'>📤 Request Body</div>");
                sb.AppendLine($"<div class='json'>{FormatJson(call.RequestBody)}</div>");
                sb.AppendLine("</div>");
            }

            if (!string.IsNullOrEmpty(call.ResponseBody))
            {
                sb.AppendLine("<div class='section'>");
                sb.AppendLine("<div class='section-title'>📥 Response Body</div>");
                sb.AppendLine($"<div class='json'>{FormatJson(call.ResponseBody)}</div>");
                sb.AppendLine("</div>");
            }

            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
        }

        sb.AppendLine("</div>");
        sb.AppendLine("</div>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        return sb.ToString();
    }

    private string FormatJson(string json)
    {
        try
        {
            var parsed = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsed, Formatting.Indented);
        }
        catch
        {
            return json;
        }
    }

    public void Dispose()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }
}

public class ApiCall
{
    public DateTime Timestamp { get; set; }
    public string Method { get; set; } = "";
    public string Url { get; set; } = "";
    public string Description { get; set; } = "";
    public string? RequestBody { get; set; }
    public int StatusCode { get; set; }
    public string? ResponseBody { get; set; }
    public bool Success { get; set; }
}