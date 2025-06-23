using Lauf.Shared.Constants;
using FluentAssertions;

namespace Lauf.Shared.Tests.Constants;

public class CacheKeysTests
{
    [Fact]
    public void Prefix_ShouldHaveCorrectValue()
    {
        // Act & Assert
        CacheKeys.Prefix.Should().Be("Lauf:");
    }

    [Fact]
    public void Users_ById_ShouldGenerateCorrectKey()
    {
        // Arrange
        var userId = 123;

        // Act
        var result = CacheKeys.Users.ById(userId);

        // Assert
        result.Should().Be("Lauf:User:123");
    }

    [Fact]
    public void Users_ByTelegramId_ShouldGenerateCorrectKey()
    {
        // Arrange
        var telegramUserId = 456789L;

        // Act
        var result = CacheKeys.Users.ByTelegramId(telegramUserId);

        // Assert
        result.Should().Be("Lauf:User:Telegram:456789");
    }

    [Fact]
    public void Users_Roles_ShouldGenerateCorrectKey()
    {
        // Arrange
        var userId = 123;

        // Act
        var result = CacheKeys.Users.Roles(userId);

        // Assert
        result.Should().Be("Lauf:User:123:Roles");
    }

    [Fact]
    public void Users_All_ShouldHaveCorrectValue()
    {
        // Act & Assert
        CacheKeys.Users.All.Should().Be("Lauf:Users:All");
    }

    [Fact]
    public void Flows_ById_ShouldGenerateCorrectKey()
    {
        // Arrange
        var flowId = 456;

        // Act
        var result = CacheKeys.Flows.ById(flowId);

        // Assert
        result.Should().Be("Lauf:Flow:456");
    }

    [Fact]
    public void Flows_AvailableForUser_ShouldGenerateCorrectKey()
    {
        // Arrange
        var userId = 789;

        // Act
        var result = CacheKeys.Flows.AvailableForUser(userId);

        // Assert
        result.Should().Be("Lauf:Flows:Available:789");
    }

    [Fact]
    public void Assignments_ById_ShouldGenerateCorrectKey()
    {
        // Arrange
        var assignmentId = 111;

        // Act
        var result = CacheKeys.Assignments.ById(assignmentId);

        // Assert
        result.Should().Be("Lauf:Assignment:111");
    }

    [Fact]
    public void Progress_ByAssignment_ShouldGenerateCorrectKey()
    {
        // Arrange
        var assignmentId = 222;

        // Act
        var result = CacheKeys.Progress.ByAssignment(assignmentId);

        // Assert
        result.Should().Be("Lauf:Progress:Assignment:222");
    }

    [Fact]
    public void Notifications_UnreadCount_ShouldGenerateCorrectKey()
    {
        // Arrange
        var userId = 333;

        // Act
        var result = CacheKeys.Notifications.UnreadCount(userId);

        // Assert
        result.Should().Be("Lauf:Notifications:User:333:UnreadCount");
    }

    [Fact]
    public void RateLimit_ForUser_ShouldGenerateCorrectKey()
    {
        // Arrange
        var userId = 444;

        // Act
        var result = CacheKeys.RateLimit.ForUser(userId);

        // Assert
        result.Should().Be("Lauf:RateLimit:User:444");
    }

    [Fact]
    public void RateLimit_ForIp_ShouldGenerateCorrectKey()
    {
        // Arrange
        var ipAddress = "192.168.1.1";

        // Act
        var result = CacheKeys.RateLimit.ForIp(ipAddress);

        // Assert
        result.Should().Be("Lauf:RateLimit:IP:192.168.1.1");
    }

    [Fact]
    public void TTL_Constants_ShouldHaveCorrectValues()
    {
        // Act & Assert
        CacheKeys.TTL.Short.Should().Be(300);    // 5 минут
        CacheKeys.TTL.Medium.Should().Be(1800);  // 30 минут
        CacheKeys.TTL.Long.Should().Be(7200);    // 2 часа
        CacheKeys.TTL.Day.Should().Be(86400);    // 24 часа
        CacheKeys.TTL.Week.Should().Be(604800);  // 7 дней
    }

    [Fact]
    public void System_Constants_ShouldHaveCorrectValues()
    {
        // Act & Assert
        CacheKeys.System.Settings.Should().Be("Lauf:System:Settings");
        CacheKeys.System.WorkingHours.Should().Be("Lauf:System:WorkingHours");
        CacheKeys.System.Holidays.Should().Be("Lauf:System:Holidays");
    }

    [Fact]
    public void Achievements_Constants_ShouldHaveCorrectValues()
    {
        // Act & Assert
        CacheKeys.Achievements.All.Should().Be("Lauf:Achievements:All");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(999)]
    [InlineData(int.MaxValue)]
    public void CacheKeys_WithNumericIds_ShouldHandleDifferentValues(int id)
    {
        // Act
        var userKey = CacheKeys.Users.ById(id);
        var flowKey = CacheKeys.Flows.ById(id);
        var assignmentKey = CacheKeys.Assignments.ById(id);

        // Assert
        userKey.Should().Be($"Lauf:User:{id}");
        flowKey.Should().Be($"Lauf:Flow:{id}");
        assignmentKey.Should().Be($"Lauf:Assignment:{id}");
    }

    [Fact]
    public void AllCacheKeys_ShouldStartWithPrefix()
    {
        // Arrange
        var keys = new[]
        {
            CacheKeys.Users.ById(1),
            CacheKeys.Flows.ById(1),
            CacheKeys.Assignments.ById(1),
            CacheKeys.Progress.ByUser(1),
            CacheKeys.Notifications.ByUser(1),
            CacheKeys.System.Settings,
            CacheKeys.Achievements.All
        };

        // Act & Assert
        keys.Should().AllSatisfy(key => key.Should().StartWith(CacheKeys.Prefix));
    }
}