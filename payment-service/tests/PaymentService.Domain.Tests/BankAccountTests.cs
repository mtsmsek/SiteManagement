using FluentAssertions;
using PaymentService.Domain;
using PaymentService.Domain.Exceptions;
using PaymentService.Domain.Shared.ValueObjects;

namespace PaymentService.Domain.Tests;

/// <summary>
/// Specifies the <see cref="BankAccount"/> aggregate: the fake bank's funds
/// holder. Debit reduces the balance and declines when it would go negative;
/// credit (top-up) increases it.
/// </summary>
public class BankAccountTests
{
    [Fact]
    public void Open_StartsWithGivenBalance()
    {
        // act
        var account = BankAccount.Open(Money.Of(500m));

        // assert
        account.Id.Should().NotBeEmpty();
        account.Balance.Should().Be(Money.Of(500m));
    }

    [Fact]
    public void Debit_WithinBalance_Reduces()
    {
        // arrange
        var account = BankAccount.Open(Money.Of(500m));

        // act
        account.Debit(Money.Of(200m));

        // assert
        account.Balance.Should().Be(Money.Of(300m));
    }

    [Fact]
    public void Debit_ExactBalance_LeavesZero()
    {
        // arrange
        var account = BankAccount.Open(Money.Of(500m));

        // act
        account.Debit(Money.Of(500m));

        // assert
        account.Balance.Should().Be(Money.Zero);
    }

    [Fact]
    public void Debit_BeyondBalance_Throws_AndLeavesBalanceUnchanged()
    {
        // arrange
        var account = BankAccount.Open(Money.Of(100m));

        // act
        var act = () => account.Debit(Money.Of(100.01m));

        // assert
        act.Should().Throw<InsufficientBalanceException>();
        account.Balance.Should().Be(Money.Of(100m));
    }

    [Fact]
    public void Credit_IncreasesBalance()
    {
        // arrange
        var account = BankAccount.Open(Money.Of(100m));

        // act
        account.Credit(Money.Of(50m));

        // assert
        account.Balance.Should().Be(Money.Of(150m));
    }
}
