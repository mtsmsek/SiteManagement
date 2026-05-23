using FluentAssertions;
using NSubstitute;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Property.Commands.DeleteSite;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Property;

namespace SiteManagement.Application.Tests.Property;

/// <summary>
/// Unit tests for <see cref="DeleteSiteCommandHandler"/> — the soft-delete path:
/// archive the aggregate and save, or 404 when the site is unknown.
/// </summary>
public class DeleteSiteCommandHandlerTests
{
    private readonly ISiteRepository _siteRepository = Substitute.For<ISiteRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Handle_ArchivesTheSiteAndSaves()
    {
        // arrange
        var site = Site.Create("Lavender Heights", "Cumhuriyet Mah. No:7");
        _siteRepository.GetByIdAsync(site.Id, Arg.Any<CancellationToken>()).Returns(site);
        var sut = CreateHandler();

        // act
        await sut.Handle(new DeleteSiteCommand(site.Id), CancellationToken.None);

        // assert
        site.IsDeleted.Should().BeTrue();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnknownSite_Throws()
    {
        // arrange
        _siteRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Site?)null);
        var sut = CreateHandler();

        // act
        var act = () => sut.Handle(new DeleteSiteCommand(Guid.NewGuid()), CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    private DeleteSiteCommandHandler CreateHandler() => new(_siteRepository, _unitOfWork, TimeProvider.System);
}
