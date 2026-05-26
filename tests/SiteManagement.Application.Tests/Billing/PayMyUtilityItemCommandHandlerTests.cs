using FluentAssertions;
using NSubstitute;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Billing.Commands.PayMyUtilityItem;
using SiteManagement.Application.Billing.Services;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Application.Shared.Resources;
using SiteManagement.Domain.Billing;
using SiteManagement.Domain.Billing.ValueObjects;
using SiteManagement.Domain.Shared.ValueObjects;

namespace SiteManagement.Application.Tests.Billing;

/// <summary>
/// Unit tests for <see cref="PayMyUtilityItemCommandHandler"/>. Ownership is the
/// pipeline's job, so these cover the billing work only: charge first, mark paid
/// on success, and leave the item unpaid (no save) when the charge is declined.
/// </summary>
public class PayMyUtilityItemCommandHandlerTests
{
    private readonly IUtilityBillPeriodRepository _utilityBillPeriodRepository = Substitute.For<IUtilityBillPeriodRepository>();
    private readonly IBillItemPaymentService _payment = Substitute.For<IBillItemPaymentService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Handle_ChargesItemThenMarksPaidAndSaves()
    {
        // arrange — a distributed utility bill period with one item
        var period = UtilityBillPeriod.Open(Guid.NewGuid(), BillingMonth.Of(2026, 1), UtilityType.Electricity, Money.Of(300m));
        period.DistributeEqually(new List<(Guid, Guid)> { (Guid.NewGuid(), Guid.NewGuid()) });
        var item = period.Items.First();
        _utilityBillPeriodRepository.GetByIdAsync(period.Id, Arg.Any<CancellationToken>()).Returns(period);
        var sut = CreateHandler();

        // act
        await sut.Handle(Command(period.Id, item.Id), CancellationToken.None);

        // assert
        await _payment.Received(1).ChargeOrThrowAsync(
            $"utility-item:{item.Id}", item.Amount.Amount, Arg.Any<CardDetails>(), Arg.Any<CancellationToken>());
        item.Status.Should().Be(BillingItemStatus.Paid);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenChargeDeclined_LeavesItemUnpaidAndDoesNotSave()
    {
        // arrange
        var period = UtilityBillPeriod.Open(Guid.NewGuid(), BillingMonth.Of(2026, 1), UtilityType.Electricity, Money.Of(300m));
        period.DistributeEqually(new List<(Guid, Guid)> { (Guid.NewGuid(), Guid.NewGuid()) });
        var item = period.Items.First();
        _utilityBillPeriodRepository.GetByIdAsync(period.Id, Arg.Any<CancellationToken>()).Returns(period);
        _payment.ChargeOrThrowAsync(Arg.Any<string>(), Arg.Any<decimal>(), Arg.Any<CardDetails>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException(new PaymentRejectedException(ErrorMessageKeys.PaymentRejected, "insufficient_balance")));
        var sut = CreateHandler();

        // act
        var act = () => sut.Handle(Command(period.Id, item.Id), CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<PaymentRejectedException>();
        item.Status.Should().Be(BillingItemStatus.Unpaid);
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnknownPeriod_Throws()
    {
        // arrange
        _utilityBillPeriodRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((UtilityBillPeriod?)null);
        var sut = CreateHandler();

        // act
        var act = () => sut.Handle(Command(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    private static PayMyUtilityItemCommand Command(Guid periodId, Guid itemId)
        => new(periodId, itemId, "4242424242424242", "123", 2030, 12);

    private PayMyUtilityItemCommandHandler CreateHandler() => new(_utilityBillPeriodRepository, _payment, _unitOfWork);
}
