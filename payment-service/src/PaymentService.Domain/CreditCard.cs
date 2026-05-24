using PaymentService.Domain.Exceptions;
using PaymentService.Domain.Shared;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Domain;

/// <summary>
/// Aggregate root for a payment instrument. Holds the card details and draws on
/// a <see cref="BankAccount"/> (referenced by id — accounts and cards are
/// separate aggregates). At charge time the gateway calls
/// <see cref="EnsureUsable"/> to decline expired cards.
/// </summary>
public sealed class CreditCard : AggregateRoot<Guid>
{
    /// <summary>The funds account this card draws on.</summary>
    public Guid BankAccountId { get; private set; }

    /// <summary>The card's primary account number.</summary>
    public CardNumber Number { get; private set; }

    /// <summary>The card's security code.</summary>
    public Cvv Cvv { get; private set; }

    /// <summary>The card's expiry.</summary>
    public ExpiryDate Expiry { get; private set; }

    // Persistence materialisation ctor.
    private CreditCard()
    {
        Number = default!;
        Cvv = default!;
        Expiry = default!;
    }

    private CreditCard(Guid id, Guid bankAccountId, CardNumber number, Cvv cvv, ExpiryDate expiry) : base(id)
    {
        BankAccountId = bankAccountId;
        Number = number;
        Cvv = cvv;
        Expiry = expiry;
    }

    /// <summary>Issues a card bound to a bank account.</summary>
    public static CreditCard Issue(Guid bankAccountId, CardNumber number, Cvv cvv, ExpiryDate expiry)
        => new(Guid.NewGuid(), bankAccountId, number, cvv, expiry);

    /// <summary>
    /// Confirms the card can be charged as of <paramref name="asOf"/>.
    /// </summary>
    /// <exception cref="CardRejectedException">Thrown when the card has expired.</exception>
    public void EnsureUsable(DateOnly asOf)
    {
        if (Expiry.IsExpired(asOf))
        {
            throw new CardRejectedException("expired");
        }
    }
}
