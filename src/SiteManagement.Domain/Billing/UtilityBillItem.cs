using SiteManagement.Domain.Shared;
using SiteManagement.Domain.Shared.ValueObjects;

namespace SiteManagement.Domain.Billing;

/// <summary>
/// One apartment's share of a <see cref="UtilityBillPeriod"/>. An inner entity
/// of the period aggregate — created and mutated only through the period.
/// </summary>
public sealed class UtilityBillItem : Entity<Guid>
{
    /// <summary>The billed apartment (Property aggregate id).</summary>
    public Guid ApartmentId { get; private set; }

    /// <summary>The resident responsible for payment (Residency aggregate id).</summary>
    public Guid ResidentId { get; private set; }

    /// <summary>This apartment's share of the utility total.</summary>
    public Money Amount { get; private set; }

    /// <summary>Payment state; starts <see cref="BillingItemStatus.Unpaid"/>.</summary>
    public BillingItemStatus Status { get; private set; }

    // EF Core materialisation ctor.
    private UtilityBillItem()
    {
        Amount = default!;
    }

    private UtilityBillItem(Guid id, Guid apartmentId, Guid residentId, Money amount) : base(id)
    {
        ApartmentId = apartmentId;
        ResidentId = residentId;
        Amount = amount;
        Status = BillingItemStatus.Unpaid;
    }

    /// <summary>Factory used by <see cref="UtilityBillPeriod"/> when distributing.</summary>
    internal static UtilityBillItem Create(Guid apartmentId, Guid residentId, Money amount)
        => new(Guid.NewGuid(), apartmentId, residentId, amount);

    /// <summary>Marks this item paid.</summary>
    internal void MarkPaid() => Status = BillingItemStatus.Paid;
}
