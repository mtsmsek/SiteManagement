using FluentAssertions;
using SiteManagement.Domain.Property;
using SiteManagement.Domain.Property.Exceptions;
using SiteManagement.Domain.Property.ValueObjects;
using SiteManagement.Domain.Tests.Doubles;

namespace SiteManagement.Domain.Tests.Property;

/// <summary>
/// Specifies the <see cref="Block"/> entity: a named container of apartments
/// that guards apartment-number uniqueness within itself and exposes its
/// children as a read-only collection (no direct list mutation from outside).
/// </summary>
public class BlockTests
{
    [Fact]
    public void Create_AssignsIdAndStartsEmpty()
    {
        // arrange + act
        var block = Block.Create(PropertyDoubles.SampleBlockName());

        // assert
        block.Id.Should().NotBe(Guid.Empty);
        block.Apartments.Should().BeEmpty();
    }

    [Fact]
    public void AddApartment_NewNumber_Succeeds()
    {
        // arrange
        var block = Block.Create(PropertyDoubles.SampleBlockName());
        var apartment = PropertyDoubles.SampleApartment(number: 1);

        // act
        block.AddApartment(apartment);

        // assert
        block.Apartments.Should().ContainSingle().Which.Should().Be(apartment);
    }

    [Fact]
    public void AddApartment_DuplicateNumber_Throws()
    {
        // arrange
        var block = Block.Create(PropertyDoubles.SampleBlockName());
        block.AddApartment(PropertyDoubles.SampleApartment(number: 1));

        // act
        var act = () => block.AddApartment(PropertyDoubles.SampleApartment(number: 1));

        // assert
        act.Should().Throw<DuplicateApartmentNumberException>();
    }

    [Fact]
    public void RemoveApartment_ExistingId_Removes()
    {
        // arrange
        var block = Block.Create(PropertyDoubles.SampleBlockName());
        var apartment = PropertyDoubles.SampleApartment(number: 1);
        block.AddApartment(apartment);

        // act
        block.RemoveApartment(apartment.Id);

        // assert
        block.Apartments.Should().BeEmpty();
    }

    [Fact]
    public void RemoveApartment_UnknownId_Throws()
    {
        // arrange
        var block = Block.Create(PropertyDoubles.SampleBlockName());

        // act
        var act = () => block.RemoveApartment(Guid.NewGuid());

        // assert
        act.Should().Throw<ApartmentNotFoundInBlockException>();
    }

    [Fact]
    public void Rename_ReplacesName()
    {
        // arrange
        var block = Block.Create(BlockName.From("A"));

        // act
        block.Rename(BlockName.From("B"));

        // assert
        block.Name.Value.Should().Be("B");
    }

    [Fact]
    public void Apartments_CollectionIsReadOnly()
    {
        // arrange
        var block = Block.Create(PropertyDoubles.SampleBlockName());

        // assert — the exposed type is a read-only collection; callers cannot
        // poke at the backing list directly. This protects the
        // "no duplicate apartment numbers" invariant.
        block.Apartments.Should().BeAssignableTo<IReadOnlyCollection<Apartment>>();
        block.Apartments.Should().NotBeAssignableTo<List<Apartment>>();
    }
}
