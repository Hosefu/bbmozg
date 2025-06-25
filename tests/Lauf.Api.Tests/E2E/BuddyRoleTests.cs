using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Lauf.Api.Tests.E2E;

/// <summary>
/// E2E тесты для роли Бадди (Наставника)
/// Проверяет назначение потоков пользователям, мониторинг прогресса и управление процессом обучения
/// </summary>
public class BuddyRoleTests : BaseE2ETest
{
    public BuddyRoleTests(WebApplicationFactory<Program> factory, ITestOutputHelper output) 
        : base(factory, output)
    {
    }

    [Fact]
    public async Task BuddyWorkflow_AssignFlowToUserAndMonitor_ShouldSucceed()
    {
        try
        {
            // === ЭТАП 1: Создание всех участников процесса ===
            _output.WriteLine("=== ЭТАП 1: Создание всех участников процесса ===");
            
            // Создаем модератора
            var moderatorInput = TestDataFactory.InputObjects.CreateUserInput(
                telegramId: 11111,
                email: "moderator@company.com",
                fullName: "Главный Модератор",
                position: "Менеджер обучения"
            );

            var moderatorResponse = await ExecuteGraphQLAsync<CreateUserResponse>(
                TestDataFactory.GraphQLQueries.CreateUser,
                new { input = moderatorInput }
            );

            // Создаем бадди
            var buddyInput = TestDataFactory.InputObjects.CreateUserInput(
                telegramId: 22222,
                email: "buddy@company.com",
                fullName: "Опытный Наставник",
                position: "Senior Developer"
            );

            var buddyResponse = await ExecuteGraphQLAsync<CreateUserResponse>(
                TestDataFactory.GraphQLQueries.CreateUser,
                new { input = buddyInput }
            );

            // Создаем нового сотрудника
            var newEmployeeInput = TestDataFactory.InputObjects.CreateUserInput(
                telegramId: 33333,
                email: "newbie@company.com",
                fullName: "Новый Сотрудник",
                position: "Junior Developer"
            );

            var newEmployeeResponse = await ExecuteGraphQLAsync<CreateUserResponse>(
                TestDataFactory.GraphQLQueries.CreateUser,
                new { input = newEmployeeInput }
            );

            var moderatorId = moderatorResponse.CreateUser.Id;
            var buddyId = buddyResponse.CreateUser.Id;
            var newEmployeeId = newEmployeeResponse.CreateUser.Id;

            _output.WriteLine($"Созданы пользователи: Модератор({moderatorId}), Бадди({buddyId}), Новичок({newEmployeeId})");

            // === ЭТАП 2: Модератор создает поток для онбординга ===
            _output.WriteLine("=== ЭТАП 2: Модератор создает поток для онбординга ===");

            var flowInput = TestDataFactory.InputObjects.CreateFlowInput(
                title: "Онбординг разработчиков",
                description: "Специальная программа для новых разработчиков",
                isSequential: true,
                allowRetry: true,
                timeLimit: 30
            );

            var flowResponse = await ExecuteGraphQLAsync<CreateFlowResponse>(
                TestDataFactory.GraphQLQueries.CreateFlow,
                new { input = flowInput }
            );

            var flowId = flowResponse.CreateFlow.Id;
            _output.WriteLine($"Создан поток: {flowId}");

            // Публикуем поток
            const string updateFlowMutation = @"
                mutation UpdateFlow($input: UpdateFlowInput!) {
                    updateFlow(input: $input) {
                        id
                        status
                    }
                }";

            await ExecuteGraphQLAsync<UpdateFlowResponse>(
                updateFlowMutation,
                new { input = new { id = flowId, status = "Published" } }
            );

            // === ЭТАП 3: Бадди назначает поток новому сотруднику ===
            _output.WriteLine("=== ЭТАП 3: Бадди назначает поток новому сотруднику ===");

            var assignFlowInput = TestDataFactory.InputObjects.AssignFlowInput(
                userId: newEmployeeId,
                flowId: flowId,
                dueDate: DateTime.UtcNow.AddDays(30),
                assignedBy: buddyId
            );

            var assignFlowResponse = await ExecuteGraphQLAsync<AssignFlowResponse>(
                TestDataFactory.GraphQLQueries.AssignFlow,
                new { input = assignFlowInput }
            );

            assignFlowResponse.AssignFlow.Should().NotBeNull();
            assignFlowResponse.AssignFlow.Success.Should().BeTrue();
            assignFlowResponse.AssignFlow.AssignmentId.Should().NotBeEmpty();

            var assignmentId = assignFlowResponse.AssignFlow.AssignmentId;
            _output.WriteLine($"Поток назначен, ID назначения: {assignmentId}");

            // === ЭТАП 4: Проверка назначений (мониторинг бадди) ===
            _output.WriteLine("=== ЭТАП 4: Проверка назначений (мониторинг бадди) ===");

            // Получаем все назначения конкретного пользователя
            var userAssignmentsResponse = await ExecuteGraphQLAsync<GetFlowAssignmentsResponse>(
                TestDataFactory.GraphQLQueries.GetFlowAssignments,
                new { userId = newEmployeeId, skip = 0, take = 10 }
            );

            userAssignmentsResponse.Assignments.Should().HaveCount(1);
            userAssignmentsResponse.Assignments[0].UserId.Should().Be(newEmployeeId);
            userAssignmentsResponse.Assignments[0].FlowId.Should().Be(flowId);
            userAssignmentsResponse.Assignments[0].Status.Should().Be("Assigned");

            // Получаем все назначения конкретного потока
            var flowAssignmentsResponse = await ExecuteGraphQLAsync<GetFlowAssignmentsResponse>(
                TestDataFactory.GraphQLQueries.GetFlowAssignments,
                new { flowId = flowId, skip = 0, take = 10 }
            );

            flowAssignmentsResponse.Assignments.Should().HaveCount(1);
            flowAssignmentsResponse.Assignments[0].Id.Should().Be(assignmentId);

            // === ЭТАП 5: Получение активных назначений ===
            _output.WriteLine("=== ЭТАП 5: Получение активных назначений ===");

            var activeAssignmentsResponse = await ExecuteGraphQLAsync<GetActiveAssignmentsResponse>(
                TestDataFactory.GraphQLQueries.GetActiveAssignments,
                new { userId = newEmployeeId }
            );

            activeAssignmentsResponse.ActiveAssignments.Should().HaveCount(1);
            activeAssignmentsResponse.ActiveAssignments[0].Id.Should().Be(assignmentId);

            // === ЭТАП 6: Проверка статистики (Dashboard бадди) ===
            _output.WriteLine("=== ЭТАП 6: Проверка статистики (Dashboard бадди) ===");

            // Получаем все назначения для мониторинга
            var allAssignmentsResponse = await ExecuteGraphQLAsync<GetFlowAssignmentsResponse>(
                TestDataFactory.GraphQLQueries.GetFlowAssignments,
                new { skip = 0, take = 100 }
            );

            allAssignmentsResponse.Assignments.Should().HaveCountGreaterOrEqualTo(1);
            
            var assignedCount = allAssignmentsResponse.Assignments.Count(a => a.Status == "Assigned");
            var inProgressCount = allAssignmentsResponse.Assignments.Count(a => a.Status == "InProgress");
            var completedCount = allAssignmentsResponse.Assignments.Count(a => a.Status == "Completed");

            _output.WriteLine($"Статистика: Назначено({assignedCount}), В процессе({inProgressCount}), Завершено({completedCount})");

            _output.WriteLine("✅ Все этапы работы бадди выполнены успешно!");
        }
        finally
        {
            GenerateHtmlReport("BuddyWorkflow");
        }
    }

    [Fact]
    public async Task BuddyCanAssignMultipleFlowsToMultipleUsers_ShouldSucceed()
    {
        try
        {
            _output.WriteLine("=== ТЕСТ: Массовое назначение потоков ===");

            // Создаем бадди
            var buddyInput = TestDataFactory.InputObjects.CreateUserInput(
                telegramId: 44444,
                email: "buddy2@company.com",
                fullName: "Массовый Наставник",
                position: "Team Lead"
            );

            var buddyResponse = await ExecuteGraphQLAsync<CreateUserResponse>(
                TestDataFactory.GraphQLQueries.CreateUser,
                new { input = buddyInput }
            );

            // Создаем нескольких пользователей
            var user1Response = await ExecuteGraphQLAsync<CreateUserResponse>(
                TestDataFactory.GraphQLQueries.CreateUser,
                new { input = TestDataFactory.InputObjects.CreateUserInput(55555, "user1@company.com", "Первый Новичок", "Designer") }
            );

            var user2Response = await ExecuteGraphQLAsync<CreateUserResponse>(
                TestDataFactory.GraphQLQueries.CreateUser,
                new { input = TestDataFactory.InputObjects.CreateUserInput(66666, "user2@company.com", "Второй Новичок", "QA Engineer") }
            );

            // Создаем несколько потоков
            var flow1Response = await ExecuteGraphQLAsync<CreateFlowResponse>(
                TestDataFactory.GraphQLQueries.CreateFlow,
                new { input = TestDataFactory.InputObjects.CreateFlowInput("Дизайн процессы", "Обучение дизайнеров") }
            );

            var flow2Response = await ExecuteGraphQLAsync<CreateFlowResponse>(
                TestDataFactory.GraphQLQueries.CreateFlow,
                new { input = TestDataFactory.InputObjects.CreateFlowInput("QA процессы", "Обучение тестировщиков") }
            );

            // Назначаем разные потоки разным пользователям
            var assignment1Response = await ExecuteGraphQLAsync<AssignFlowResponse>(
                TestDataFactory.GraphQLQueries.AssignFlow,
                new { input = TestDataFactory.InputObjects.AssignFlowInput(user1Response.CreateUser.Id, flow1Response.CreateFlow.Id, DateTime.UtcNow.AddDays(21), buddyResponse.CreateUser.Id) }
            );

            var assignment2Response = await ExecuteGraphQLAsync<AssignFlowResponse>(
                TestDataFactory.GraphQLQueries.AssignFlow,
                new { input = TestDataFactory.InputObjects.AssignFlowInput(user2Response.CreateUser.Id, flow2Response.CreateFlow.Id, DateTime.UtcNow.AddDays(21), buddyResponse.CreateUser.Id) }
            );

            // Проверяем, что все назначения созданы
            assignment1Response.AssignFlow.Success.Should().BeTrue();
            assignment2Response.AssignFlow.Success.Should().BeTrue();

            _output.WriteLine($"Созданы назначения: {assignment1Response.AssignFlow.AssignmentId}, {assignment2Response.AssignFlow.AssignmentId}");
        }
        finally
        {
            GenerateHtmlReport("BuddyMultipleAssignments");
        }
    }

    [Fact]
    public async Task BuddyCanMonitorOverdueAssignments_ShouldSucceed()
    {
        try
        {
            _output.WriteLine("=== ТЕСТ: Мониторинг просроченных назначений ===");

            // Получаем просроченные назначения
            var overdueAssignmentsResponse = await ExecuteGraphQLAsync<GetOverdueAssignmentsResponse>(
                TestDataFactory.GraphQLQueries.GetOverdueAssignments
            );

            // В новой тестовой среде просроченных назначений пока не должно быть
            overdueAssignmentsResponse.OverdueAssignments.Should().NotBeNull();
            _output.WriteLine($"Найдено просроченных назначений: {overdueAssignmentsResponse.OverdueAssignments.Count}");
        }
        finally
        {
            GenerateHtmlReport("BuddyOverdueMonitoring");
        }
    }
}

#region Additional Response Models

public class AssignFlowResponse
{
    public AssignFlowResult AssignFlow { get; set; } = null!;
}

public class AssignFlowResult
{
    public Guid AssignmentId { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = "";
}

public class GetFlowAssignmentsResponse
{
    public List<FlowAssignmentDto> Assignments { get; set; } = new();
}

public class GetActiveAssignmentsResponse
{
    public List<FlowAssignmentDto> ActiveAssignments { get; set; } = new();
}

public class GetOverdueAssignmentsResponse
{
    public List<FlowAssignmentDto> OverdueAssignments { get; set; } = new();
}

public class FlowAssignmentDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid FlowId { get; set; }
    public string Status { get; set; } = "";
    public DateTime AssignedAt { get; set; }
    public DateTime? Deadline { get; set; }
    public int Progress { get; set; }
}

#endregion