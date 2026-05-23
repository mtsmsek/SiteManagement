using FluentAssertions;
using NSubstitute;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Property.Commands.AddApartment;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Property;
using SiteManagement.Domain.Property.ValueObjects;

namespace SiteManagement.Application.Tests.Property;

/// <summary>
/// Unit tests for <see cref="AddApartmentCommandHandler"/>: resolve the site that
/// owns the block, add an apartment, flip the new child to "added", and save —
/// or 404 when no site contains the block.
/// </summary>
public class AddApartmentCommandHandlerTests
{
    private readonly ISiteRepository _siteRepository = Substitute.For<ISiteRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Handle_AddsTheApartment_MarksItAdded_AndSaves()
    {
        // arrange — a site containing the targeted block
        var site = Site.Create("Lavender Heights", "Address");
        var block = site.AddBlock(BlockName.From("A"));
        _siteRepository.FindContainingBlockAsync(block.Id, Arg.Any<CancellationToken>()).Returns(site);
        var sut = CreateHandler();

        // act
        var result = await sut.Handle(new AddApartmentCommand(block.Id, 1, 1, "2+1"), CancellationToken.None);

        // assert
        result.ApartmentId.Should().NotBeEmpty();
        _unitOfWork.Received(1).MarkAsAdded(Arg.Any<Apartment>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnknownBlock_Throws()
    {
        // arrange
        _siteRepository.FindContainingBlockAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Site?)null);
        var sut = CreateHandler();

        // act
        var act = () => sut.Handle(new AddApartmentCommand(Guid.NewGuid(), 1, 1, "2+1"), CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    private AddApartmentCommandHandler CreateHandler() => new(_siteRepository, _unitOfWork);
}
