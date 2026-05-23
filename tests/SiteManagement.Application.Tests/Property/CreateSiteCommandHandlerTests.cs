using FluentAssertions;
using NSubstitute;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Property.Commands.CreateSite;
using SiteManagement.Domain.Property;

namespace SiteManagement.Application.Tests.Property;

/// <summary>
/// Unit tests for <see cref="CreateSiteCommandHandler"/>: build a new site
/// aggregate, register it with the repository, and save.
/// </summary>
public class CreateSiteCommandHandlerTests
{
    private readonly ISiteRepository _siteRepository = Substitute.For<ISiteRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Handle_CreatesTheSiteAddsItAndSaves()
    {
        // arrange
        var sut = CreateHandler();
        var cmd = new CreateSiteCommand("Lavender Heights", "Cumhuriyet Mah. No:7");

        // act
        var result = await sut.Handle(cmd, CancellationToken.None);

        // assert
        result.SiteId.Should().NotBeEmpty();
        await _siteRepository.Received(1).AddAsync(Arg.Any<Site>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    private CreateSiteCommandHandler CreateHandler() => new(_siteRepository, _unitOfWork);
}
