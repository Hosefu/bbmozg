using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Lauf.Api;
using Lauf.Infrastructure.Persistence;
using System.Text;
using System.Text.Json;
using Xunit;
using Xunit.Abstractions;

namespace Lauf.Api.Tests.E2E;

/// <summary>
/// Базовый класс для E2E тестов API с настройкой тестового сервера
/// </summary>
public abstract class BaseE2ETest : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly WebApplicationFactory<Program> _factory;
    protected readonly HttpClient _client;
    protected readonly ITestOutputHelper _output;
    protected readonly List<ApiCall> _apiCalls = new();

    protected BaseE2ETest(WebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                // Удаляем реальную БД и заменяем на InMemory
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
                });

                // Отключаем логирование для чистоты тестов
                services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning));
            });
        });

        _client = _factory.CreateClient();
        _output = output;
    }

    /// <summary>
    /// Выполняет GraphQL запрос с логированием для отчета
    /// </summary>
    protected async Task<T> ExecuteGraphQLAsync<T>(string query, object? variables = null, string? operationName = null)
    {
        var request = new
        {
            query,
            variables,
            operationName
        };

        var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var apiCall = new ApiCall
        {
            Timestamp = DateTime.UtcNow,
            Method = "POST",
            Url = "/graphql",
            RequestBody = json,
            RequestHeaders = content.Headers.ToDictionary(h => h.Key, h => string.Join(", ", h.Value))
        };

        _output.WriteLine($"[{apiCall.Timestamp:HH:mm:ss.fff}] Отправка GraphQL запроса:");
        _output.WriteLine($"Query: {query}");
        if (variables != null)
            _output.WriteLine($"Variables: {JsonSerializer.Serialize(variables, new JsonSerializerOptions { WriteIndented = true })}");

        var response = await _client.PostAsync("/graphql", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        apiCall.StatusCode = (int)response.StatusCode;
        apiCall.ResponseBody = responseBody;
        apiCall.ResponseHeaders = response.Headers.ToDictionary(h => h.Key, h => string.Join(", ", h.Value));

        _apiCalls.Add(apiCall);

        _output.WriteLine($"Ответ ({response.StatusCode}): {responseBody}");
        _output.WriteLine("---");

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"GraphQL запрос failed: {response.StatusCode} - {responseBody}");
        }

        var result = JsonSerializer.Deserialize<GraphQLResponse<T>>(responseBody, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        if (result?.Errors?.Any() == true)
        {
            throw new Exception($"GraphQL ошибки: {string.Join(", ", result.Errors.Select(e => e.Message))}");
        }

        return result!.Data;
    }

    /// <summary>
    /// Выполняет REST API запрос с логированием
    /// </summary>
    protected async Task<T> ExecuteRestAsync<T>(HttpMethod method, string url, object? body = null)
    {
        var request = new HttpRequestMessage(method, url);
        
        string? requestBody = null;
        if (body != null)
        {
            requestBody = JsonSerializer.Serialize(body, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
        }

        var apiCall = new ApiCall
        {
            Timestamp = DateTime.UtcNow,
            Method = method.Method,
            Url = url,
            RequestBody = requestBody,
            RequestHeaders = request.Headers.ToDictionary(h => h.Key, h => string.Join(", ", h.Value))
        };

        _output.WriteLine($"[{apiCall.Timestamp:HH:mm:ss.fff}] Отправка {method.Method} запроса на {url}");
        if (requestBody != null)
            _output.WriteLine($"Body: {requestBody}");

        var response = await _client.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();

        apiCall.StatusCode = (int)response.StatusCode;
        apiCall.ResponseBody = responseBody;
        apiCall.ResponseHeaders = response.Headers.ToDictionary(h => h.Key, h => string.Join(", ", h.Value));

        _apiCalls.Add(apiCall);

        _output.WriteLine($"Ответ ({response.StatusCode}): {responseBody}");
        _output.WriteLine("---");

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"REST запрос failed: {response.StatusCode} - {responseBody}");
        }

        return JsonSerializer.Deserialize<T>(responseBody, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        })!;
    }

    /// <summary>
    /// Генерирует HTML отчет с деталями всех API вызовов
    /// </summary>
    protected void GenerateHtmlReport(string testName)
    {
        if (!_apiCalls.Any()) return;

        var reportDir = Path.Combine(Directory.GetCurrentDirectory(), "TestReports");
        Directory.CreateDirectory(reportDir);

        var reportPath = Path.Combine(reportDir, $"{testName}_{DateTime.Now:yyyyMMdd_HHmmss}.html");

        var html = GenerateHtmlContent(testName);
        File.WriteAllText(reportPath, html);

        _output.WriteLine($"HTML отчет сохранен: {reportPath}");
    }

    private string GenerateHtmlContent(string testName)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html>");
        sb.AppendLine("<head>");
        sb.AppendLine($"<title>E2E Test Report: {testName}</title>");
        sb.AppendLine("<style>");
        sb.AppendLine(@"
            body { font-family: Arial, sans-serif; margin: 20px; }
            .api-call { border: 1px solid #ddd; margin: 10px 0; padding: 15px; border-radius: 5px; }
            .api-call.success { border-left: 5px solid #4CAF50; }
            .api-call.error { border-left: 5px solid #f44336; }
            .timestamp { color: #666; font-size: 0.9em; }
            .method { font-weight: bold; color: #2196F3; }
            .url { font-family: monospace; background: #f5f5f5; padding: 2px 5px; }
            .status-code { font-weight: bold; }
            .status-code.success { color: #4CAF50; }
            .status-code.error { color: #f44336; }
            .json { background: #f8f8f8; padding: 10px; border-radius: 3px; font-family: monospace; font-size: 0.9em; white-space: pre-wrap; }
            .headers { font-size: 0.8em; color: #666; }
            .section { margin: 10px 0; }
            .section-title { font-weight: bold; margin-bottom: 5px; }
        ");
        sb.AppendLine("</style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine($"<h1>E2E Test Report: {testName}</h1>");
        sb.AppendLine($"<p>Выполнен: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");
        sb.AppendLine($"<p>Всего API вызовов: {_apiCalls.Count}</p>");

        foreach (var call in _apiCalls)
        {
            var cssClass = call.StatusCode >= 200 && call.StatusCode < 300 ? "success" : "error";
            var statusClass = call.StatusCode >= 200 && call.StatusCode < 300 ? "success" : "error";

            sb.AppendLine($"<div class='api-call {cssClass}'>");
            sb.AppendLine($"<div class='timestamp'>{call.Timestamp:HH:mm:ss.fff}</div>");
            sb.AppendLine($"<div><span class='method'>{call.Method}</span> <span class='url'>{call.Url}</span> - <span class='status-code {statusClass}'>{call.StatusCode}</span></div>");

            if (!string.IsNullOrEmpty(call.RequestBody))
            {
                sb.AppendLine("<div class='section'>");
                sb.AppendLine("<div class='section-title'>Request Body:</div>");
                sb.AppendLine($"<div class='json'>{FormatJson(call.RequestBody)}</div>");
                sb.AppendLine("</div>");
            }

            if (call.RequestHeaders.Any())
            {
                sb.AppendLine("<div class='section'>");
                sb.AppendLine("<div class='section-title'>Request Headers:</div>");
                sb.AppendLine("<div class='headers'>");
                foreach (var header in call.RequestHeaders)
                {
                    sb.AppendLine($"{header.Key}: {header.Value}<br>");
                }
                sb.AppendLine("</div>");
                sb.AppendLine("</div>");
            }

            if (!string.IsNullOrEmpty(call.ResponseBody))
            {
                sb.AppendLine("<div class='section'>");
                sb.AppendLine("<div class='section-title'>Response Body:</div>");
                sb.AppendLine($"<div class='json'>{FormatJson(call.ResponseBody)}</div>");
                sb.AppendLine("</div>");
            }

            sb.AppendLine("</div>");
        }

        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        return sb.ToString();
    }

    private string FormatJson(string json)
    {
        try
        {
            var parsed = JsonSerializer.Deserialize<object>(json);
            return JsonSerializer.Serialize(parsed, new JsonSerializerOptions { WriteIndented = true });
        }
        catch
        {
            return json;
        }
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}

/// <summary>
/// Модель для хранения информации об API вызове
/// </summary>
public class ApiCall
{
    public DateTime Timestamp { get; set; }
    public string Method { get; set; } = "";
    public string Url { get; set; } = "";
    public string? RequestBody { get; set; }
    public Dictionary<string, string> RequestHeaders { get; set; } = new();
    public int StatusCode { get; set; }
    public string? ResponseBody { get; set; }
    public Dictionary<string, string> ResponseHeaders { get; set; } = new();
}

/// <summary>
/// Модель GraphQL ответа
/// </summary>
public class GraphQLResponse<T>
{
    public T Data { get; set; } = default!;
    public List<GraphQLError>? Errors { get; set; }
}

/// <summary>
/// Модель GraphQL ошибки
/// </summary>
public class GraphQLError
{
    public string Message { get; set; } = "";
    public List<GraphQLErrorLocation>? Locations { get; set; }
    public List<object>? Path { get; set; }
}

/// <summary>
/// Модель локации GraphQL ошибки
/// </summary>
public class GraphQLErrorLocation
{
    public int Line { get; set; }
    public int Column { get; set; }
}