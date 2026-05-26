using FluentAssertions;
using NSubstitute;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Billing.Commands.PayMyDuesItem;
using SiteManagement.Application.Billing.Services;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Application.Shared.Resources;
using SiteManagement.Domain.Billing;
using SiteManagement.Domain.Billing.ValueObjects;
using SiteManagement.Domain.Shared.ValueObjects;

namespace SiteManagement.Application.Tests.Billing;

/// <summary>
/// Unit tests for <see cref="PayMyDuesItemCommandHandler"/>. Ownership is the
/// pipeline's job, so these cover the billing work only: charge first, mark paid
/// on success, and leave the item unpaid (no save) when the charge is declined.
/// </summary>
public class PayMyDuesItemCommandHandlerTests
{
    private readonly IDuesPeriodRepository _duesPeriodRepository = Substitute.For<IDuesPeriodRepository>();
    private readonly IBillItemPaymentService _payment = Substitute.For<IBillItemPaymentService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Handle_ChargesItemThenMarksPaidAndSaves()
    {
        // arrange — a distributed dues period with one item
        var period = DuesPeriod.Open(Guid.NewGuid(), BillingMonth.Of(2026, 1), Money.Of(500m));
        var item = period.AddItemFor(Guid.NewGuid(), Guid.NewGuid());
        _duesPeriodRepository.GetByIdAsync(period.Id, Arg.Any<CancellationToken>()).Returns(period);
        var sut = CreateHandler();

        // act
        await sut.Handle(Command(period.Id, item.Id), CancellationToken.None);

        // assert
        await _payment.Received(1).ChargeOrThrowAsync(
            $"dues-item:{item.Id}", 500m, Arg.Any<CardDetails>(), Arg.Any<CancellationToken>());
        item.Status.Should().Be(BillingItemStatus.Paid);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenChargeDeclined_LeavesItemUnpaidAndDoesNotSave()
    {
        // arrange
        var period = DuesPeriod.Open(Guid.NewGuid(), BillingMonth.Of(2026, 1), Money.Of(500m));
        var item = period.AddItemFor(Guid.NewGuid(), Guid.NewGuid());
        _duesPeriodRepository.GetByIdAsync(period.Id, Arg.Any<CancellationToken>()).Returns(period);
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
        _duesPeriodRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((DuesPeriod?)null);
        var sut = CreateHandler();

        // act
        var act = () => sut.Handle(Command(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    private static PayMyDuesItemCommand Command(Guid periodId, Guid itemId)
        => new(periodId, itemId, "4242424242424242", "123", 2030, 12);

    private PayMyDuesItemCommandHandler CreateHandler() => new(_duesPeriodRepository, _payment, _unitOfWork);
}
