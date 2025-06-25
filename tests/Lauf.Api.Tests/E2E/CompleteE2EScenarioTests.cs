using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Lauf.Api.Tests.E2E;

/// <summary>
/// Полный E2E сценарий тестирования всех ролей системы Lauf
/// Эмулирует реальный процесс: от создания контента модератором до завершения обучения пользователем
/// </summary>
public class CompleteE2EScenarioTests : BaseE2ETest
{
    public CompleteE2EScenarioTests(WebApplicationFactory<Program> factory, ITestOutputHelper output) 
        : base(factory, output)
    {
    }

    [Fact]
    public async Task CompleteOnboardingProcess_AllRolesWorkflow_ShouldSucceed()
    {
        try
        {
            _output.WriteLine("🚀 === НАЧАЛО ПОЛНОГО E2E СЦЕНАРИЯ ОНБОРДИНГА ===");
            _output.WriteLine("");

            // ===============================================
            // ЧАСТЬ 1: МОДЕРАТОР СОЗДАЕТ СИСТЕМУ ОБУЧЕНИЯ
            // ===============================================
            _output.WriteLine("👤 === ЧАСТЬ 1: МОДЕРАТОР НАСТРАИВАЕТ СИСТЕМУ ===");

            // Создаем модератора
            var moderatorResponse = await ExecuteGraphQLAsync<CreateUserResponse>(
                TestDataFactory.GraphQLQueries.CreateUser,
                new { input = TestDataFactory.InputObjects.CreateUserInput(
                    telegramId: 100001,
                    email: "head.moderator@lauf.com",
                    fullName: "Главный Модератор Системы",
                    position: "Head of Learning & Development"
                )}
            );

            var moderatorId = moderatorResponse.CreateUser.Id;
            _output.WriteLine($"✅ Создан модератор: {moderatorResponse.CreateUser.FullName} (ID: {moderatorId})");

            // Создаем основной поток онбординга
            var mainFlowResponse = await ExecuteGraphQLAsync<CreateFlowResponse>(
                TestDataFactory.GraphQLQueries.CreateFlow,
                new { input = TestDataFactory.InputObjects.CreateFlowInput(
                    title: "🎯 Корпоративный онбординг 2024",
                    description: "Комплексная программа адаптации новых сотрудников с изучением корпоративной культуры, процессов и получением практических навыков",
                    isSequential: true,
                    allowRetry: true,
                    timeLimit: 30
                )}
            );

            var mainFlowId = mainFlowResponse.CreateFlow.Id;
            _output.WriteLine($"✅ Создан основной поток: {mainFlowResponse.CreateFlow.Title} (ID: {mainFlowId})");

            // Создаем специализированные потоки
            var techFlowResponse = await ExecuteGraphQLAsync<CreateFlowResponse>(
                TestDataFactory.GraphQLQueries.CreateFlow,
                new { input = TestDataFactory.InputObjects.CreateFlowInput(
                    title: "💻 Техническая адаптация разработчиков",
                    description: "Специальная программа для технических специалистов",
                    timeLimit: 21
                )}
            );

            var hrFlowResponse = await ExecuteGraphQLAsync<CreateFlowResponse>(
                TestDataFactory.GraphQLQueries.CreateFlow,
                new { input = TestDataFactory.InputObjects.CreateFlowInput(
                    title: "📋 HR процедуры и документооборот",
                    description: "Изучение внутренних HR процессов и документооборота",
                    timeLimit: 14
                )}
            );

            var techFlowId = techFlowResponse.CreateFlow.Id;
            var hrFlowId = hrFlowResponse.CreateFlow.Id;

            _output.WriteLine($"✅ Создан технический поток: {techFlowResponse.CreateFlow.Title} (ID: {techFlowId})");
            _output.WriteLine($"✅ Создан HR поток: {hrFlowResponse.CreateFlow.Title} (ID: {hrFlowId})");

            // Публикуем все потоки
            const string updateFlowMutation = @"
                mutation UpdateFlow($input: UpdateFlowInput!) {
                    updateFlow(input: $input) {
                        id
                        status
                    }
                }";

            await ExecuteGraphQLAsync<UpdateFlowResponse>(updateFlowMutation, new { input = new { id = mainFlowId, status = "Published" } });
            await ExecuteGraphQLAsync<UpdateFlowResponse>(updateFlowMutation, new { input = new { id = techFlowId, status = "Published" } });
            await ExecuteGraphQLAsync<UpdateFlowResponse>(updateFlowMutation, new { input = new { id = hrFlowId, status = "Published" } });

            _output.WriteLine("✅ Все потоки опубликованы и готовы к использованию");

            // Проверяем созданные потоки
            var allFlowsResponse = await ExecuteGraphQLAsync<GetFlowsResponse>(
                TestDataFactory.GraphQLQueries.GetFlows,
                new { skip = 0, take = 20 }
            );

            allFlowsResponse.Flows.Should().HaveCountGreaterOrEqualTo(3);
            _output.WriteLine($"📊 Всего в системе потоков: {allFlowsResponse.Flows.Count}");
            _output.WriteLine("");

            // ===============================================
            // ЧАСТЬ 2: СОЗДАНИЕ КОМАНДЫ НАСТАВНИКОВ
            // ===============================================
            _output.WriteLine("👥 === ЧАСТЬ 2: ФОРМИРОВАНИЕ КОМАНДЫ НАСТАВНИКОВ ===");

            // Создаем старшего наставника
            var seniorBuddyResponse = await ExecuteGraphQLAsync<CreateUserResponse>(
                TestDataFactory.GraphQLQueries.CreateUser,
                new { input = TestDataFactory.InputObjects.CreateUserInput(
                    telegramId: 200001,
                    email: "senior.buddy@lauf.com",
                    fullName: "Старший Наставник Команды",
                    position: "Senior Team Lead & Mentor"
                )}
            );

            // Создаем технического наставника
            var techBuddyResponse = await ExecuteGraphQLAsync<CreateUserResponse>(
                TestDataFactory.GraphQLQueries.CreateUser,
                new { input = TestDataFactory.InputObjects.CreateUserInput(
                    telegramId: 200002,
                    email: "tech.buddy@lauf.com",
                    fullName: "Технический Наставник",
                    position: "Senior Software Engineer & Mentor"
                )}
            );

            // Создаем HR наставника
            var hrBuddyResponse = await ExecuteGraphQLAsync<CreateUserResponse>(
                TestDataFactory.GraphQLQueries.CreateUser,
                new { input = TestDataFactory.InputObjects.CreateUserInput(
                    telegramId: 200003,
                    email: "hr.buddy@lauf.com",
                    fullName: "HR Наставник",
                    position: "HR Business Partner & Mentor"
                )}
            );

            var seniorBuddyId = seniorBuddyResponse.CreateUser.Id;
            var techBuddyId = techBuddyResponse.CreateUser.Id;
            var hrBuddyId = hrBuddyResponse.CreateUser.Id;

            _output.WriteLine($"✅ Создан старший наставник: {seniorBuddyResponse.CreateUser.FullName}");
            _output.WriteLine($"✅ Создан технический наставник: {techBuddyResponse.CreateUser.FullName}");
            _output.WriteLine($"✅ Создан HR наставник: {hrBuddyResponse.CreateUser.FullName}");
            _output.WriteLine("");

            // ===============================================
            // ЧАСТЬ 3: НОВЫЕ СОТРУДНИКИ ПРИСОЕДИНЯЮТСЯ
            // ===============================================
            _output.WriteLine("🆕 === ЧАСТЬ 3: РЕГИСТРАЦИЯ НОВЫХ СОТРУДНИКОВ ===");

            // Создаем разных новых сотрудников
            var juniorDevResponse = await ExecuteGraphQLAsync<CreateUserResponse>(
                TestDataFactory.GraphQLQueries.CreateUser,
                new { input = TestDataFactory.InputObjects.CreateUserInput(
                    telegramId: 300001,
                    email: "junior.dev@lauf.com",
                    fullName: "Молодой Разработчик",
                    position: "Junior Software Developer"
                )}
            );

            var designerResponse = await ExecuteGraphQLAsync<CreateUserResponse>(
                TestDataFactory.GraphQLQueries.CreateUser,
                new { input = TestDataFactory.InputObjects.CreateUserInput(
                    telegramId: 300002,
                    email: "new.designer@lauf.com",
                    fullName: "Креативный Дизайнер",
                    position: "UI/UX Designer"
                )}
            );

            var qaEngineerResponse = await ExecuteGraphQLAsync<CreateUserResponse>(
                TestDataFactory.GraphQLQueries.CreateUser,
                new { input = TestDataFactory.InputObjects.CreateUserInput(
                    telegramId: 300003,
                    email: "qa.engineer@lauf.com",
                    fullName: "Тестировщик Качества",
                    position: "QA Engineer"
                )}
            );

            var hrSpecialistResponse = await ExecuteGraphQLAsync<CreateUserResponse>(
                TestDataFactory.GraphQLQueries.CreateUser,
                new { input = TestDataFactory.InputObjects.CreateUserInput(
                    telegramId: 300004,
                    email: "hr.specialist@lauf.com",
                    fullName: "HR Специалист",
                    position: "HR Specialist"
                )}
            );

            var juniorDevId = juniorDevResponse.CreateUser.Id;
            var designerId = designerResponse.CreateUser.Id;
            var qaEngineerId = qaEngineerResponse.CreateUser.Id;
            var hrSpecialistId = hrSpecialistResponse.CreateUser.Id;

            _output.WriteLine($"✅ Новые сотрудники зарегистрированы:");
            _output.WriteLine($"   - {juniorDevResponse.CreateUser.FullName} ({juniorDevResponse.CreateUser.Position})");
            _output.WriteLine($"   - {designerResponse.CreateUser.FullName} ({designerResponse.CreateUser.Position})");
            _output.WriteLine($"   - {qaEngineerResponse.CreateUser.FullName} ({qaEngineerResponse.CreateUser.Position})");
            _output.WriteLine($"   - {hrSpecialistResponse.CreateUser.FullName} ({hrSpecialistResponse.CreateUser.Position})");

            // Проверим всех пользователей
            var allUsersResponse = await ExecuteGraphQLAsync<GetUsersResponse>(
                TestDataFactory.GraphQLQueries.GetUsers,
                new { skip = 0, take = 50 }
            );

            allUsersResponse.Users.Should().HaveCountGreaterOrEqualTo(8);
            _output.WriteLine($"📊 Всего пользователей в системе: {allUsersResponse.Users.Count}");
            _output.WriteLine("");

            // ===============================================
            // ЧАСТЬ 4: НАСТАВНИКИ НАЗНАЧАЮТ ПРОГРАММЫ ОБУЧЕНИЯ
            // ===============================================
            _output.WriteLine("📋 === ЧАСТЬ 4: НАСТАВНИКИ НАЗНАЧАЮТ ПРОГРАММЫ ===");

            // Старший наставник назначает основной поток всем
            var assignment1 = await ExecuteGraphQLAsync<AssignFlowResponse>(
                TestDataFactory.GraphQLQueries.AssignFlow,
                new { input = TestDataFactory.InputObjects.AssignFlowInput(juniorDevId, mainFlowId, DateTime.UtcNow.AddDays(30), seniorBuddyId) }
            );

            var assignment2 = await ExecuteGraphQLAsync<AssignFlowResponse>(
                TestDataFactory.GraphQLQueries.AssignFlow,
                new { input = TestDataFactory.InputObjects.AssignFlowInput(designerId, mainFlowId, DateTime.UtcNow.AddDays(30), seniorBuddyId) }
            );

            var assignment3 = await ExecuteGraphQLAsync<AssignFlowResponse>(
                TestDataFactory.GraphQLQueries.AssignFlow,
                new { input = TestDataFactory.InputObjects.AssignFlowInput(qaEngineerId, mainFlowId, DateTime.UtcNow.AddDays(30), seniorBuddyId) }
            );

            var assignment4 = await ExecuteGraphQLAsync<AssignFlowResponse>(
                TestDataFactory.GraphQLQueries.AssignFlow,
                new { input = TestDataFactory.InputObjects.AssignFlowInput(hrSpecialistId, mainFlowId, DateTime.UtcNow.AddDays(30), seniorBuddyId) }
            );

            // Технический наставник назначает техническую программу разработчику
            var techAssignment = await ExecuteGraphQLAsync<AssignFlowResponse>(
                TestDataFactory.GraphQLQueries.AssignFlow,
                new { input = TestDataFactory.InputObjects.AssignFlowInput(juniorDevId, techFlowId, DateTime.UtcNow.AddDays(21), techBuddyId) }
            );

            // HR наставник назначает HR программу HR специалисту
            var hrAssignment = await ExecuteGraphQLAsync<AssignFlowResponse>(
                TestDataFactory.GraphQLQueries.AssignFlow,
                new { input = TestDataFactory.InputObjects.AssignFlowInput(hrSpecialistId, hrFlowId, DateTime.UtcNow.AddDays(14), hrBuddyId) }
            );

            _output.WriteLine("✅ Назначения выполнены:");
            _output.WriteLine($"   - Основной поток назначен 4 сотрудникам");
            _output.WriteLine($"   - Технический поток назначен разработчику");
            _output.WriteLine($"   - HR поток назначен HR специалисту");

            // Проверим статистику назначений
            var allAssignmentsResponse = await ExecuteGraphQLAsync<GetFlowAssignmentsResponse>(
                TestDataFactory.GraphQLQueries.GetFlowAssignments,
                new { skip = 0, take = 100 }
            );

            allAssignmentsResponse.Assignments.Should().HaveCount(6);
            _output.WriteLine($"📊 Всего назначений в системе: {allAssignmentsResponse.Assignments.Count}");
            _output.WriteLine("");

            // ===============================================
            // ЧАСТЬ 5: СОТРУДНИКИ НАЧИНАЮТ ОБУЧЕНИЕ
            // ===============================================
            _output.WriteLine("🎓 === ЧАСТЬ 5: СОТРУДНИКИ НАЧИНАЮТ ОБУЧЕНИЕ ===");

            // Молодой разработчик начинает основной поток
            var startMainFlow = await ExecuteGraphQLAsync<StartFlowResponse>(
                TestDataFactory.GraphQLQueries.StartFlow,
                new { input = TestDataFactory.InputObjects.StartFlowInput(assignment1.AssignFlow.AssignmentId) }
            );

            // Молодой разработчик начинает технический поток
            var startTechFlow = await ExecuteGraphQLAsync<StartFlowResponse>(
                TestDataFactory.GraphQLQueries.StartFlow,
                new { input = TestDataFactory.InputObjects.StartFlowInput(techAssignment.AssignFlow.AssignmentId) }
            );

            // Дизайнер начинает основной поток
            var startDesignerFlow = await ExecuteGraphQLAsync<StartFlowResponse>(
                TestDataFactory.GraphQLQueries.StartFlow,
                new { input = TestDataFactory.InputObjects.StartFlowInput(assignment2.AssignFlow.AssignmentId) }
            );

            _output.WriteLine("✅ Сотрудники начали обучение:");
            _output.WriteLine($"   - Разработчик: основной поток (статус: {startMainFlow.StartFlow.Status})");
            _output.WriteLine($"   - Разработчик: технический поток (статус: {startTechFlow.StartFlow.Status})");
            _output.WriteLine($"   - Дизайнер: основной поток (статус: {startDesignerFlow.StartFlow.Status})");

            // ===============================================
            // ЧАСТЬ 6: МОНИТОРИНГ ПРОГРЕССА НАСТАВНИКАМИ
            // ===============================================
            _output.WriteLine("📊 === ЧАСТЬ 6: МОНИТОРИНГ ПРОГРЕССА ===");

            // Получаем активные назначения разработчика
            var devActiveAssignments = await ExecuteGraphQLAsync<GetActiveAssignmentsResponse>(
                TestDataFactory.GraphQLQueries.GetActiveAssignments,
                new { userId = juniorDevId }
            );

            devActiveAssignments.ActiveAssignments.Should().HaveCount(2);
            _output.WriteLine($"👨‍💻 У разработчика активных потоков: {devActiveAssignments.ActiveAssignments.Count}");

            // Получаем активные назначения дизайнера
            var designerActiveAssignments = await ExecuteGraphQLAsync<GetActiveAssignmentsResponse>(
                TestDataFactory.GraphQLQueries.GetActiveAssignments,
                new { userId = designerId }
            );

            designerActiveAssignments.ActiveAssignments.Should().HaveCount(1);
            _output.WriteLine($"🎨 У дизайнера активных потоков: {designerActiveAssignments.ActiveAssignments.Count}");

            // Проверяем назначения по потокам
            var mainFlowAssignments = await ExecuteGraphQLAsync<GetFlowAssignmentsResponse>(
                TestDataFactory.GraphQLQueries.GetFlowAssignments,
                new { flowId = mainFlowId, skip = 0, take = 10 }
            );

            var techFlowAssignments = await ExecuteGraphQLAsync<GetFlowAssignmentsResponse>(
                TestDataFactory.GraphQLQueries.GetFlowAssignments,
                new { flowId = techFlowId, skip = 0, take = 10 }
            );

            _output.WriteLine($"📋 Основной поток: {mainFlowAssignments.Assignments.Count} назначений");
            _output.WriteLine($"💻 Технический поток: {techFlowAssignments.Assignments.Count} назначений");

            // ===============================================
            // ЧАСТЬ 7: ЗАВЕРШЕНИЕ ОБУЧЕНИЯ И РЕЗУЛЬТАТЫ
            // ===============================================
            _output.WriteLine("🏆 === ЧАСТЬ 7: ЗАВЕРШЕНИЕ ОБУЧЕНИЯ ===");

            // Дизайнер завершает основной поток
            const string completeFlowMutation = @"
                mutation CompleteFlow($input: CompleteFlowInput!) {
                    completeFlow(input: $input) {
                        id
                        status
                        completedAt
                        progress
                    }
                }";

            var completeDesignerFlow = await ExecuteGraphQLAsync<CompleteFlowResponse>(
                completeFlowMutation,
                new { input = new { 
                    assignmentId = assignment2.AssignFlow.AssignmentId,
                    completionNotes = "Отлично изучил корпоративную культуру и процессы дизайна. Готов к работе!"
                }}
            );

            completeDesignerFlow.CompleteFlow.Status.Should().Be("Completed");
            completeDesignerFlow.CompleteFlow.Progress.Should().Be(100);

            // Разработчик завершает технический поток
            var completeTechFlow = await ExecuteGraphQLAsync<CompleteFlowResponse>(
                completeFlowMutation,
                new { input = new { 
                    assignmentId = techAssignment.AssignFlow.AssignmentId,
                    completionNotes = "Успешно освоил технический стек и готов к проектной работе"
                }}
            );

            completeTechFlow.CompleteFlow.Status.Should().Be("Completed");
            completeTechFlow.CompleteFlow.Progress.Should().Be(100);

            _output.WriteLine("✅ Завершенные программы:");
            _output.WriteLine($"   - Дизайнер завершил основной поток (прогресс: {completeDesignerFlow.CompleteFlow.Progress}%)");
            _output.WriteLine($"   - Разработчик завершил технический поток (прогресс: {completeTechFlow.CompleteFlow.Progress}%)");

            // ===============================================
            // ЧАСТЬ 8: ФИНАЛЬНАЯ СТАТИСТИКА И ОТЧЕТЫ
            // ===============================================
            _output.WriteLine("📈 === ЧАСТЬ 8: ФИНАЛЬНАЯ СТАТИСТИКА ===");

            // Получаем финальную статистику всех назначений
            var finalAssignmentsResponse = await ExecuteGraphQLAsync<GetFlowAssignmentsResponse>(
                TestDataFactory.GraphQLQueries.GetFlowAssignments,
                new { skip = 0, take = 100 }
            );

            var assignedCount = finalAssignmentsResponse.Assignments.Count(a => a.Status == "Assigned");
            var inProgressCount = finalAssignmentsResponse.Assignments.Count(a => a.Status == "InProgress");
            var completedCount = finalAssignmentsResponse.Assignments.Count(a => a.Status == "Completed");

            _output.WriteLine($"📊 ИТОГОВАЯ СТАТИСТИКА СИСТЕМЫ:");
            _output.WriteLine($"   👥 Пользователей: {allUsersResponse.Users.Count}");
            _output.WriteLine($"   📚 Потоков обучения: {allFlowsResponse.Flows.Count}");
            _output.WriteLine($"   📋 Всего назначений: {finalAssignmentsResponse.Assignments.Count}");
            _output.WriteLine($"   ⏳ Назначено: {assignedCount}");
            _output.WriteLine($"   🔄 В процессе: {inProgressCount}");
            _output.WriteLine($"   ✅ Завершено: {completedCount}");

            // Проверяем, что система работает корректно
            finalAssignmentsResponse.Assignments.Should().HaveCount(6);
            completedCount.Should().BeGreaterOrEqualTo(2);
            
            _output.WriteLine("");
            _output.WriteLine("🎉 === ПОЛНЫЙ E2E СЦЕНАРИЙ ЗАВЕРШЕН УСПЕШНО! ===");
            _output.WriteLine("🚀 Система Lauf полностью протестирована со всеми ролями и процессами!");
        }
        finally
        {
            GenerateHtmlReport("CompleteOnboardingProcess");
        }
    }

    [Fact]
    public async Task SystemStressTest_MultipleUsersAndFlows_ShouldHandleLoad()
    {
        try
        {
            _output.WriteLine("⚡ === НАГРУЗОЧНЫЙ ТЕСТ СИСТЕМЫ ===");

            // Создаем большое количество пользователей и потоков для проверки производительности
            var tasks = new List<Task>();

            // Создаем 10 модераторов
            for (int i = 1; i <= 10; i++)
            {
                tasks.Add(ExecuteGraphQLAsync<CreateUserResponse>(
                    TestDataFactory.GraphQLQueries.CreateUser,
                    new { input = TestDataFactory.InputObjects.CreateUserInput(
                        telegramId: 400000 + i,
                        email: $"moderator{i}@stress.test",
                        fullName: $"Стресс Модератор {i}",
                        position: "Test Moderator"
                    )}
                ));
            }

            // Создаем 20 бадди
            for (int i = 1; i <= 20; i++)
            {
                tasks.Add(ExecuteGraphQLAsync<CreateUserResponse>(
                    TestDataFactory.GraphQLQueries.CreateUser,
                    new { input = TestDataFactory.InputObjects.CreateUserInput(
                        telegramId: 500000 + i,
                        email: $"buddy{i}@stress.test",
                        fullName: $"Стресс Бадди {i}",
                        position: "Test Buddy"
                    )}
                ));
            }

            // Создаем 50 обычных пользователей
            for (int i = 1; i <= 50; i++)
            {
                tasks.Add(ExecuteGraphQLAsync<CreateUserResponse>(
                    TestDataFactory.GraphQLQueries.CreateUser,
                    new { input = TestDataFactory.InputObjects.CreateUserInput(
                        telegramId: 600000 + i,
                        email: $"user{i}@stress.test",
                        fullName: $"Стресс Пользователь {i}",
                        position: "Test User"
                    )}
                ));
            }

            await Task.WhenAll(tasks);
            
            _output.WriteLine($"✅ Создано пользователей: {tasks.Count}");

            // Проверяем общее количество пользователей
            var stressUsersResponse = await ExecuteGraphQLAsync<GetUsersResponse>(
                TestDataFactory.GraphQLQueries.GetUsers,
                new { skip = 0, take = 200 }
            );

            stressUsersResponse.Users.Should().HaveCountGreaterOrEqualTo(80);
            _output.WriteLine($"📊 Всего пользователей после нагрузочного теста: {stressUsersResponse.Users.Count}");
        }
        finally
        {
            GenerateHtmlReport("SystemStressTest");
        }
    }
}

