using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Lauf.Application.Commands.FlowAssignment;
using Lauf.Application.Services.Interfaces;
using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Entities.Users;
using Lauf.Domain.Interfaces;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Domain.Services.Interfaces;
using Lauf.Domain.Enums;
using Xunit;

namespace Lauf.Application.Tests.Commands.FlowAssignment;

/// <summary>
/// Тесты для AssignFlowCommandHandler
/// </summary>
public class AssignFlowCommandHandlerTests
{
    private readonly Mock<IFlowRepository> _flowRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IFlowAssignmentRepository> _assignmentRepositoryMock;
    private readonly Mock<IFlowSnapshotService> _snapshotServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<ILogger<AssignFlowCommandHandler>> _loggerMock;
    private readonly AssignFlowCommandHandler _handler;

    public AssignFlowCommandHandlerTests()
    {
        _flowRepositoryMock = new Mock<IFlowRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _assignmentRepositoryMock = new Mock<IFlowAssignmentRepository>();
        _snapshotServiceMock = new Mock<IFlowSnapshotService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _loggerMock = new Mock<ILogger<AssignFlowCommandHandler>>();

        _handler = new AssignFlowCommandHandler(
            _flowRepositoryMock.Object,
            _userRepositoryMock.Object,
            _assignmentRepositoryMock.Object,
            _snapshotServiceMock.Object,
            _unitOfWorkMock.Object,
            _currentUserServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldAssignFlowSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var flowId = Guid.NewGuid();
        var assignmentId = Guid.NewGuid();
        var snapshotId = Guid.NewGuid();
        var createdById = Guid.NewGuid();

        var command = new AssignFlowCommand
        {
            UserId = userId,
            FlowId = flowId,
            CreatedById = createdById,
            Deadline = DateTime.UtcNow.AddDays(30),
            Priority = 5
        };

        var user = new User { Id = userId, IsActive = true };
        var flow = new Flow { Id = flowId, Status = FlowStatus.Published };
        var assignment = new FlowAssignment 
        { 
            Id = assignmentId,
            UserId = userId,
            FlowId = flowId,
            Status = AssignmentStatus.Assigned
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _flowRepositoryMock
            .Setup(x => x.GetByIdAsync(flowId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(flow);

        _assignmentRepositoryMock
            .Setup(x => x.GetByUserAndFlowAsync(userId, flowId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((FlowAssignment?)null);

        _snapshotServiceMock
            .Setup(x => x.CreateSnapshotAsync(flowId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(snapshotId);

        _assignmentRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<FlowAssignment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(assignment);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.AssignmentId.Should().Be(assignmentId);
        result.SnapshotId.Should().Be(snapshotId);
        result.EstimatedCompletionDate.Should().BeCloseTo(DateTime.UtcNow.AddDays(30), TimeSpan.FromMinutes(1));

        _snapshotServiceMock.Verify(x => x.CreateSnapshotAsync(flowId, It.IsAny<CancellationToken>()), Times.Once);
        _assignmentRepositoryMock.Verify(x => x.AddAsync(It.IsAny<FlowAssignment>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_UserNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        var command = new AssignFlowCommand
        {
            UserId = Guid.NewGuid(),
            FlowId = Guid.NewGuid(),
            CreatedById = Guid.NewGuid()
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _handler.Handle(command, CancellationToken.None));
        
        exception.Message.Should().Contain("Пользователь не найден");
    }

    [Fact]
    public async Task Handle_FlowNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        var command = new AssignFlowCommand
        {
            UserId = Guid.NewGuid(),
            FlowId = Guid.NewGuid(),
            CreatedById = Guid.NewGuid()
        };

        var user = new User { Id = command.UserId, IsActive = true };

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _flowRepositoryMock
            .Setup(x => x.GetByIdAsync(command.FlowId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Flow?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _handler.Handle(command, CancellationToken.None));
        
        exception.Message.Should().Contain("Поток не найден");
    }

    [Fact]
    public async Task Handle_FlowNotPublished_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var command = new AssignFlowCommand
        {
            UserId = Guid.NewGuid(),
            FlowId = Guid.NewGuid(),
            CreatedById = Guid.NewGuid()
        };

        var user = new User { Id = command.UserId, IsActive = true };
        var flow = new Flow { Id = command.FlowId, Status = FlowStatus.Draft };

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _flowRepositoryMock
            .Setup(x => x.GetByIdAsync(command.FlowId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(flow);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));
        
        exception.Message.Should().Contain("только опубликованные потоки");
    }

    [Fact]
    public async Task Handle_UserAlreadyAssigned_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var command = new AssignFlowCommand
        {
            UserId = Guid.NewGuid(),
            FlowId = Guid.NewGuid(),
            CreatedById = Guid.NewGuid()
        };

        var user = new User { Id = command.UserId, IsActive = true };
        var flow = new Flow { Id = command.FlowId, Status = FlowStatus.Published };
        var existingAssignment = new FlowAssignment 
        { 
            UserId = command.UserId,
            FlowId = command.FlowId,
            Status = AssignmentStatus.Assigned
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _flowRepositoryMock
            .Setup(x => x.GetByIdAsync(command.FlowId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(flow);

        _assignmentRepositoryMock
            .Setup(x => x.GetByUserAndFlowAsync(command.UserId, command.FlowId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingAssignment);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));
        
        exception.Message.Should().Contain("уже назначен");
    }

    [Fact]
    public async Task Handle_InactiveUser_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var command = new AssignFlowCommand
        {
            UserId = Guid.NewGuid(),
            FlowId = Guid.NewGuid(),
            CreatedById = Guid.NewGuid()
        };

        var user = new User { Id = command.UserId, IsActive = false };

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None));
        
        exception.Message.Should().Contain("неактивному пользователю");
    }
}