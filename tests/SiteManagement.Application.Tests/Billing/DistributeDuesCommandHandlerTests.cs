using FluentAssertions;
using NSubstitute;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Billing.Commands.DistributeDues;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Application.Tenancy.Queries;
using SiteManagement.Domain.Billing;
using SiteManagement.Domain.Billing.ValueObjects;
using SiteManagement.Domain.Shared.ValueObjects;

namespace SiteManagement.Application.Tests.Billing;

/// <summary>
/// Unit tests for <see cref="DistributeDuesCommandHandler"/>: resolve the site's
/// occupants, add a dues item for each, register the brand-new items with the
/// tracker (the bug this guards against), and save.
/// </summary>
public class DistributeDuesCommandHandlerTests
{
    private readonly IDuesPeriodRepository _duesPeriodRepository = Substitute.For<IDuesPeriodRepository>();
    private readonly ITenancyQueries _tenancyQueries = Substitute.For<ITenancyQueries>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Handle_AddsAnItemPerOccupant_MarksThemAdded_AndSaves()
    {
        // arrange — a period over a site with a single occupant
        var siteId = Guid.NewGuid();
        var period = DuesPeriod.Open(siteId, BillingMonth.Of(2026, 1), Money.Of(500m));
        _duesPeriodRepository.GetByIdAsync(period.Id, Arg.Any<CancellationToken>()).Returns(period);

        var occupant = new ApartmentOccupantDto(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Ada Lovelace", "Owner", new DateOnly(2026, 1, 1));
        _tenancyQueries.GetActiveOccupantsForSiteAsync(siteId, Arg.Any<CancellationToken>())
            .Returns(new List<ApartmentOccupantDto> { occupant });

        var sut = CreateHandler();

        // act
        await sut.Handle(new DistributeDuesCommand(period.Id), CancellationToken.None);

        // assert
        period.Items.Should().ContainSingle()
            .Which.ApartmentId.Should().Be(occupant.ApartmentId);
        _unitOfWork.Received(1).MarkAsAdded(Arg.Any<DuesItem>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnknownPeriod_Throws()
    {
        // arrange
        _duesPeriodRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((DuesPeriod?)null);
        var sut = CreateHandler();

        // act
        var act = () => sut.Handle(new DistributeDuesCommand(Guid.NewGuid()), CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    private DistributeDuesCommandHandler CreateHandler()
        => new(_duesPeriodRepository, _tenancyQueries, _unitOfWork);
}
