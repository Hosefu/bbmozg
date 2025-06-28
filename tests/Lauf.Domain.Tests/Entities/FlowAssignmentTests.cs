using FluentAssertions;
using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Entities.Users;
using Lauf.Domain.Enums;
using Lauf.Shared.Helpers;
using Xunit;

namespace Lauf.Domain.Tests.Entities;

public class FlowAssignmentTests
{
    [Fact]
    public void FlowAssignment_WithValidParameters_ShouldBeSetCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var flowId = Guid.NewGuid();
        var flow = CreateTestFlow(flowId);

        // Act
        var assignment = new FlowAssignment(userId, flow.Id, flow.ActiveContent.Id, Guid.NewGuid());

        // Assert
        assignment.UserId.Should().Be(userId);
        assignment.FlowId.Should().Be(flowId);
        assignment.Status.Should().Be(AssignmentStatus.Assigned);
        assignment.AssignedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void FlowAssignment_Status_ShouldStartAsAssigned()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var flow = CreateTestFlow(Guid.NewGuid());

        // Act
        var assignment = new FlowAssignment(userId, flow.Id, flow.ActiveContent.Id, Guid.NewGuid());

        // Assert
        assignment.Status.Should().Be(AssignmentStatus.Assigned);
    }

    [Fact]
    public void StartProgress_ShouldChangeStatusToInProgress()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var flow = CreateTestFlow(Guid.NewGuid());
        var assignment = new FlowAssignment(userId, flow.Id, flow.ActiveContent.Id, Guid.NewGuid());

        // Act
        assignment.Start();

        // Assert
        assignment.Status.Should().Be(AssignmentStatus.InProgress);
    }

    [Fact]
    public void Complete_ShouldChangeStatusToCompleted()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var flow = CreateTestFlow(Guid.NewGuid());
        var assignment = new FlowAssignment(userId, flow.Id, flow.ActiveContent.Id, Guid.NewGuid());

        // Act
        assignment.Complete();

        // Assert
        assignment.Status.Should().Be(AssignmentStatus.Completed);
        assignment.CompletedAt.Should().NotBeNull();
        assignment.CompletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void AddBuddy_ShouldAddBuddyToCollection()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var flow = CreateTestFlow(Guid.NewGuid());
        var assignment = new FlowAssignment(userId, flow.Id, flow.ActiveContent.Id, Guid.NewGuid());
        var buddy = CreateTestUser();

        // Act
        assignment.AddBuddy(buddy);

        // Assert
        assignment.Buddies.Should().Contain(buddy);
        assignment.Buddy.Should().Be(buddy.Id);
    }

    [Fact]
    public void RemoveBuddy_ShouldRemoveBuddyFromCollection()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var flow = CreateTestFlow(Guid.NewGuid());
        var assignment = new FlowAssignment(userId, flow.Id, flow.ActiveContent.Id, Guid.NewGuid());
        var buddy = CreateTestUser();
        assignment.AddBuddy(buddy);

        // Act
        assignment.RemoveBuddy(buddy);

        // Assert
        assignment.Buddies.Should().NotContain(buddy);
        assignment.Buddies.Should().BeEmpty();
    }

    [Fact]
    public void FlowAssignment_Properties_ShouldHaveCorrectDefaults()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var flow = CreateTestFlow(Guid.NewGuid());

        // Act
        var assignment = new FlowAssignment(userId, flow.Id, flow.ActiveContent.Id, Guid.NewGuid());

        // Assert
        assignment.Status.Should().Be(AssignmentStatus.Assigned);
        assignment.Id.Should().NotBe(Guid.Empty);
        assignment.AssignedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    private static Flow CreateTestFlow(Guid flowId, int daysPerStep = 7, int stepCount = 1)
    {
        var createdBy = Guid.NewGuid();
        var flow = new Flow("Test Flow", "Test Description", createdBy);
        
        // Создаем настройки
        var settings = new FlowSettings();
        
        // Создаем контент
        var content = new FlowContent(flow.Id, 1, createdBy);
        
        // Добавляем шаги
        for (int i = 0; i < stepCount; i++)
        {
            var step = new FlowStep(
                content.Id,
                $"Step {i + 1}",
                $"Description for step {i + 1}",
                LexoRankHelper.Middle());
            content.Steps.Add(step);
        }

        // В новой архитектуре нет публичных методов SetSettings/SetActiveContent
        // Используем рефлексию или создаем flow правильно
        
        return flow;
    }

    private static User CreateTestUser()
    {
        return new User
        {
            Id = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "User",
            TelegramUsername = "testuser"
        };
    }
}