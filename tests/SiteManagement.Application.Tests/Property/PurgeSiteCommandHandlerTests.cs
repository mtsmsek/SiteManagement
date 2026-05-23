using FluentAssertions;
using NSubstitute;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Property.Commands.PurgeSite;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Property;

namespace SiteManagement.Application.Tests.Property;

/// <summary>
/// Unit tests for <see cref="PurgeSiteCommandHandler"/> — the hard-delete path:
/// load (even an archived) site bypassing the filter, remove it, and save.
/// </summary>
public class PurgeSiteCommandHandlerTests
{
    private readonly ISiteRepository _siteRepository = Substitute.For<ISiteRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Handle_RemovesTheSiteAndSaves()
    {
        // arrange
        var site = Site.Create("Sunset Park", "Address");
        _siteRepository.FindIncludingArchivedAsync(site.Id, Arg.Any<CancellationToken>()).Returns(site);
        var sut = CreateHandler();

        // act
        await sut.Handle(new PurgeSiteCommand(site.Id), CancellationToken.None);

        // assert
        _siteRepository.Received(1).Remove(site);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnknownSite_Throws()
    {
        // arrange
        _siteRepository.FindIncludingArchivedAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Site?)null);
        var sut = CreateHandler();

        // act
        var act = () => sut.Handle(new PurgeSiteCommand(Guid.NewGuid()), CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    private PurgeSiteCommandHandler CreateHandler() => new(_siteRepository, _unitOfWork);
}
