using FluentAssertions;
using SiteManagement.Domain.Billing;
using SiteManagement.Domain.Billing.Events;
using SiteManagement.Domain.Billing.Exceptions;
using SiteManagement.Domain.Billing.ValueObjects;
using SiteManagement.Domain.Shared.ValueObjects;

namespace SiteManagement.Domain.Tests.Billing;

/// <summary>
/// Specifies the <see cref="DuesPeriod"/> aggregate: a monthly dues run for a
/// site at a fixed per-apartment amount. Items are added one per occupied
/// apartment; the period is locked on close (which raises an event); items
/// can be marked paid.
/// </summary>
public class DuesPeriodTests
{
    private static readonly Guid SiteId = Guid.NewGuid();
    private static readonly BillingMonth Month = BillingMonth.Of(2026, 5);
    private static readonly Money PerApartment = Money.Of(750m);

    private static DuesPeriod OpenPeriod() => DuesPeriod.Open(SiteId, Month, PerApartment);

    [Fact]
    public void Open_SetsFieldsAndIsNotClosed()
    {
        // act
        var period = OpenPeriod();

        // assert
        period.SiteId.Should().Be(SiteId);
        period.Month.Should().Be(Month);
        period.PerApartmentAmount.Should().Be(PerApartment);
        period.IsClosed.Should().BeFalse();
        period.Items.Should().BeEmpty();
    }

    [Fact]
    public void AddItemFor_AddsItemAtPerApartmentAmount_Unpaid()
    {
        // arrange
        var period = OpenPeriod();
        var apartmentId = Guid.NewGuid();
        var residentId = Guid.NewGuid();

        // act
        period.AddItemFor(apartmentId, residentId);

        // assert
        period.Items.Should().ContainSingle()
            .Which.Should().Match<DuesItem>(i =>
                i.ApartmentId == apartmentId
                && i.ResidentId == residentId
                && i.Amount == PerApartment
                && i.Status == BillingItemStatus.Unpaid);
    }

    [Fact]
    public void AddItemFor_DuplicateApartment_Throws()
    {
        // arrange
        var period = OpenPeriod();
        var apartmentId = Guid.NewGuid();
        period.AddItemFor(apartmentId, Guid.NewGuid());

        // act
        var act = () => period.AddItemFor(apartmentId, Guid.NewGuid());

        // assert
        act.Should().Throw<DuplicateBillingItemException>();
    }

    [Fact]
    public void AddItemFor_AfterClose_Throws()
    {
        // arrange
        var period = OpenPeriod();
        period.AddItemFor(Guid.NewGuid(), Guid.NewGuid());
        period.Close();

        // act
        var act = () => period.AddItemFor(Guid.NewGuid(), Guid.NewGuid());

        // assert
        act.Should().Throw<PeriodAlreadyClosedException>();
    }

    [Fact]
    public void Close_LocksPeriodAndRaisesEvent()
    {
        // arrange
        var period = OpenPeriod();
        period.AddItemFor(Guid.NewGuid(), Guid.NewGuid());

        // act
        period.Close();

        // assert
        period.IsClosed.Should().BeTrue();
        period.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<DuesPeriodClosed>()
            .Which.Should().Match<DuesPeriodClosed>(e => e.DuesPeriodId == period.Id && e.SiteId == SiteId);
    }

    [Fact]
    public void Close_AlreadyClosed_Throws()
    {
        // arrange
        var period = OpenPeriod();
        period.Close();

        // act
        var act = () => period.Close();

        // assert
        act.Should().Throw<PeriodAlreadyClosedException>();
    }

    [Fact]
    public void MarkItemPaid_FlipsStatus()
    {
        // arrange
        var period = OpenPeriod();
        period.AddItemFor(Guid.NewGuid(), Guid.NewGuid());
        var itemId = period.Items.Single().Id;

        // act
        period.MarkItemPaid(itemId);

        // assert
        period.Items.Single().Status.Should().Be(BillingItemStatus.Paid);
    }

    [Fact]
    public void MarkItemPaid_UnknownItem_Throws()
    {
        // arrange
        var period = OpenPeriod();

        // act
        var act = () => period.MarkItemPaid(Guid.NewGuid());

        // assert
        act.Should().Throw<BillingItemNotFoundException>();
    }
}
