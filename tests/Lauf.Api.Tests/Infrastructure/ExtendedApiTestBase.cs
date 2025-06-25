using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Lauf.Infrastructure.Persistence;
using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;
using System.Net;
using Xunit.Abstractions;

namespace Lauf.Api.Tests.Infrastructure;

/// <summary>
/// Расширенная базовая клас для API тестов с детальным логированием
/// </summary>
public abstract class ExtendedApiTestBase : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    protected readonly WebApplicationFactory<Program> Factory;
    protected readonly HttpClient Client;
    protected readonly ApplicationDbContext Context;
    protected readonly ITestOutputHelper Output;
    private readonly List<ApiCallLog> _apiCalls = new();

    protected ExtendedApiTestBase(WebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        Factory = factory;
        Output = output;
        
        // Создаем клиент с логированием
        Client = CreateClientWithLogging();
        
        // Получаем контекст БД
        var scope = Factory.Services.CreateScope();
        Context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        // Убеждаемся, что БД создана
        Context.Database.EnsureCreated();
    }

    /// <summary>
    /// Создает HTTP клиент с детальным логированием запросов и ответов
    /// </summary>
    private HttpClient CreateClientWithLogging()
    {
        return Factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Настраиваем InMemory БД для тестов
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
                });

                // Добавляем логирование
                services.AddLogging(configure => configure.AddConsole().SetMinimumLevel(LogLevel.Information));
            });
        }).CreateClient();
    }

    /// <summary>
    /// Отправляет GET запрос с логированием
    /// </summary>
    protected async Task<HttpResponseMessage> GetAsync(string url, string? description = null)
    {
        var startTime = DateTime.UtcNow;
        
        Output.WriteLine($"🔍 GET {url} - {description ?? ""}");
        
        var response = await Client.GetAsync(url);
        
        await LogApiCall("GET", url, null, response, startTime, description);
        
        return response;
    }

    /// <summary>
    /// Отправляет POST запрос с логированием
    /// </summary>
    protected async Task<HttpResponseMessage> PostAsync<T>(string url, T data, string? description = null)
    {
        var startTime = DateTime.UtcNow;
        var jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        
        Output.WriteLine($"📤 POST {url} - {description ?? ""}");
        Output.WriteLine($"   Request: {jsonData}");
        
        var response = await Client.PostAsJsonAsync(url, data);
        
        await LogApiCall("POST", url, jsonData, response, startTime, description);
        
        return response;
    }

    /// <summary>
    /// Отправляет PUT запрос с логированием
    /// </summary>
    protected async Task<HttpResponseMessage> PutAsync<T>(string url, T data, string? description = null)
    {
        var startTime = DateTime.UtcNow;
        var jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        
        Output.WriteLine($"✏️ PUT {url} - {description ?? ""}");
        Output.WriteLine($"   Request: {jsonData}");
        
        var response = await Client.PutAsJsonAsync(url, data);
        
        await LogApiCall("PUT", url, jsonData, response, startTime, description);
        
        return response;
    }

    /// <summary>
    /// Отправляет DELETE запрос с логированием
    /// </summary>
    protected async Task<HttpResponseMessage> DeleteAsync(string url, string? description = null)
    {
        var startTime = DateTime.UtcNow;
        
        Output.WriteLine($"🗑️ DELETE {url} - {description ?? ""}");
        
        var response = await Client.DeleteAsync(url);
        
        await LogApiCall("DELETE", url, null, response, startTime, description);
        
        return response;
    }

    /// <summary>
    /// Логирует информацию о API вызове
    /// </summary>
    private async Task LogApiCall(string method, string url, string? requestBody, HttpResponseMessage response, DateTime startTime, string? description)
    {
        var duration = DateTime.UtcNow - startTime;
        var responseBody = await response.Content.ReadAsStringAsync();
        
        var logEntry = new ApiCallLog
        {
            Method = method,
            Url = url,
            RequestBody = requestBody,
            ResponseStatusCode = response.StatusCode,
            ResponseBody = responseBody,
            Duration = duration,
            Description = description,
            Timestamp = startTime
        };

        _apiCalls.Add(logEntry);

        // Логируем ответ
        var statusIcon = response.IsSuccessStatusCode ? "✅" : "❌";
        Output.WriteLine($"   {statusIcon} Response: {(int)response.StatusCode} {response.StatusCode} ({duration.TotalMilliseconds:F0}ms)");
        
        if (!string.IsNullOrEmpty(responseBody))
        {
            try
            {
                var formattedJson = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(responseBody), Formatting.Indented);
                Output.WriteLine($"   Response Body: {formattedJson}");
            }
            catch
            {
                Output.WriteLine($"   Response Body: {responseBody}");
            }
        }
        
        Output.WriteLine("");
    }

    /// <summary>
    /// Проверяет успешность ответа и возвращает десериализованные данные
    /// </summary>
    protected async Task<T> AssertSuccessAndGetData<T>(HttpResponseMessage response, string? expectedDescription = null)
    {
        response.StatusCode.Should().Be(HttpStatusCode.OK, $"Expected successful response for {expectedDescription ?? "operation"}");
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
        
        var data = JsonConvert.DeserializeObject<T>(content);
        data.Should().NotBeNull();
        
        return data!;
    }

    /// <summary>
    /// Очищает базу данных
    /// </summary>
    protected async Task ClearDatabase()
    {
        Context.FlowAssignments.RemoveRange(Context.FlowAssignments);
        Context.FlowSteps.RemoveRange(Context.FlowSteps);
        Context.Flows.RemoveRange(Context.Flows);
        Context.Users.RemoveRange(Context.Users);
        Context.Roles.RemoveRange(Context.Roles);
        
        await Context.SaveChangesAsync();
        
        Output.WriteLine("🧹 База данных очищена");
    }

    /// <summary>
    /// Генерирует HTML отчёт о всех API вызовах
    /// </summary>
    protected void GenerateApiCallReport(string testName)
    {
        var html = GenerateHtmlReport(testName);
        var fileName = $"TestResults/api-calls-{testName.Replace(" ", "-")}-{DateTime.Now:yyyyMMdd-HHmmss}.html";
        
        Directory.CreateDirectory("TestResults");
        File.WriteAllText(fileName, html);
        
        Output.WriteLine($"📊 HTML отчёт API вызовов: {fileName}");
    }

    /// <summary>
    /// Генерирует HTML отчёт
    /// </summary>
    private string GenerateHtmlReport(string testName)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang='ru'>");
        sb.AppendLine("<head>");
        sb.AppendLine("    <meta charset='UTF-8'>");
        sb.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
        sb.AppendLine($"    <title>API Calls Report - {testName}</title>");
        sb.AppendLine("    <style>");
        sb.AppendLine("        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5; }");
        sb.AppendLine("        .container { max-width: 1200px; margin: 0 auto; background-color: white; padding: 20px; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }");
        sb.AppendLine("        .header { border-bottom: 2px solid #007acc; padding-bottom: 20px; margin-bottom: 30px; }");
        sb.AppendLine("        .header h1 { color: #007acc; margin: 0; }");
        sb.AppendLine("        .stats { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 20px; margin-bottom: 30px; }");
        sb.AppendLine("        .stat-card { background: #f8f9fa; padding: 15px; border-radius: 5px; text-align: center; }");
        sb.AppendLine("        .api-call { border: 1px solid #e0e0e0; border-radius: 5px; margin-bottom: 15px; overflow: hidden; }");
        sb.AppendLine("        .api-call-header { background: #f8f9fa; padding: 15px; cursor: pointer; display: flex; justify-content: space-between; align-items: center; }");
        sb.AppendLine("        .api-call-header:hover { background: #e9ecef; }");
        sb.AppendLine("        .method { font-weight: bold; padding: 3px 8px; border-radius: 3px; color: white; }");
        sb.AppendLine("        .method.GET { background: #28a745; }");
        sb.AppendLine("        .method.POST { background: #007bff; }");
        sb.AppendLine("        .method.PUT { background: #ffc107; color: #000; }");
        sb.AppendLine("        .method.DELETE { background: #dc3545; }");
        sb.AppendLine("        .status { padding: 3px 8px; border-radius: 3px; color: white; }");
        sb.AppendLine("        .status.success { background: #28a745; }");
        sb.AppendLine("        .status.error { background: #dc3545; }");
        sb.AppendLine("        .api-call-body { padding: 15px; background: white; }");
        sb.AppendLine("        .json { background: #f8f9fa; padding: 10px; border-radius: 3px; border-left: 4px solid #007acc; overflow-x: auto; }");
        sb.AppendLine("        pre { margin: 0; white-space: pre-wrap; }");
        sb.AppendLine("        .collapsible { display: none; }");
        sb.AppendLine("        .collapsible.show { display: block; }");
        sb.AppendLine("    </style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("    <div class='container'>");
        sb.AppendLine("        <div class='header'>");
        sb.AppendLine($"            <h1>🚀 API Test Report: {testName}</h1>");
        sb.AppendLine($"            <p>Сгенерировано: {DateTime.Now:dd.MM.yyyy HH:mm:ss}</p>");
        sb.AppendLine("        </div>");

        // Статистика
        var totalCalls = _apiCalls.Count;
        var successCalls = _apiCalls.Count(c => (int)c.ResponseStatusCode >= 200 && (int)c.ResponseStatusCode < 300);
        var avgDuration = _apiCalls.Any() ? _apiCalls.Average(c => c.Duration.TotalMilliseconds) : 0;
        var totalDuration = _apiCalls.Sum(c => c.Duration.TotalMilliseconds);

        sb.AppendLine("        <div class='stats'>");
        sb.AppendLine($"            <div class='stat-card'><h3>{totalCalls}</h3><p>Всего вызовов</p></div>");
        sb.AppendLine($"            <div class='stat-card'><h3>{successCalls}</h3><p>Успешных</p></div>");
        sb.AppendLine($"            <div class='stat-card'><h3>{avgDuration:F0}ms</h3><p>Среднее время</p></div>");
        sb.AppendLine($"            <div class='stat-card'><h3>{totalDuration:F0}ms</h3><p>Общее время</p></div>");
        sb.AppendLine("        </div>");

        // API вызовы
        for (int i = 0; i < _apiCalls.Count; i++)
        {
            var call = _apiCalls[i];
            var isSuccess = (int)call.ResponseStatusCode >= 200 && (int)call.ResponseStatusCode < 300;
            
            sb.AppendLine("        <div class='api-call'>");
            sb.AppendLine($"            <div class='api-call-header' onclick='toggleCollapse({i})'>");
            sb.AppendLine("                <div>");
            sb.AppendLine($"                    <span class='method {call.Method}'>{call.Method}</span>");
            sb.AppendLine($"                    <strong>{call.Url}</strong>");
            sb.AppendLine($"                    {(!string.IsNullOrEmpty(call.Description) ? $"- {call.Description}" : "")}");
            sb.AppendLine("                </div>");
            sb.AppendLine("                <div>");
            sb.AppendLine($"                    <span class='status {(isSuccess ? "success" : "error")}'>{(int)call.ResponseStatusCode} {call.ResponseStatusCode}</span>");
            sb.AppendLine($"                    <span style='margin-left: 10px;'>{call.Duration.TotalMilliseconds:F0}ms</span>");
            sb.AppendLine("                </div>");
            sb.AppendLine("            </div>");
            sb.AppendLine($"            <div id='collapse-{i}' class='api-call-body collapsible'>");
            
            if (!string.IsNullOrEmpty(call.RequestBody))
            {
                sb.AppendLine("                <h4>📤 Request Body:</h4>");
                sb.AppendLine("                <div class='json'>");
                sb.AppendLine($"                    <pre>{call.RequestBody}</pre>");
                sb.AppendLine("                </div>");
            }
            
            sb.AppendLine("                <h4>📥 Response Body:</h4>");
            sb.AppendLine("                <div class='json'>");
            sb.AppendLine($"                    <pre>{call.ResponseBody}</pre>");
            sb.AppendLine("                </div>");
            sb.AppendLine("            </div>");
            sb.AppendLine("        </div>");
        }

        sb.AppendLine("    </div>");
        sb.AppendLine("    <script>");
        sb.AppendLine("        function toggleCollapse(index) {");
        sb.AppendLine("            const element = document.getElementById('collapse-' + index);");
        sb.AppendLine("            element.classList.toggle('show');");
        sb.AppendLine("        }");
        sb.AppendLine("    </script>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        return sb.ToString();
    }

    public void Dispose()
    {
        Client?.Dispose();
        Context?.Dispose();
    }
}

/// <summary>
/// Информация об API вызове для логирования
/// </summary>
public class ApiCallLog
{
    public string Method { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? RequestBody { get; set; }
    public HttpStatusCode ResponseStatusCode { get; set; }
    public string ResponseBody { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public string? Description { get; set; }
    public DateTime Timestamp { get; set; }
}