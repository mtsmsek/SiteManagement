using FluentAssertions;
using SiteManagement.Domain.Residency.Exceptions;
using SiteManagement.Domain.Residency.ValueObjects;

namespace SiteManagement.Domain.Tests.Residency.ValueObjects;

/// <summary>
/// Specifies the <see cref="Email"/> value object: simple <c>local@domain.tld</c>
/// shape, lower-cased for equality, length-capped.
/// </summary>
public class EmailTests
{
    [Theory]
    [InlineData("a@b.co")]
    [InlineData("ada@lovelace.org")]
    [InlineData("first.last+tag@example.com")]
    public void From_ValidValue_ExposesValue(string raw)
    {
        // act
        var email = Email.From(raw);

        // assert
        email.Value.Should().Be(raw.ToLowerInvariant());
    }

    [Fact]
    public void From_LowerCasesValue()
    {
        // act
        var email = Email.From("Ada@LoveLace.ORG");

        // assert
        email.Value.Should().Be("ada@lovelace.org");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("plain")]
    [InlineData("plain@")]
    [InlineData("@plain.com")]
    [InlineData("a@b")]                  // no TLD
    [InlineData("a@.com")]               // empty domain segment
    [InlineData("a b@example.com")]      // whitespace
    public void From_InvalidValue_Throws(string raw)
    {
        // act
        var act = () => Email.From(raw);

        // assert
        act.Should().Throw<InvalidEmailException>();
    }

    [Fact]
    public void From_Null_Throws()
    {
        // act
        var act = () => Email.From(null!);

        // assert
        act.Should().Throw<InvalidEmailException>();
    }

    [Fact]
    public void Equality_IgnoresCasing()
    {
        // arrange
        var a = Email.From("Ada@Example.com");
        var b = Email.From("ada@example.com");

        // assert
        a.Should().Be(b);
        a.GetHashCode().Should().Be(b.GetHashCode());
    }
}
