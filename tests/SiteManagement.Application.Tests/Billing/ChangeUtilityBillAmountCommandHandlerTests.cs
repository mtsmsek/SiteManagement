using FluentAssertions;
using NSubstitute;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Billing.Commands.ChangeUtilityBillAmount;
using SiteManagement.Application.Billing.Services;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Billing;
using SiteManagement.Domain.Billing.ValueObjects;
using SiteManagement.Domain.Shared.ValueObjects;

namespace SiteManagement.Application.Tests.Billing;

/// <summary>
/// Unit tests for <see cref="ChangeUtilityBillAmountCommandHandler"/>: re-split
/// the total, hand the resulting over-payments to the credit service, and save.
/// </summary>
public class ChangeUtilityBillAmountCommandHandlerTests
{
    private readonly IUtilityBillPeriodRepository _utilityBillPeriodRepository =
        Substitute.For<IUtilityBillPeriodRepository>();
    private readonly IResidentCreditService _creditService = Substitute.For<IResidentCreditService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Handle_ReSplitsTotalAcrossItems_AndSaves()
    {
        // arrange — 1,000 split across two unpaid items (500 each)
        var period = UtilityBillPeriod.Open(Guid.NewGuid(), BillingMonth.Of(2026, 1), UtilityType.Electricity, Money.Of(1_000m));
        period.DistributeEqually([(Guid.NewGuid(), Guid.NewGuid()), (Guid.NewGuid(), Guid.NewGuid())]);
        _utilityBillPeriodRepository.GetByIdAsync(period.Id, Arg.Any<CancellationToken>()).Returns(period);
        var sut = CreateHandler();

        // act — correct down to 200 (100 each)
        await sut.Handle(new ChangeUtilityBillAmountCommand(period.Id, 200m), CancellationToken.None);

        // assert
        period.TotalAmount.Should().Be(Money.Of(200m));
        period.Items.Should().AllSatisfy(i => i.Amount.Should().Be(Money.Of(100m)));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CreditsOverpaymentsFromPaidItems()
    {
        // arrange — 1,000 across two items; one paid at 500, corrected to 200 total (100 each)
        var period = UtilityBillPeriod.Open(Guid.NewGuid(), BillingMonth.Of(2026, 1), UtilityType.Electricity, Money.Of(1_000m));
        period.DistributeEqually([(Guid.NewGuid(), Guid.NewGuid()), (Guid.NewGuid(), Guid.NewGuid())]);
        period.MarkItemPaid(period.Items.First().Id);
        _utilityBillPeriodRepository.GetByIdAsync(period.Id, Arg.Any<CancellationToken>()).Returns(period);
        var sut = CreateHandler();

        // act
        await sut.Handle(new ChangeUtilityBillAmountCommand(period.Id, 200m), CancellationToken.None);

        // assert — the paid resident is credited 500 - 100 = 400
        await _creditService.Received(1).ApplyCreditsAsync(
            Arg.Is<IReadOnlyList<OverpaymentCredit>>(c => c.Count == 1 && c[0].Amount == Money.Of(400m)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnknownPeriod_Throws()
    {
        // arrange
        _utilityBillPeriodRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((UtilityBillPeriod?)null);
        var sut = CreateHandler();

        // act
        var act = () => sut.Handle(new ChangeUtilityBillAmountCommand(Guid.NewGuid(), 200m), CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    private ChangeUtilityBillAmountCommandHandler CreateHandler()
        => new(_utilityBillPeriodRepository, _creditService, _unitOfWork);
}
