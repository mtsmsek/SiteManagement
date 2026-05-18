using FluentAssertions;
using SiteManagement.Domain.Property;
using SiteManagement.Domain.Property.Exceptions;
using SiteManagement.Domain.Property.ValueObjects;

namespace SiteManagement.Domain.Tests.Property.ValueObjects;

/// <summary>
/// Specifies the <see cref="Floor"/> value object: signed range, negative
/// values mean basement, ground floor is 0.
/// </summary>
public class FloorTests
{
    [Theory]
    [InlineData(PropertyLimits.FloorMin)]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(PropertyLimits.FloorMax)]
    public void From_ValidValue_ExposesValue(int value)
    {
        // act
        var floor = Floor.From(value);

        // assert
        floor.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(PropertyLimits.FloorMin - 1)]
    [InlineData(PropertyLimits.FloorMax + 1)]
    public void From_OutOfRange_Throws(int value)
    {
        // act
        var act = () => Floor.From(value);

        // assert
        act.Should().Throw<FloorOutOfRangeException>();
    }

    [Theory]
    [InlineData(-1, true)]
    [InlineData(0, false)]
    [InlineData(5, false)]
    public void IsBasement_ReflectsSign(int value, bool expected)
    {
        // arrange
        var floor = Floor.From(value);

        // assert
        floor.IsBasement.Should().Be(expected);
    }
}
