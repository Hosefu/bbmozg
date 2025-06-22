using BuddyBot.Shared.Constants;
using FluentAssertions;

namespace BuddyBot.Shared.Tests.Constants;

public class RolesTests
{
    [Fact]
    public void Roles_ShouldHaveCorrectConstantValues()
    {
        // Arrange & Act & Assert
        Roles.Admin.Should().Be("Admin");
        Roles.HRSpecialist.Should().Be("HRSpecialist");
        Roles.Buddy.Should().Be("Buddy");
        Roles.Employee.Should().Be("Employee");
    }

    [Fact]
    public void AllRoles_ShouldContainAllDefinedRoles()
    {
        // Arrange
        var expectedRoles = new[] { "Admin", "HRSpecialist", "Buddy", "Employee" };

        // Act & Assert
        Roles.AllRoles.Should().BeEquivalentTo(expectedRoles);
        Roles.AllRoles.Should().HaveCount(4);
    }

    [Fact]
    public void AdminRoles_ShouldContainOnlyAdministrativeRoles()
    {
        // Arrange
        var expectedAdminRoles = new[] { "Admin", "HRSpecialist" };

        // Act & Assert
        Roles.AdminRoles.Should().BeEquivalentTo(expectedAdminRoles);
        Roles.AdminRoles.Should().HaveCount(2);
    }

    [Fact]
    public void MentorRoles_ShouldContainRolesWithMentorshipRights()
    {
        // Arrange
        var expectedMentorRoles = new[] { "Admin", "HRSpecialist", "Buddy" };

        // Act & Assert
        Roles.MentorRoles.Should().BeEquivalentTo(expectedMentorRoles);
        Roles.MentorRoles.Should().HaveCount(3);
    }

    [Fact]
    public void AllRoles_ShouldNotContainNullOrEmptyValues()
    {
        // Act & Assert
        Roles.AllRoles.Should().NotContainNulls();
        Roles.AllRoles.Should().NotContain(string.Empty);
        Roles.AllRoles.All(role => !string.IsNullOrWhiteSpace(role)).Should().BeTrue();
    }

    [Fact]
    public void AllRoles_ShouldNotHaveDuplicates()
    {
        // Act & Assert
        Roles.AllRoles.Should().OnlyHaveUniqueItems();
    }

    [Theory]
    [InlineData("Admin")]
    [InlineData("HRSpecialist")]
    [InlineData("Buddy")]
    [InlineData("Employee")]
    public void DefinedRole_ShouldBeInAllRoles(string role)
    {
        // Act & Assert
        Roles.AllRoles.Should().Contain(role);
    }
}