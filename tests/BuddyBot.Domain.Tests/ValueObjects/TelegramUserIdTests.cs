using BuddyBot.Domain.ValueObjects;
using FluentAssertions;

namespace BuddyBot.Domain.Tests.ValueObjects;

public class TelegramUserIdTests
{
    [Fact]
    public void Constructor_WithValidValue_ShouldCreateInstance()
    {
        // Arrange
        var value = 123456789L;

        // Act
        var telegramUserId = new TelegramUserId(value);

        // Assert
        telegramUserId.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-123456)]
    public void Constructor_WithInvalidValue_ShouldThrowArgumentException(long invalidValue)
    {
        // Act & Assert
        var act = () => new TelegramUserId(invalidValue);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Telegram User ID должен быть положительным числом*");
    }

    [Fact]
    public void FromString_WithValidString_ShouldCreateInstance()
    {
        // Arrange
        var stringValue = "123456789";

        // Act
        var telegramUserId = TelegramUserId.FromString(stringValue);

        // Assert
        telegramUserId.Value.Should().Be(123456789L);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void FromString_WithInvalidString_ShouldThrowArgumentException(string? invalidValue)
    {
        // Act & Assert
        var act = () => TelegramUserId.FromString(invalidValue!);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Значение не может быть пустым*");
    }

    [Fact]
    public void FromString_WithNonNumericString_ShouldThrowArgumentException()
    {
        // Act & Assert
        var act = () => TelegramUserId.FromString("abc123");
        act.Should().Throw<ArgumentException>()
            .WithMessage("Значение должно быть числом*");
    }

    [Fact]
    public void FromLong_ShouldCreateInstance()
    {
        // Arrange
        var value = 987654321L;

        // Act
        var telegramUserId = TelegramUserId.FromLong(value);

        // Assert
        telegramUserId.Value.Should().Be(value);
    }

    [Fact]
    public void FromInt_ShouldCreateInstance()
    {
        // Arrange
        var value = 12345;

        // Act
        var telegramUserId = TelegramUserId.FromInt(value);

        // Assert
        telegramUserId.Value.Should().Be(value);
    }

    [Fact]
    public void IsValidUserRange_WithValidRange_ShouldReturnTrue()
    {
        // Arrange
        var telegramUserId = new TelegramUserId(123456789L);

        // Act
        var result = telegramUserId.IsValidUserRange();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValidUserRange_WithInvalidRange_ShouldReturnFalse()
    {
        // Arrange
        var telegramUserId = new TelegramUserId(15_000_000_000L); // Too large

        // Act
        var result = telegramUserId.IsValidUserRange();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ToString_ShouldReturnStringRepresentation()
    {
        // Arrange
        var value = 123456789L;
        var telegramUserId = new TelegramUserId(value);

        // Act
        var result = telegramUserId.ToString();

        // Assert
        result.Should().Be("123456789");
    }

    [Fact]
    public void Equals_WithSameValue_ShouldReturnTrue()
    {
        // Arrange
        var value = 123456789L;
        var telegramUserId1 = new TelegramUserId(value);
        var telegramUserId2 = new TelegramUserId(value);

        // Act & Assert
        telegramUserId1.Equals(telegramUserId2).Should().BeTrue();
        (telegramUserId1 == telegramUserId2).Should().BeTrue();
        (telegramUserId1 != telegramUserId2).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithDifferentValue_ShouldReturnFalse()
    {
        // Arrange
        var telegramUserId1 = new TelegramUserId(123456789L);
        var telegramUserId2 = new TelegramUserId(987654321L);

        // Act & Assert
        telegramUserId1.Equals(telegramUserId2).Should().BeFalse();
        (telegramUserId1 == telegramUserId2).Should().BeFalse();
        (telegramUserId1 != telegramUserId2).Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_WithSameValue_ShouldReturnSameHashCode()
    {
        // Arrange
        var value = 123456789L;
        var telegramUserId1 = new TelegramUserId(value);
        var telegramUserId2 = new TelegramUserId(value);

        // Act & Assert
        telegramUserId1.GetHashCode().Should().Be(telegramUserId2.GetHashCode());
    }

    [Fact]
    public void ImplicitConversion_ToLong_ShouldWork()
    {
        // Arrange
        var telegramUserId = new TelegramUserId(123456789L);

        // Act
        long value = telegramUserId;

        // Assert
        value.Should().Be(123456789L);
    }

    [Fact]
    public void ExplicitConversion_FromLong_ShouldWork()
    {
        // Arrange
        var value = 123456789L;

        // Act
        var telegramUserId = (TelegramUserId)value;

        // Assert
        telegramUserId.Value.Should().Be(value);
    }

    [Fact]
    public void ImplicitConversion_ToString_ShouldWork()
    {
        // Arrange
        var telegramUserId = new TelegramUserId(123456789L);

        // Act
        string value = telegramUserId;

        // Assert
        value.Should().Be("123456789");
    }
}