using Lauf.Shared.Helpers;
using FluentAssertions;

namespace Lauf.Shared.Tests.Helpers;

public class PasswordHelperTests
{
    [Fact]
    public void HashPassword_ShouldCreateHashedPassword()
    {
        // Arrange
        var password = "TestPassword123!";

        // Act
        var hash = PasswordHelper.HashPassword(password);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        hash.Should().NotBe(password);
        hash.Length.Should().BeGreaterThan(50); // Base64 encoded hash + salt should be substantial
    }

    [Fact]
    public void HashPassword_WithSamePassword_ShouldCreateDifferentHashes()
    {
        // Arrange
        var password = "TestPassword123!";

        // Act
        var hash1 = PasswordHelper.HashPassword(password);
        var hash2 = PasswordHelper.HashPassword(password);

        // Assert
        hash1.Should().NotBe(hash2); // Different salts should produce different hashes
    }

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        var password = "TestPassword123!";
        var hash = PasswordHelper.HashPassword(password);

        // Act
        var result = PasswordHelper.VerifyPassword(password, hash);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_WithIncorrectPassword_ShouldReturnFalse()
    {
        // Arrange
        var password = "TestPassword123!";
        var wrongPassword = "WrongPassword123!";
        var hash = PasswordHelper.HashPassword(password);

        // Act
        var result = PasswordHelper.VerifyPassword(wrongPassword, hash);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(null, "hash")]
    [InlineData("", "hash")]
    [InlineData("password", null)]
    [InlineData("password", "")]
    public void VerifyPassword_WithNullOrEmpty_ShouldReturnFalse(string? password, string? hash)
    {
        // Act
        var result = PasswordHelper.VerifyPassword(password!, hash!);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(8, true, true, true, false)]
    [InlineData(12, true, true, true, true)]
    [InlineData(16, false, true, true, false)]
    public void GeneratePassword_ShouldGenerateCorrectPassword(int length, bool includeUppercase, bool includeLowercase, bool includeNumbers, bool includeSpecialChars)
    {
        // Act
        var password = PasswordHelper.GeneratePassword(length, includeUppercase, includeLowercase, includeNumbers, includeSpecialChars);

        // Assert
        password.Should().NotBeNullOrEmpty();
        password.Length.Should().Be(length);

        if (includeUppercase)
            password.Should().MatchRegex("[A-Z]");
        if (includeLowercase)
            password.Should().MatchRegex("[a-z]");
        if (includeNumbers)
            password.Should().MatchRegex("[0-9]");
        if (includeSpecialChars)
            password.Should().MatchRegex("[^a-zA-Z0-9]");
    }

    [Fact]
    public void GeneratePassword_WithNoCharacterTypes_ShouldThrowException()
    {
        // Act & Assert
        var action = () => PasswordHelper.GeneratePassword(8, false, false, false, false);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("Password123!", 70, 90)] // Strong password
    [InlineData("password", 0, 40)]       // Weak password
    [InlineData("PASSWORD", 0, 40)]       // Weak password
    [InlineData("12345678", 0, 40)]       // Weak password
    [InlineData("Passw0rd!", 40, 80)]     // Medium password
    [InlineData("", 0, 0)]                // Empty password
    public void CheckPasswordStrength_ShouldReturnCorrectRange(string password, int minExpected, int maxExpected)
    {
        // Act
        var strength = PasswordHelper.CheckPasswordStrength(password);

        // Assert
        strength.Should().BeInRange(minExpected, maxExpected);
    }

    [Theory]
    [InlineData("Password123!", "Сильный")]
    [InlineData("password", "Слабый")]
    [InlineData("Passw0rd!", "Сильный")]
    [InlineData("", "Очень слабый")]
    public void GetPasswordStrengthDescription_ShouldReturnCorrectDescription(string password, string expectedCategory)
    {
        // Act
        var description = PasswordHelper.GetPasswordStrengthDescription(password);

        // Assert
        description.Should().Be(expectedCategory);
    }

    [Theory]
    [InlineData("Password123!", 8, true, true, true, false, true)]
    [InlineData("password", 8, false, true, false, false, true)]
    [InlineData("PASSWORD", 8, true, false, false, false, true)]
    [InlineData("12345678", 8, false, false, true, false, true)]
    [InlineData("Pass123!", 8, true, true, true, true, true)]
    [InlineData("short", 8, true, true, true, false, false)]
    public void IsPasswordValid_ShouldReturnCorrectResult(string password, int minLength, bool requireUppercase, bool requireLowercase, bool requireNumbers, bool requireSpecialChars, bool expected)
    {
        // Act
        var result = PasswordHelper.IsPasswordValid(password, minLength, requireUppercase, requireLowercase, requireNumbers, requireSpecialChars);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void GetPasswordValidationErrors_WithValidPassword_ShouldReturnEmptyList()
    {
        // Arrange
        var password = "Password123!";

        // Act
        var errors = PasswordHelper.GetPasswordValidationErrors(password);

        // Assert
        errors.Should().BeEmpty();
    }

    [Fact]
    public void GetPasswordValidationErrors_WithInvalidPassword_ShouldReturnErrors()
    {
        // Arrange
        var password = "weak";

        // Act
        var errors = PasswordHelper.GetPasswordValidationErrors(password, 8, true, true, true, true);

        // Assert
        errors.Should().NotBeEmpty();
        errors.Should().Contain(e => e.Contains("минимум 8 символов"));
        errors.Should().Contain(e => e.Contains("заглавные буквы"));
        errors.Should().Contain(e => e.Contains("цифры"));
        errors.Should().Contain(e => e.Contains("специальные символы"));
    }

    [Fact]
    public void GetPasswordValidationErrors_WithEmptyPassword_ShouldReturnSingleError()
    {
        // Arrange
        var password = "";

        // Act
        var errors = PasswordHelper.GetPasswordValidationErrors(password);

        // Assert
        errors.Should().HaveCount(1);
        errors[0].Should().Contain("не может быть пустым");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void HashPassword_WithNullOrEmpty_ShouldThrowException(string? password)
    {
        // Act & Assert
        var action = () => PasswordHelper.HashPassword(password!);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void GeneratePassword_WithZeroLength_ShouldThrowException()
    {
        // Act & Assert
        var action = () => PasswordHelper.GeneratePassword(0);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void VerifyPassword_WithInvalidHash_ShouldReturnFalse()
    {
        // Arrange
        var password = "TestPassword123!";
        var invalidHash = "invalid_base64_hash";

        // Act
        var result = PasswordHelper.VerifyPassword(password, invalidHash);

        // Assert
        result.Should().BeFalse();
    }
}