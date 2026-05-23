using FluentAssertions;
using NSubstitute;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Billing.Commands.MarkUtilityBillItemPaid;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Domain.Billing;
using SiteManagement.Domain.Billing.ValueObjects;
using SiteManagement.Domain.Shared.ValueObjects;

namespace SiteManagement.Application.Tests.Billing;

/// <summary>
/// Unit tests for <see cref="MarkUtilityBillItemPaidCommandHandler"/>: load the
/// period, mark the item paid, save — or 404 when the period is unknown.
/// </summary>
public class MarkUtilityBillItemPaidCommandHandlerTests
{
    private readonly IUtilityBillPeriodRepository _utilityBillPeriodRepository = Substitute.For<IUtilityBillPeriodRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Handle_MarksTheItemPaidAndSaves()
    {
        // arrange — a distributed utility bill period with one item
        var period = UtilityBillPeriod.Open(Guid.NewGuid(), BillingMonth.Of(2026, 1), UtilityType.Electricity, Money.Of(300m));
        period.DistributeEqually(new List<(Guid, Guid)> { (Guid.NewGuid(), Guid.NewGuid()) });
        var item = period.Items.First();
        _utilityBillPeriodRepository.GetByIdAsync(period.Id, Arg.Any<CancellationToken>()).Returns(period);
        var sut = CreateHandler();

        // act
        await sut.Handle(new MarkUtilityBillItemPaidCommand(period.Id, item.Id), CancellationToken.None);

        // assert
        item.Status.Should().Be(BillingItemStatus.Paid);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_UnknownPeriod_Throws()
    {
        // arrange
        _utilityBillPeriodRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((UtilityBillPeriod?)null);
        var sut = CreateHandler();

        // act
        var act = () => sut.Handle(new MarkUtilityBillItemPaidCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    private MarkUtilityBillItemPaidCommandHandler CreateHandler() => new(_utilityBillPeriodRepository, _unitOfWork);
}
