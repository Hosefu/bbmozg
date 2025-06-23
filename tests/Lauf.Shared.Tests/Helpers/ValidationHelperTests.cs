using System.ComponentModel.DataAnnotations;
using Lauf.Shared.Helpers;
using FluentAssertions;

namespace Lauf.Shared.Tests.Helpers;

public class ValidationHelperTests
{
    [Theory]
    [InlineData("test@example.com", true)]
    [InlineData("user.name@domain.co.uk", true)]
    [InlineData("invalid.email", false)]
    [InlineData("@example.com", false)]
    [InlineData("test@", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsValidEmail_ShouldReturnCorrectResult(string? email, bool expected)
    {
        // Act
        var result = ValidationHelper.IsValidEmail(email);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("+79161234567", true)]
    [InlineData("89161234567", true)]
    [InlineData("79161234567", true)]
    [InlineData("+7 916 123-45-67", true)]
    [InlineData("8(916)123-45-67", true)]
    [InlineData("123", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsValidPhone_ShouldReturnCorrectResult(string? phone, bool expected)
    {
        // Act
        var result = ValidationHelper.IsValidPhone(phone);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("https://example.com", true)]
    [InlineData("http://www.example.com", true)]
    [InlineData("https://sub.domain.com/path", true)]
    [InlineData("invalid-url", false)]
    [InlineData("ftp://example.com", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsValidUrl_ShouldReturnCorrectResult(string? url, bool expected)
    {
        // Act
        var result = ValidationHelper.IsValidUrl(url);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("89161234567", "+79161234567")]
    [InlineData("79161234567", "+79161234567")]
    [InlineData("+79161234567", "+79161234567")]
    [InlineData("8 (916) 123-45-67", "+79161234567")]
    [InlineData("invalid", null)]
    [InlineData("", null)]
    [InlineData(null, null)]
    public void NormalizePhone_ShouldReturnCorrectResult(string? phone, string? expected)
    {
        // Act
        var result = ValidationHelper.NormalizePhone(phone);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("user123", 3, 50, true)]
    [InlineData("user-name", 3, 50, true)]
    [InlineData("user_name", 3, 50, true)]
    [InlineData("ab", 3, 50, false)] // Too short
    [InlineData("user@name", 3, 50, false)] // Invalid character
    [InlineData("", 3, 50, false)]
    [InlineData(null, 3, 50, false)]
    public void IsValidUsername_ShouldReturnCorrectResult(string? username, int minLength, int maxLength, bool expected)
    {
        // Act
        var result = ValidationHelper.IsValidUsername(username, minLength, maxLength);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(5.5, 1.0, 10.0, true, true)]
    [InlineData(1.0, 1.0, 10.0, true, true)]
    [InlineData(10.0, 1.0, 10.0, true, true)]
    [InlineData(0.5, 1.0, 10.0, true, false)]
    [InlineData(10.5, 1.0, 10.0, true, false)]
    [InlineData(1.0, 1.0, 10.0, false, false)]
    [InlineData(10.0, 1.0, 10.0, false, false)]
    public void IsInRange_ShouldReturnCorrectResult(decimal value, decimal min, decimal max, bool inclusive, bool expected)
    {
        // Act
        var result = ValidationHelper.IsInRange(value, min, max, inclusive);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("123e4567-e89b-12d3-a456-426614174000", true)]
    [InlineData("550e8400-e29b-41d4-a716-446655440000", true)]
    [InlineData("invalid-guid", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsValidGuid_ShouldReturnCorrectResult(string? guid, bool expected)
    {
        // Act
        var result = ValidationHelper.IsValidGuid(guid);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void IsValidDateRange_WithValidRange_ShouldReturnTrue()
    {
        // Arrange
        var date = new DateTime(2024, 6, 15);
        var minDate = new DateTime(2024, 1, 1);
        var maxDate = new DateTime(2024, 12, 31);

        // Act
        var result = ValidationHelper.IsValidDateRange(date, minDate, maxDate);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValidDateRange_OutOfRange_ShouldReturnFalse()
    {
        // Arrange
        var date = new DateTime(2025, 6, 15);
        var minDate = new DateTime(2024, 1, 1);
        var maxDate = new DateTime(2024, 12, 31);

        // Act
        var result = ValidationHelper.IsValidDateRange(date, minDate, maxDate);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("1990-01-01", 18, 65, true)]
    [InlineData("2010-01-01", 18, 65, false)] // Too young
    [InlineData("1950-01-01", 18, 65, false)] // Too old
    public void IsValidAge_ShouldReturnCorrectResult(string birthDateString, int minAge, int maxAge, bool expected)
    {
        // Arrange
        var birthDate = DateTime.Parse(birthDateString);

        // Act
        var result = ValidationHelper.IsValidAge(birthDate, minAge, maxAge);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("hello world", new[] { '@', '#' }, true)]
    [InlineData("hello@world", new[] { '@', '#' }, false)]
    [InlineData("hello#world", new[] { '@', '#' }, false)]
    [InlineData("", new[] { '@' }, true)]
    [InlineData(null, new[] { '@' }, true)]
    public void ContainsNoForbiddenChars_ShouldReturnCorrectResult(string? input, char[] forbiddenChars, bool expected)
    {
        // Act
        var result = ValidationHelper.ContainsNoForbiddenChars(input, forbiddenChars);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("safe text", true)]
    [InlineData("SELECT * FROM users", false)]
    [InlineData("DROP TABLE users", false)]
    [InlineData("text with 'quotes'", false)]
    [InlineData("text with -- comment", false)]
    [InlineData("", true)]
    [InlineData(null, true)]
    public void IsSqlSafe_ShouldReturnCorrectResult(string? input, bool expected)
    {
        // Act
        var result = ValidationHelper.IsSqlSafe(input);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ValidateObject_WithValidObject_ShouldReturnNoErrors()
    {
        // Arrange
        var validObject = new TestValidationModel
        {
            Name = "Test Name",
            Email = "test@example.com",
            Age = 25
        };

        // Act
        var errors = ValidationHelper.ValidateObject(validObject);

        // Assert
        errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidateObject_WithInvalidObject_ShouldReturnErrors()
    {
        // Arrange
        var invalidObject = new TestValidationModel
        {
            Name = "", // Required field
            Email = "invalid-email", // Invalid format
            Age = -1 // Invalid range
        };

        // Act
        var errors = ValidationHelper.ValidateObject(invalidObject);

        // Assert
        errors.Should().NotBeEmpty();
        errors.Should().HaveCountGreaterThan(2);
    }

    [Fact]
    public void IsValid_WithValidObject_ShouldReturnTrue()
    {
        // Arrange
        var validObject = new TestValidationModel
        {
            Name = "Test Name",
            Email = "test@example.com",
            Age = 25
        };

        // Act
        var result = ValidationHelper.IsValid(validObject);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateObjectToDictionary_ShouldReturnCorrectStructure()
    {
        // Arrange
        var invalidObject = new TestValidationModel
        {
            Name = "",
            Email = "invalid-email",
            Age = -1
        };

        // Act
        var errors = ValidationHelper.ValidateObjectToDictionary(invalidObject);

        // Assert
        errors.Should().NotBeEmpty();
        errors.Should().ContainKey(nameof(TestValidationModel.Name));
        errors.Should().ContainKey(nameof(TestValidationModel.Email));
        errors.Should().ContainKey(nameof(TestValidationModel.Age));
    }

    [Fact]
    public void CreateValidationResult_WithValid_ShouldReturnSuccess()
    {
        // Act
        var result = ValidationHelper.CreateValidationResult(true, "Error message");

        // Assert
        result.Should().Be(ValidationResult.Success);
    }

    [Fact]
    public void CreateValidationResult_WithInvalid_ShouldReturnError()
    {
        // Arrange
        var errorMessage = "This is an error";
        var memberName = "PropertyName";

        // Act
        var result = ValidationHelper.CreateValidationResult(false, errorMessage, memberName);

        // Assert
        result.Should().NotBe(ValidationResult.Success);
        result.ErrorMessage.Should().Be(errorMessage);
        result.MemberNames.Should().Contain(memberName);
    }

    [Fact]
    public void ValidateDetailed_ShouldReturnDetailedResult()
    {
        // Arrange
        var invalidObject = new TestValidationModel
        {
            Name = "",
            Email = "invalid",
            Age = -1
        };

        // Act
        var result = ValidationHelper.ValidateDetailed(invalidObject);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        result.Warnings.Should().BeEmpty();
        result.Metadata.Should().BeEmpty();
    }

    public class TestValidationModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Range(0, 120, ErrorMessage = "Age must be between 0 and 120")]
        public int Age { get; set; }
    }
}