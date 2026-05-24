using FluentAssertions;
using PaymentService.Domain;
using PaymentService.Domain.Exceptions;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Domain.Tests;

/// <summary>
/// Specifies the <see cref="CreditCard"/> aggregate: a payment instrument that
/// draws on a <see cref="BankAccount"/> (by id) and can be checked for
/// usability (not expired) at charge time.
/// </summary>
public class CreditCardTests
{
    private static readonly Guid AccountId = Guid.NewGuid();
    private const string ValidPan = "4242424242424242";

    private static CreditCard IssueCard(int expYear, int expMonth)
        => CreditCard.Issue(
            AccountId,
            CardNumber.From(ValidPan),
            Cvv.From("123"),
            ExpiryDate.Of(expYear, expMonth));

    [Fact]
    public void Issue_LinksAccountAndStoresNumber()
    {
        // act
        var card = IssueCard(2030, 12);

        // assert
        card.Id.Should().NotBeEmpty();
        card.BankAccountId.Should().Be(AccountId);
        card.Number.Value.Should().Be(ValidPan);
    }

    [Fact]
    public void EnsureUsable_NotExpired_DoesNotThrow()
    {
        // arrange — valid through 2030-12
        var card = IssueCard(2030, 12);

        // act
        var act = () => card.EnsureUsable(new DateOnly(2026, 5, 25));

        // assert
        act.Should().NotThrow();
    }

    [Fact]
    public void EnsureUsable_Expired_Throws()
    {
        // arrange — expired end of 2024-01
        var card = IssueCard(2024, 1);

        // act
        var act = () => card.EnsureUsable(new DateOnly(2026, 5, 25));

        // assert
        act.Should().Throw<CardRejectedException>();
    }
}
