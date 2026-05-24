using PaymentService.Domain.Exceptions;
using PaymentService.Domain.Shared;
using PaymentService.Domain.Shared.ValueObjects;

namespace PaymentService.Domain;

/// <summary>
/// Aggregate root for the fake bank's funds: a balance that charges debit
/// against and top-ups credit to. A <see cref="CreditCard"/> draws on one of
/// these. Debit refuses to take the balance negative — the gateway's
/// "insufficient funds" decline.
/// </summary>
public sealed class BankAccount : AggregateRoot<Guid>
{
    /// <summary>Current available funds.</summary>
    public Money Balance { get; private set; }

    // Persistence materialisation ctor.
    private BankAccount() => Balance = default!;

    private BankAccount(Guid id, Money balance) : base(id) => Balance = balance;

    /// <summary>Opens an account with a starting balance.</summary>
    public static BankAccount Open(Money openingBalance) => new(Guid.NewGuid(), openingBalance);

    /// <summary>
    /// Rebuilds an account from persisted state (id + balance). For the
    /// persistence layer only — bypasses no invariants, just reconstitutes.
    /// </summary>
    public static BankAccount Restore(Guid id, Money balance) => new(id, balance);

    /// <summary>
    /// Charges <paramref name="amount"/> against the balance.
    /// </summary>
    /// <exception cref="InsufficientBalanceException">Thrown when funds are insufficient; the balance is left unchanged.</exception>
    public void Debit(Money amount)
    {
        if (!Balance.IsGreaterThanOrEqualTo(amount))
        {
            throw new InsufficientBalanceException(Id);
        }

        Balance = Balance.Subtract(amount);
    }

    /// <summary>Adds funds (a top-up).</summary>
    public void Credit(Money amount) => Balance = Balance.Add(amount);
}
