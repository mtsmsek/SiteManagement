using FluentAssertions;
using PaymentService.Domain.Shared.Exceptions;
using PaymentService.Domain.Shared.ValueObjects;

namespace PaymentService.Domain.Tests.Shared.ValueObjects;

/// <summary>
/// Specifies PaymentService's own <see cref="Money"/> value object (a copy of
/// the main API's, kept independent): single-currency TRY, non-negative,
/// 2-decimal rounding, value equality, with the comparison helpers the
/// bank-account balance checks need.
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
    public void Of_NegativeAmount_Throws()
    {
        // act
        var act = () => Money.Of(-0.01m);

        // assert
        act.Should().Throw<NegativeMoneyException>();
    }

    [Theory]
    [InlineData(10.005, 10.01)]
    [InlineData(10.004, 10.00)]
    public void Of_RoundsToTwoDecimals(decimal raw, decimal expected)
    {
        // assert
        Money.Of(raw).Amount.Should().Be(expected);
    }

    [Fact]
    public void Add_SumsAmounts()
    {
        // assert
        Money.Of(100m).Add(Money.Of(49.99m)).Amount.Should().Be(149.99m);
    }

    [Fact]
    public void Subtract_WithinBalance_Reduces()
    {
        // assert
        Money.Of(100m).Subtract(Money.Of(30m)).Amount.Should().Be(70m);
    }

    [Fact]
    public void Subtract_BelowZero_Throws()
    {
        // act
        var act = () => Money.Of(10m).Subtract(Money.Of(10.01m));

        // assert
        act.Should().Throw<NegativeMoneyException>();
    }

    [Theory]
    [InlineData(100, 50, true)]
    [InlineData(50, 50, true)]
    [InlineData(49.99, 50, false)]
    public void IsGreaterThanOrEqualTo_ComparesAmounts(decimal balance, decimal amount, bool expected)
    {
        // assert — drives the "enough balance?" check
        Money.Of(balance).IsGreaterThanOrEqualTo(Money.Of(amount)).Should().Be(expected);
    }

    [Fact]
    public void Equality_SameAmount_AreEqual()
    {
        // assert
        Money.Of(50m).Should().Be(Money.Of(50m));
    }

    [Fact]
    public void Zero_HasZeroAmount()
    {
        // assert
        Money.Zero.Amount.Should().Be(0m);
    }
}
