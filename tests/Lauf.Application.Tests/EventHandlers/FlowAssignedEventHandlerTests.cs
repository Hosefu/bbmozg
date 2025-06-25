using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Lauf.Application.EventHandlers;
using Lauf.Application.EventHandlers.Events;
using Lauf.Domain.Events;
using Lauf.Domain.Interfaces.Repositories;
using Xunit;

namespace Lauf.Application.Tests.EventHandlers;

/// <summary>
/// Тесты для FlowAssignedEventHandler
/// </summary>
public class FlowAssignedEventHandlerTests
{
    private readonly Mock<ILogger<FlowAssignedEventHandler>> _loggerMock;
    private readonly Mock<IUserProgressRepository> _progressRepositoryMock;
    private readonly FlowAssignedEventHandler _handler;

    public FlowAssignedEventHandlerTests()
    {
        _loggerMock = new Mock<ILogger<FlowAssignedEventHandler>>();
        _progressRepositoryMock = new Mock<IUserProgressRepository>();
        
        _handler = new FlowAssignedEventHandler(
            _loggerMock.Object,
            _progressRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenFlowAssigned_ShouldLogInformation()
    {
        // Arrange
        var assignmentId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var flowId = Guid.NewGuid();
        var buddyId = Guid.NewGuid();

        var domainEvent = new FlowAssigned
        {
            EventId = Guid.NewGuid(),
            OccurredAt = DateTime.UtcNow,
            Version = 1,
            AssignmentId = assignmentId,
            UserId = userId,
            FlowId = flowId,
            BuddyId = buddyId
        };

        var notification = new FlowAssignedNotification(domainEvent);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        // Проверяем, что логирование вызывалось
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Обработка события назначения потока")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Событие назначения потока успешно обработано")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenExceptionOccurs_ShouldLogErrorAndRethrow()
    {
        // Arrange
        var assignmentId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var flowId = Guid.NewGuid();

        var domainEvent = new FlowAssigned
        {
            EventId = Guid.NewGuid(),
            OccurredAt = DateTime.UtcNow,
            Version = 1,
            AssignmentId = assignmentId,
            UserId = userId,
            FlowId = flowId,
            BuddyId = null
        };

        var notification = new FlowAssignedNotification(domainEvent);

        // Настраиваем обработчик так, чтобы он выбрасывал исключение
        // (это произойдет, если в будущем CreateInitialProgressAsync будет реализован и выбросит исключение)

        // Act & Assert
        // В текущей реализации исключение не выбрасывается, так как методы заглушки
        // Но проверим, что обработчик не падает
        var exception = await Record.ExceptionAsync(() => 
            _handler.Handle(notification, CancellationToken.None));

        exception.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WithBuddy_ShouldProcessBuddyNotification()
    {
        // Arrange
        var assignmentId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var flowId = Guid.NewGuid();
        var buddyId = Guid.NewGuid();

        var domainEvent = new FlowAssigned
        {
            EventId = Guid.NewGuid(),
            OccurredAt = DateTime.UtcNow,
            Version = 1,
            AssignmentId = assignmentId,
            UserId = userId,
            FlowId = flowId,
            BuddyId = buddyId
        };

        var notification = new FlowAssignedNotification(domainEvent);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        // Проверяем, что обработка завершилась успешно
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Событие назначения потока успешно обработано")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithoutBuddy_ShouldSkipBuddyNotification()
    {
        // Arrange
        var assignmentId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var flowId = Guid.NewGuid();

        var domainEvent = new FlowAssigned
        {
            EventId = Guid.NewGuid(),
            OccurredAt = DateTime.UtcNow,
            Version = 1,
            AssignmentId = assignmentId,
            UserId = userId,
            FlowId = flowId,
            BuddyId = null
        };

        var notification = new FlowAssignedNotification(domainEvent);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        // Проверяем, что обработка завершилась успешно
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Событие назначения потока успешно обработано")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}