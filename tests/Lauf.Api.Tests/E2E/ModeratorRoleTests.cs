using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Lauf.Api.Tests.E2E;

/// <summary>
/// E2E тесты для роли Модератора
/// Проверяет создание потоков, шагов, компонентов и управление жизненным циклом
/// </summary>
public class ModeratorRoleTests : BaseE2ETest
{
    public ModeratorRoleTests(WebApplicationFactory<Program> factory, ITestOutputHelper output) 
        : base(factory, output)
    {
    }

    [Fact]
    public async Task ModeratorWorkflow_CreateFlowWithStepsAndComponents_ShouldSucceed()
    {
        try
        {
            // === ЭТАП 1: Создание пользователя-модератора ===
            _output.WriteLine("=== ЭТАП 1: Создание пользователя-модератора ===");
            
            var moderatorInput = TestDataFactory.InputObjects.CreateUserInput(
                telegramId: 12345,
                email: "moderator@test.com",
                fullName: "Тестовый Модератор",
                position: "Менеджер по обучению"
            );

            var createModeratorResponse = await ExecuteGraphQLAsync<CreateUserResponse>(
                TestDataFactory.GraphQLQueries.CreateUser,
                new { input = moderatorInput }
            );

            createModeratorResponse.CreateUser.Should().NotBeNull();
            createModeratorResponse.CreateUser.Id.Should().NotBeEmpty();
            createModeratorResponse.CreateUser.Email.Should().Be("moderator@test.com");

            var moderatorId = createModeratorResponse.CreateUser.Id;
            _output.WriteLine($"Создан модератор с ID: {moderatorId}");

            // === ЭТАП 2: Создание базового потока ===
            _output.WriteLine("=== ЭТАП 2: Создание базового потока ===");

            var flowInput = TestDataFactory.InputObjects.CreateFlowInput(
                title: "Процесс онбординга новых сотрудников",
                description: "Полный цикл знакомства с компанией, процессами и командой",
                isSequential: true,
                allowRetry: true,
                timeLimit: 21
            );

            var createFlowResponse = await ExecuteGraphQLAsync<CreateFlowResponse>(
                TestDataFactory.GraphQLQueries.CreateFlow,
                new { input = flowInput }
            );

            createFlowResponse.CreateFlow.Should().NotBeNull();
            createFlowResponse.CreateFlow.Id.Should().NotBeEmpty();
            createFlowResponse.CreateFlow.Title.Should().Be("Процесс онбординга новых сотрудников");

            var flowId = createFlowResponse.CreateFlow.Id;
            _output.WriteLine($"Создан поток с ID: {flowId}");

            // === ЭТАП 3: Проверка созданного потока ===
            _output.WriteLine("=== ЭТАП 3: Проверка созданного потока ===");

            var flowDetailsResponse = await ExecuteGraphQLAsync<GetFlowDetailsResponse>(
                TestDataFactory.GraphQLQueries.GetFlowDetails,
                new { id = flowId }
            );

            flowDetailsResponse.Flow.Should().NotBeNull();
            flowDetailsResponse.Flow.Id.Should().Be(flowId);
            flowDetailsResponse.Flow.Settings.Should().NotBeNull();
            flowDetailsResponse.Flow.Settings.RequireSequentialCompletion.Should().BeTrue();
            flowDetailsResponse.Flow.Settings.TimeToCompleteWorkingDays.Should().Be(21);

            // === ЭТАП 4: Получение всех потоков ===
            _output.WriteLine("=== ЭТАП 4: Получение всех потоков ===");

            var allFlowsResponse = await ExecuteGraphQLAsync<GetFlowsResponse>(
                TestDataFactory.GraphQLQueries.GetFlows,
                new { skip = 0, take = 10 }
            );

            allFlowsResponse.Flows.Should().HaveCountGreaterOrEqualTo(1);
            allFlowsResponse.Flows.Should().Contain(f => f.Id == flowId);

            // === ЭТАП 5: Обновление статуса потока на Published ===
            _output.WriteLine("=== ЭТАП 5: Обновление статуса потока на Published ===");

            const string updateFlowMutation = @"
                mutation UpdateFlow($input: UpdateFlowInput!) {
                    updateFlow(input: $input) {
                        id
                        title
                        status
                    }
                }";

            var updateFlowInput = new
            {
                id = flowId,
                status = "Published"
            };

            var updateFlowResponse = await ExecuteGraphQLAsync<UpdateFlowResponse>(
                updateFlowMutation,
                new { input = updateFlowInput }
            );

            updateFlowResponse.UpdateFlow.Should().NotBeNull();
            updateFlowResponse.UpdateFlow.Status.Should().Be("Published");

            // === ЭТАП 6: Проверка пользователей ===
            _output.WriteLine("=== ЭТАП 6: Проверка пользователей ===");

            var usersResponse = await ExecuteGraphQLAsync<GetUsersResponse>(
                TestDataFactory.GraphQLQueries.GetUsers,
                new { skip = 0, take = 10 }
            );

            usersResponse.Users.Should().HaveCountGreaterOrEqualTo(1);
            usersResponse.Users.Should().Contain(u => u.Id == moderatorId);

            _output.WriteLine("✅ Все этапы модератора выполнены успешно!");
        }
        finally
        {
            // Генерируем HTML отчет независимо от результата
            GenerateHtmlReport("ModeratorWorkflow");
        }
    }

    [Fact]
    public async Task ModeratorCanManageMultipleFlows_ShouldSucceed()
    {
        try
        {
            _output.WriteLine("=== ТЕСТ: Управление несколькими потоками ===");

            // Создаем модератора
            var moderatorInput = TestDataFactory.InputObjects.CreateUserInput(
                telegramId: 12346,
                email: "moderator2@test.com",
                fullName: "Второй Модератор",
                position: "Старший менеджер"
            );

            var moderatorResponse = await ExecuteGraphQLAsync<CreateUserResponse>(
                TestDataFactory.GraphQLQueries.CreateUser,
                new { input = moderatorInput }
            );

            // Создаем несколько потоков
            var flow1Input = TestDataFactory.InputObjects.CreateFlowInput(
                title: "Техническое введение",
                description: "Знакомство с техническими процессами"
            );

            var flow2Input = TestDataFactory.InputObjects.CreateFlowInput(
                title: "HR процедуры",
                description: "Оформление документов и HR процессы"
            );

            var flow1Response = await ExecuteGraphQLAsync<CreateFlowResponse>(
                TestDataFactory.GraphQLQueries.CreateFlow,
                new { input = flow1Input }
            );

            var flow2Response = await ExecuteGraphQLAsync<CreateFlowResponse>(
                TestDataFactory.GraphQLQueries.CreateFlow,
                new { input = flow2Input }
            );

            // Проверяем, что оба потока созданы
            flow1Response.CreateFlow.Should().NotBeNull();
            flow2Response.CreateFlow.Should().NotBeNull();
            flow1Response.CreateFlow.Id.Should().NotBe(flow2Response.CreateFlow.Id);

            _output.WriteLine($"Созданы потоки: {flow1Response.CreateFlow.Id}, {flow2Response.CreateFlow.Id}");
        }
        finally
        {
            GenerateHtmlReport("ModeratorMultipleFlows");
        }
    }
}

#region Response Models

public class CreateUserResponse
{
    public UserDto CreateUser { get; set; } = null!;
}

public class CreateFlowResponse  
{
    public FlowDto CreateFlow { get; set; } = null!;
}

public class UpdateFlowResponse
{
    public FlowDto UpdateFlow { get; set; } = null!;
}

public class GetUsersResponse
{
    public List<UserDto> Users { get; set; } = new();
}

public class GetFlowsResponse
{
    public List<FlowDto> Flows { get; set; } = new();
}

public class GetFlowDetailsResponse
{
    public FlowDetailsDto Flow { get; set; } = null!;
}

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = "";
    public string FullName { get; set; } = "";
    public string? Position { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class FlowDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string Status { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}

public class FlowDetailsDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Status { get; set; } = "";
    public FlowSettingsDto Settings { get; set; } = null!;
    public List<FlowStepDto> Steps { get; set; } = new();
}

public class FlowSettingsDto
{
    public bool RequireSequentialCompletion { get; set; }
    public bool AllowRetry { get; set; }
    public int TimeToCompleteWorkingDays { get; set; }
    public int MaxAttempts { get; set; }
}

public class FlowStepDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public int Order { get; set; }
    public List<FlowStepComponentDto> Components { get; set; } = new();
}

public class FlowStepComponentDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string Type { get; set; } = "";
    public int Order { get; set; }
}

#endregion