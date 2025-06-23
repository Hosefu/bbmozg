using Lauf.Shared.Extensions;
using FluentAssertions;

namespace Lauf.Shared.Tests.Extensions;

public class StringExtensionsTests
{
    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("test", false)]
    [InlineData(" ", false)]
    public void IsNullOrEmpty_ShouldReturnCorrectResult(string? input, bool expected)
    {
        // Act
        var result = input.IsNullOrEmpty();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("   ", true)]
    [InlineData("test", false)]
    [InlineData(" test ", false)]
    public void IsNullOrWhiteSpace_ShouldReturnCorrectResult(string? input, bool expected)
    {
        // Act
        var result = input.IsNullOrWhiteSpace();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("Hello, World!", 10, "Hello, ...")]
    [InlineData("Short", 10, "Short")]
    [InlineData("Exactly10!", 10, "Exactly10!")]
    [InlineData("", 5, "")]
    [InlineData(null, 5, "")]
    public void Truncate_ShouldTruncateCorrectly(string? input, int maxLength, string expected)
    {
        // Act
        var result = input.Truncate(maxLength);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("hello world", "Hello World")]
    [InlineData("HELLO WORLD", "Hello World")]
    [InlineData("hELLo WoRLd", "Hello World")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void ToTitleCase_ShouldConvertCorrectly(string? input, string expected)
    {
        // Act
        var result = input.ToTitleCase();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("<p>Hello <b>World</b></p>", "Hello World")]
    [InlineData("<div>Test</div>", "Test")]
    [InlineData("No HTML here", "No HTML here")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void StripHtml_ShouldRemoveHtmlTags(string? input, string expected)
    {
        // Act
        var result = input.StripHtml();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("Hello World", "hello-world")]
    [InlineData("Hello   World", "hello-world")]
    [InlineData("Привет Мир", "privet-mir")]
    [InlineData("Hello, World!", "hello-world")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void ToSlug_ShouldCreateSlug(string? input, string expected)
    {
        // Act
        var result = input.ToSlug();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("Hello world", 2)]
    [InlineData("One", 1)]
    [InlineData("", 0)]
    [InlineData("   ", 0)]
    [InlineData(null, 0)]
    [InlineData("Word1\tWord2\nWord3", 3)]
    public void WordCount_ShouldCountWordsCorrectly(string? input, int expected)
    {
        // Act
        var result = input.WordCount();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("1234567890", 2, 2, "12******90")]
    [InlineData("test", 1, 1, "t**t")]
    [InlineData("ab", 1, 1, "ab")]
    [InlineData("", 1, 1, "")]
    [InlineData(null, 1, 1, "")]
    public void Mask_ShouldMaskCorrectly(string? input, int visibleStart, int visibleEnd, string expected)
    {
        // Act
        var result = input.Mask(visibleStart, visibleEnd);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("test@example.com", true)]
    [InlineData("invalid.email", false)]
    [InlineData("@example.com", false)]
    [InlineData("test@", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsValidEmail_ShouldValidateCorrectly(string? input, bool expected)
    {
        // Act
        var result = input.IsValidEmail();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("123", 123)]
    [InlineData("0", 0)]
    [InlineData("-123", -123)]
    [InlineData("invalid", 0)]
    [InlineData("", 0)]
    [InlineData(null, 0)]
    public void ToIntSafe_ShouldConvertCorrectly(string? input, int expected)
    {
        // Act
        var result = input.ToIntSafe();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ToIntSafe_WithCustomDefault_ShouldUseDefaultValue()
    {
        // Arrange
        var input = "invalid";
        var defaultValue = 999;

        // Act
        var result = input.ToIntSafe(defaultValue);

        // Assert
        result.Should().Be(defaultValue);
    }

    [Fact]
    public void Truncate_WithCustomEllipsis_ShouldUseCustomEllipsis()
    {
        // Arrange
        var input = "Hello, World!";
        var maxLength = 10;
        var customEllipsis = "***";

        // Act
        var result = input.Truncate(maxLength, customEllipsis);

        // Assert
        result.Should().Be("Hello, ***");
    }

    [Fact]
    public void Truncate_WhenMaxLengthIsNegative_ShouldReturnOriginal()
    {
        // Arrange
        var input = "Hello, World!";
        var maxLength = -1;

        // Act
        var result = input.Truncate(maxLength);

        // Assert
        result.Should().Be(input);
    }

    [Fact]
    public void Mask_WithCustomMaskChar_ShouldUseCustomChar()
    {
        // Arrange
        var input = "1234567890";
        var maskChar = '#';

        // Act
        var result = input.Mask(2, 2, maskChar);

        // Assert
        result.Should().Be("12######90");
    }
}