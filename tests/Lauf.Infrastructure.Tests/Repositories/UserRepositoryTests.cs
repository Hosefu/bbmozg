using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Lauf.Domain.Entities.Users;
using Lauf.Domain.ValueObjects;
using Lauf.Infrastructure.Persistence;
using Lauf.Infrastructure.Persistence.Repositories;
using Lauf.Infrastructure.Persistence.Interceptors;
using Lauf.Application.Services.Interfaces;
using Moq;
using Xunit;

namespace Lauf.Infrastructure.Tests.Repositories;

/// <summary>
/// Тесты для UserRepository
/// </summary>
public class UserRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var mockCurrentUserService = new Mock<ICurrentUserService>();
        mockCurrentUserService.Setup(x => x.GetUserId()).Returns(Guid.NewGuid());

        var auditInterceptor = new AuditInterceptor(mockCurrentUserService.Object);
        
        // Для in-memory тестов DomainEventInterceptor не нужен
        _context = new ApplicationDbContext(options, auditInterceptor, null!);
        _repository = new UserRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserExists_ShouldReturnUser()
    {
        // Arrange
        var user = CreateTestUser();
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(user.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.Email.Should().Be(user.Email);
        result.FirstName.Should().Be(user.FirstName);
        result.LastName.Should().Be(user.LastName);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByEmailAsync_WhenUserExists_ShouldReturnUser()
    {
        // Arrange
        var user = CreateTestUser();
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync(user.Email);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task GetByTelegramIdAsync_WhenUserExists_ShouldReturnUser()
    {
        // Arrange
        var user = CreateTestUser();
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByTelegramIdAsync(user.TelegramUserId);

        // Assert
        result.Should().NotBeNull();
        result!.TelegramUserId.Should().Be(user.TelegramUserId);
    }

    [Fact]
    public async Task AddAsync_ShouldAddUserToDatabase()
    {
        // Arrange
        var user = CreateTestUser();

        // Act
        var result = await _repository.AddAsync(user);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(user.Id);

        var userInDb = await _context.Users.FindAsync(user.Id);
        userInDb.Should().NotBeNull();
        userInDb!.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task ExistsByEmailAsync_WhenUserExists_ShouldReturnTrue()
    {
        // Arrange
        var user = CreateTestUser();
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsByEmailAsync(user.Email);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByEmailAsync_WhenUserDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentEmail = "nonexistent@example.com";

        // Act
        var result = await _repository.ExistsByEmailAsync(nonExistentEmail);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetActiveUsersAsync_ShouldReturnOnlyActiveUsers()
    {
        // Arrange
        var activeUser = CreateTestUser();
        var inactiveUser = CreateTestUser();
        inactiveUser.Email = "inactive@example.com";
        inactiveUser.TelegramUserId = new TelegramUserId(987654321);
        inactiveUser.IsActive = false;

        await _context.Users.AddRangeAsync(activeUser, inactiveUser);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetActiveUsersAsync();

        // Assert
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(activeUser.Id);
        result.First().IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUserInDatabase()
    {
        // Arrange
        var user = CreateTestUser();
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Изменяем пользователя
        user.FirstName = "Обновленное Имя";
        user.Department = "Новый Отдел";

        // Act
        var result = await _repository.UpdateAsync(user);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be("Обновленное Имя");
        result.Department.Should().Be("Новый Отдел");

        var userInDb = await _context.Users.FindAsync(user.Id);
        userInDb!.FirstName.Should().Be("Обновленное Имя");
        userInDb.Department.Should().Be("Новый Отдел");
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnMatchingUsers()
    {
        // Arrange
        var user1 = CreateTestUser();
        user1.FirstName = "Иван";
        user1.Email = "ivan@example.com";

        var user2 = CreateTestUser();
        user2.Email = "petr@example.com";
        user2.FirstName = "Петр";
        user2.TelegramUserId = new TelegramUserId(987654321);

        await _context.Users.AddRangeAsync(user1, user2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.SearchAsync("иван", 0, 10);

        // Assert
        result.Should().HaveCount(1);
        result.First().FirstName.Should().Be("Иван");
    }

    private static User CreateTestUser()
    {
        return new User
        {
            Id = Guid.NewGuid(),
            FirstName = "Тест",
            LastName = "Пользователь",
            Email = "test@example.com",
            Position = "Разработчик",
            Department = "IT",
            TelegramUserId = new TelegramUserId(123456789),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}