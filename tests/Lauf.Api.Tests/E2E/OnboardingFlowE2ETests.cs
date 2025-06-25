using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Lauf.Api.Tests.Infrastructure;
using Lauf.Application.DTOs.Users;
using Lauf.Application.DTOs.Flows;
using Lauf.Application.Commands.FlowManagement;
using Lauf.Application.Commands.FlowAssignment;
using Lauf.Domain.Entities.Users;
using Lauf.Domain.Entities.Flows;
using Lauf.Domain.ValueObjects;
using Lauf.Domain.Enums;
using System.Net;
using Xunit;
using Xunit.Abstractions;

namespace Lauf.Api.Tests.E2E;

/// <summary>
/// Комплексные E2E тесты полного процесса онбординга
/// </summary>
public class OnboardingFlowE2ETests : ExtendedApiTestBase
{
    public OnboardingFlowE2ETests(WebApplicationFactory<Program> factory, ITestOutputHelper output) 
        : base(factory, output)
    {
    }

    [Fact]
    public async Task CompleteOnboardingFlow_ShouldExecuteSuccessfully()
    {
        // === ЭТАП 1: НАСТРОЙКА ДАННЫХ ===
        Output.WriteLine("🚀 === ЭТАП 1: НАСТРОЙКА ДАННЫХ ===");
        
        await ClearDatabase();
        await SetupInitialData();

        // === ЭТАП 2: МОДЕРАТОР СОЗДАЕТ ПОТОК ===
        Output.WriteLine("👨‍💼 === ЭТАП 2: МОДЕРАТОР СОЗДАЕТ ПОТОК ===");
        
        var flowId = await ModeratorCreatesFlow();

        // === ЭТАП 3: МОДЕРАТОР ДОБАВЛЯЕТ ШАГИ И КОМПОНЕНТЫ ===
        Output.WriteLine("🔧 === ЭТАП 3: МОДЕРАТОР ДОБАВЛЯЕТ ШАГИ И КОМПОНЕНТЫ ===");
        
        await ModeratorAddsStepsAndComponents(flowId);

        // === ЭТАП 4: МОДЕРАТОР ПУБЛИКУЕТ ПОТОК ===
        Output.WriteLine("📢 === ЭТАП 4: МОДЕРАТОР ПУБЛИКУЕТ ПОТОК ===");
        
        await ModeratorPublishesFlow(flowId);

        // === ЭТАП 5: ПОЛЬЗОВАТЕЛЬ РЕГИСТРИРУЕТСЯ ===
        Output.WriteLine("👤 === ЭТАП 5: ПОЛЬЗОВАТЕЛЬ РЕГИСТРИРУЕТСЯ ===");
        
        var userId = await UserRegisters();

        // === ЭТАП 6: БАДДИ РЕГИСТРИРУЕТСЯ ===
        Output.WriteLine("🤝 === ЭТАП 6: БАДДИ РЕГИСТРИРУЕТСЯ ===");
        
        var buddyId = await BuddyRegisters();

        // === ЭТАП 7: МОДЕРАТОР НАЗНАЧАЕТ ПОТОК ПОЛЬЗОВАТЕЛЮ ===
        Output.WriteLine("🎯 === ЭТАП 7: МОДЕРАТОР НАЗНАЧАЕТ ПОТОК ПОЛЬЗОВАТЕЛЮ ===");
        
        var assignmentId = await ModeratorAssignsFlowToUser(flowId, userId, buddyId);

        // === ЭТАП 8: ПОЛЬЗОВАТЕЛЬ ПРОСМАТРИВАЕТ НАЗНАЧЕННЫЕ ПОТОКИ ===
        Output.WriteLine("👀 === ЭТАП 8: ПОЛЬЗОВАТЕЛЬ ПРОСМАТРИВАЕТ НАЗНАЧЕННЫЕ ПОТОКИ ===");
        
        await UserViewsAssignedFlows(userId);

        // === ЭТАП 9: ПОЛЬЗОВАТЕЛЬ ИЗУЧАЕТ ДЕТАЛИ ПОТОКА ===
        Output.WriteLine("📖 === ЭТАП 9: ПОЛЬЗОВАТЕЛЬ ИЗУЧАЕТ ДЕТАЛИ ПОТОКА ===");
        
        await UserViewsFlowDetails(flowId, userId);

        // === ЭТАП 10: ПОЛЬЗОВАТЕЛЬ НАЧИНАЕТ ПРОХОЖДЕНИЕ ===
        Output.WriteLine("▶️ === ЭТАП 10: ПОЛЬЗОВАТЕЛЬ НАЧИНАЕТ ПРОХОЖДЕНИЕ ===");
        
        await UserStartsFlow(assignmentId);

        // === ЭТАП 11: ПОЛЬЗОВАТЕЛЬ ПРОХОДИТ КОМПОНЕНТЫ ===
        Output.WriteLine("📚 === ЭТАП 11: ПОЛЬЗОВАТЕЛЬ ПРОХОДИТ КОМПОНЕНТЫ ===");
        
        await UserCompletesComponents(assignmentId);

        // === ЭТАП 12: БАДДИ ОТСЛЕЖИВАЕТ ПРОГРЕСС ===
        Output.WriteLine("📊 === ЭТАП 12: БАДДИ ОТСЛЕЖИВАЕТ ПРОГРЕСС ===");
        
        await BuddyTracksProgress(userId, assignmentId);

        // === ЭТАП 13: ПОЛЬЗОВАТЕЛЬ ЗАВЕРШАЕТ ПОТОК ===
        Output.WriteLine("🎉 === ЭТАП 13: ПОЛЬЗОВАТЕЛЬ ЗАВЕРШАЕТ ПОТОК ===");
        
        await UserCompletesFlow(assignmentId);

        // === ЭТАП 14: ПРОВЕРКА ФИНАЛЬНОГО СОСТОЯНИЯ ===
        Output.WriteLine("✅ === ЭТАП 14: ПРОВЕРКА ФИНАЛЬНОГО СОСТОЯНИЯ ===");
        
        await VerifyFinalState(flowId, userId, assignmentId);

        // Генерируем HTML отчёт
        GenerateApiCallReport("CompleteOnboardingFlow");
        
        Output.WriteLine("🎊 === ТЕСТ ЗАВЕРШЕН УСПЕШНО! ===");
    }

    private async Task SetupInitialData()
    {
        // Создаем администраторскую роль
        var adminRole = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Administrator",
            Description = "Администратор системы",
            Permissions = "admin,manage_flows,manage_users",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var buddyRole = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Buddy",
            Description = "Наставник",
            Permissions = "view_flows,mentor_users",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        Context.Roles.AddRange(adminRole, buddyRole);

        // Создаем модератора
        var moderator = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "Анна",
            LastName = "Администратор",
            Email = "admin@lauf.com",
            Position = "HR Director",
            TelegramUserId = new TelegramUserId(1000001),
            IsActive = true,
            Language = "ru",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        Context.Users.Add(moderator);
        await Context.SaveChangesAsync();

        Output.WriteLine($"✅ Создан модератор: {moderator.FirstName} {moderator.LastName} (ID: {moderator.Id})");
    }

    private async Task<Guid> ModeratorCreatesFlow()
    {
        var createFlowCommand = new CreateFlowCommand
        {
            Title = "🚀 Онбординг разработчика",
            Description = "Полный процесс адаптации нового разработчика в команде",
            Category = "Разработка",
            Tags = "разработка,онбординг,адаптация",
            EstimatedDurationMinutes = 2400, // 40 часов
            RequiredRole = "Developer",
            IsRequired = true,
            Priority = FlowPriority.High,
            CreatedById = Context.Users.First().Id
        };

        var response = await PostAsync("/api/flows", createFlowCommand, "Создание потока онбординга");
        
        var createdFlow = await AssertSuccessAndGetData<FlowDto>(response, "создание потока");
        createdFlow.Title.Should().Be(createFlowCommand.Title);
        createdFlow.Status.Should().Be(FlowStatus.Draft);

        Output.WriteLine($"✅ Создан поток: {createdFlow.Title} (ID: {createdFlow.Id})");
        
        return createdFlow.Id;
    }

    private async Task ModeratorAddsStepsAndComponents(Guid flowId)
    {
        // Добавляем шаги к потоку (имитация через прямое добавление в БД)
        var step1 = new FlowStep
        {
            Id = Guid.NewGuid(),
            FlowId = flowId,
            Title = "📋 Знакомство с компанией",
            Description = "Изучение истории, ценностей и структуры компании",
            Order = 1,
            EstimatedDurationMinutes = 480, // 8 часов
            IsRequired = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var step2 = new FlowStep
        {
            Id = Guid.NewGuid(),
            FlowId = flowId,
            Title = "💻 Настройка рабочего места",
            Description = "Установка необходимого ПО и настройка среды разработки",
            Order = 2,
            EstimatedDurationMinutes = 240, // 4 часа
            IsRequired = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var step3 = new FlowStep
        {
            Id = Guid.NewGuid(),
            FlowId = flowId,
            Title = "🎯 Первый проект",
            Description = "Выполнение первого учебного проекта",
            Order = 3,
            EstimatedDurationMinutes = 1680, // 28 часов
            IsRequired = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        Context.FlowSteps.AddRange(step1, step2, step3);
        await Context.SaveChangesAsync();

        Output.WriteLine($"✅ Добавлены шаги: {step1.Title}, {step2.Title}, {step3.Title}");

        // Проверяем, что шаги добавились через API
        var getFlowResponse = await GetAsync($"/api/flows/{flowId}", "Получение потока с шагами");
        var flowWithSteps = await AssertSuccessAndGetData<FlowDto>(getFlowResponse, "получение потока с шагами");
        flowWithSteps.TotalSteps.Should().Be(3);
    }

    private async Task ModeratorPublishesFlow(Guid flowId)
    {
        // Обновляем статус потока на Published
        var updateCommand = new UpdateFlowCommand
        {
            Id = flowId,
            Title = "🚀 Онбординг разработчика",
            Description = "Полный процесс адаптации нового разработчика в команде",
            Category = "Разработка",
            Tags = "разработка,онбординг,адаптация",
            Status = FlowStatus.Published,
            EstimatedDurationMinutes = 2400
        };

        var response = await PutAsync($"/api/flows/{flowId}", updateCommand, "Публикация потока");
        
        var updatedFlow = await AssertSuccessAndGetData<FlowDto>(response, "публикация потока");
        updatedFlow.Status.Should().Be(FlowStatus.Published);

        Output.WriteLine($"✅ Поток опубликован: {updatedFlow.Title}");
    }

    private async Task<Guid> UserRegisters()
    {
        var createUserCommand = new CreateUserCommand
        {
            FirstName = "Иван",
            LastName = "Разработчик",
            Email = "ivan.developer@lauf.com",
            Position = "Junior Developer",
            TelegramUserId = 2000001,
            Language = "ru"
        };

        var response = await PostAsync("/api/users", createUserCommand, "Регистрация нового пользователя");
        
        var createdUser = await AssertSuccessAndGetData<UserDto>(response, "регистрация пользователя");
        createdUser.FirstName.Should().Be(createUserCommand.FirstName);
        createdUser.Email.Should().Be(createUserCommand.Email);

        Output.WriteLine($"✅ Зарегистрирован пользователь: {createdUser.FirstName} {createdUser.LastName} (ID: {createdUser.Id})");
        
        return createdUser.Id;
    }

    private async Task<Guid> BuddyRegisters()
    {
        var createBuddyCommand = new CreateUserCommand
        {
            FirstName = "Петр",
            LastName = "Ментор",
            Email = "petr.mentor@lauf.com",
            Position = "Senior Developer",
            TelegramUserId = 3000001,
            Language = "ru"
        };

        var response = await PostAsync("/api/users", createBuddyCommand, "Регистрация наставника");
        
        var createdBuddy = await AssertSuccessAndGetData<UserDto>(response, "регистрация наставника");
        createdBuddy.FirstName.Should().Be(createBuddyCommand.FirstName);

        Output.WriteLine($"✅ Зарегистрирован наставник: {createdBuddy.FirstName} {createdBuddy.LastName} (ID: {createdBuddy.Id})");
        
        return createdBuddy.Id;
    }

    private async Task<Guid> ModeratorAssignsFlowToUser(Guid flowId, Guid userId, Guid buddyId)
    {
        var assignCommand = new AssignFlowCommand
        {
            FlowId = flowId,
            UserId = userId,
            BuddyId = buddyId,
            DueDate = DateTime.UtcNow.AddDays(30),
            Priority = AssignmentPriority.High,
            Notes = "Первое назначение онбординга для нового разработчика"
        };

        var response = await PostAsync("/api/flow-assignments", assignCommand, "Назначение потока пользователю");
        
        var assignment = await AssertSuccessAndGetData<FlowAssignmentDto>(response, "назначение потока");
        assignment.FlowId.Should().Be(flowId);
        assignment.UserId.Should().Be(userId);
        assignment.BuddyId.Should().Be(buddyId);
        assignment.Status.Should().Be(AssignmentStatus.Assigned);

        Output.WriteLine($"✅ Поток назначен пользователю (Assignment ID: {assignment.Id})");
        
        return assignment.Id;
    }

    private async Task UserViewsAssignedFlows(Guid userId)
    {
        var response = await GetAsync($"/api/flow-assignments/user/{userId}", "Просмотр назначенных потоков");
        
        var assignments = await AssertSuccessAndGetData<IEnumerable<FlowAssignmentDto>>(response, "получение назначений пользователя");
        assignments.Should().NotBeEmpty();
        assignments.Should().ContainSingle(a => a.UserId == userId);

        Output.WriteLine($"✅ Пользователь просмотрел назначенные потоки: {assignments.Count()} назначений");
    }

    private async Task UserViewsFlowDetails(Guid flowId, Guid userId)
    {
        var response = await GetAsync($"/api/flows/{flowId}/details?userId={userId}", "Просмотр деталей потока");
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();

        Output.WriteLine("✅ Пользователь изучил детали потока");
    }

    private async Task UserStartsFlow(Guid assignmentId)
    {
        var startCommand = new StartFlowCommand
        {
            AssignmentId = assignmentId
        };

        var response = await PostAsync($"/api/flow-assignments/{assignmentId}/start", startCommand, "Начало прохождения потока");
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        Output.WriteLine("✅ Пользователь начал прохождение потока");
    }

    private async Task UserCompletesComponents(Guid assignmentId)
    {
        // Имитируем прохождение компонентов
        // В реальном приложении здесь были бы API для отметки прогресса по компонентам
        
        Output.WriteLine("✅ Пользователь проходит компоненты обучения (имитация)");
        
        // Проверяем прогресс
        var progressResponse = await GetAsync($"/api/flow-assignments/{assignmentId}/progress", "Проверка прогресса");
        progressResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task BuddyTracksProgress(Guid userId, Guid assignmentId)
    {
        var progressResponse = await GetAsync($"/api/flow-assignments/{assignmentId}/progress", "Наставник отслеживает прогресс");
        progressResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        Output.WriteLine("✅ Наставник отследил прогресс подопечного");
    }

    private async Task UserCompletesFlow(Guid assignmentId)
    {
        var completeCommand = new CompleteFlowCommand
        {
            AssignmentId = assignmentId,
            CompletionNotes = "Успешно завершен весь процесс онбординга"
        };

        var response = await PostAsync($"/api/flow-assignments/{assignmentId}/complete", completeCommand, "Завершение потока");
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        Output.WriteLine("✅ Пользователь завершил поток");
    }

    private async Task VerifyFinalState(Guid flowId, Guid userId, Guid assignmentId)
    {
        // Проверяем финальное состояние назначения
        var assignmentResponse = await GetAsync($"/api/flow-assignments/{assignmentId}", "Проверка финального состояния");
        var assignment = await AssertSuccessAndGetData<FlowAssignmentDto>(assignmentResponse, "получение финального состояния");
        
        assignment.Status.Should().Be(AssignmentStatus.Completed);

        // Проверяем статистику потока
        var statsResponse = await GetAsync($"/api/flows/{flowId}/stats", "Проверка статистики потока");
        statsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        Output.WriteLine("✅ Финальное состояние проверено - все корректно!");
    }
}