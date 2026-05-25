using FluentAssertions;
using NSubstitute;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Billing.Commands.ChangeDuesAmount;
using SiteManagement.Application.Billing.Services;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Billing;
using SiteManagement.Domain.Billing.ValueObjects;
using SiteManagement.Domain.Shared.ValueObjects;

namespace SiteManagement.Application.Tests.Billing;

/// <summary>
/// Unit tests for <see cref="ChangeDuesAmountCommandHandler"/>: re-rate the
/// period, hand the resulting over-payments to the credit service, and save.
/// </summary>
public class ChangeDuesAmountCommandHandlerTests
{
    private readonly IDuesPeriodRepository _duesPeriodRepository = Substitute.For<IDuesPeriodRepository>();
    private readonly IResidentCreditService _creditService = Substitute.For<IResidentCreditService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Handle_ReRatesUnpaidItems_AndSaves()
    {
        // arrange — a period with one unpaid item at 500
        var period = DuesPeriod.Open(Guid.NewGuid(), BillingMonth.Of(2026, 1), Money.Of(500m));
        period.AddItemFor(Guid.NewGuid(), Guid.NewGuid());
        _duesPeriodRepository.GetByIdAsync(period.Id, Arg.Any<CancellationToken>()).Returns(period);
        var sut = CreateHandler();

        // act — correct down to 100
        await sut.Handle(new ChangeDuesAmountCommand(period.Id, 100m), CancellationToken.None);

        // assert
        period.PerApartmentAmount.Should().Be(Money.Of(100m));
        period.Items.Single().Amount.Should().Be(Money.Of(100m));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CreditsOverpaymentsFromPaidItems()
    {
        // arrange — a paid item at 500, corrected down to 100 (overpaid by 400)
        var period = DuesPeriod.Open(Guid.NewGuid(), BillingMonth.Of(2026, 1), Money.Of(500m));
        period.AddItemFor(Guid.NewGuid(), Guid.NewGuid());
        period.MarkItemPaid(period.Items.Single().Id);
        _duesPeriodRepository.GetByIdAsync(period.Id, Arg.Any<CancellationToken>()).Returns(period);
        var sut = CreateHandler();

        // act
        await sut.Handle(new ChangeDuesAmountCommand(period.Id, 100m), CancellationToken.None);

        // assert — a single 400 credit was posted
        await _creditService.Received(1).ApplyCreditsAsync(
            Arg.Is<IReadOnlyList<OverpaymentCredit>>(c => c.Count == 1 && c[0].Amount == Money.Of(400m)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnknownPeriod_Throws()
    {
        // arrange
        _duesPeriodRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((DuesPeriod?)null);
        var sut = CreateHandler();

        // act
        var act = () => sut.Handle(new ChangeDuesAmountCommand(Guid.NewGuid(), 100m), CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    private ChangeDuesAmountCommandHandler CreateHandler()
        => new(_duesPeriodRepository, _creditService, _unitOfWork);
}
