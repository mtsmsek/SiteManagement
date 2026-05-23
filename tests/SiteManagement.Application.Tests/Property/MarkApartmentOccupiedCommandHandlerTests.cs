using FluentAssertions;
using NSubstitute;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Property.Commands.MarkApartmentOccupied;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Property;
using SiteManagement.Domain.Property.ValueObjects;

namespace SiteManagement.Application.Tests.Property;

/// <summary>
/// Unit tests for <see cref="MarkApartmentOccupiedCommandHandler"/>: resolve the
/// owning site, flip the apartment to occupied, and save — or 404 when no site
/// contains the apartment.
/// </summary>
public class MarkApartmentOccupiedCommandHandlerTests
{
    private readonly ISiteRepository _siteRepository = Substitute.For<ISiteRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Handle_MarksTheApartmentOccupiedAndSaves()
    {
        // arrange — a site containing an empty apartment
        var site = Site.Create("Lavender Heights", "Address");
        var block = site.AddBlock(BlockName.From("A"));
        var apt = Apartment.Create(ApartmentNumber.From(1), Floor.From(1), ApartmentType.From("2+1"));
        block.AddApartment(apt);
        _siteRepository.FindContainingApartmentAsync(apt.Id, Arg.Any<CancellationToken>()).Returns(site);
        var sut = CreateHandler();

        // act
        await sut.Handle(new MarkApartmentOccupiedCommand(apt.Id), CancellationToken.None);

        // assert
        apt.Status.Should().Be(OccupancyStatus.Occupied);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnknownApartment_Throws()
    {
        // arrange
        _siteRepository.FindContainingApartmentAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Site?)null);
        var sut = CreateHandler();

        // act
        var act = () => sut.Handle(new MarkApartmentOccupiedCommand(Guid.NewGuid()), CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    private MarkApartmentOccupiedCommandHandler CreateHandler() => new(_siteRepository, _unitOfWork);
}
