using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Enums;
using Lauf.Infrastructure.Persistence;
using Lauf.Infrastructure.Persistence.Repositories;
using Xunit;

namespace Lauf.Infrastructure.Tests.Repositories;

/// <summary>
/// Интеграционные тесты для FlowRepository
/// </summary>
public class FlowRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly FlowRepository _repository;
    private readonly Mock<ILogger<FlowRepository>> _loggerMock;

    public FlowRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _loggerMock = new Mock<ILogger<FlowRepository>>();
        _repository = new FlowRepository(_context, _loggerMock.Object);
    }

    [Fact]
    public async Task AddAsync_ValidFlow_ShouldAddFlowToDatabase()
    {
        // Arrange
        var flow = new Flow
        {
            Id = Guid.NewGuid(),
            Title = "Тестовый поток",
            Description = "Описание тестового потока",
            Status = FlowStatus.Draft,
            Version = 1,
            Priority = 5,
            IsRequired = false,
            CreatedById = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _repository.AddAsync(flow);
        await _context.SaveChangesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(flow.Id);

        var savedFlow = await _context.Flows.FindAsync(flow.Id);
        savedFlow.Should().NotBeNull();
        savedFlow!.Title.Should().Be(flow.Title);
        savedFlow.Status.Should().Be(FlowStatus.Draft);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingFlow_ShouldReturnFlow()
    {
        // Arrange
        var flow = new Flow
        {
            Id = Guid.NewGuid(),
            Title = "Тестовый поток",
            Description = "Описание",
            Status = FlowStatus.Published,
            Version = 1,
            CreatedById = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Flows.Add(flow);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(flow.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(flow.Id);
        result.Title.Should().Be(flow.Title);
        result.Status.Should().Be(FlowStatus.Published);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingFlow_ShouldReturnNull()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistingId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByStatusAsync_WithPublishedStatus_ShouldReturnOnlyPublishedFlows()
    {
        // Arrange
        var publishedFlow1 = new Flow
        {
            Id = Guid.NewGuid(),
            Title = "Опубликованный поток 1",
            Description = "Описание",
            Status = FlowStatus.Published,
            Version = 1,
            CreatedById = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var publishedFlow2 = new Flow
        {
            Id = Guid.NewGuid(),
            Title = "Опубликованный поток 2",
            Description = "Описание",
            Status = FlowStatus.Published,
            Version = 1,
            CreatedById = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var draftFlow = new Flow
        {
            Id = Guid.NewGuid(),
            Title = "Черновик потока",
            Description = "Описание",
            Status = FlowStatus.Draft,
            Version = 1,
            CreatedById = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Flows.AddRange(publishedFlow1, publishedFlow2, draftFlow);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByStatusAsync(FlowStatus.Published);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(f => f.Id == publishedFlow1.Id);
        result.Should().Contain(f => f.Id == publishedFlow2.Id);
        result.Should().NotContain(f => f.Id == draftFlow.Id);
    }

    [Fact]
    public async Task UpdateAsync_ExistingFlow_ShouldUpdateFlow()
    {
        // Arrange
        var flow = new Flow
        {
            Id = Guid.NewGuid(),
            Title = "Исходное название",
            Description = "Исходное описание",
            Status = FlowStatus.Draft,
            Version = 1,
            CreatedById = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Flows.Add(flow);
        await _context.SaveChangesAsync();

        // Act
        flow.Title = "Обновленное название";
        flow.Description = "Обновленное описание";
        flow.Status = FlowStatus.Published;
        flow.UpdatedAt = DateTime.UtcNow;

        var result = await _repository.UpdateAsync(flow);
        await _context.SaveChangesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Обновленное название");
        result.Description.Should().Be("Обновленное описание");
        result.Status.Should().Be(FlowStatus.Published);

        var updatedFlow = await _context.Flows.FindAsync(flow.Id);
        updatedFlow!.Title.Should().Be("Обновленное название");
        updatedFlow.Status.Should().Be(FlowStatus.Published);
    }

    [Fact]
    public async Task DeleteAsync_ExistingFlow_ShouldRemoveFlow()
    {
        // Arrange
        var flow = new Flow
        {
            Id = Guid.NewGuid(),
            Title = "Поток для удаления",
            Description = "Описание",
            Status = FlowStatus.Draft,
            Version = 1,
            CreatedById = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Flows.Add(flow);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(flow.Id);
        await _context.SaveChangesAsync();

        // Assert
        var deletedFlow = await _context.Flows.FindAsync(flow.Id);
        deletedFlow.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdWithStepsAsync_FlowWithSteps_ShouldReturnFlowWithSteps()
    {
        // Arrange
        var flow = new Flow
        {
            Id = Guid.NewGuid(),
            Title = "Поток с шагами",
            Description = "Описание",
            Status = FlowStatus.Published,
            Version = 1,
            CreatedById = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var step1 = new FlowStep
        {
            Id = Guid.NewGuid(),
            FlowId = flow.Id,
            Title = "Шаг 1",
            Description = "Описание шага 1",
            Order = 1,
            IsRequired = true,
            Status = StepStatus.Active,
            EstimatedDurationMinutes = 30,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var step2 = new FlowStep
        {
            Id = Guid.NewGuid(),
            FlowId = flow.Id,
            Title = "Шаг 2",
            Description = "Описание шага 2",
            Order = 2,
            IsRequired = false,
            Status = StepStatus.Active,
            EstimatedDurationMinutes = 45,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        flow.Steps = new List<FlowStep> { step1, step2 };

        _context.Flows.Add(flow);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdWithStepsAsync(flow.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(flow.Id);
        result.Steps.Should().HaveCount(2);
        result.Steps.Should().Contain(s => s.Id == step1.Id);
        result.Steps.Should().Contain(s => s.Id == step2.Id);
    }

    [Fact]
    public async Task SearchByTitleAsync_WithMatchingTitle_ShouldReturnMatchingFlows()
    {
        // Arrange
        var matchingFlow1 = new Flow
        {
            Id = Guid.NewGuid(),
            Title = "JavaScript основы",
            Description = "Описание",
            Status = FlowStatus.Published,
            Version = 1,
            CreatedById = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var matchingFlow2 = new Flow
        {
            Id = Guid.NewGuid(),
            Title = "Продвинутый JavaScript",
            Description = "Описание",
            Status = FlowStatus.Published,
            Version = 1,
            CreatedById = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var nonMatchingFlow = new Flow
        {
            Id = Guid.NewGuid(),
            Title = "Python основы",
            Description = "Описание",
            Status = FlowStatus.Published,
            Version = 1,
            CreatedById = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Flows.AddRange(matchingFlow1, matchingFlow2, nonMatchingFlow);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.SearchByTitleAsync("JavaScript");

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(f => f.Id == matchingFlow1.Id);
        result.Should().Contain(f => f.Id == matchingFlow2.Id);
        result.Should().NotContain(f => f.Id == nonMatchingFlow.Id);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}