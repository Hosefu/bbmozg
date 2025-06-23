using Lauf.Shared.Extensions;
using FluentAssertions;

namespace Lauf.Shared.Tests.Extensions;

public class DateTimeExtensionsTests
{
    [Theory]
    [InlineData("2024-01-15", true)]  // Monday
    [InlineData("2024-01-16", true)]  // Tuesday
    [InlineData("2024-01-17", true)]  // Wednesday
    [InlineData("2024-01-18", true)]  // Thursday
    [InlineData("2024-01-19", true)]  // Friday
    [InlineData("2024-01-20", false)] // Saturday
    [InlineData("2024-01-21", false)] // Sunday
    public void IsWorkingDay_ShouldIdentifyWorkingDaysCorrectly(string dateString, bool expected)
    {
        // Arrange
        var date = DateTime.Parse(dateString);

        // Act
        var result = date.IsWorkingDay();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("2024-01-20", true)]  // Saturday
    [InlineData("2024-01-21", true)]  // Sunday
    [InlineData("2024-01-15", false)] // Monday
    public void IsWeekend_ShouldIdentifyWeekendsCorrectly(string dateString, bool expected)
    {
        // Arrange
        var date = DateTime.Parse(dateString);

        // Act
        var result = date.IsWeekend();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void StartOfDay_ShouldReturnMidnight()
    {
        // Arrange
        var date = new DateTime(2024, 1, 15, 14, 30, 45);

        // Act
        var result = date.StartOfDay();

        // Assert
        result.Should().Be(new DateTime(2024, 1, 15, 0, 0, 0));
    }

    [Fact]
    public void EndOfDay_ShouldReturnEndOfDay()
    {
        // Arrange
        var date = new DateTime(2024, 1, 15, 14, 30, 45);

        // Act
        var result = date.EndOfDay();

        // Assert
        result.Should().Be(new DateTime(2024, 1, 15, 23, 59, 59, 999).AddTicks(9999));
    }

    [Fact]
    public void StartOfWeek_ShouldReturnMonday()
    {
        // Arrange (Wednesday)
        var date = new DateTime(2024, 1, 17, 14, 30, 45);

        // Act
        var result = date.StartOfWeek();

        // Assert
        result.Should().Be(new DateTime(2024, 1, 15, 0, 0, 0)); // Monday
    }

    [Fact]
    public void EndOfWeek_ShouldReturnSunday()
    {
        // Arrange (Wednesday)
        var date = new DateTime(2024, 1, 17, 14, 30, 45);

        // Act
        var result = date.EndOfWeek();

        // Assert
        result.Date.Should().Be(new DateTime(2024, 1, 21)); // Sunday
        result.Hour.Should().Be(23);
        result.Minute.Should().Be(59);
        result.Second.Should().Be(59);
    }

    [Fact]
    public void StartOfMonth_ShouldReturnFirstDayOfMonth()
    {
        // Arrange
        var date = new DateTime(2024, 1, 15, 14, 30, 45);

        // Act
        var result = date.StartOfMonth();

        // Assert
        result.Should().Be(new DateTime(2024, 1, 1));
    }

    [Fact]
    public void EndOfMonth_ShouldReturnLastDayOfMonth()
    {
        // Arrange
        var date = new DateTime(2024, 1, 15, 14, 30, 45);

        // Act
        var result = date.EndOfMonth();

        // Assert
        result.Date.Should().Be(new DateTime(2024, 1, 31));
        result.Hour.Should().Be(23);
        result.Minute.Should().Be(59);
        result.Second.Should().Be(59);
    }

    [Fact]
    public void AddWorkingDays_ShouldSkipWeekends()
    {
        // Arrange (Friday)
        var startDate = new DateTime(2024, 1, 19);

        // Act
        var result = startDate.AddWorkingDays(3);

        // Assert
        result.Should().Be(new DateTime(2024, 1, 24)); // Wednesday (skipping weekend)
    }

    [Fact]
    public void AddWorkingDays_Negative_ShouldGoBackwards()
    {
        // Arrange (Wednesday)
        var startDate = new DateTime(2024, 1, 17);

        // Act
        var result = startDate.AddWorkingDays(-2);

        // Assert
        result.Should().Be(new DateTime(2024, 1, 15)); // Monday
    }

    [Fact]
    public void WorkingDaysBetween_ShouldCountCorrectly()
    {
        // Arrange (Monday to Friday)
        var startDate = new DateTime(2024, 1, 15); // Monday
        var endDate = new DateTime(2024, 1, 19);   // Friday

        // Act
        var result = startDate.WorkingDaysBetween(endDate);

        // Assert
        result.Should().Be(4); // Mon, Tue, Wed, Thu (excluding end date)
    }

    [Theory]
    [InlineData("2000-01-01", "2024-01-01", 24)]
    [InlineData("2000-02-29", "2024-02-28", 23)] // Leap year consideration
    [InlineData("2000-03-01", "2024-02-29", 23)]
    public void AgeInYears_ShouldCalculateCorrectly(string birthDateString, string asOfDateString, int expectedAge)
    {
        // Arrange
        var birthDate = DateTime.Parse(birthDateString);
        var asOfDate = DateTime.Parse(asOfDateString);

        // Act
        var result = birthDate.AgeInYears(asOfDate);

        // Assert
        result.Should().Be(expectedAge);
    }

    [Theory]
    [InlineData("2024-01-15", "2024-01-10", "2024-01-20", true)]
    [InlineData("2024-01-15", "2024-01-15", "2024-01-15", true)]
    [InlineData("2024-01-05", "2024-01-10", "2024-01-20", false)]
    [InlineData("2024-01-25", "2024-01-10", "2024-01-20", false)]
    public void IsBetween_Inclusive_ShouldReturnCorrectResult(string dateString, string startString, string endString, bool expected)
    {
        // Arrange
        var date = DateTime.Parse(dateString);
        var startDate = DateTime.Parse(startString);
        var endDate = DateTime.Parse(endString);

        // Act
        var result = date.IsBetween(startDate, endDate, inclusive: true);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("2024-01-15", "2024-01-10", "2024-01-20", true)]
    [InlineData("2024-01-10", "2024-01-10", "2024-01-20", false)]
    [InlineData("2024-01-20", "2024-01-10", "2024-01-20", false)]
    public void IsBetween_Exclusive_ShouldReturnCorrectResult(string dateString, string startString, string endString, bool expected)
    {
        // Arrange
        var date = DateTime.Parse(dateString);
        var startDate = DateTime.Parse(startString);
        var endDate = DateTime.Parse(endString);

        // Act
        var result = date.IsBetween(startDate, endDate, inclusive: false);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ToRelativeString_ShouldReturnCorrectRelativeTime()
    {
        // Arrange
        var now = new DateTime(2024, 1, 15, 12, 0, 0);
        var twoHoursAgo = now.AddHours(-2);

        // Act
        var result = twoHoursAgo.ToRelativeString(now);

        // Assert
        result.Should().Be("2 часа назад");
    }

    [Fact]
    public void ToRelativeString_JustNow_ShouldReturnJustNow()
    {
        // Arrange
        var now = new DateTime(2024, 1, 15, 12, 0, 0);
        var thirtySecondsAgo = now.AddSeconds(-30);

        // Act
        var result = thirtySecondsAgo.ToRelativeString(now);

        // Assert
        result.Should().Be("только что");
    }

    [Fact]
    public void ToUnixTimestamp_ShouldConvertCorrectly()
    {
        // Arrange
        var date = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act
        var result = date.ToUnixTimestamp();

        // Assert
        result.Should().Be(1704067200); // Unix timestamp for 2024-01-01 00:00:00 UTC
    }

    [Fact]
    public void FromUnixTimestamp_ShouldConvertCorrectly()
    {
        // Arrange
        var timestamp = 1704067200L; // 2024-01-01 00:00:00 UTC

        // Act
        var result = DateTimeExtensions.FromUnixTimestamp(timestamp);

        // Assert
        result.Should().Be(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));
    }
}