using Lauf.Domain.ValueObjects;
using Lauf.Domain.Enums;
using Xunit;

namespace Lauf.Domain.Tests.ValueObjects;

/// <summary>
/// Тесты для DeadlineCalculation Value Object
/// </summary>
public class DeadlineCalculationTests
{
    [Fact]
    public void Constructor_ValidWorkingDays_CreatesDeadlineCalculation()
    {
        // Arrange
        var workingDays = 5;
        var startDate = new DateTime(2024, 1, 15); // Понедельник

        // Act
        var deadline = new DeadlineCalculation(workingDays, startDate);

        // Assert
        Assert.Equal(workingDays, deadline.WorkingDays);
        Assert.Equal(startDate.Date, deadline.StartDate);
        Assert.NotEqual(default, deadline.DeadlineDate);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    public void Constructor_InvalidWorkingDays_ThrowsArgumentException(int invalidWorkingDays)
    {
        // Arrange
        var startDate = DateTime.Now.Date;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new DeadlineCalculation(invalidWorkingDays, startDate));
    }

    [Fact]
    public void Constructor_WithCustomWorkingDays_UsesProvidedDays()
    {
        // Arrange
        var workingDays = 3;
        var startDate = new DateTime(2024, 1, 15);
        var customWorkingDays = new List<Lauf.Domain.Enums.DayOfWeek>
        {
            Lauf.Domain.Enums.DayOfWeek.Monday,
            Lauf.Domain.Enums.DayOfWeek.Wednesday,
            Lauf.Domain.Enums.DayOfWeek.Friday
        };

        // Act
        var deadline = new DeadlineCalculation(workingDays, startDate, customWorkingDays);

        // Assert
        Assert.Equal(customWorkingDays, deadline.WorkingDaysOfWeek);
    }

    [Fact]
    public void Constructor_WithHolidays_ExcludesHolidaysFromCalculation()
    {
        // Arrange
        var workingDays = 5;
        var startDate = new DateTime(2024, 1, 15); // Понедельник
        var holidays = new List<DateTime>
        {
            new DateTime(2024, 1, 17) // Среда - праздник
        };

        // Act
        var deadline = new DeadlineCalculation(workingDays, startDate, null, holidays);

        // Assert
        Assert.Contains(holidays[0], deadline.Holidays);
        // Дедлайн должен быть позже из-за праздника
        var expectedWithoutHoliday = startDate.AddDays(7); // 5 рабочих дней без праздника
        Assert.True(deadline.DeadlineDate > expectedWithoutHoliday);
    }

    [Fact]
    public void IsOverdue_CurrentDateAfterDeadline_ReturnsTrue()
    {
        // Arrange
        var workingDays = 1;
        var startDate = new DateTime(2024, 1, 15);
        var deadline = new DeadlineCalculation(workingDays, startDate);
        var currentDate = deadline.DeadlineDate.AddDays(1);

        // Act
        var isOverdue = deadline.IsOverdue(currentDate);

        // Assert
        Assert.True(isOverdue);
    }

    [Fact]
    public void IsOverdue_CurrentDateBeforeDeadline_ReturnsFalse()
    {
        // Arrange
        var workingDays = 5;
        var startDate = new DateTime(2024, 1, 15);
        var deadline = new DeadlineCalculation(workingDays, startDate);
        var currentDate = deadline.DeadlineDate.AddDays(-1);

        // Act
        var isOverdue = deadline.IsOverdue(currentDate);

        // Assert
        Assert.False(isOverdue);
    }

    [Fact]
    public void DaysUntilDeadline_ReturnsCorrectDayCount()
    {
        // Arrange
        var workingDays = 5;
        var startDate = new DateTime(2024, 1, 15);
        var deadline = new DeadlineCalculation(workingDays, startDate);
        var currentDate = deadline.DeadlineDate.AddDays(-3);

        // Act
        var daysUntil = deadline.DaysUntilDeadline(currentDate);

        // Assert
        Assert.Equal(3, daysUntil);
    }

    [Fact]
    public void DaysUntilDeadline_CurrentDateAfterDeadline_ReturnsNegative()
    {
        // Arrange
        var workingDays = 1;
        var startDate = new DateTime(2024, 1, 15);
        var deadline = new DeadlineCalculation(workingDays, startDate);
        var currentDate = deadline.DeadlineDate.AddDays(2);

        // Act
        var daysUntil = deadline.DaysUntilDeadline(currentDate);

        // Assert
        Assert.Equal(-2, daysUntil);
    }

    [Theory]
    [InlineData(3, 2, true)]   // 2 дня до дедлайна, предупреждение за 3 дня
    [InlineData(3, 3, true)]   // 3 дня до дедлайна, предупреждение за 3 дня
    [InlineData(3, 4, false)]  // 4 дня до дедлайна, предупреждение за 3 дня
    [InlineData(3, 0, false)]  // Дедлайн сегодня
    [InlineData(3, -1, false)] // Дедлайн прошел
    public void IsApproaching_VariousScenarios_ReturnsExpectedResult(int warningDays, int daysUntilDeadline, bool expected)
    {
        // Arrange
        var workingDays = 10;
        var startDate = new DateTime(2024, 1, 15);
        var deadline = new DeadlineCalculation(workingDays, startDate);
        var currentDate = deadline.DeadlineDate.AddDays(-daysUntilDeadline);

        // Act
        var isApproaching = deadline.IsApproaching(currentDate, warningDays);

        // Assert
        Assert.Equal(expected, isApproaching);
    }

    [Fact]
    public void CalculateDeadline_ExcludesWeekends()
    {
        // Arrange - начинаем в пятницу
        var workingDays = 1;
        var startDate = new DateTime(2024, 1, 19); // Пятница
        var deadline = new DeadlineCalculation(workingDays, startDate);

        // Act & Assert
        // Один рабочий день после пятницы должен быть понедельник
        var expectedDeadline = new DateTime(2024, 1, 22); // Понедельник
        Assert.Equal(expectedDeadline, deadline.DeadlineDate);
    }

    [Fact]
    public void CalculateDeadline_WithCustomWorkingDays_OnlyCountsSpecifiedDays()
    {
        // Arrange - только понедельник и среда рабочие дни
        var workingDays = 2;
        var startDate = new DateTime(2024, 1, 15); // Понедельник
        var customWorkingDays = new List<Lauf.Domain.Enums.DayOfWeek>
        {
            Lauf.Domain.Enums.DayOfWeek.Monday,
            Lauf.Domain.Enums.DayOfWeek.Wednesday
        };

        // Act
        var deadline = new DeadlineCalculation(workingDays, startDate, customWorkingDays);

        // Assert
        // 2 рабочих дня от понедельника 15.01: среда 17.01 (день 1) и понедельник 22.01 (день 2)
        var expectedDeadline = new DateTime(2024, 1, 22); // Понедельник  
        Assert.Equal(expectedDeadline, deadline.DeadlineDate);
    }
}