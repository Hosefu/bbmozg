using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Lauf.Api.Tests.E2E;

/// <summary>
/// E2E тесты для роли Пользователя
/// Проверяет полный цикл прохождения обучения: получение назначений, изучение материалов, 
/// выполнение тестов и заданий, отслеживание прогресса
/// </summary>
public class UserRoleTests : BaseE2ETest
{
    public UserRoleTests(WebApplicationFactory<Program> factory, ITestOutputHelper output) 
        : base(factory, output)
    {
    }

    [Fact]
    public async Task UserLearningJourney_CompleteFlowWithAllComponents_ShouldSucceed()
    {
        try
        {
            // === ЭТАП 1: Подготовка - создание всех участников ===
            _output.WriteLine("=== ЭТАП 1: Подготовка - создание всех участников ===");
            
            // Создаем модератора для настройки потока
            var moderatorResponse = await ExecuteGraphQLAsync<CreateUserResponse>(
                TestDataFactory.GraphQLQueries.CreateUser,
                new { input = TestDataFactory.InputObjects.CreateUserInput(77777, "moderator3@company.com", "Модератор Обучения", "Learning Manager") }
            );

            // Создаем бадди для назначения
            var buddyResponse = await ExecuteGraphQLAsync<CreateUserResponse>(
                TestDataFactory.GraphQLQueries.CreateUser,
                new { input = TestDataFactory.InputObjects.CreateUserInput(88888, "buddy3@company.com", "Персональный Наставник", "Senior Specialist") }
            );

            // Создаем ученика
            var learnerResponse = await ExecuteGraphQLAsync<CreateUserResponse>(
                TestDataFactory.GraphQLQueries.CreateUser,
                new { input = TestDataFactory.InputObjects.CreateUserInput(99999, "learner@company.com", "Активный Ученик", "Trainee") }
            );

            var moderatorId = moderatorResponse.CreateUser.Id;
            var buddyId = buddyResponse.CreateUser.Id;
            var learnerId = learnerResponse.CreateUser.Id;

            _output.WriteLine($"Участники: Модератор({moderatorId}), Бадди({buddyId}), Ученик({learnerId})");

            // === ЭТАП 2: Модератор создает детальный поток обучения ===
            _output.WriteLine("=== ЭТАП 2: Модератор создает детальный поток обучения ===");

            var flowResponse = await ExecuteGraphQLAsync<CreateFlowResponse>(
                TestDataFactory.GraphQLQueries.CreateFlow,
                new { input = TestDataFactory.InputObjects.CreateFlowInput(
                    "Полный курс адаптации",
                    "Комплексная программа введения в должность с теорией и практикой",
                    isSequential: true,
                    allowRetry: true,
                    timeLimit: 45
                )}
            );

            var flowId = flowResponse.CreateFlow.Id;

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

            _output.WriteLine($"Создан и опубликован поток: {flowId}");

            // === ЭТАП 3: Бадди назначает поток ученику ===
            _output.WriteLine("=== ЭТАП 3: Бадди назначает поток ученику ===");

            var assignFlowResponse = await ExecuteGraphQLAsync<AssignFlowResponse>(
                TestDataFactory.GraphQLQueries.AssignFlow,
                new { input = TestDataFactory.InputObjects.AssignFlowInput(
                    userId: learnerId,
                    flowId: flowId,
                    dueDate: DateTime.UtcNow.AddDays(45),
                    assignedBy: buddyId
                )}
            );

            assignFlowResponse.AssignFlow.Success.Should().BeTrue();
            var assignmentId = assignFlowResponse.AssignFlow.AssignmentId;

            _output.WriteLine($"Поток назначен ученику, ID назначения: {assignmentId}");

            // === ЭТАП 4: Ученик проверяет свои назначения ===
            _output.WriteLine("=== ЭТАП 4: Ученик проверяет свои назначения ===");

            var userAssignmentsResponse = await ExecuteGraphQLAsync<GetFlowAssignmentsResponse>(
                TestDataFactory.GraphQLQueries.GetFlowAssignments,
                new { userId = learnerId, skip = 0, take = 10 }
            );

            userAssignmentsResponse.Assignments.Should().HaveCount(1);
            userAssignmentsResponse.Assignments[0].Status.Should().Be("Assigned");
            userAssignmentsResponse.Assignments[0].Progress.Should().Be(0);

            // === ЭТАП 5: Ученик начинает прохождение потока ===
            _output.WriteLine("=== ЭТАП 5: Ученик начинает прохождение потока ===");

            var startFlowResponse = await ExecuteGraphQLAsync<StartFlowResponse>(
                TestDataFactory.GraphQLQueries.StartFlow,
                new { input = TestDataFactory.InputObjects.StartFlowInput(assignmentId) }
            );

            startFlowResponse.StartFlow.Should().NotBeNull();
            startFlowResponse.StartFlow.Status.Should().Be("InProgress");
            startFlowResponse.StartFlow.StartedAt.Should().NotBeNull();

            _output.WriteLine($"Поток запущен, статус: {startFlowResponse.StartFlow.Status}");

            // === ЭТАП 6: Получение детальной информации о потоке ===
            _output.WriteLine("=== ЭТАП 6: Получение детальной информации о потоке ===");

            var flowDetailsResponse = await ExecuteGraphQLAsync<GetFlowDetailsResponse>(
                TestDataFactory.GraphQLQueries.GetFlowDetails,
                new { id = flowId }
            );

            flowDetailsResponse.Flow.Should().NotBeNull();
            flowDetailsResponse.Flow.Title.Should().Be("Полный курс адаптации");
            flowDetailsResponse.Flow.Settings.RequireSequentialCompletion.Should().BeTrue();

            // === ЭТАП 7: Проверка активных назначений ===
            _output.WriteLine("=== ЭТАП 7: Проверка активных назначений ===");

            var activeAssignmentsResponse = await ExecuteGraphQLAsync<GetActiveAssignmentsResponse>(
                TestDataFactory.GraphQLQueries.GetActiveAssignments,
                new { userId = learnerId }
            );

            activeAssignmentsResponse.ActiveAssignments.Should().HaveCount(1);
            activeAssignmentsResponse.ActiveAssignments[0].Status.Should().Be("InProgress");

            // === ЭТАП 8: Получение прогресса пользователя ===
            _output.WriteLine("=== ЭТАП 8: Получение прогресса пользователя ===");

            const string getUserProgressQuery = @"
                query GetUserProgress($assignmentId: UUID!) {
                    assignment: getFlowAssignment(id: $assignmentId) {
                        id
                        status
                        progress
                        startedAt
                        completedAt
                        user {
                            id
                            fullName
                        }
                        flow {
                            id
                            title
                        }
                    }
                }";

            var userProgressResponse = await ExecuteGraphQLAsync<GetUserProgressResponse>(
                getUserProgressQuery,
                new { assignmentId }
            );

            userProgressResponse.Assignment.Should().NotBeNull();
            userProgressResponse.Assignment.Status.Should().Be("InProgress");
            userProgressResponse.Assignment.User.FullName.Should().Be("Активный Ученик");
            userProgressResponse.Assignment.Flow.Title.Should().Be("Полный курс адаптации");

            // === ЭТАП 9: Симуляция завершения потока ===
            _output.WriteLine("=== ЭТАП 9: Симуляция завершения потока ===");

            const string completeFlowMutation = @"
                mutation CompleteFlow($input: CompleteFlowInput!) {
                    completeFlow(input: $input) {
                        id
                        status
                        completedAt
                        progress
                    }
                }";

            var completeFlowResponse = await ExecuteGraphQLAsync<CompleteFlowResponse>(
                completeFlowMutation,
                new { input = new { 
                    assignmentId,
                    completionNotes = "Успешно изучил все материалы и выполнил задания"
                }}
            );

            completeFlowResponse.CompleteFlow.Should().NotBeNull();
            completeFlowResponse.CompleteFlow.Status.Should().Be("Completed");
            completeFlowResponse.CompleteFlow.Progress.Should().Be(100);
            completeFlowResponse.CompleteFlow.CompletedAt.Should().NotBeNull();

            _output.WriteLine($"Поток завершен! Прогресс: {completeFlowResponse.CompleteFlow.Progress}%");

            // === ЭТАП 10: Финальная проверка статуса ===
            _output.WriteLine("=== ЭТАП 10: Финальная проверка статуса ===");

            var finalStatusResponse = await ExecuteGraphQLAsync<GetUserProgressResponse>(
                getUserProgressQuery,
                new { assignmentId }
            );

            finalStatusResponse.Assignment.Status.Should().Be("Completed");
            finalStatusResponse.Assignment.Progress.Should().Be(100);

            _output.WriteLine("✅ Полный цикл обучения пользователя завершен успешно!");
        }
        finally
        {
            GenerateHtmlReport("UserLearningJourney");
        }
    }

    [Fact]
    public async Task UserCanViewMultipleAssignments_ShouldSucceed()
    {
        try
        {
            _output.WriteLine("=== ТЕСТ: Пользователь с несколькими назначениями ===");

            // Создаем пользователя
            var userResponse = await ExecuteGraphQLAsync<CreateUserResponse>(
                TestDataFactory.GraphQLQueries.CreateUser,
                new { input = TestDataFactory.InputObjects.CreateUserInput(111111, "multiuser@company.com", "Многозадачный Ученик", "Multi Trainee") }
            );

            // Создаем несколько потоков
            var flow1Response = await ExecuteGraphQLAsync<CreateFlowResponse>(
                TestDataFactory.GraphQLQueries.CreateFlow,
                new { input = TestDataFactory.InputObjects.CreateFlowInput("Техническая подготовка", "Изучение технических аспектов") }
            );

            var flow2Response = await ExecuteGraphQLAsync<CreateFlowResponse>(
                TestDataFactory.GraphQLQueries.CreateFlow,
                new { input = TestDataFactory.InputObjects.CreateFlowInput("Корпоративная культура", "Знакомство с культурой компании") }
            );

            // Назначаем оба потока пользователю
            await ExecuteGraphQLAsync<AssignFlowResponse>(
                TestDataFactory.GraphQLQueries.AssignFlow,
                new { input = TestDataFactory.InputObjects.AssignFlowInput(userResponse.CreateUser.Id, flow1Response.CreateFlow.Id) }
            );

            await ExecuteGraphQLAsync<AssignFlowResponse>(
                TestDataFactory.GraphQLQueries.AssignFlow,
                new { input = TestDataFactory.InputObjects.AssignFlowInput(userResponse.CreateUser.Id, flow2Response.CreateFlow.Id) }
            );

            // Проверяем, что пользователь видит оба назначения
            var userAssignmentsResponse = await ExecuteGraphQLAsync<GetFlowAssignmentsResponse>(
                TestDataFactory.GraphQLQueries.GetFlowAssignments,
                new { userId = userResponse.CreateUser.Id, skip = 0, take = 10 }
            );

            userAssignmentsResponse.Assignments.Should().HaveCount(2);
            userAssignmentsResponse.Assignments.Should().Contain(a => a.FlowId == flow1Response.CreateFlow.Id);
            userAssignmentsResponse.Assignments.Should().Contain(a => a.FlowId == flow2Response.CreateFlow.Id);

            _output.WriteLine($"Пользователь имеет {userAssignmentsResponse.Assignments.Count} назначений");
        }
        finally
        {
            GenerateHtmlReport("UserMultipleAssignments");
        }
    }
}

#region Additional Response Models for User Tests

public class StartFlowResponse
{
    public StartFlowResult StartFlow { get; set; } = null!;
}

public class StartFlowResult
{
    public Guid Id { get; set; }
    public string Status { get; set; } = "";
    public DateTime? StartedAt { get; set; }
}

public class CompleteFlowResponse
{
    public CompleteFlowResult CompleteFlow { get; set; } = null!;
}

public class CompleteFlowResult
{
    public Guid Id { get; set; }
    public string Status { get; set; } = "";
    public DateTime? CompletedAt { get; set; }
    public int Progress { get; set; }
}

public class GetUserProgressResponse
{
    public UserFlowAssignmentDto Assignment { get; set; } = null!;
}

public class UserFlowAssignmentDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = "";
    public int Progress { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public UserInfoDto User { get; set; } = null!;
    public FlowInfoDto Flow { get; set; } = null!;
}

public class UserInfoDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = "";
}

public class FlowInfoDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
}

#endregion