using FluentAssertions;
using SiteManagement.Domain.Property.Exceptions;
using SiteManagement.Domain.Property.ValueObjects;

namespace SiteManagement.Domain.Tests.Property.ValueObjects;

/// <summary>
/// Specifies the <see cref="ApartmentType"/> value object: parsing rules,
/// equality semantics, and the invariant that the raw input matches the
/// <c>N+M</c> shape with N &gt;= 1 and M &gt;= 0.
/// </summary>
public class ApartmentTypeTests
{
    [Theory]
    [InlineData("1+0", 1, 0)]
    [InlineData("2+1", 2, 1)]
    [InlineData("3+2", 3, 2)]
    [InlineData("10+1", 10, 1)]
    public void From_ValidInput_ParsesRoomsAndLivingRooms(string raw, int rooms, int livingRooms)
    {
        // arrange + act
        var type = ApartmentType.From(raw);

        // assert
        type.Rooms.Should().Be(rooms);
        type.LivingRooms.Should().Be(livingRooms);
    }

    [Fact]
    public void From_ValidInput_NormalisesToString()
    {
        // arrange + act
        var type = ApartmentType.From("2+1");

        // assert
        type.ToString().Should().Be("2+1");
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("2")]
    [InlineData("2+")]
    [InlineData("+1")]
    [InlineData("2+1+1")]
    [InlineData("two+one")]
    [InlineData("-1+1")]
    [InlineData("2+-1")]
    [InlineData("0+1")]
    public void From_InvalidInput_Throws(string raw)
    {
        // act
        var act = () => ApartmentType.From(raw);

        // assert
        act.Should().Throw<InvalidApartmentTypeException>();
    }

    [Fact]
    public void From_NullInput_Throws()
    {
        // act
        var act = () => ApartmentType.From(null!);

        // assert
        act.Should().Throw<InvalidApartmentTypeException>();
    }

    [Fact]
    public void Equality_TwoInstancesWithSameRawValue_AreEqual()
    {
        // arrange
        var a = ApartmentType.From("2+1");
        var b = ApartmentType.From("2+1");

        // act + assert
        a.Should().Be(b);
        (a == b).Should().BeTrue();
        a.GetHashCode().Should().Be(b.GetHashCode());
    }
}
