using Lauf.Shared.Helpers;
using FluentAssertions;
using System.Text.Json;

namespace Lauf.Shared.Tests.Helpers;

public class JsonHelperTests
{
    private readonly TestObject _testObject = new()
    {
        Id = 1,
        Name = "Test",
        IsActive = true,
        Tags = new[] { "tag1", "tag2" }
    };

    private readonly string _testJson = """{"id":1,"name":"Test","isActive":true,"tags":["tag1","tag2"]}""";

    [Fact]
    public void Serialize_ShouldSerializeObjectCorrectly()
    {
        // Act
        var result = JsonHelper.Serialize(_testObject);

        // Assert
        result.Should().Be(_testJson);
    }

    [Fact]
    public void Serialize_WithPrettyOptions_ShouldFormatWithIndentation()
    {
        // Act
        var result = JsonHelper.Serialize(_testObject, JsonHelper.PrettyOptions);

        // Assert
        result.Should().Contain("\n");
        result.Should().Contain("  ");
    }

    [Fact]
    public void SerializeSafe_WithValidObject_ShouldReturnJson()
    {
        // Act
        var result = JsonHelper.SerializeSafe(_testObject);

        // Assert
        result.Should().Be(_testJson);
    }

    [Fact]
    public void SerializeSafe_WithNull_ShouldReturnNull()
    {
        // Act
        var result = JsonHelper.SerializeSafe<object?>(null);

        // Assert
        result.Should().Be("null");
    }

    [Fact]
    public void Deserialize_ShouldDeserializeCorrectly()
    {
        // Act
        var result = JsonHelper.Deserialize<TestObject>(_testJson);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Test");
        result.IsActive.Should().BeTrue();
        result.Tags.Should().BeEquivalentTo(new[] { "tag1", "tag2" });
    }

    [Fact]
    public void Deserialize_WithNullOrEmpty_ShouldReturnDefault()
    {
        // Act
        var result1 = JsonHelper.Deserialize<TestObject>(null);
        var result2 = JsonHelper.Deserialize<TestObject>("");
        var result3 = JsonHelper.Deserialize<TestObject>("   ");

        // Assert
        result1.Should().BeNull();
        result2.Should().BeNull();
        result3.Should().BeNull();
    }

    [Fact]
    public void DeserializeSafe_WithValidJson_ShouldReturnObject()
    {
        // Act
        var result = JsonHelper.DeserializeSafe<TestObject>(_testJson);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
    }

    [Fact]
    public void DeserializeSafe_WithInvalidJson_ShouldReturnDefault()
    {
        // Arrange
        var defaultValue = new TestObject { Id = 999, Name = "Default" };

        // Act
        var result = JsonHelper.DeserializeSafe("invalid json", defaultValue);

        // Assert
        result.Should().Be(defaultValue);
    }

    [Theory]
    [InlineData("""{"test": "value"}""", true)]
    [InlineData("""[1, 2, 3]""", true)]
    [InlineData("invalid json", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsValidJson_ShouldReturnCorrectResult(string? json, bool expected)
    {
        // Act
        var result = JsonHelper.IsValidJson(json!);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void PrettyPrint_ShouldFormatJsonWithIndentation()
    {
        // Arrange
        var compactJson = """{"test":"value","nested":{"key":"value"}}""";

        // Act
        var result = JsonHelper.PrettyPrint(compactJson);

        // Assert
        result.Should().Contain("\n");
        result.Should().Contain("  ");
        JsonHelper.IsValidJson(result).Should().BeTrue();
    }

    [Fact]
    public void Minify_ShouldRemoveWhitespace()
    {
        // Arrange
        var prettyJson = """
        {
          "test": "value",
          "nested": {
            "key": "value"
          }
        }
        """;

        // Act
        var result = JsonHelper.Minify(prettyJson);

        // Assert
        result.Should().NotContain("\n");
        result.Should().NotContain("  ");
        JsonHelper.IsValidJson(result).Should().BeTrue();
    }

    [Fact]
    public void Clone_ShouldCreateDeepCopy()
    {
        // Act
        var result = JsonHelper.Clone(_testObject);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeSameAs(_testObject);
        result!.Id.Should().Be(_testObject.Id);
        result.Name.Should().Be(_testObject.Name);
        result.Tags.Should().BeEquivalentTo(_testObject.Tags);
    }

    [Fact]
    public void CloneSafe_WithValidObject_ShouldReturnClone()
    {
        // Act
        var result = JsonHelper.CloneSafe(_testObject);

        // Assert
        result.Should().NotBeSameAs(_testObject);
        result.Id.Should().Be(_testObject.Id);
    }

    [Fact]
    public void CloneSafe_WithNull_ShouldReturnDefault()
    {
        // Arrange
        var defaultValue = new TestObject { Id = 999 };

        // Act
        var result = JsonHelper.CloneSafe<TestObject?>(null, defaultValue);

        // Assert
        result.Should().Be(defaultValue);
    }

    [Fact]
    public void MergeJson_ShouldMergeObjectsCorrectly()
    {
        // Arrange
        var json1 = """{"prop1": "value1", "prop2": "value2"}""";
        var json2 = """{"prop2": "newValue2", "prop3": "value3"}""";

        // Act
        var result = JsonHelper.MergeJson(json1, json2);

        // Assert
        var merged = JsonHelper.Deserialize<Dictionary<string, object>>(result);
        merged.Should().NotBeNull();
        merged!["prop1"].ToString().Should().Be("value1");
        merged["prop2"].ToString().Should().Be("newValue2"); // Overwritten
        merged["prop3"].ToString().Should().Be("value3");
    }

    [Fact]
    public void GetValueByPath_ShouldExtractValueCorrectly()
    {
        // Arrange
        var json = """{"user": {"profile": {"name": "John Doe"}}}""";

        // Act
        var result = JsonHelper.GetValueByPath(json, "user.profile.name");

        // Assert
        result.Should().Be("John Doe");
    }

    [Fact]
    public void GetValueByPath_WithInvalidPath_ShouldReturnNull()
    {
        // Arrange
        var json = """{"user": {"profile": {"name": "John Doe"}}}""";

        // Act
        var result = JsonHelper.GetValueByPath(json, "user.profile.age");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void DefaultOptions_ShouldHaveCorrectSettings()
    {
        // Assert
        JsonHelper.DefaultOptions.PropertyNamingPolicy.Should().Be(JsonNamingPolicy.CamelCase);
        JsonHelper.DefaultOptions.WriteIndented.Should().BeFalse();
        JsonHelper.DefaultOptions.PropertyNameCaseInsensitive.Should().BeTrue();
    }

    [Fact]
    public void PrettyOptions_ShouldHaveCorrectSettings()
    {
        // Assert
        JsonHelper.PrettyOptions.PropertyNamingPolicy.Should().Be(JsonNamingPolicy.CamelCase);
        JsonHelper.PrettyOptions.WriteIndented.Should().BeTrue();
        JsonHelper.PrettyOptions.PropertyNameCaseInsensitive.Should().BeTrue();
    }

    public class TestObject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string[] Tags { get; set; } = Array.Empty<string>();
    }
}