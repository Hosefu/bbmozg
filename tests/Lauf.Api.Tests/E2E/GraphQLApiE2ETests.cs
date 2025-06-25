using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Lauf.Api.Tests.Infrastructure;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Lauf.Api.Tests.E2E;

/// <summary>
/// E2E тесты для GraphQL API
/// </summary>
public class GraphQLApiE2ETests : ExtendedApiTestBase
{
    public GraphQLApiE2ETests(WebApplicationFactory<Program> factory, ITestOutputHelper output) 
        : base(factory, output)
    {
    }

    [Fact]
    public async Task GraphQL_CompleteQueriesAndMutations_ShouldWorkCorrectly()
    {
        Output.WriteLine("🔍 === ТЕСТИРОВАНИЕ GRAPHQL API ===");
        
        await ClearDatabase();

        // === ЭТАП 1: ТЕСТИРОВАНИЕ ИНТРОСПЕКЦИИ ===
        Output.WriteLine("🔍 === ЭТАП 1: ТЕСТИРОВАНИЕ ИНТРОСПЕКЦИИ ===");
        
        await TestIntrospectionQuery();

        // === ЭТАП 2: БАЗОВЫЕ ЗАПРОСЫ ===
        Output.WriteLine("📊 === ЭТАП 2: БАЗОВЫЕ ЗАПРОСЫ ===");
        
        await TestBasicQueries();

        // === ЭТАП 3: СОЗДАНИЕ ДАННЫХ ЧЕРЕЗ МУТАЦИИ ===
        Output.WriteLine("✏️ === ЭТАП 3: СОЗДАНИЕ ДАННЫХ ЧЕРЕЗ МУТАЦИИ ===");
        
        var userId = await TestCreateUserMutation();
        var flowId = await TestCreateFlowMutation();

        // === ЭТАП 4: СЛОЖНЫЕ ЗАПРОСЫ С ДАННЫМИ ===
        Output.WriteLine("🔍 === ЭТАП 4: СЛОЖНЫЕ ЗАПРОСЫ С ДАННЫМИ ===");
        
        await TestComplexQueries(userId, flowId);

        // === ЭТАП 5: НАЗНАЧЕНИЕ ПОТОКА ===
        Output.WriteLine("🎯 === ЭТАП 5: НАЗНАЧЕНИЕ ПОТОКА ===");
        
        var assignmentId = await TestAssignFlowMutation(flowId, userId);

        // === ЭТАП 6: ЗАПРОСЫ ПРОГРЕССА ===
        Output.WriteLine("📈 === ЭТАП 6: ЗАПРОСЫ ПРОГРЕССА ===");
        
        await TestProgressQueries(assignmentId);

        // Генерируем HTML отчёт
        GenerateApiCallReport("GraphQLApiE2E");
        
        Output.WriteLine("🎊 === GRAPHQL ТЕСТЫ ЗАВЕРШЕНЫ УСПЕШНО! ===");
    }

    private async Task TestIntrospectionQuery()
    {
        var introspectionQuery = new
        {
            query = @"
                query IntrospectionQuery {
                    __schema {
                        types {
                            name
                            kind
                        }
                        queryType {
                            name
                        }
                        mutationType {
                            name
                        }
                    }
                }"
        };

        var response = await PostGraphQLQuery(introspectionQuery, "Интроспекция GraphQL схемы");
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("__schema");
        content.Should().Contain("Query");
        content.Should().Contain("Mutation");

        Output.WriteLine("✅ Интроспекция GraphQL схемы работает корректно");
    }

    private async Task TestBasicQueries()
    {
        // Тест запроса пользователей
        var usersQuery = new
        {
            query = @"
                query GetUsers {
                    users {
                        id
                        firstName
                        lastName
                        email
                        isActive
                    }
                }"
        };

        var response = await PostGraphQLQuery(usersQuery, "Запрос списка пользователей");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Тест запроса потоков
        var flowsQuery = new
        {
            query = @"
                query GetFlows {
                    flows {
                        id
                        title
                        description
                        status
                        totalSteps
                    }
                }"
        };

        response = await PostGraphQLQuery(flowsQuery, "Запрос списка потоков");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        Output.WriteLine("✅ Базовые запросы работают корректно");
    }

    private async Task<Guid> TestCreateUserMutation()
    {
        var createUserMutation = new
        {
            query = @"
                mutation CreateUser($input: CreateUserInput!) {
                    createUser(input: $input) {
                        id
                        firstName
                        lastName
                        email
                        position
                        isActive
                    }
                }",
            variables = new
            {
                input = new
                {
                    firstName = "Алексей",
                    lastName = "GraphQL",
                    email = "alexey.graphql@lauf.com",
                    position = "QA Engineer",
                    telegramUserId = 4000001,
                    language = "ru"
                }
            }
        };

        var response = await PostGraphQLQuery(createUserMutation, "Создание пользователя через GraphQL");
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<dynamic>(content);
        
        result.Should().NotBeNull();
        result.data.Should().NotBeNull();
        result.data.createUser.Should().NotBeNull();
        
        var userId = Guid.Parse(result.data.createUser.id.ToString());
        
        Output.WriteLine($"✅ Пользователь создан через GraphQL: {result.data.createUser.firstName} {result.data.createUser.lastName} (ID: {userId})");
        
        return userId;
    }

    private async Task<Guid> TestCreateFlowMutation()
    {
        var createFlowMutation = new
        {
            query = @"
                mutation CreateFlow($input: CreateFlowInput!) {
                    createFlow(input: $input) {
                        id
                        title
                        description
                        category
                        status
                        estimatedDurationMinutes
                    }
                }",
            variables = new
            {
                input = new
                {
                    title = "🧪 GraphQL Test Flow",
                    description = "Поток для тестирования GraphQL API",
                    category = "Тестирование",
                    tags = "test,graphql,api",
                    estimatedDurationMinutes = 120,
                    requiredRole = "QA",
                    isRequired = false,
                    priority = "MEDIUM"
                }
            }
        };

        var response = await PostGraphQLQuery(createFlowMutation, "Создание потока через GraphQL");
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<dynamic>(content);
        
        result.Should().NotBeNull();
        result.data.Should().NotBeNull();
        result.data.createFlow.Should().NotBeNull();
        
        var flowId = Guid.Parse(result.data.createFlow.id.ToString());
        
        Output.WriteLine($"✅ Поток создан через GraphQL: {result.data.createFlow.title} (ID: {flowId})");
        
        return flowId;
    }

    private async Task TestComplexQueries(Guid userId, Guid flowId)
    {
        // Сложный запрос с вложенными данными
        var complexQuery = new
        {
            query = @"
                query GetFlowDetails($flowId: ID!, $userId: ID) {
                    flowDetails(flowId: $flowId, userId: $userId) {
                        id
                        title
                        description
                        status
                        totalSteps
                        estimatedDurationMinutes
                        steps {
                            id
                            title
                            description
                            order
                            isRequired
                            estimatedDurationMinutes
                        }
                        statistics {
                            totalAssignments
                            activeAssignments
                            completedAssignments
                            averageProgress
                        }
                        userProgress {
                            overallProgress
                            currentStep
                            completedSteps
                            totalSteps
                            status
                        }
                    }
                }",
            variables = new
            {
                flowId = flowId.ToString(),
                userId = userId.ToString()
            }
        };

        var response = await PostGraphQLQuery(complexQuery, "Сложный запрос деталей потока");
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("flowDetails");

        Output.WriteLine("✅ Сложные запросы с вложенными данными работают корректно");
    }

    private async Task<Guid> TestAssignFlowMutation(Guid flowId, Guid userId)
    {
        var assignFlowMutation = new
        {
            query = @"
                mutation AssignFlow($input: AssignFlowInput!) {
                    assignFlow(input: $input) {
                        id
                        flowId
                        userId
                        status
                        priority
                        dueDate
                        notes
                    }
                }",
            variables = new
            {
                input = new
                {
                    flowId = flowId.ToString(),
                    userId = userId.ToString(),
                    dueDate = DateTime.UtcNow.AddDays(7).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    priority = "HIGH",
                    notes = "Назначение через GraphQL для тестирования"
                }
            }
        };

        var response = await PostGraphQLQuery(assignFlowMutation, "Назначение потока через GraphQL");
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<dynamic>(content);
        
        result.Should().NotBeNull();
        result.data.Should().NotBeNull();
        result.data.assignFlow.Should().NotBeNull();
        
        var assignmentId = Guid.Parse(result.data.assignFlow.id.ToString());
        
        Output.WriteLine($"✅ Поток назначен через GraphQL (Assignment ID: {assignmentId})");
        
        return assignmentId;
    }

    private async Task TestProgressQueries(Guid assignmentId)
    {
        var progressQuery = new
        {
            query = @"
                query GetProgress($assignmentId: ID!) {
                    progressByAssignment(assignmentId: $assignmentId) {
                        assignmentId
                        overallProgress
                        flowInfo {
                            id
                            title
                            totalSteps
                            estimatedHours
                        }
                        userInfo {
                            id
                            firstName
                            lastName
                            email
                        }
                        status
                        startedAt
                        dueDate
                        isOverdue
                        daysUntilDeadline
                        timeStats {
                            daysSinceStart
                            totalTimeSpent
                            averageTimePerDay
                            estimatedTimeToCompletion
                        }
                    }
                }",
            variables = new
            {
                assignmentId = assignmentId.ToString()
            }
        };

        var response = await PostGraphQLQuery(progressQuery, "Запрос прогресса назначения");
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("progressByAssignment");

        Output.WriteLine("✅ Запросы прогресса работают корректно");
    }

    /// <summary>
    /// Отправляет GraphQL запрос
    /// </summary>
    private async Task<HttpResponseMessage> PostGraphQLQuery(object query, string description)
    {
        var json = JsonConvert.SerializeObject(query, Formatting.Indented);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        Output.WriteLine($"🔍 GraphQL Query: {description}");
        Output.WriteLine($"   Query: {json}");
        
        var startTime = DateTime.UtcNow;
        var response = await Client.PostAsync("/graphql", content);
        var duration = DateTime.UtcNow - startTime;
        
        var responseBody = await response.Content.ReadAsStringAsync();
        
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
        
        return response;
    }
}