using FluentAssertions;
using SiteManagement.Domain.Billing;
using SiteManagement.Domain.Shared.ValueObjects;

namespace SiteManagement.Domain.Tests.Billing;

/// <summary>
/// Specifies the <see cref="ResidentCreditAccount"/> aggregate: a per-resident
/// balance of money the site owes back after an over-charged item was corrected
/// downward. Credit is added when a paid item's amount is reduced, and consumed
/// against a later bill — but only when it fully covers that bill, so no item is
/// ever left partially settled.
/// </summary>
public class ResidentCreditAccountTests
{
    private static readonly Guid ResidentId = Guid.NewGuid();

    [Fact]
    public void Open_StartsWithZeroBalance()
    {
        // act
        var account = ResidentCreditAccount.Open(ResidentId);

        // assert
        account.ResidentId.Should().Be(ResidentId);
        account.Balance.Should().Be(Money.Zero);
    }

    [Fact]
    public void AddCredit_IncreasesBalance()
    {
        // arrange
        var account = ResidentCreditAccount.Open(ResidentId);

        // act
        account.AddCredit(Money.Of(13_500m));
        account.AddCredit(Money.Of(500m));

        // assert
        account.Balance.Should().Be(Money.Of(14_000m));
    }

    [Fact]
    public void Consume_WhenBalanceCoversTheBill_AppliesItAndReducesBalance()
    {
        // arrange — a resident holding 13,500 credit, billed 1,500 next period
        var account = ResidentCreditAccount.Open(ResidentId);
        account.AddCredit(Money.Of(13_500m));

        // act
        var applied = account.Consume(Money.Of(1_500m));

        // assert — the bill is fully covered and the balance drops by that much
        applied.Should().Be(Money.Of(1_500m));
        account.Balance.Should().Be(Money.Of(12_000m));
    }

    [Fact]
    public void Consume_WhenBalanceIsExactlyTheBill_EmptiesTheAccount()
    {
        // arrange
        var account = ResidentCreditAccount.Open(ResidentId);
        account.AddCredit(Money.Of(1_500m));

        // act
        var applied = account.Consume(Money.Of(1_500m));

        // assert
        applied.Should().Be(Money.Of(1_500m));
        account.Balance.Should().Be(Money.Zero);
    }

    [Fact]
    public void Consume_WhenBalanceCannotFullyCoverTheBill_AppliesNothing()
    {
        // arrange — only 1,000 credit against a 1,500 bill: partial settlement
        // is not allowed, so the credit is left untouched to accrue further.
        var account = ResidentCreditAccount.Open(ResidentId);
        account.AddCredit(Money.Of(1_000m));

        // act
        var applied = account.Consume(Money.Of(1_500m));

        // assert
        applied.Should().Be(Money.Zero);
        account.Balance.Should().Be(Money.Of(1_000m));
    }
}
