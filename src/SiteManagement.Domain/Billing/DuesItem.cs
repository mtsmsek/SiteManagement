using SiteManagement.Domain.Shared;
using SiteManagement.Domain.Shared.ValueObjects;

namespace SiteManagement.Domain.Billing;

/// <summary>
/// One apartment's share of a <see cref="DuesPeriod"/>: who owes how much and
/// whether it is paid. An inner entity of the period aggregate — created and
/// mutated only through the period, never on its own.
/// </summary>
public sealed class DuesItem : Entity<Guid>
{
    /// <summary>The billed apartment (Property aggregate id).</summary>
    public Guid ApartmentId { get; private set; }

    /// <summary>The resident responsible for payment (Residency aggregate id).</summary>
    public Guid ResidentId { get; private set; }

    /// <summary>The amount owed for this period.</summary>
    public Money Amount { get; private set; }

    /// <summary>Payment state; starts <see cref="BillingItemStatus.Unpaid"/>.</summary>
    public BillingItemStatus Status { get; private set; }

    // EF Core materialisation ctor.
    private DuesItem()
    {
        Amount = default!;
    }

    private DuesItem(Guid id, Guid apartmentId, Guid residentId, Money amount) : base(id)
    {
        ApartmentId = apartmentId;
        ResidentId = residentId;
        Amount = amount;
        Status = BillingItemStatus.Unpaid;
    }

    /// <summary>Factory used by <see cref="DuesPeriod"/> when distributing.</summary>
    internal static DuesItem Create(Guid apartmentId, Guid residentId, Money amount)
        => new(Guid.NewGuid(), apartmentId, residentId, amount);

    /// <summary>Marks this item paid. Idempotent.</summary>
    internal void MarkPaid() => Status = BillingItemStatus.Paid;
}
