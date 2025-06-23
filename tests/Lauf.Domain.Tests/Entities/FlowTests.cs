using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Enums;
using FluentAssertions;

namespace Lauf.Domain.Tests.Entities;

public class FlowTests
{
    [Fact]
    public void Flow_DefaultValues_ShouldBeSetCorrectly()
    {
        // Act
        var flow = new Flow("Test Flow", "Test Description");

        // Assert
        flow.Status.Should().Be(FlowStatus.Draft);
        flow.Version.Should().Be(1);
        flow.Priority.Should().Be(0);
        flow.IsRequired.Should().BeFalse();
        flow.Steps.Should().BeEmpty();
        flow.Assignments.Should().BeEmpty();
        flow.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        flow.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void TotalSteps_ShouldReturnCorrectCount()
    {
        // Arrange
        var flow = new Flow("Test Flow", "Test Description");
        flow.Steps.Add(new FlowStep(flow.Id, "Step 1", "Step 1 Description", 1));
        flow.Steps.Add(new FlowStep(flow.Id, "Step 2", "Step 2 Description", 2));

        // Act
        var totalSteps = flow.TotalSteps;

        // Assert
        totalSteps.Should().Be(2);
    }

    [Fact]
    public void EstimatedDurationMinutes_ShouldReturnSumOfStepDurations()
    {
        // Arrange
        var flow = new Flow("Test Flow", "Test Description");
        flow.Steps.Add(new FlowStep(flow.Id, "Step 1", "Step 1 Description", 1, true, 30));
        flow.Steps.Add(new FlowStep(flow.Id, "Step 2", "Step 2 Description", 2, true, 45));

        // Act
        var totalDuration = flow.EstimatedDurationMinutes;

        // Assert
        totalDuration.Should().Be(75);
    }

    [Fact]
    public void CanBePublished_WithValidFlow_ShouldReturnTrue()
    {
        // Arrange
        var flow = new Flow("Test Flow", "Test Description")
        {
            Status = FlowStatus.Draft,
            Settings = new FlowSettings()
        };
        flow.Steps.Add(new FlowStep(flow.Id, "Step 1", "Step 1 Description", 1));

        // Act
        var canBePublished = flow.CanBePublished();

        // Assert
        canBePublished.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "Description", false)] // Empty title
    [InlineData("Title", "", false)] // Empty description
    [InlineData("Title", "Description", false)] // No steps
    public void CanBePublished_WithInvalidFlow_ShouldReturnFalse(string title, string description, bool addStep)
    {
        // Arrange
        var flow = new Flow(title, description)
        {
            Status = FlowStatus.Draft,
            Settings = new FlowSettings()
        };

        if (addStep)
        {
            flow.Steps.Add(new FlowStep(flow.Id, "Step 1", "Step 1 Description", 1));
        }

        // Act
        var canBePublished = flow.CanBePublished();

        // Assert
        canBePublished.Should().BeFalse();
    }

    [Fact]
    public void CanBePublished_WithPublishedStatus_ShouldReturnFalse()
    {
        // Arrange
        var flow = new Flow("Test Flow", "Test Description")
        {
            Status = FlowStatus.Published,
            Settings = new FlowSettings()
        };
        flow.Steps.Add(new FlowStep(flow.Id, "Step 1", "Step 1 Description", 1));

        // Act
        var canBePublished = flow.CanBePublished();

        // Assert
        canBePublished.Should().BeFalse();
    }

    [Fact]
    public void Publish_WithValidFlow_ShouldUpdateStatusAndDates()
    {
        // Arrange
        var flow = new Flow("Test Flow", "Test Description")
        {
            Status = FlowStatus.Draft,
            Settings = new FlowSettings()
        };
        flow.Steps.Add(new FlowStep(flow.Id, "Step 1", "Step 1 Description", 1));

        // Act
        flow.Publish();

        // Assert
        flow.Status.Should().Be(FlowStatus.Published);
        flow.PublishedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        flow.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Publish_WithInvalidFlow_ShouldThrowException()
    {
        // Arrange
        var flow = new Flow("", "Test Description") // Invalid title
        {
            Status = FlowStatus.Draft
        };

        // Act & Assert
        var act = () => flow.Publish();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Поток не может быть опубликован в текущем состоянии");
    }

    [Fact]
    public void Archive_ShouldUpdateStatusAndDate()
    {
        // Arrange
        var flow = new Flow("Test Flow", "Test Description")
        {
            Status = FlowStatus.Published
        };

        // Act
        flow.Archive();

        // Assert
        flow.Status.Should().Be(FlowStatus.Archived);
        flow.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void ReturnToDraft_ShouldUpdateStatusAndClearPublishedDate()
    {
        // Arrange
        var flow = new Flow("Test Flow", "Test Description")
        {
            Status = FlowStatus.Published,
            PublishedAt = DateTime.UtcNow.AddDays(-1)
        };

        // Act
        flow.ReturnToDraft();

        // Assert
        flow.Status.Should().Be(FlowStatus.Draft);
        flow.PublishedAt.Should().BeNull();
        flow.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void IncrementVersion_ShouldIncreaseVersionAndUpdateDate()
    {
        // Arrange
        var flow = new Flow("Test Flow", "Test Description")
        {
            Version = 1
        };

        // Act
        flow.IncrementVersion();

        // Assert
        flow.Version.Should().Be(2);
        flow.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void AddStep_ShouldAddStepWithCorrectOrder()
    {
        // Arrange
        var flow = new Flow("Test Flow", "Test Description") { Id = Guid.NewGuid() };
        var step = new FlowStep(flow.Id, "New Step", "New Step Description", 1);

        // Act
        flow.AddStep(step);

        // Assert
        flow.Steps.Should().HaveCount(1);
        flow.Steps.First().Should().Be(step);
        step.Order.Should().Be(1);
        step.FlowId.Should().Be(flow.Id);
        flow.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void AddStep_MultipleSteps_ShouldMaintainCorrectOrder()
    {
        // Arrange
        var flow = new Flow("Test Flow", "Test Description") { Id = Guid.NewGuid() };
        var step1 = new FlowStep(flow.Id, "Step 1", "Step 1 Description", 1);
        var step2 = new FlowStep(flow.Id, "Step 2", "Step 2 Description", 2);

        // Act
        flow.AddStep(step1);
        flow.AddStep(step2);

        // Assert
        flow.Steps.Should().HaveCount(2);
        step1.Order.Should().Be(1);
        step2.Order.Should().Be(2);
    }

    [Fact]
    public void RemoveStep_ShouldRemoveStepAndReorderRemaining()
    {
        // Arrange
        var flow = new Flow("Test Flow", "Test Description") { Id = Guid.NewGuid() };
        var step1 = new FlowStep(flow.Id, "Step 1", "Step 1 Description", 1) { Id = Guid.NewGuid() };
        var step2 = new FlowStep(flow.Id, "Step 2", "Step 2 Description", 2) { Id = Guid.NewGuid() };
        var step3 = new FlowStep(flow.Id, "Step 3", "Step 3 Description", 3) { Id = Guid.NewGuid() };

        flow.AddStep(step1);
        flow.AddStep(step2);
        flow.AddStep(step3);

        // Act
        flow.RemoveStep(step2.Id);

        // Assert
        flow.Steps.Should().HaveCount(2);
        flow.Steps.Should().NotContain(step2);
        step1.Order.Should().Be(1);
        step3.Order.Should().Be(2);
    }

    [Fact]
    public void RemoveStep_WithNonExistentId_ShouldNotThrow()
    {
        // Arrange
        var flow = new Flow("Test Flow", "Test Description") { Id = Guid.NewGuid() };
        var step = new FlowStep(flow.Id, "Step 1", "Step 1 Description", 1) { Id = Guid.NewGuid() };
        flow.AddStep(step);

        // Act
        var act = () => flow.RemoveStep(Guid.NewGuid());

        // Assert
        act.Should().NotThrow();
        flow.Steps.Should().HaveCount(1);
    }
}