using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Lauf.Application.Commands.FlowManagement;
using Lauf.Application.Services.Interfaces;
using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Interfaces;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Domain.Enums;
using Xunit;

namespace Lauf.Application.Tests.Commands.FlowManagement;

/// <summary>
/// Тесты для CreateFlowCommandHandler
/// </summary>
public class CreateFlowCommandHandlerTests
{
    private readonly Mock<IFlowRepository> _flowRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<CreateFlowCommandHandler>> _loggerMock;
    private readonly CreateFlowCommandHandler _handler;

    public CreateFlowCommandHandlerTests()
    {
        _flowRepositoryMock = new Mock<IFlowRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<CreateFlowCommandHandler>>();

        _handler = new CreateFlowCommandHandler(
            _flowRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserServiceMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateFlowSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateFlowCommand
        {
            Title = "Тестовый поток",
            Description = "Описание тестового потока",
            Category = "Обучение",
            Priority = 5,
            IsRequired = true,
            CreatedById = userId,
            Settings = new CreateFlowSettingsCommand
            {
                RequireSequentialCompletion = true,
                AllowRetry = false,
                ShowProgress = true
            }
        };

        var createdFlow = new Flow
        {
            Id = Guid.NewGuid(),
            Title = command.Title,
            Description = command.Description,
            Status = FlowStatus.Draft,
            CreatedAt = DateTime.UtcNow
        };

        _currentUserServiceMock
            .Setup(x => x.GetCurrentUserId())
            .Returns(userId);

        _flowRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Flow>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdFlow);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.FlowId.Should().Be(createdFlow.Id);
        result.Title.Should().Be(command.Title);
        result.Status.Should().Be("Draft");

        _flowRepositoryMock.Verify(x => x.AddAsync(It.Is<Flow>(f => 
            f.Title == command.Title && 
            f.Description == command.Description), 
            It.IsAny<CancellationToken>()), Times.Once);
        
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCurrentUserIsNull_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var command = new CreateFlowCommand
        {
            Title = "Тестовый поток",
            Description = "Описание"
        };

        _currentUserServiceMock
            .Setup(x => x.GetCurrentUserId())
            .Returns((Guid?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenSaveFails_ShouldReturnFailureResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateFlowCommand
        {
            Title = "Тестовый поток",
            Description = "Описание",
            CreatedById = userId
        };

        _currentUserServiceMock
            .Setup(x => x.GetCurrentUserId())
            .Returns(userId);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0); // Симулируем неудачное сохранение

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("Не удалось создать поток");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task Handle_WithInvalidTitle_ShouldThrowArgumentException(string invalidTitle)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateFlowCommand
        {
            Title = invalidTitle,
            Description = "Описание",
            CreatedById = userId
        };

        _currentUserServiceMock
            .Setup(x => x.GetCurrentUserId())
            .Returns(userId);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _handler.Handle(command, CancellationToken.None));
    }
}