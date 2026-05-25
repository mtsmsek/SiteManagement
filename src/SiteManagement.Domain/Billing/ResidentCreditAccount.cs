using SiteManagement.Domain.Shared;
using SiteManagement.Domain.Shared.ValueObjects;

namespace SiteManagement.Domain.Billing;

/// <summary>
/// A resident's running credit balance: money the site owes back after a paid
/// billing item was corrected to a lower amount (an over-charge). The site does
/// not refund cash — the overpayment is held here and applied to a future bill.
/// Lives in the Billing context and references the resident by id only; the
/// Residency aggregate is untouched.
/// </summary>
public sealed class ResidentCreditAccount : AggregateRoot<Guid>
{
    /// <summary>The resident this credit belongs to (Residency aggregate id).</summary>
    public Guid ResidentId { get; private set; }

    /// <summary>The amount currently owed back to the resident.</summary>
    public Money Balance { get; private set; }

    // EF Core materialisation ctor.
    private ResidentCreditAccount()
    {
        Balance = default!;
    }

    private ResidentCreditAccount(Guid id, Guid residentId, Money balance) : base(id)
    {
        ResidentId = residentId;
        Balance = balance;
    }

    /// <summary>Opens an empty credit account for a resident.</summary>
    public static ResidentCreditAccount Open(Guid residentId)
        => new(Guid.NewGuid(), residentId, Money.Zero);

    /// <summary>Rebuilds an account from persisted state. Persistence layer only.</summary>
    public static ResidentCreditAccount Restore(Guid id, Guid residentId, Money balance)
        => new(id, residentId, balance);

    /// <summary>Adds an over-payment to the balance.</summary>
    public void AddCredit(Money amount) => Balance = Balance.Add(amount);

    /// <summary>
    /// Applies credit to a bill of <paramref name="owed"/>. Credit is only used
    /// when it fully covers the bill — partial settlement is not allowed, so an
    /// item is never left half-paid; insufficient credit is left to accrue.
    /// </summary>
    /// <returns>The amount applied: the full bill, or <see cref="Money.Zero"/>.</returns>
    public Money Consume(Money owed)
    {
        if (Balance.Amount < owed.Amount)
        {
            return Money.Zero;
        }

        Balance = Balance.Subtract(owed);
        return owed;
    }
}
