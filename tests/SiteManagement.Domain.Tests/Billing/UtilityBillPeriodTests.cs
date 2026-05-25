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

    [Fact]
    public void ChangeTotalAmount_NotYetDistributed_JustUpdatesTotal()
    {
        // arrange — opened but never distributed
        var period = OpenPeriod(150_000m);

        // act
        var credits = period.ChangeTotalAmount(Money.Of(15_000m));

        // assert
        period.TotalAmount.Should().Be(Money.Of(15_000m));
        period.Items.Should().BeEmpty();
        credits.Should().BeEmpty();
    }

    [Fact]
    public void ChangeTotalAmount_Distributed_ReSplitsAcrossItems_SumsToNewTotal()
    {
        // arrange — 150,000 over 10 apartments (15,000 each), none paid
        var period = OpenPeriod(150_000m);
        period.DistributeEqually(Occupants(10));

        // act — correct the total down to 15,000 (1,500 each)
        var credits = period.ChangeTotalAmount(Money.Of(15_000m));

        // assert — items re-split and still sum to the new total; no paid items, no credit
        period.TotalAmount.Should().Be(Money.Of(15_000m));
        period.Items.Should().AllSatisfy(i => i.Amount.Should().Be(Money.Of(1_500m)));
        SumOfItems(period).Should().Be(Money.Of(15_000m));
        credits.Should().BeEmpty();
    }

    [Fact]
    public void ChangeTotalAmount_CreditsPaidResidentsTheDifference()
    {
        // arrange — 150,000 over 10 apartments; two residents already paid 15,000
        var period = OpenPeriod(150_000m);
        period.DistributeEqually(Occupants(10));
        var paid = period.Items.Take(2).ToList();
        foreach (var item in paid)
        {
            period.MarkItemPaid(item.Id);
        }

        // act — correct down to 15,000; the fair share is now 1,500 each
        var credits = period.ChangeTotalAmount(Money.Of(15_000m));

        // assert — each paid resident is credited 15,000 - 1,500 = 13,500
        credits.Should().HaveCount(2);
        credits.Should().AllSatisfy(c => c.Amount.Should().Be(Money.Of(13_500m)));
        credits.Select(c => c.ResidentId).Should().BeEquivalentTo(paid.Select(i => i.ResidentId));
        period.Items.Should().AllSatisfy(i => i.Amount.Should().Be(Money.Of(1_500m)));
    }

    [Fact]
    public void ChangeTotalAmount_AfterClose_Throws()
    {
        // arrange
        var period = OpenPeriod(100m);
        period.DistributeEqually(Occupants(2));
        period.Close();

        // act
        var act = () => period.ChangeTotalAmount(Money.Of(50m));

        // assert
        act.Should().Throw<PeriodAlreadyClosedException>();
    }

    private static Money SumOfItems(UtilityBillPeriod period)
        => period.Items.Aggregate(Money.Zero, (acc, i) => acc.Add(i.Amount));
}
