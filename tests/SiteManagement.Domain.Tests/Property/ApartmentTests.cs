using FluentAssertions;
using SiteManagement.Domain.Property;
using SiteManagement.Domain.Property.Exceptions;
using SiteManagement.Domain.Property.ValueObjects;
using SiteManagement.Domain.Tests.Doubles;

namespace SiteManagement.Domain.Tests.Property;

/// <summary>
/// Specifies the <see cref="Apartment"/> aggregate's invariants: a freshly
/// created apartment starts empty, can be marked occupied/empty in turn,
/// and rejects double-occupy / double-vacate attempts.
/// </summary>
public class ApartmentTests
{
    [Fact]
    public void Create_AssignsIdAndDefaultsToEmpty()
    {
        // arrange + act
        var apartment = PropertyDoubles.SampleApartment();

        // assert
        apartment.Id.Should().NotBe(Guid.Empty);
        apartment.Status.Should().Be(OccupancyStatus.Empty);
        apartment.Number.Value.Should().Be(1);
    }

    [Fact]
    public void MarkAsOccupied_WhenEmpty_TransitionsToOccupied()
    {
        // arrange
        var apartment = PropertyDoubles.SampleApartment();

        // act
        apartment.MarkAsOccupied();

        // assert
        apartment.Status.Should().Be(OccupancyStatus.Occupied);
    }

    [Fact]
    public void MarkAsOccupied_WhenAlreadyOccupied_Throws()
    {
        // arrange
        var apartment = PropertyDoubles.SampleApartment();
        apartment.MarkAsOccupied();

        // act
        var act = apartment.MarkAsOccupied;

        // assert
        act.Should().Throw<ApartmentAlreadyOccupiedException>();
    }

    [Fact]
    public void MarkAsEmpty_WhenOccupied_TransitionsToEmpty()
    {
        // arrange
        var apartment = PropertyDoubles.SampleApartment();
        apartment.MarkAsOccupied();

        // act
        apartment.MarkAsEmpty();

        // assert
        apartment.Status.Should().Be(OccupancyStatus.Empty);
    }

    [Fact]
    public void MarkAsEmpty_WhenAlreadyEmpty_Throws()
    {
        // arrange
        var apartment = PropertyDoubles.SampleApartment();

        // act
        var act = apartment.MarkAsEmpty;

        // assert
        act.Should().Throw<ApartmentNotOccupiedException>();
    }

    [Fact]
    public void ChangeType_ReplacesType()
    {
        // arrange
        var apartment = PropertyDoubles.SampleApartment();
        var newType = ApartmentType.From("3+1");

        // act
        apartment.ChangeType(newType);

        // assert
        apartment.Type.Should().Be(newType);
    }
}
