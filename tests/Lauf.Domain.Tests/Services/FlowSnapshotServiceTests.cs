using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Entities.Snapshots;
using Lauf.Domain.Enums;
using Lauf.Domain.Exceptions;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Domain.Services;
using Moq;
using Xunit;

namespace Lauf.Domain.Tests.Services;

/// <summary>
/// Тесты для FlowSnapshotService
/// </summary>
public class FlowSnapshotServiceTests
{
    private readonly Mock<IFlowRepository> _flowRepositoryMock;
    private readonly Mock<IFlowSnapshotRepository> _snapshotRepositoryMock;
    private readonly FlowSnapshotService _service;

    public FlowSnapshotServiceTests()
    {
        _flowRepositoryMock = new Mock<IFlowRepository>();
        _snapshotRepositoryMock = new Mock<IFlowSnapshotRepository>();
        _service = new FlowSnapshotService(_flowRepositoryMock.Object, _snapshotRepositoryMock.Object);
    }

    [Fact]
    public void Constructor_NullFlowRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new FlowSnapshotService(null!, _snapshotRepositoryMock.Object));
    }

    [Fact]
    public void Constructor_NullSnapshotRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new FlowSnapshotService(_flowRepositoryMock.Object, null!));
    }

    [Fact]
    public async Task CreateFlowSnapshotAsync_ValidFlow_CreatesSnapshot()
    {
        // Arrange
        var flow = CreateTestFlow();
        var existingSnapshots = new List<FlowSnapshot>();
        
        _snapshotRepositoryMock
            .Setup(x => x.GetByOriginalFlowIdAsync(flow.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingSnapshots);

        _snapshotRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<FlowSnapshot>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateFlowSnapshotAsync(flow);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(flow.Id, result.OriginalFlowId);
        Assert.Equal(flow.Title, result.Title);
        Assert.Equal(flow.Description, result.Description);
        Assert.Equal(1, result.Version); // First version
        Assert.Equal(flow.Steps.Count, result.Steps.Count);

        _snapshotRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<FlowSnapshot>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateFlowSnapshotAsync_WithExistingSnapshots_CreatesNextVersion()
    {
        // Arrange
        var flow = CreateTestFlow();
        var existingSnapshots = new List<FlowSnapshot>
        {
            CreateTestFlowSnapshot(flow.Id, version: 1),
            CreateTestFlowSnapshot(flow.Id, version: 2)
        };
        
        _snapshotRepositoryMock
            .Setup(x => x.GetByOriginalFlowIdAsync(flow.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingSnapshots);

        _snapshotRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<FlowSnapshot>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateFlowSnapshotAsync(flow);

        // Assert
        Assert.Equal(3, result.Version); // Next version
    }

    [Fact]
    public async Task CreateFlowSnapshotAsync_NullFlow_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _service.CreateFlowSnapshotAsync(null!));
    }

    [Fact]
    public async Task CreateFlowSnapshotAsync_ByFlowId_FlowNotFound_ThrowsFlowNotFoundException()
    {
        // Arrange
        var flowId = Guid.NewGuid();
        
        _flowRepositoryMock
            .Setup(x => x.GetWithDetailsAsync(flowId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Flow)null!);

        // Act & Assert
        await Assert.ThrowsAsync<FlowNotFoundException>(() => 
            _service.CreateFlowSnapshotAsync(flowId, 1));
    }

    [Fact]
    public async Task GetFlowSnapshotAsync_ValidId_ReturnsSnapshot()
    {
        // Arrange
        var snapshotId = Guid.NewGuid();
        var expectedSnapshot = CreateTestFlowSnapshot(Guid.NewGuid());
        
        _snapshotRepositoryMock
            .Setup(x => x.GetByIdWithDetailsAsync(snapshotId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedSnapshot);

        // Act
        var result = await _service.GetFlowSnapshotAsync(snapshotId);

        // Assert
        Assert.Equal(expectedSnapshot, result);
    }

    [Fact]
    public async Task GetFlowSnapshotByAssignmentAsync_ValidAssignmentId_ReturnsSnapshot()
    {
        // Arrange
        var assignmentId = Guid.NewGuid();
        var expectedSnapshot = CreateTestFlowSnapshot(Guid.NewGuid());
        
        _snapshotRepositoryMock
            .Setup(x => x.GetByAssignmentIdAsync(assignmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedSnapshot);

        // Act
        var result = await _service.GetFlowSnapshotByAssignmentAsync(assignmentId);

        // Assert
        Assert.Equal(expectedSnapshot, result);
    }

    [Fact]
    public async Task ValidateSnapshotIntegrityAsync_ValidSnapshot_ReturnsTrue()
    {
        // Arrange
        var snapshotId = Guid.NewGuid();
        var snapshot = CreateValidSnapshot();
        
        _snapshotRepositoryMock
            .Setup(x => x.GetByIdWithDetailsAsync(snapshotId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(snapshot);

        // Act
        var result = await _service.ValidateSnapshotIntegrityAsync(snapshotId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ValidateSnapshotIntegrityAsync_SnapshotNotFound_ReturnsFalse()
    {
        // Arrange
        var snapshotId = Guid.NewGuid();
        
        _snapshotRepositoryMock
            .Setup(x => x.GetByIdWithDetailsAsync(snapshotId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((FlowSnapshot)null!);

        // Act
        var result = await _service.ValidateSnapshotIntegrityAsync(snapshotId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ValidateSnapshotIntegrityAsync_InvalidSnapshot_ReturnsFalse()
    {
        // Arrange
        var snapshotId = Guid.NewGuid();
        var invalidSnapshot = CreateInvalidSnapshot();
        
        _snapshotRepositoryMock
            .Setup(x => x.GetByIdWithDetailsAsync(snapshotId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invalidSnapshot);

        // Act
        var result = await _service.ValidateSnapshotIntegrityAsync(snapshotId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CleanupOldSnapshotsAsync_HasOldSnapshots_DeletesCorrectAmount()
    {
        // Arrange
        var olderThanDays = 365;
        var keepMinimum = 1;
        var flowId = Guid.NewGuid();
        
        var oldSnapshots = new List<FlowSnapshot>
        {
            CreateTestFlowSnapshot(flowId, version: 1),
            CreateTestFlowSnapshot(flowId, version: 2),
            CreateTestFlowSnapshot(flowId, version: 3)
        };
        
        _snapshotRepositoryMock
            .Setup(x => x.GetOldSnapshotsAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(oldSnapshots);

        _snapshotRepositoryMock
            .Setup(x => x.HasActiveAssignmentsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _snapshotRepositoryMock
            .Setup(x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CleanupOldSnapshotsAsync(olderThanDays, keepMinimum);

        // Assert
        Assert.Equal(2, result); // Should delete 2 out of 3, keeping 1
        _snapshotRepositoryMock.Verify(
            x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }

    private static Flow CreateTestFlow()
    {
        var flow = new Flow("Test Flow", "Test Description");
        var step = new FlowStep(flow.Id, "Test Step", "Step Description", 0, false, 60);
        var component = new FlowStepComponent(step.Id, ComponentType.Article, "Test Component", "Component Description", 0, true, 30);
        
        step.AddComponent(component);
        flow.AddStep(step);
        
        return flow;
    }

    private static FlowSnapshot CreateTestFlowSnapshot(Guid originalFlowId, int version = 1)
    {
        return new FlowSnapshot(
            originalFlowId,
            "Test Flow",
            "Test Description",
            FlowStatus.Published,
            10,
            5,
            true,
            "[]",
            version);
    }

    private static FlowSnapshot CreateValidSnapshot()
    {
        var snapshot = CreateTestFlowSnapshot(Guid.NewGuid());
        var stepSnapshot = new FlowStepSnapshot(
            Guid.NewGuid(),
            snapshot.Id,
            "Test Step",
            "Step Description",
            0,
            false,
            60);
        
        var componentSnapshot = new ComponentSnapshot(
            Guid.NewGuid(),
            stepSnapshot.Id,
            ComponentType.Article,
            "Test Component",
            "Component Description",
            0,
            true,
            30,
            "{\"content\": \"test\"}",
            "{}");
        
        stepSnapshot.AddComponentSnapshot(componentSnapshot);
        snapshot.AddStepSnapshot(stepSnapshot);
        
        return snapshot;
    }

    private static FlowSnapshot CreateInvalidSnapshot()
    {
        // Создаем снапшот с пустым названием (невалидный)
        return new FlowSnapshot(
            Guid.NewGuid(),
            "", // Пустое название делает снапшот невалидным
            "Description",
            FlowStatus.Published,
            10,
            5,
            true,
            "[]",
            1);
    }
}