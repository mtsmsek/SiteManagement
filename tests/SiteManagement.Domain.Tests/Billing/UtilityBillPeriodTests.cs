using FluentAssertions;
using SiteManagement.Domain.Billing;
using SiteManagement.Domain.Billing.Events;
using SiteManagement.Domain.Billing.Exceptions;
using SiteManagement.Domain.Billing.ValueObjects;
using SiteManagement.Domain.Shared.ValueObjects;

namespace SiteManagement.Domain.Tests.Billing;

/// <summary>
/// Specifies the <see cref="UtilityBillPeriod"/> aggregate: a monthly utility
/// bill (electricity/water/gas) whose total is split equally across the
/// occupied apartments, with the rounding remainder folded into the last
/// share so the items always sum back to the total.
/// </summary>
public class UtilityBillPeriodTests
{
    private static readonly Guid SiteId = Guid.NewGuid();
    private static readonly BillingMonth Month = BillingMonth.Of(2026, 5);

    private static UtilityBillPeriod OpenPeriod(decimal total)
        => UtilityBillPeriod.Open(SiteId, Month, UtilityType.Electricity, Money.Of(total));

    private static (Guid ApartmentId, Guid ResidentId)[] Occupants(int n)
        => Enumerable.Range(0, n).Select(_ => (Guid.NewGuid(), Guid.NewGuid())).ToArray();

    [Fact]
    public void Open_SetsFieldsAndIsNotClosed()
    {
        // act
        var period = OpenPeriod(900m);

        // assert
        period.SiteId.Should().Be(SiteId);
        period.Month.Should().Be(Month);
        period.UtilityType.Should().Be(UtilityType.Electricity);
        period.TotalAmount.Should().Be(Money.Of(900m));
        period.IsClosed.Should().BeFalse();
        period.Items.Should().BeEmpty();
    }

    [Fact]
    public void DistributeEqually_DivisibleTotal_SplitsEvenly()
    {
        // arrange
        var period = OpenPeriod(900m);
        var occupants = Occupants(3);

        // act
        period.DistributeEqually(occupants);

        // assert
        period.Items.Should().HaveCount(3);
        period.Items.Should().AllSatisfy(i => i.Amount.Should().Be(Money.Of(300m)));
        SumOfItems(period).Should().Be(Money.Of(900m));
    }

    [Fact]
    public void DistributeEqually_WithRemainder_FoldsIntoLastShare_SumsToTotal()
    {
        // arrange — 100.00 / 3 -> 33.33, 33.33, 33.34
        var period = OpenPeriod(100m);
        var occupants = Occupants(3);

        // act
        period.DistributeEqually(occupants);

        // assert
        var amounts = period.Items.Select(i => i.Amount.Amount).ToList();
        amounts.Should().Contain(33.33m);
        amounts.Should().Contain(33.34m);
        SumOfItems(period).Should().Be(Money.Of(100m));
    }

    [Fact]
    public void DistributeEqually_ItemsStartUnpaid_WithApartmentAndResident()
    {
        // arrange
        var period = OpenPeriod(200m);
        var occupants = Occupants(2);

        // act
        period.DistributeEqually(occupants);

        // assert
        period.Items.Should().AllSatisfy(i => i.Status.Should().Be(BillingItemStatus.Unpaid));
        period.Items.Select(i => i.ApartmentId).Should().BeEquivalentTo(occupants.Select(o => o.ApartmentId));
    }

    [Fact]
    public void DistributeEqually_NoOccupants_Throws()
    {
        // arrange
        var period = OpenPeriod(100m);

        // act
        var act = () => period.DistributeEqually([]);

        // assert
        act.Should().Throw<EmptyDistributionException>();
    }

    [Fact]
    public void DistributeEqually_AfterClose_Throws()
    {
        // arrange
        var period = OpenPeriod(100m);
        period.DistributeEqually(Occupants(2));
        period.Close();

        // act
        var act = () => period.DistributeEqually(Occupants(2));

        // assert
        act.Should().Throw<PeriodAlreadyClosedException>();
    }

    [Fact]
    public void Close_LocksPeriodAndRaisesEvent()
    {
        // arrange
        var period = OpenPeriod(100m);
        period.DistributeEqually(Occupants(2));

        // act
        period.Close();

        // assert
        period.IsClosed.Should().BeTrue();
        period.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<UtilityBillPeriodClosed>()
            .Which.Should().Match<UtilityBillPeriodClosed>(e =>
                e.UtilityBillPeriodId == period.Id
                && e.SiteId == SiteId
                && e.UtilityType == UtilityType.Electricity);
    }

    [Fact]
    public void MarkItemPaid_FlipsStatus()
    {
        // arrange
        var period = OpenPeriod(100m);
        period.DistributeEqually(Occupants(2));
        var itemId = period.Items.First().Id;

        // act
        period.MarkItemPaid(itemId);

        // assert
        period.Items.First(i => i.Id == itemId).Status.Should().Be(BillingItemStatus.Paid);
    }

    private static Money SumOfItems(UtilityBillPeriod period)
        => period.Items.Aggregate(Money.Zero, (acc, i) => acc.Add(i.Amount));
}
