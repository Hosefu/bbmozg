using FluentAssertions;
using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Enums;
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
        var snapshotId = Guid.NewGuid();
        var dueDate = DateTime.UtcNow.AddDays(7);
        var buddyId = Guid.NewGuid();
        var assignedById = Guid.NewGuid();
        var notes = "Test notes";
        var priority = 5;

        // Act
        var assignment = new FlowAssignment(userId, flowId, snapshotId, dueDate, buddyId, assignedById, notes, priority);

        // Assert
        assignment.UserId.Should().Be(userId);
        assignment.FlowId.Should().Be(flowId);
        assignment.FlowSnapshotId.Should().Be(snapshotId);
        assignment.DueDate.Should().Be(dueDate);
        assignment.BuddyId.Should().Be(buddyId);
        assignment.AssignedById.Should().Be(assignedById);
        assignment.AdminNotes.Should().Be(notes);
        assignment.Priority.Should().Be(priority);
        assignment.Status.Should().Be(AssignmentStatus.Assigned);
        assignment.ProgressPercent.Should().Be(0);
        assignment.CompletedSteps.Should().Be(0);
        assignment.TotalSteps.Should().Be(0);
        assignment.AttemptCount.Should().Be(1);
        assignment.UserFeedback.Should().Be(string.Empty);
        assignment.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        assignment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CanStart_WhenAssignedAndNotStarted_ShouldReturnTrue()
    {
        // Arrange
        var assignment = CreateDefaultAssignment();

        // Act
        var result = assignment.CanStart();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanStart_WhenAlreadyStarted_ShouldReturnFalse()
    {
        // Arrange
        var assignment = CreateDefaultAssignment();
        assignment.Start();

        // Act
        var result = assignment.CanStart();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Start_WhenCanStart_ShouldUpdateStatus()
    {
        // Arrange
        var assignment = CreateDefaultAssignment();

        // Act
        assignment.Start();

        // Assert
        assignment.Status.Should().Be(AssignmentStatus.InProgress);
        assignment.StartedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        assignment.LastActivityAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        assignment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Start_WhenAlreadyStarted_ShouldThrowException()
    {
        // Arrange
        var assignment = CreateDefaultAssignment();
        assignment.Start();

        // Act & Assert
        var action = () => assignment.Start();
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Pause_WhenInProgress_ShouldUpdateStatus()
    {
        // Arrange
        var assignment = CreateDefaultAssignment();
        assignment.Start();
        var reason = "Test pause reason";

        // Act
        assignment.Pause(reason);

        // Assert
        assignment.Status.Should().Be(AssignmentStatus.Paused);
        assignment.PausedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        assignment.PauseReason.Should().Be(reason);
        assignment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Pause_WhenNotInProgress_ShouldThrowException()
    {
        // Arrange
        var assignment = CreateDefaultAssignment();

        // Act & Assert
        var action = () => assignment.Pause("Test reason");
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Resume_WhenPaused_ShouldUpdateStatus()
    {
        // Arrange
        var assignment = CreateDefaultAssignment();
        assignment.Start();
        assignment.Pause("Test reason");

        // Act
        assignment.Resume();

        // Assert
        assignment.Status.Should().Be(AssignmentStatus.InProgress);
        assignment.PausedAt.Should().BeNull();
        assignment.PauseReason.Should().BeNull();
        assignment.LastActivityAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        assignment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Resume_WhenNotPaused_ShouldThrowException()
    {
        // Arrange
        var assignment = CreateDefaultAssignment();
        assignment.Start();

        // Act & Assert
        var action = () => assignment.Resume();
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Complete_WhenInProgress_ShouldUpdateStatus()
    {
        // Arrange
        var assignment = CreateDefaultAssignment();
        assignment.Start();
        var finalScore = 85;

        // Act
        assignment.Complete(finalScore);

        // Assert
        assignment.Status.Should().Be(AssignmentStatus.Completed);
        assignment.CompletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        assignment.FinalScore.Should().Be(finalScore);
        assignment.ProgressPercent.Should().Be(100);
        assignment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Complete_WhenNotInProgress_ShouldThrowException()
    {
        // Arrange
        var assignment = CreateDefaultAssignment();

        // Act & Assert
        var action = () => assignment.Complete();
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Cancel_WhenNotCompleted_ShouldUpdateStatus()
    {
        // Arrange
        var assignment = CreateDefaultAssignment();
        assignment.Start();
        var reason = "Test cancellation reason";

        // Act
        assignment.Cancel(reason);

        // Assert
        assignment.Status.Should().Be(AssignmentStatus.Cancelled);
        assignment.AdminNotes.Should().Be(reason);
        assignment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Cancel_WhenCompleted_ShouldThrowException()
    {
        // Arrange
        var assignment = CreateDefaultAssignment();
        assignment.Start();
        assignment.Complete();

        // Act & Assert
        var action = () => assignment.Cancel("Test reason");
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ExtendDeadline_WithFutureDate_ShouldUpdateDeadline()
    {
        // Arrange
        var assignment = CreateDefaultAssignment();
        var newDeadline = DateTime.UtcNow.AddDays(14);

        // Act
        assignment.ExtendDeadline(newDeadline);

        // Assert
        assignment.DueDate.Should().Be(newDeadline);
        assignment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void ExtendDeadline_WithPastDate_ShouldThrowException()
    {
        // Arrange
        var assignment = CreateDefaultAssignment();
        var pastDate = DateTime.UtcNow.AddDays(-1);

        // Act & Assert
        var action = () => assignment.ExtendDeadline(pastDate);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UpdateProgress_WithValidValues_ShouldUpdateProgress()
    {
        // Arrange
        var assignment = CreateDefaultAssignment();
        var completedSteps = 3;
        var totalSteps = 5;

        // Act
        assignment.UpdateProgress(completedSteps, totalSteps);

        // Assert
        assignment.CompletedSteps.Should().Be(completedSteps);
        assignment.TotalSteps.Should().Be(totalSteps);
        assignment.ProgressPercent.Should().Be(60); // 3/5 * 100
        assignment.LastActivityAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        assignment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateProgress_WithZeroTotalSteps_ShouldSetProgressToZero()
    {
        // Arrange
        var assignment = CreateDefaultAssignment();

        // Act
        assignment.UpdateProgress(0, 0);

        // Assert
        assignment.ProgressPercent.Should().Be(0);
    }

    [Fact]
    public void AssignBuddy_WithValidId_ShouldAssignBuddy()
    {
        // Arrange
        var assignment = CreateDefaultAssignment();
        var newBuddyId = Guid.NewGuid();

        // Act
        assignment.AssignBuddy(newBuddyId);

        // Assert
        assignment.BuddyId.Should().Be(newBuddyId);
        assignment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void IsOverdue_WhenPastDueAndInProgress_ShouldReturnTrue()
    {
        // Arrange
        var assignment = CreateAssignmentWithDueDate(DateTime.UtcNow.AddDays(-1));
        assignment.Start();

        // Act
        var result = assignment.IsOverdue();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsOverdue_WhenNotPastDue_ShouldReturnFalse()
    {
        // Arrange
        var assignment = CreateAssignmentWithDueDate(DateTime.UtcNow.AddDays(1));
        assignment.Start();

        // Act
        var result = assignment.IsOverdue();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsOverdue_WhenPastDueButCompleted_ShouldReturnFalse()
    {
        // Arrange
        var assignment = CreateAssignmentWithDueDate(DateTime.UtcNow.AddDays(-1));
        assignment.Start();
        assignment.Complete();

        // Act
        var result = assignment.IsOverdue();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsDeadlineApproaching_WhenWithinThreshold_ShouldReturnTrue()
    {
        // Arrange
        var assignment = CreateAssignmentWithDueDate(DateTime.UtcNow.AddDays(2));
        assignment.Start();

        // Act
        var result = assignment.IsDeadlineApproaching(3);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsDeadlineApproaching_WhenOutsideThreshold_ShouldReturnFalse()
    {
        // Arrange
        var assignment = CreateAssignmentWithDueDate(DateTime.UtcNow.AddDays(5));
        assignment.Start();

        // Act
        var result = assignment.IsDeadlineApproaching(3);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void AddUserFeedback_WithValidData_ShouldUpdateFeedback()
    {
        // Arrange
        var assignment = CreateDefaultAssignment();
        var feedback = "Great course!";
        var rating = 5;

        // Act
        assignment.AddUserFeedback(feedback, rating);

        // Assert
        assignment.UserFeedback.Should().Be(feedback);
        assignment.UserRating.Should().Be(rating);
        assignment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void AddUserFeedback_WithInvalidRating_ShouldNotSetRating()
    {
        // Arrange
        var assignment = CreateDefaultAssignment();
        var feedback = "Great course!";
        var invalidRating = 10;

        // Act
        assignment.AddUserFeedback(feedback, invalidRating);

        // Assert
        assignment.UserFeedback.Should().Be(feedback);
        assignment.UserRating.Should().BeNull();
    }

    private static FlowAssignment CreateDefaultAssignment()
    {
        return new FlowAssignment(
            Guid.NewGuid(),
            Guid.NewGuid(), 
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(7),
            null,
            Guid.NewGuid(),
            "Test notes",
            5);
    }

    private static FlowAssignment CreateAssignmentWithDueDate(DateTime dueDate)
    {
        return new FlowAssignment(
            Guid.NewGuid(),
            Guid.NewGuid(), 
            Guid.NewGuid(),
            dueDate,
            null,
            Guid.NewGuid(),
            "Test notes",
            5);
    }
}