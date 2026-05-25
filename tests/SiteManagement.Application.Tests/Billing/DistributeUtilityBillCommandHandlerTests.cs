using FluentAssertions;
using NSubstitute;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Billing.Commands.DistributeUtilityBill;
using SiteManagement.Application.Billing.Services;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Application.Tenancy.Queries;
using SiteManagement.Domain.Billing;
using SiteManagement.Domain.Billing.ValueObjects;
using SiteManagement.Domain.Shared.ValueObjects;

namespace SiteManagement.Application.Tests.Billing;

/// <summary>
/// Unit tests for <see cref="DistributeUtilityBillCommandHandler"/>: resolve the
/// site's occupants, split the total across them, register the brand-new items
/// with the tracker, settle any covered by credit, and save.
/// </summary>
public class DistributeUtilityBillCommandHandlerTests
{
    private readonly IUtilityBillPeriodRepository _utilityBillPeriodRepository = Substitute.For<IUtilityBillPeriodRepository>();
    private readonly ITenancyQueries _tenancyQueries = Substitute.For<ITenancyQueries>();
    private readonly IResidentCreditService _creditService = Substitute.For<IResidentCreditService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Handle_AddsAnItemPerOccupant_MarksThemAdded_AndSaves()
    {
        // arrange — a period over a site with a single occupant
        var siteId = Guid.NewGuid();
        var period = UtilityBillPeriod.Open(siteId, BillingMonth.Of(2026, 1), UtilityType.Electricity, Money.Of(300m));
        _utilityBillPeriodRepository.GetByIdAsync(period.Id, Arg.Any<CancellationToken>()).Returns(period);

        var occupant = new ApartmentOccupantDto(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Ada Lovelace", "Owner", new DateOnly(2026, 1, 1));
        _tenancyQueries.GetActiveOccupantsForSiteAsync(siteId, Arg.Any<CancellationToken>())
            .Returns(new List<ApartmentOccupantDto> { occupant });

        var sut = CreateHandler();

        // act
        await sut.Handle(new DistributeUtilityBillCommand(period.Id), CancellationToken.None);

        // assert
        period.Items.Should().ContainSingle()
            .Which.ApartmentId.Should().Be(occupant.ApartmentId);
        _unitOfWork.Received(1).MarkAsAdded(Arg.Any<UtilityBillItem>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnknownPeriod_Throws()
    {
        // arrange
        _utilityBillPeriodRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((UtilityBillPeriod?)null);
        var sut = CreateHandler();

        // act
        var act = () => sut.Handle(new DistributeUtilityBillCommand(Guid.NewGuid()), CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    private DistributeUtilityBillCommandHandler CreateHandler()
        => new(_utilityBillPeriodRepository, _tenancyQueries, _creditService, _unitOfWork);
}
