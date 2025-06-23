using BuddyBot.Shared.Constants;
using FluentAssertions;

namespace BuddyBot.Shared.Tests.Constants;

public class NotificationTypesTests
{
    [Fact]
    public void NotificationTypes_ShouldHaveCorrectConstantValues()
    {
        // Arrange & Act & Assert
        NotificationTypes.Info.Should().Be("Info");
        NotificationTypes.Warning.Should().Be("Warning");
        NotificationTypes.Error.Should().Be("Error");
        NotificationTypes.Success.Should().Be("Success");
        NotificationTypes.Reminder.Should().Be("Reminder");
        NotificationTypes.NewAssignment.Should().Be("NewAssignment");
        NotificationTypes.DeadlineApproaching.Should().Be("DeadlineApproaching");
        NotificationTypes.DeadlineOverdue.Should().Be("DeadlineOverdue");
        NotificationTypes.ComponentCompleted.Should().Be("ComponentCompleted");
        NotificationTypes.StepCompleted.Should().Be("StepCompleted");
        NotificationTypes.FlowCompleted.Should().Be("FlowCompleted");
        NotificationTypes.StepUnlocked.Should().Be("StepUnlocked");
        NotificationTypes.AchievementEarned.Should().Be("AchievementEarned");
        NotificationTypes.System.Should().Be("System");
    }

    [Fact]
    public void AllTypes_ShouldContainAllDefinedTypes()
    {
        // Arrange
        var expectedTypes = new[]
        {
            "Info", "Warning", "Error", "Success", "Reminder",
            "NewAssignment", "DeadlineApproaching", "DeadlineOverdue",
            "ComponentCompleted", "StepCompleted", "FlowCompleted",
            "StepUnlocked", "AchievementEarned", "System"
        };

        // Act & Assert
        NotificationTypes.AllTypes.Should().BeEquivalentTo(expectedTypes);
        NotificationTypes.AllTypes.Should().HaveCount(14);
    }

    [Fact]
    public void CriticalTypes_ShouldContainOnlyCriticalNotifications()
    {
        // Arrange
        var expectedCriticalTypes = new[] { "Error", "DeadlineOverdue" };

        // Act & Assert
        NotificationTypes.CriticalTypes.Should().BeEquivalentTo(expectedCriticalTypes);
        NotificationTypes.CriticalTypes.Should().HaveCount(2);
    }

    [Fact]
    public void PositiveTypes_ShouldContainOnlyPositiveNotifications()
    {
        // Arrange
        var expectedPositiveTypes = new[]
        {
            "Success", "ComponentCompleted", "StepCompleted",
            "FlowCompleted", "StepUnlocked", "AchievementEarned"
        };

        // Act & Assert
        NotificationTypes.PositiveTypes.Should().BeEquivalentTo(expectedPositiveTypes);
        NotificationTypes.PositiveTypes.Should().HaveCount(6);
    }

    [Fact]
    public void AllTypes_ShouldNotContainNullOrEmptyValues()
    {
        // Act & Assert
        NotificationTypes.AllTypes.Should().NotContainNulls();
        NotificationTypes.AllTypes.Should().NotContain(string.Empty);
        NotificationTypes.AllTypes.All(type => !string.IsNullOrWhiteSpace(type)).Should().BeTrue();
    }

    [Fact]
    public void AllTypes_ShouldNotHaveDuplicates()
    {
        // Act & Assert
        NotificationTypes.AllTypes.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void CriticalTypes_ShouldBeSubsetOfAllTypes()
    {
        // Act & Assert
        NotificationTypes.CriticalTypes.All(type => NotificationTypes.AllTypes.Contains(type)).Should().BeTrue();
    }

    [Fact]
    public void PositiveTypes_ShouldBeSubsetOfAllTypes()
    {
        // Act & Assert
        NotificationTypes.PositiveTypes.All(type => NotificationTypes.AllTypes.Contains(type)).Should().BeTrue();
    }

    [Theory]
    [InlineData("Error")]
    [InlineData("DeadlineOverdue")]
    public void CriticalType_ShouldBeInCriticalTypes(string type)
    {
        // Act & Assert
        NotificationTypes.CriticalTypes.Should().Contain(type);
    }

    [Theory]
    [InlineData("Success")]
    [InlineData("ComponentCompleted")]
    [InlineData("AchievementEarned")]
    public void PositiveType_ShouldBeInPositiveTypes(string type)
    {
        // Act & Assert
        NotificationTypes.PositiveTypes.Should().Contain(type);
    }
}