using FluentAssertions;
using SiteManagement.Domain.Residency.Exceptions;
using SiteManagement.Domain.Residency.ValueObjects;

namespace SiteManagement.Domain.Tests.Residency.ValueObjects;

/// <summary>
/// Specifies the <see cref="TcNo"/> value object: length, digit-only,
/// leading-digit rule, and the official Turkish citizenship-number checksum.
/// </summary>
public class TcNoTests
{
    // Two well-known synthetic but algorithm-valid TC numbers used as
    // golden samples across the test suite. They obey the formula but
    // don't correspond to any real person.
    private const string ValidTc1 = "10000000146";
    private const string ValidTc2 = "12345678950";

    [Theory]
    [InlineData(ValidTc1)]
    [InlineData(ValidTc2)]
    public void From_ValidNumber_ExposesValue(string raw)
    {
        // act
        var tc = TcNo.From(raw);

        // assert
        tc.Value.Should().Be(raw);
    }

    [Fact]
    public void From_TrimsSurroundingWhitespace()
    {
        // arrange + act
        var tc = TcNo.From($"  {ValidTc1}  ");

        // assert
        tc.Value.Should().Be(ValidTc1);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("123456789")]            // 9 digits — too short
    [InlineData("123456789012")]         // 12 digits — too long
    [InlineData("1234567890A")]          // contains letter
    [InlineData("00000000000")]          // starts with 0
    public void From_BadShape_Throws(string raw)
    {
        // act
        var act = () => TcNo.From(raw);

        // assert
        act.Should().Throw<InvalidTcNoException>();
    }

    [Fact]
    public void From_Null_Throws()
    {
        // act
        var act = () => TcNo.From(null!);

        // assert
        act.Should().Throw<InvalidTcNoException>();
    }

    [Theory]
    [InlineData("12345678901")]          // checksum digits wrong
    [InlineData("10000000147")]          // last digit off by one (valid sample is ...146)
    public void From_ChecksumFails_Throws(string raw)
    {
        // act
        var act = () => TcNo.From(raw);

        // assert
        act.Should().Throw<InvalidTcNoException>();
    }

    [Fact]
    public void Equality_SameValue_AreEqual()
    {
        // arrange
        var a = TcNo.From(ValidTc1);
        var b = TcNo.From(ValidTc1);

        // assert
        a.Should().Be(b);
        a.GetHashCode().Should().Be(b.GetHashCode());
    }
}
