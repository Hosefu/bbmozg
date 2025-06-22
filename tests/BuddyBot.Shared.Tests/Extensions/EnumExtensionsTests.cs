using System.ComponentModel;
using BuddyBot.Shared.Extensions;
using FluentAssertions;

namespace BuddyBot.Shared.Tests.Extensions;

public enum TestEnum
{
    [Description("Первое значение")]
    First,
    
    [Description("Второе значение")]
    Second,
    
    Third // Без описания
}

[Flags]
public enum TestFlags
{
    None = 0,
    Flag1 = 1,
    Flag2 = 2,
    Flag3 = 4,
    All = Flag1 | Flag2 | Flag3
}

public class EnumExtensionsTests
{
    [Theory]
    [InlineData(TestEnum.First, "Первое значение")]
    [InlineData(TestEnum.Second, "Второе значение")]
    [InlineData(TestEnum.Third, "Third")]
    public void GetDescription_ShouldReturnCorrectDescription(TestEnum value, string expected)
    {
        // Act
        var result = value.GetDescription();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void GetAllValues_ShouldReturnAllEnumValues()
    {
        // Act
        var result = EnumExtensions.GetAllValues<TestEnum>();

        // Assert
        result.Should().BeEquivalentTo(new[] { TestEnum.First, TestEnum.Second, TestEnum.Third });
        result.Should().HaveCount(3);
    }

    [Fact]
    public void GetAllValuesWithDescriptions_ShouldReturnCorrectDictionary()
    {
        // Act
        var result = EnumExtensions.GetAllValuesWithDescriptions<TestEnum>();

        // Assert
        result.Should().HaveCount(3);
        result[TestEnum.First].Should().Be("Первое значение");
        result[TestEnum.Second].Should().Be("Второе значение");
        result[TestEnum.Third].Should().Be("Third");
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(1, true)]
    [InlineData(2, true)]
    [InlineData(99, false)]
    public void IsValidEnumValue_ShouldReturnCorrectResult(int value, bool expected)
    {
        // Act
        var result = EnumExtensions.IsValidEnumValue<TestEnum>(value);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("First", TestEnum.First)]
    [InlineData("Second", TestEnum.Second)]
    [InlineData("Invalid", TestEnum.First)]
    [InlineData("", TestEnum.First)]
    [InlineData(null, TestEnum.First)]
    public void ParseSafe_String_ShouldReturnCorrectValue(string? value, TestEnum expected)
    {
        // Act
        var result = EnumExtensions.ParseSafe(value, TestEnum.First);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(0, TestEnum.First)]
    [InlineData(1, TestEnum.Second)]
    [InlineData(2, TestEnum.Third)]
    [InlineData(99, TestEnum.First)]
    public void ParseSafe_Int_ShouldReturnCorrectValue(int value, TestEnum expected)
    {
        // Act
        var result = EnumExtensions.ParseSafe(value, TestEnum.First);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(TestEnum.First, TestEnum.Second)]
    [InlineData(TestEnum.Second, TestEnum.Third)]
    [InlineData(TestEnum.Third, TestEnum.First)]
    public void GetNext_ShouldReturnNextValue(TestEnum current, TestEnum expected)
    {
        // Act
        var result = current.GetNext();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(TestEnum.First, TestEnum.Third)]
    [InlineData(TestEnum.Second, TestEnum.First)]
    [InlineData(TestEnum.Third, TestEnum.Second)]
    public void GetPrevious_ShouldReturnPreviousValue(TestEnum current, TestEnum expected)
    {
        // Act
        var result = current.GetPrevious();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(TestFlags.Flag1, TestFlags.Flag1, true)]
    [InlineData(TestFlags.All, TestFlags.Flag1, true)]
    [InlineData(TestFlags.All, TestFlags.Flag2, true)]
    [InlineData(TestFlags.Flag1, TestFlags.Flag2, false)]
    [InlineData(TestFlags.None, TestFlags.Flag1, false)]
    public void HasFlag_ShouldReturnCorrectResult(TestFlags value, TestFlags flag, bool expected)
    {
        // Act
        var result = value.HasFlag(flag);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void AddFlag_ShouldAddFlagCorrectly()
    {
        // Arrange
        var value = TestFlags.Flag1;

        // Act
        var result = value.AddFlag(TestFlags.Flag2);

        // Assert
        result.Should().Be(TestFlags.Flag1 | TestFlags.Flag2);
        result.HasFlag(TestFlags.Flag1).Should().BeTrue();
        result.HasFlag(TestFlags.Flag2).Should().BeTrue();
    }

    [Fact]
    public void RemoveFlag_ShouldRemoveFlagCorrectly()
    {
        // Arrange
        var value = TestFlags.Flag1 | TestFlags.Flag2;

        // Act
        var result = value.RemoveFlag(TestFlags.Flag2);

        // Assert
        result.Should().Be(TestFlags.Flag1);
        result.HasFlag(TestFlags.Flag1).Should().BeTrue();
        result.HasFlag(TestFlags.Flag2).Should().BeFalse();
    }

    [Fact]
    public void ToggleFlag_ShouldToggleFlagCorrectly()
    {
        // Arrange
        var value = TestFlags.Flag1;

        // Act
        var result1 = value.ToggleFlag(TestFlags.Flag2); // Add
        var result2 = result1.ToggleFlag(TestFlags.Flag2); // Remove

        // Assert
        result1.HasFlag(TestFlags.Flag2).Should().BeTrue();
        result2.HasFlag(TestFlags.Flag2).Should().BeFalse();
    }

    [Fact]
    public void GetFlags_ShouldReturnSetFlags()
    {
        // Arrange
        var value = TestFlags.Flag1 | TestFlags.Flag3;

        // Act
        var result = value.GetFlags().ToList();

        // Assert
        result.Should().Contain(TestFlags.Flag1);
        result.Should().Contain(TestFlags.Flag3);
        result.Should().NotContain(TestFlags.Flag2);
        // Note: GetFlags returns all flags that match, including None if it matches
        // This is expected behavior for flag enums
    }

    [Fact]
    public void GetCount_ShouldReturnCorrectCount()
    {
        // Act
        var result = EnumExtensions.GetCount<TestEnum>();

        // Assert
        result.Should().Be(3);
    }

    [Fact]
    public void GetRandom_ShouldReturnValidEnumValue()
    {
        // Act
        var result = EnumExtensions.GetRandom<TestEnum>();

        // Assert
        Enum.IsDefined(typeof(TestEnum), result).Should().BeTrue();
    }

    [Fact]
    public void ToSelectList_ShouldCreateCorrectSelectList()
    {
        // Act
        var result = EnumExtensions.ToSelectList<TestEnum>();

        // Assert
        result.Should().HaveCount(3);
        result.All(item => item.Selected == false).Should().BeTrue();
        
        var firstItem = result.First(item => item.Value.Equals(TestEnum.First));
        firstItem.Text.Should().Be("Первое значение");
        
        var thirdItem = result.First(item => item.Value.Equals(TestEnum.Third));
        thirdItem.Text.Should().Be("Third");
    }

    [Fact]
    public void ParseSafe_CaseInsensitive_ShouldWork()
    {
        // Act
        var result1 = EnumExtensions.ParseSafe("first", TestEnum.Third, ignoreCase: true);
        var result2 = EnumExtensions.ParseSafe("SECOND", TestEnum.Third, ignoreCase: true);

        // Assert
        result1.Should().Be(TestEnum.First);
        result2.Should().Be(TestEnum.Second);
    }

    [Fact]
    public void ParseSafe_CaseSensitive_ShouldReturnDefault()
    {
        // Act
        var result = EnumExtensions.ParseSafe("first", TestEnum.Third, ignoreCase: false);

        // Assert
        result.Should().Be(TestEnum.Third); // Default value
    }
}