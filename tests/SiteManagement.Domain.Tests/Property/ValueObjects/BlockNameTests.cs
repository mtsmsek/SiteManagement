using FluentAssertions;
using SiteManagement.Domain.Property;
using SiteManagement.Domain.Property.Exceptions;
using SiteManagement.Domain.Property.ValueObjects;

namespace SiteManagement.Domain.Tests.Property.ValueObjects;

/// <summary>
/// Specifies the <see cref="BlockName"/> value object: trimmed, non-empty,
/// length capped by <see cref="PropertyLimits.BlockNameMaxLength"/>.
/// </summary>
public class BlockNameTests
{
    [Theory]
    [InlineData("A")]
    [InlineData("Block-7")]
    [InlineData("Lavender Court")]
    public void From_ValidValue_ExposesValue(string value)
    {
        // act
        var name = BlockName.From(value);

        // assert
        name.Value.Should().Be(value);
    }

    [Fact]
    public void From_TrimsSurroundingWhitespace()
    {
        // arrange + act
        var name = BlockName.From("  A  ");

        // assert
        name.Value.Should().Be("A");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void From_EmptyOrWhitespace_Throws(string value)
    {
        // act
        var act = () => BlockName.From(value);

        // assert
        act.Should().Throw<InvalidBlockNameException>();
    }

    [Fact]
    public void From_TooLong_Throws()
    {
        // arrange
        var tooLong = new string('A', PropertyLimits.BlockNameMaxLength + 1);

        // act
        var act = () => BlockName.From(tooLong);

        // assert
        act.Should().Throw<InvalidBlockNameException>();
    }

    [Fact]
    public void Equality_IsCaseInsensitive()
    {
        // arrange
        var a = BlockName.From("a");
        var b = BlockName.From("A");

        // assert — two block names that compare equal must hash equal too,
        // otherwise dictionary/set lookups break the duplicate-name invariant.
        a.Should().Be(b);
        a.GetHashCode().Should().Be(b.GetHashCode());
    }
}
