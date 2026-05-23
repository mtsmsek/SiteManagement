using FluentAssertions;
using NSubstitute;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Property.Commands.AddBlock;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Property;

namespace SiteManagement.Application.Tests.Property;

/// <summary>
/// Unit tests for <see cref="AddBlockCommandHandler"/>: load the site, add a
/// block, flip the new child to "added", and save — or 404 when the site is unknown.
/// </summary>
public class AddBlockCommandHandlerTests
{
    private readonly ISiteRepository _siteRepository = Substitute.For<ISiteRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Handle_AddsTheBlock_MarksItAdded_AndSaves()
    {
        // arrange
        var site = Site.Create("Lavender Heights", "Address");
        _siteRepository.GetByIdAsync(site.Id, Arg.Any<CancellationToken>()).Returns(site);
        var sut = CreateHandler();

        // act
        var result = await sut.Handle(new AddBlockCommand(site.Id, "A"), CancellationToken.None);

        // assert
        result.BlockId.Should().NotBeEmpty();
        _unitOfWork.Received(1).MarkAsAdded(Arg.Any<Block>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnknownSite_Throws()
    {
        // arrange
        _siteRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Site?)null);
        var sut = CreateHandler();

        // act
        var act = () => sut.Handle(new AddBlockCommand(Guid.NewGuid(), "A"), CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    private AddBlockCommandHandler CreateHandler() => new(_siteRepository, _unitOfWork);
}
