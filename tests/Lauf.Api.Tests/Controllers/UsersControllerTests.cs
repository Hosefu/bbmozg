using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Lauf.Api.Tests.Infrastructure;
using Lauf.Application.DTOs.Users;
using Lauf.Domain.Entities.Users;
using Lauf.Domain.ValueObjects;
using Xunit;

namespace Lauf.Api.Tests.Controllers;

/// <summary>
/// Интеграционные тесты для UsersController
/// </summary>
public class UsersControllerTests : ApiTestBase
{
    public UsersControllerTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetUsers_WithNoUsers_ShouldReturnEmptyList()
    {
        // Arrange
        await ClearDatabase();

        // Act
        var response = await Client.GetAsync("/api/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var users = await response.Content.ReadFromJsonAsync<IEnumerable<UserDto>>();
        users.Should().NotBeNull();
        users.Should().BeEmpty();
    }

    [Fact]
    public async Task GetUsers_WithExistingUsers_ShouldReturnUsersList()
    {
        // Arrange
        await ClearDatabase();
        
        var user1 = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "Иван",
            LastName = "Иванов",
            Email = "ivan@example.com",
            TelegramUserId = new TelegramUserId(123456789),
            IsActive = true,
            Language = "ru",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var user2 = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "Петр",
            LastName = "Петров",
            Email = "petr@example.com",
            TelegramUserId = new TelegramUserId(987654321),
            IsActive = true,
            Language = "ru",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        Context.Users.AddRange(user1, user2);
        await Context.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync("/api/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var users = await response.Content.ReadFromJsonAsync<IEnumerable<UserDto>>();
        users.Should().NotBeNull();
        users.Should().HaveCount(2);
        users.Should().Contain(u => u.FirstName == "Иван" && u.LastName == "Иванов");
        users.Should().Contain(u => u.FirstName == "Петр" && u.LastName == "Петров");
    }

    [Fact]
    public async Task GetUser_WithExistingId_ShouldReturnUser()
    {
        // Arrange
        await ClearDatabase();
        
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "Анна",
            LastName = "Смирнова",
            Email = "anna@example.com",
            Position = "Разработчик",
            TelegramUserId = new TelegramUserId(555666777),
            IsActive = true,
            Language = "ru",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync($"/api/users/{user.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var returnedUser = await response.Content.ReadFromJsonAsync<UserDto>();
        returnedUser.Should().NotBeNull();
        returnedUser!.Id.Should().Be(user.Id);
        returnedUser.FirstName.Should().Be("Анна");
        returnedUser.LastName.Should().Be("Смирнова");
        returnedUser.Email.Should().Be("anna@example.com");
        returnedUser.Position.Should().Be("Разработчик");
    }

    [Fact]
    public async Task GetUser_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        await ClearDatabase();
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/users/{nonExistingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetUsers_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        await ClearDatabase();
        
        // Создаем 15 пользователей
        var users = new List<User>();
        for (int i = 1; i <= 15; i++)
        {
            users.Add(new User
            {
                Id = Guid.NewGuid(),
                FirstName = $"User{i:D2}",
                LastName = "Test",
                Email = $"user{i}@example.com",
                TelegramUserId = new TelegramUserId(1000000 + i),
                IsActive = true,
                Language = "ru",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        Context.Users.AddRange(users);
        await Context.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync("/api/users?skip=5&take=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var returnedUsers = await response.Content.ReadFromJsonAsync<IEnumerable<UserDto>>();
        returnedUsers.Should().NotBeNull();
        returnedUsers.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetUsers_WithInvalidPaginationParameters_ShouldHandleGracefully()
    {
        // Arrange
        await ClearDatabase();

        // Act
        var response = await Client.GetAsync("/api/users?skip=-1&take=0");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var users = await response.Content.ReadFromJsonAsync<IEnumerable<UserDto>>();
        users.Should().NotBeNull();
        users.Should().BeEmpty();
    }

    [Theory]
    [InlineData("invalid-guid")]
    [InlineData("")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public async Task GetUser_WithInvalidId_ShouldReturnBadRequest(string invalidId)
    {
        // Act
        var response = await Client.GetAsync($"/api/users/{invalidId}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }
}