using Lauf.Domain.Entities.Snapshots;
using Lauf.Domain.Enums;
using Xunit;

namespace Lauf.Domain.Tests.Entities.Snapshots;

/// <summary>
/// Тесты для сущности FlowSnapshot
/// </summary>
public class FlowSnapshotTests
{
    [Fact]
    public void Constructor_ValidParameters_CreatesFlowSnapshot()
    {
        // Arrange
        var originalFlowId = Guid.NewGuid();
        var title = "Test Flow";
        var description = "Test Description";
        var status = FlowStatus.Published;
        var estimatedHours = 10;
        var workingDays = 5;
        var isRequired = true;
        var tags = "[\"tag1\", \"tag2\"]";
        var version = 1;

        // Act
        var snapshot = new FlowSnapshot(
            originalFlowId,
            title,
            description,
            status,
            estimatedHours,
            workingDays,
            isRequired,
            tags,
            version);

        // Assert
        Assert.NotEqual(Guid.Empty, snapshot.Id);
        Assert.Equal(originalFlowId, snapshot.OriginalFlowId);
        Assert.Equal(title, snapshot.Title);
        Assert.Equal(description, snapshot.Description);
        Assert.Equal(status, snapshot.Status);
        Assert.Equal(estimatedHours, snapshot.EstimatedHours);
        Assert.Equal(workingDays, snapshot.WorkingDaysToComplete);
        Assert.Equal(isRequired, snapshot.IsRequired);
        Assert.Equal(tags, snapshot.Tags);
        Assert.Equal(version, snapshot.Version);
        Assert.True(snapshot.CreatedAt <= DateTime.UtcNow);
        Assert.Empty(snapshot.Steps);
    }

    [Fact]
    public void Constructor_NullTitle_ThrowsArgumentNullException()
    {
        // Arrange
        var originalFlowId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new FlowSnapshot(
            originalFlowId,
            null!,
            "description",
            FlowStatus.Published,
            10,
            5,
            true,
            "[]"));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Constructor_NegativeEstimatedHours_ThrowsArgumentException(int invalidHours)
    {
        // Arrange
        var originalFlowId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new FlowSnapshot(
            originalFlowId,
            "title",
            "description",
            FlowStatus.Published,
            invalidHours,
            5,
            true,
            "[]"));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-5)]
    public void Constructor_NegativeWorkingDays_ThrowsArgumentException(int invalidDays)
    {
        // Arrange
        var originalFlowId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new FlowSnapshot(
            originalFlowId,
            "title",
            "description",
            FlowStatus.Published,
            10,
            invalidDays,
            true,
            "[]"));
    }

    [Fact]
    public void AddStepSnapshot_ValidStep_AddsToCollection()
    {
        // Arrange
        var flowSnapshot = CreateTestFlowSnapshot();
        var stepSnapshot = CreateTestStepSnapshot(flowSnapshot.Id);

        // Act
        flowSnapshot.AddStepSnapshot(stepSnapshot);

        // Assert
        Assert.Single(flowSnapshot.Steps);
        Assert.Contains(stepSnapshot, flowSnapshot.Steps);
    }

    [Fact]
    public void AddStepSnapshot_NullStep_ThrowsArgumentNullException()
    {
        // Arrange
        var flowSnapshot = CreateTestFlowSnapshot();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => flowSnapshot.AddStepSnapshot(null!));
    }

    [Fact]
    public void GetTotalComponentsCount_WithStepsAndComponents_ReturnsCorrectCount()
    {
        // Arrange
        var flowSnapshot = CreateTestFlowSnapshot();
        
        var step1 = CreateTestStepSnapshot(flowSnapshot.Id);
        step1.AddComponentSnapshot(CreateTestComponentSnapshot(step1.Id, 0));
        step1.AddComponentSnapshot(CreateTestComponentSnapshot(step1.Id, 1));
        
        var step2 = CreateTestStepSnapshot(flowSnapshot.Id);
        step2.AddComponentSnapshot(CreateTestComponentSnapshot(step2.Id, 0));
        
        flowSnapshot.AddStepSnapshot(step1);
        flowSnapshot.AddStepSnapshot(step2);

        // Act
        var totalComponents = flowSnapshot.GetTotalComponentsCount();

        // Assert
        Assert.Equal(3, totalComponents);
    }

    [Fact]
    public void GetTotalComponentsCount_NoSteps_ReturnsZero()
    {
        // Arrange
        var flowSnapshot = CreateTestFlowSnapshot();

        // Act
        var totalComponents = flowSnapshot.GetTotalComponentsCount();

        // Assert
        Assert.Equal(0, totalComponents);
    }

    [Fact]
    public void GetOrderedSteps_ReturnsStepsInCorrectOrder()
    {
        // Arrange
        var flowSnapshot = CreateTestFlowSnapshot();
        
        var step3 = CreateTestStepSnapshot(flowSnapshot.Id, order: 2);
        var step1 = CreateTestStepSnapshot(flowSnapshot.Id, order: 0);
        var step2 = CreateTestStepSnapshot(flowSnapshot.Id, order: 1);
        
        flowSnapshot.AddStepSnapshot(step3);
        flowSnapshot.AddStepSnapshot(step1);
        flowSnapshot.AddStepSnapshot(step2);

        // Act
        var orderedSteps = flowSnapshot.GetOrderedSteps();

        // Assert
        Assert.Equal(3, orderedSteps.Count);
        Assert.Equal(0, orderedSteps[0].Order);
        Assert.Equal(1, orderedSteps[1].Order);
        Assert.Equal(2, orderedSteps[2].Order);
    }

    [Fact]
    public void Constructor_DefaultValues_SetsCorrectDefaults()
    {
        // Arrange
        var originalFlowId = Guid.NewGuid();

        // Act
        var snapshot = new FlowSnapshot(
            originalFlowId,
            "title",
            null!, // description
            FlowStatus.Published,
            10,
            5,
            true,
            null!); // tags

        // Assert
        Assert.Equal(string.Empty, snapshot.Description);
        Assert.Equal("[]", snapshot.Tags);
        Assert.Equal(1, snapshot.Version); // default version
    }

    private static FlowSnapshot CreateTestFlowSnapshot()
    {
        return new FlowSnapshot(
            Guid.NewGuid(),
            "Test Flow",
            "Test Description",
            FlowStatus.Published,
            10,
            5,
            true,
            "[]",
            1);
    }

    private static FlowStepSnapshot CreateTestStepSnapshot(Guid flowSnapshotId, int order = 0)
    {
        return new FlowStepSnapshot(
            Guid.NewGuid(),
            flowSnapshotId,
            $"Step {order}",
            "Step Description",
            order,
            false,
            60);
    }

    private static ComponentSnapshot CreateTestComponentSnapshot(Guid stepSnapshotId, int order = 0)
    {
        return new ComponentSnapshot(
            Guid.NewGuid(),
            stepSnapshotId,
            ComponentType.Article,
            $"Component {order}",
            "Component Description",
            order,
            true,
            30,
            "{}",
            "{}");
    }
}