using FluentAssertions;
using SiteManagement.Domain.Property;
using SiteManagement.Domain.Property.Exceptions;
using SiteManagement.Domain.Property.ValueObjects;

namespace SiteManagement.Domain.Tests.Property.ValueObjects;

/// <summary>
/// Specifies the <see cref="ApartmentNumber"/> value object: must fall in
/// the inclusive [<see cref="PropertyLimits.ApartmentNumberMin"/>..
/// <see cref="PropertyLimits.ApartmentNumberMax"/>] range; equality by value.
/// </summary>
public class ApartmentNumberTests
{
    [Theory]
    [InlineData(PropertyLimits.ApartmentNumberMin)]
    [InlineData(PropertyLimits.ApartmentNumberMax)]
    [InlineData(42)]
    public void From_ValidValue_ExposesValue(int value)
    {
        // act
        var number = ApartmentNumber.From(value);

        // assert
        number.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(PropertyLimits.ApartmentNumberMax + 1)]
    public void From_OutOfRange_Throws(int value)
    {
        // act
        var act = () => ApartmentNumber.From(value);

        // assert
        act.Should().Throw<ApartmentNumberOutOfRangeException>();
    }

    [Fact]
    public void Equality_SameValue_AreEqual()
    {
        // arrange
        var a = ApartmentNumber.From(7);
        var b = ApartmentNumber.From(7);

        // assert
        a.Should().Be(b);
    }
}
