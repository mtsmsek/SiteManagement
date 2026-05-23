using FluentAssertions;
using NSubstitute;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Property.Commands.RestoreSite;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Property;

namespace SiteManagement.Application.Tests.Property;

/// <summary>
/// Unit tests for <see cref="RestoreSiteCommandHandler"/>: load the archived
/// site (bypassing the filter) and clear its IsDeleted flag.
/// </summary>
public class RestoreSiteCommandHandlerTests
{
    private readonly ISiteRepository _siteRepository = Substitute.For<ISiteRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Handle_RestoresTheArchivedSiteAndSaves()
    {
        // arrange — an archived site
        var site = Site.Create("Maple Court", "Address");
        site.Archive(DateTime.UtcNow);
        _siteRepository.FindIncludingArchivedAsync(site.Id, Arg.Any<CancellationToken>()).Returns(site);
        var sut = CreateHandler();

        // act
        await sut.Handle(new RestoreSiteCommand(site.Id), CancellationToken.None);

        // assert
        site.IsDeleted.Should().BeFalse();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnknownSite_Throws()
    {
        // arrange
        _siteRepository.FindIncludingArchivedAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Site?)null);
        var sut = CreateHandler();

        // act
        var act = () => sut.Handle(new RestoreSiteCommand(Guid.NewGuid()), CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    private RestoreSiteCommandHandler CreateHandler() => new(_siteRepository, _unitOfWork);
}
