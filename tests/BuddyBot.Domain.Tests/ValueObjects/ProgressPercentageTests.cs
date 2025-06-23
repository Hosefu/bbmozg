using BuddyBot.Domain.ValueObjects;
using Xunit;

namespace BuddyBot.Domain.Tests.ValueObjects;

/// <summary>
/// Тесты для ProgressPercentage Value Object
/// </summary>
public class ProgressPercentageTests
{
    [Fact]
    public void Constructor_ValidValue_CreatesProgressPercentage()
    {
        // Arrange
        var value = 50.5m;

        // Act
        var progress = new ProgressPercentage(value);

        // Assert
        Assert.Equal(50.5m, progress.Value);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    [InlineData(-0.1)]
    [InlineData(100.1)]
    public void Constructor_InvalidValue_ThrowsArgumentException(decimal invalidValue)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new ProgressPercentage(invalidValue));
    }

    [Fact]
    public void Constructor_RoundsToTwoDecimalPlaces()
    {
        // Arrange
        var value = 50.123456789m;

        // Act
        var progress = new ProgressPercentage(value);

        // Assert
        Assert.Equal(50.12m, progress.Value);
    }

    [Theory]
    [InlineData(0, 10, 0)]
    [InlineData(5, 10, 50)]
    [InlineData(10, 10, 100)]
    [InlineData(3, 7, 42.86)]
    public void FromRatio_ValidRatio_ReturnsCorrectPercentage(int completed, int total, decimal expected)
    {
        // Act
        var progress = ProgressPercentage.FromRatio(completed, total);

        // Assert
        Assert.Equal(expected, progress.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void FromRatio_InvalidTotal_ReturnsZeroPercent(int invalidTotal)
    {
        // Act
        var progress = ProgressPercentage.FromRatio(5, invalidTotal);

        // Assert
        Assert.Equal(0m, progress.Value);
    }

    [Fact]
    public void ImplicitConversion_ToDecimal_ReturnsValue()
    {
        // Arrange
        var progress = new ProgressPercentage(75.5m);

        // Act
        decimal value = progress;

        // Assert
        Assert.Equal(75.5m, value);
    }

    [Fact]
    public void ExplicitConversion_FromDecimal_CreatesProgressPercentage()
    {
        // Arrange
        var value = 25.25m;

        // Act
        var progress = (ProgressPercentage)value;

        // Assert
        Assert.Equal(25.25m, progress.Value);
    }

    [Fact]
    public void ToString_ReturnsFormattedString()
    {
        // Arrange
        var progress = new ProgressPercentage(87.5m);

        // Act
        var result = progress.ToString();

        // Assert
        Assert.Equal("87.5%", result);
    }

    [Fact]
    public void Equality_SameValues_AreEqual()
    {
        // Arrange
        var progress1 = new ProgressPercentage(50m);
        var progress2 = new ProgressPercentage(50m);

        // Act & Assert
        Assert.Equal(progress1, progress2);
        Assert.True(progress1 == progress2);
        Assert.False(progress1 != progress2);
    }

    [Fact]
    public void Equality_DifferentValues_AreNotEqual()
    {
        // Arrange
        var progress1 = new ProgressPercentage(50m);
        var progress2 = new ProgressPercentage(60m);

        // Act & Assert
        Assert.NotEqual(progress1, progress2);
        Assert.False(progress1 == progress2);
        Assert.True(progress1 != progress2);
    }
}