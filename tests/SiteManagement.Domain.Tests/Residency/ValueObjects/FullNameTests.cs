using FluentAssertions;
using SiteManagement.Domain.Residency;
using SiteManagement.Domain.Residency.Exceptions;
using SiteManagement.Domain.Residency.ValueObjects;

namespace SiteManagement.Domain.Tests.Residency.ValueObjects;

/// <summary>
/// Specifies the <see cref="FullName"/> value object: first + last name are
/// both required, each trimmed and length-capped, equality is by value.
/// </summary>
public class FullNameTests
{
    [Fact]
    public void Create_ValidParts_ExposesValues()
    {
        // act
        var name = FullName.Create("Ada", "Lovelace");

        // assert
        name.FirstName.Should().Be("Ada");
        name.LastName.Should().Be("Lovelace");
        name.ToString().Should().Be("Ada Lovelace");
    }

    [Fact]
    public void Create_TrimsParts()
    {
        // act
        var name = FullName.Create("  Ada  ", "  Lovelace  ");

        // assert
        name.FirstName.Should().Be("Ada");
        name.LastName.Should().Be("Lovelace");
    }

    [Theory]
    [InlineData("", "Lovelace")]
    [InlineData("Ada", "")]
    [InlineData("   ", "Lovelace")]
    [InlineData("Ada", "   ")]
    public void Create_EmptyPart_Throws(string first, string last)
    {
        // act
        var act = () => FullName.Create(first, last);

        // assert
        act.Should().Throw<InvalidFullNameException>();
    }

    [Fact]
    public void Create_FirstNameTooLong_Throws()
    {
        // arrange
        var tooLong = new string('a', ResidencyLimits.NameComponentMaxLength + 1);

        // act
        var act = () => FullName.Create(tooLong, "Lovelace");

        // assert
        act.Should().Throw<InvalidFullNameException>();
    }

    [Fact]
    public void Create_LastNameTooLong_Throws()
    {
        // arrange
        var tooLong = new string('a', ResidencyLimits.NameComponentMaxLength + 1);

        // act
        var act = () => FullName.Create("Ada", tooLong);

        // assert
        act.Should().Throw<InvalidFullNameException>();
    }

    [Fact]
    public void Equality_SameParts_AreEqual()
    {
        // arrange
        var a = FullName.Create("Ada", "Lovelace");
        var b = FullName.Create("Ada", "Lovelace");

        // assert
        a.Should().Be(b);
    }
}
