using FluentAssertions;
using SiteManagement.Domain.Billing.Exceptions;
using SiteManagement.Domain.Billing.ValueObjects;

namespace SiteManagement.Domain.Tests.Billing.ValueObjects;

/// <summary>
/// Specifies the <see cref="BillingMonth"/> value object: a year + month a
/// billing period covers (e.g. 2026-05). Month must be 1..12; the value is
/// rendered as "yyyy-MM" and ordered chronologically.
/// </summary>
public class BillingMonthTests
{
    [Fact]
    public void Of_ValidYearMonth_ExposesParts()
    {
        // act
        var month = BillingMonth.Of(2026, 5);

        // assert
        month.Year.Should().Be(2026);
        month.Month.Should().Be(5);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    [InlineData(-1)]
    public void Of_MonthOutOfRange_Throws(int month)
    {
        // act
        var act = () => BillingMonth.Of(2026, month);

        // assert
        act.Should().Throw<InvalidBillingMonthException>();
    }

    [Theory]
    [InlineData(1899)]
    [InlineData(3001)]
    public void Of_YearOutOfRange_Throws(int year)
    {
        // act
        var act = () => BillingMonth.Of(year, 5);

        // assert
        act.Should().Throw<InvalidBillingMonthException>();
    }

    [Fact]
    public void ToString_FormatsAsYearDashMonth()
    {
        // assert — zero-padded month
        BillingMonth.Of(2026, 5).ToString().Should().Be("2026-05");
        BillingMonth.Of(2026, 12).ToString().Should().Be("2026-12");
    }

    [Fact]
    public void Equality_SameYearMonth_AreEqual()
    {
        // assert — value semantics
        BillingMonth.Of(2026, 5).Should().Be(BillingMonth.Of(2026, 5));
        BillingMonth.Of(2026, 5).Should().NotBe(BillingMonth.Of(2026, 6));
    }
}
