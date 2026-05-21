using FluentAssertions;
using SiteManagement.Domain.Shared.Exceptions;
using SiteManagement.Domain.Shared.ValueObjects;

namespace SiteManagement.Domain.Tests.Shared.ValueObjects;

/// <summary>
/// Specifies the <see cref="Money"/> value object: single-currency (TRY)
/// non-negative amount with 2-decimal rounding and value-equality. Arithmetic
/// keeps the result non-negative; equal distribution splits a total across N
/// shares with the rounding remainder folded into the last share.
/// </summary>
public class MoneyTests
{
    [Fact]
    public void Of_ValidAmount_ExposesAmountAndTryCurrency()
    {
        // act
        var money = Money.Of(150.50m);

        // assert
        money.Amount.Should().Be(150.50m);
        money.Currency.Should().Be(Money.TurkishLira);
    }

    [Fact]
    public void Zero_HasZeroAmount()
    {
        // assert
        Money.Zero.Amount.Should().Be(0m);
    }

    [Fact]
    public void Of_NegativeAmount_Throws()
    {
        // act
        var act = () => Money.Of(-0.01m);

        // assert
        act.Should().Throw<NegativeMoneyException>();
    }

    [Theory]
    [InlineData(10.005, 10.01)] // banker's-agnostic: round half away from zero
    [InlineData(10.004, 10.00)]
    [InlineData(10.999, 11.00)]
    public void Of_RoundsToTwoDecimals(decimal raw, decimal expected)
    {
        // act
        var money = Money.Of(raw);

        // assert
        money.Amount.Should().Be(expected);
    }

    [Fact]
    public void Add_SumsAmounts()
    {
        // arrange
        var a = Money.Of(100m);
        var b = Money.Of(49.99m);

        // act
        var sum = a.Add(b);

        // assert
        sum.Amount.Should().Be(149.99m);
    }

    [Fact]
    public void Subtract_WithinBalance_Reduces()
    {
        // arrange
        var a = Money.Of(100m);
        var b = Money.Of(30m);

        // act
        var result = a.Subtract(b);

        // assert
        result.Amount.Should().Be(70m);
    }

    [Fact]
    public void Subtract_BelowZero_Throws()
    {
        // arrange
        var a = Money.Of(10m);
        var b = Money.Of(10.01m);

        // act
        var act = () => a.Subtract(b);

        // assert
        act.Should().Throw<NegativeMoneyException>();
    }

    [Fact]
    public void Multiply_ScalesAmount()
    {
        // arrange
        var unit = Money.Of(12.50m);

        // act
        var total = unit.Multiply(4);

        // assert
        total.Amount.Should().Be(50.00m);
    }

    [Fact]
    public void DistributeEqually_DivisibleTotal_SplitsEvenly()
    {
        // arrange
        var total = Money.Of(90m);

        // act
        var shares = total.DistributeEqually(3);

        // assert
        shares.Should().HaveCount(3);
        shares.Should().AllSatisfy(s => s.Amount.Should().Be(30m));
        shares.Aggregate(Money.Zero, (acc, s) => acc.Add(s)).Should().Be(total);
    }

    [Fact]
    public void DistributeEqually_WithRemainder_FoldsRemainderIntoLastShare()
    {
        // arrange — 100.00 / 3 = 33.33 each, 0.01 remainder
        var total = Money.Of(100m);

        // act
        var shares = total.DistributeEqually(3);

        // assert — first shares 33.33, last share absorbs the cent so the sum is exact
        shares.Should().HaveCount(3);
        shares[0].Amount.Should().Be(33.33m);
        shares[1].Amount.Should().Be(33.33m);
        shares[2].Amount.Should().Be(33.34m);
        shares.Aggregate(Money.Zero, (acc, s) => acc.Add(s)).Should().Be(total);
    }

    [Fact]
    public void DistributeEqually_NonPositiveCount_Throws()
    {
        // act
        var act = () => Money.Of(100m).DistributeEqually(0);

        // assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Equality_SameAmount_AreEqual()
    {
        // assert — value semantics
        Money.Of(50m).Should().Be(Money.Of(50m));
        (Money.Of(50m) == Money.Of(50m)).Should().BeTrue();
    }

    [Fact]
    public void ToString_FormatsWithCurrency()
    {
        // assert
        Money.Of(1234.5m).ToString().Should().Be("1234.50 TRY");
    }
}
