using BuddyBot.Domain.Entities.Flows;
using BuddyBot.Domain.Enums;
using FluentAssertions;

namespace BuddyBot.Domain.Tests.Entities;

public class FlowAssignmentTests
{
    [Fact]
    public void FlowAssignment_DefaultValues_ShouldBeSetCorrectly()
    {
        // Act
        var assignment = new FlowAssignment();

        // Assert
        assignment.Status.Should().Be(AssignmentStatus.Assigned);
        assignment.Priority.Should().Be(0);
        assignment.ProgressPercent.Should().Be(0);
        assignment.CompletedSteps.Should().Be(0);
        assignment.TotalSteps.Should().Be(0);
        assignment.AttemptCount.Should().Be(1);
        assignment.AssignedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        assignment.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        assignment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CanStart_WithAssignedStatus_ShouldReturnTrue()
    {
        // Arrange
        var assignment = new FlowAssignment
        {
            Status = AssignmentStatus.Assigned,
            StartedAt = null
        };

        // Act
        var canStart = assignment.CanStart();

        // Assert
        canStart.Should().BeTrue();
    }

    [Theory]
    [InlineData(AssignmentStatus.InProgress)]
    [InlineData(AssignmentStatus.Completed)]
    [InlineData(AssignmentStatus.Cancelled)]
    [InlineData(AssignmentStatus.Paused)]
    public void CanStart_WithNonAssignedStatus_ShouldReturnFalse(AssignmentStatus status)
    {
        // Arrange
        var assignment = new FlowAssignment
        {
            Status = status,
            StartedAt = null
        };

        // Act
        var canStart = assignment.CanStart();

        // Assert
        canStart.Should().BeFalse();
    }

    [Fact]
    public void CanStart_WithAlreadyStarted_ShouldReturnFalse()
    {
        // Arrange
        var assignment = new FlowAssignment
        {
            Status = AssignmentStatus.Assigned,
            StartedAt = DateTime.UtcNow
        };

        // Act
        var canStart = assignment.CanStart();

        // Assert
        canStart.Should().BeFalse();
    }

    [Fact]
    public void Start_WithValidAssignment_ShouldUpdateStatusAndDates()
    {
        // Arrange
        var assignment = new FlowAssignment
        {
            Status = AssignmentStatus.Assigned,
            StartedAt = null
        };

        // Act
        assignment.Start();

        // Assert
        assignment.Status.Should().Be(AssignmentStatus.InProgress);
        assignment.StartedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        assignment.LastActivityAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        assignment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Start_WithInvalidAssignment_ShouldThrowException()
    {
        // Arrange
        var assignment = new FlowAssignment
        {
            Status = AssignmentStatus.InProgress
        };

        // Act & Assert
        var act = () => assignment.Start();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Назначение не может быть запущено в текущем состоянии");
    }

    [Fact]
    public void Pause_WithInProgressAssignment_ShouldUpdateStatusAndReason()
    {
        // Arrange
        var assignment = new FlowAssignment
        {
            Status = AssignmentStatus.InProgress
        };
        var reason = "User requested pause";

        // Act
        assignment.Pause(reason);

        // Assert
        assignment.Status.Should().Be(AssignmentStatus.Paused);
        assignment.PausedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        assignment.PauseReason.Should().Be(reason);
        assignment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Pause_WithNonInProgressAssignment_ShouldThrowException()
    {
        // Arrange
        var assignment = new FlowAssignment
        {
            Status = AssignmentStatus.Assigned
        };

        // Act & Assert
        var act = () => assignment.Pause("reason");
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Только активные назначения могут быть поставлены на паузу");
    }

    [Fact]
    public void Resume_WithPausedAssignment_ShouldUpdateStatus()
    {
        // Arrange
        var assignment = new FlowAssignment
        {
            Status = AssignmentStatus.Paused,
            PausedAt = DateTime.UtcNow.AddHours(-1),
            PauseReason = "User requested"
        };

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
    public void Resume_WithNonPausedAssignment_ShouldThrowException()
    {
        // Arrange
        var assignment = new FlowAssignment
        {
            Status = AssignmentStatus.InProgress
        };

        // Act & Assert
        var act = () => assignment.Resume();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Только приостановленные назначения могут быть возобновлены");
    }

    [Fact]
    public void Complete_WithInProgressAssignment_ShouldUpdateStatusAndProgress()
    {
        // Arrange
        var assignment = new FlowAssignment
        {
            Status = AssignmentStatus.InProgress
        };
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
    public void Complete_WithNonInProgressAssignment_ShouldThrowException()
    {
        // Arrange
        var assignment = new FlowAssignment
        {
            Status = AssignmentStatus.Assigned
        };

        // Act & Assert
        var act = () => assignment.Complete();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Только активные назначения могут быть завершены");
    }

    [Fact]
    public void Cancel_ShouldUpdateStatusAndReason()
    {
        // Arrange
        var assignment = new FlowAssignment
        {
            Status = AssignmentStatus.InProgress
        };
        var reason = "User requested cancellation";

        // Act
        assignment.Cancel(reason);

        // Assert
        assignment.Status.Should().Be(AssignmentStatus.Cancelled);
        assignment.AdminNotes.Should().Be(reason);
        assignment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Cancel_WithCompletedAssignment_ShouldThrowException()
    {
        // Arrange
        var assignment = new FlowAssignment
        {
            Status = AssignmentStatus.Completed
        };

        // Act & Assert
        var act = () => assignment.Cancel("reason");
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Завершенные назначения не могут быть отменены");
    }

    [Fact]
    public void ExtendDeadline_WithFutureDate_ShouldUpdateDeadline()
    {
        // Arrange
        var assignment = new FlowAssignment();
        var newDeadline = DateTime.UtcNow.AddDays(7);

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
        var assignment = new FlowAssignment();
        var pastDate = DateTime.UtcNow.AddDays(-1);

        // Act & Assert
        var act = () => assignment.ExtendDeadline(pastDate);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Новый дедлайн должен быть в будущем");
    }

    [Fact]
    public void UpdateProgress_ShouldCalculateCorrectPercentage()
    {
        // Arrange
        var assignment = new FlowAssignment();

        // Act
        assignment.UpdateProgress(3, 10);

        // Assert
        assignment.CompletedSteps.Should().Be(3);
        assignment.TotalSteps.Should().Be(10);
        assignment.ProgressPercent.Should().Be(30);
        assignment.LastActivityAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        assignment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateProgress_WithZeroTotalSteps_ShouldSetProgressToZero()
    {
        // Arrange
        var assignment = new FlowAssignment();

        // Act
        assignment.UpdateProgress(0, 0);

        // Assert
        assignment.ProgressPercent.Should().Be(0);
    }

    [Fact]
    public void IsOverdue_WithPastDueDateAndInProgress_ShouldReturnTrue()
    {
        // Arrange
        var assignment = new FlowAssignment
        {
            DueDate = DateTime.UtcNow.AddDays(-1),
            Status = AssignmentStatus.InProgress
        };

        // Act
        var isOverdue = assignment.IsOverdue();

        // Assert
        isOverdue.Should().BeTrue();
    }

    [Fact]
    public void IsOverdue_WithFutureDueDate_ShouldReturnFalse()
    {
        // Arrange
        var assignment = new FlowAssignment
        {
            DueDate = DateTime.UtcNow.AddDays(1),
            Status = AssignmentStatus.InProgress
        };

        // Act
        var isOverdue = assignment.IsOverdue();

        // Assert
        isOverdue.Should().BeFalse();
    }

    [Fact]
    public void IsOverdue_WithNoDueDate_ShouldReturnFalse()
    {
        // Arrange
        var assignment = new FlowAssignment
        {
            DueDate = null,
            Status = AssignmentStatus.InProgress
        };

        // Act
        var isOverdue = assignment.IsOverdue();

        // Assert
        isOverdue.Should().BeFalse();
    }

    [Fact]
    public void IsDeadlineApproaching_WithNearDeadline_ShouldReturnTrue()
    {
        // Arrange
        var assignment = new FlowAssignment
        {
            DueDate = DateTime.UtcNow.AddDays(2),
            Status = AssignmentStatus.InProgress
        };

        // Act
        var isApproaching = assignment.IsDeadlineApproaching(3);

        // Assert
        isApproaching.Should().BeTrue();
    }

    [Fact]
    public void IsDeadlineApproaching_WithDistantDeadline_ShouldReturnFalse()
    {
        // Arrange
        var assignment = new FlowAssignment
        {
            DueDate = DateTime.UtcNow.AddDays(5),
            Status = AssignmentStatus.InProgress
        };

        // Act
        var isApproaching = assignment.IsDeadlineApproaching(3);

        // Assert
        isApproaching.Should().BeFalse();
    }

    [Fact]
    public void AddUserFeedback_ShouldUpdateFeedbackAndRating()
    {
        // Arrange
        var assignment = new FlowAssignment();
        var feedback = "Great course!";
        var rating = 5;

        // Act
        assignment.AddUserFeedback(feedback, rating);

        // Assert
        assignment.UserFeedback.Should().Be(feedback);
        assignment.UserRating.Should().Be(rating);
        assignment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void AddUserFeedback_WithInvalidRating_ShouldNotSetRating(int invalidRating)
    {
        // Arrange
        var assignment = new FlowAssignment();
        var feedback = "Test feedback";

        // Act
        assignment.AddUserFeedback(feedback, invalidRating);

        // Assert
        assignment.UserFeedback.Should().Be(feedback);
        assignment.UserRating.Should().BeNull();
    }
}