using Lauf.Domain.Entities.Users;
using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Enums;
using Lauf.Domain.ValueObjects;

namespace Lauf.Api.Tests.E2E;

/// <summary>
/// Фабрика для создания тестовых данных
/// </summary>
public static class TestDataFactory
{
    /// <summary>
    /// Создает тестового пользователя с ролью Модератор
    /// </summary>
    public static User CreateModerator()
    {
        return new User(
            new TelegramUserId(12345),
            "moderator@test.com",
            "Test",
            "Moderator",
            "Модератор системы",
            "ru"
        );
    }

    /// <summary>
    /// Создает тестового пользователя с ролью Бадди
    /// </summary>
    public static User CreateBuddy()
    {
        return new User(
            new TelegramUserId(23456),
            "buddy@test.com",
            "Test",
            "Buddy",
            "Наставник",
            "ru"
        );
    }

    /// <summary>
    /// Создает тестового пользователя с ролью User
    /// </summary>
    public static User CreateRegularUser()
    {
        return new User(
            new TelegramUserId(34567),
            "user@test.com",
            "Test",
            "User",
            "Обычный пользователь",
            "ru"
        );
    }

    /// <summary>
    /// Создает тестовый поток обучения
    /// </summary>
    public static Flow CreateTestFlow(User creator)
    {
        var flow = Flow.Create(
            "Тестовый поток онбординга",
            "Полный процесс знакомства с компанией",
            creator.Id
        );

        // Настройки потока
        var settings = new FlowSettings(
            requireSequentialCompletion: true,
            allowRetry: true,
            timeToCompleteWorkingDays: 14,
            maxAttempts: 3
        );
        flow.UpdateSettings(settings);

        return flow;
    }

    /// <summary>
    /// Создает базовую тестовую информацию о компонентах потока
    /// </summary>
    public static string GetTestFlowDescription(string title)
    {
        return $"Описание для потока: {title}. Включает изучение материалов, прохождение тестов и выполнение практических заданий.";
    }

    /// <summary>
    /// GraphQL запросы для тестов
    /// </summary>
    public static class GraphQLQueries
    {
        public const string CreateFlow = @"
            mutation CreateFlow($input: CreateFlowInput!) {
                createFlow(input: $input) {
                    id
                    title
                    status
                }
            }";

        public const string GetFlows = @"
            query GetFlows($skip: Int, $take: Int) {
                flows: getFlows(skip: $skip, take: $take) {
                    id
                    title
                    status
                    createdAt
                }
            }";

        public const string GetFlowDetails = @"
            query GetFlowDetails($id: UUID!) {
                flow: getFlow(id: $id) {
                    id
                    title
                    description
                    status
                    settings {
                        requireSequentialCompletion
                        allowRetry
                        timeToCompleteWorkingDays
                        maxAttempts
                    }
                    steps {
                        id
                        title
                        description
                        order
                        components {
                            id
                            title
                            type
                            order
                        }
                    }
                }
            }";

        public const string AssignFlow = @"
            mutation AssignFlow($input: AssignFlowInput!) {
                assignFlow(input: $input) {
                    assignmentId
                    success
                    message
                }
            }";

        public const string GetFlowAssignments = @"
            query GetFlowAssignments($userId: UUID, $flowId: UUID, $skip: Int, $take: Int) {
                assignments: getFlowAssignments(userId: $userId, flowId: $flowId, skip: $skip, take: $take) {
                    id
                    userId
                    flowId
                    status
                    assignedAt
                    deadline
                    progress
                }
            }";

        public const string StartFlow = @"
            mutation StartFlow($input: StartFlowInput!) {
                startFlow(input: $input) {
                    id
                    status
                    startedAt
                }
            }";

        public const string CreateUser = @"
            mutation CreateUser($input: CreateUserInput!) {
                createUser(input: $input) {
                    id
                    email
                    fullName
                    position
                    isActive
                }
            }";

        public const string GetUsers = @"
            query GetUsers($skip: Int, $take: Int) {
                users: getUsers(skip: $skip, take: $take) {
                    id
                    email
                    fullName
                    position
                    isActive
                    createdAt
                }
            }";

        public const string GetActiveAssignments = @"
            query GetActiveAssignments($userId: UUID!) {
                activeAssignments: getActiveAssignments(userId: $userId) {
                    id
                    userId
                    flowId
                    status
                    assignedAt
                    deadline
                    progress
                }
            }";

        public const string GetOverdueAssignments = @"
            query GetOverdueAssignments {
                overdueAssignments: getOverdueAssignments {
                    id
                    userId
                    flowId
                    status
                    deadline
                    assignedAt
                }
            }";
    }

    /// <summary>
    /// Input объекты для GraphQL мутаций
    /// </summary>
    public static class InputObjects
    {
        public static object CreateFlowInput(string title, string description, bool isSequential = true, bool allowRetry = true, int? timeLimit = 14)
        {
            return new
            {
                title,
                description,
                isSequential,
                allowRetry,
                timeLimit
            };
        }

        public static object CreateUserInput(long telegramId, string email, string fullName, string position)
        {
            return new
            {
                telegramId,
                email,
                fullName,
                position
            };
        }

        public static object AssignFlowInput(Guid userId, Guid flowId, DateTime? dueDate = null, Guid? assignedBy = null)
        {
            return new
            {
                userId,
                flowId,
                dueDate = dueDate?.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                assignedBy
            };
        }

        public static object StartFlowInput(Guid assignmentId)
        {
            return new
            {
                assignmentId
            };
        }
    }
}