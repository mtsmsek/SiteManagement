using SiteManagement.Domain.Billing.Events;
using SiteManagement.Domain.Billing.Exceptions;
using SiteManagement.Domain.Billing.ValueObjects;
using SiteManagement.Domain.Shared;
using SiteManagement.Domain.Shared.ValueObjects;

namespace SiteManagement.Domain.Billing;

/// <summary>
/// Aggregate root for a month's utility bill (electricity/water/gas) at a site.
/// The admin opens it with the invoice total, then distributes that total
/// equally across the occupied apartments — the rounding remainder is folded
/// into the last share by <see cref="Money.DistributeEqually"/> so the items
/// always sum back to the total. Closing locks the period and raises
/// <see cref="UtilityBillPeriodClosed"/>.
/// </summary>
public sealed class UtilityBillPeriod : AggregateRoot<Guid>
{
    private readonly List<UtilityBillItem> _items = [];

    /// <summary>The site this bill belongs to (Property aggregate id).</summary>
    public Guid SiteId { get; private set; }

    /// <summary>The month being billed.</summary>
    public BillingMonth Month { get; private set; }

    /// <summary>Which utility this bill is for.</summary>
    public UtilityType UtilityType { get; private set; }

    /// <summary>The invoice total to split across occupied apartments.</summary>
    public Money TotalAmount { get; private set; }

    /// <summary>True once the period is locked; no further distribution.</summary>
    public bool IsClosed { get; private set; }

    /// <summary>Read-only view over the per-apartment items.</summary>
    public IReadOnlyCollection<UtilityBillItem> Items => _items.AsReadOnly();

    // EF Core materialisation ctor.
    private UtilityBillPeriod()
    {
        Month = default!;
        TotalAmount = default!;
    }

    private UtilityBillPeriod(Guid id, Guid siteId, BillingMonth month, UtilityType utilityType, Money totalAmount)
        : base(id)
    {
        SiteId = siteId;
        Month = month;
        UtilityType = utilityType;
        TotalAmount = totalAmount;
    }

    /// <summary>Opens an empty utility bill period for a given total.</summary>
    public static UtilityBillPeriod Open(Guid siteId, BillingMonth month, UtilityType utilityType, Money totalAmount)
        => new(Guid.NewGuid(), siteId, month, utilityType, totalAmount);

    /// <summary>
    /// Splits the total equally across the supplied occupied apartments,
    /// creating one item each. Replaces any prior distribution.
    /// </summary>
    /// <exception cref="PeriodAlreadyClosedException">Thrown when the period is closed.</exception>
    /// <exception cref="EmptyDistributionException">Thrown when there are no occupants to split between.</exception>
    public void DistributeEqually(IReadOnlyList<(Guid ApartmentId, Guid ResidentId)> occupants)
    {
        EnsureOpen();

        if (occupants.Count == 0)
        {
            throw new EmptyDistributionException();
        }

        var shares = TotalAmount.DistributeEqually(occupants.Count);

        _items.Clear();
        for (var i = 0; i < occupants.Count; i++)
        {
            _items.Add(UtilityBillItem.Create(occupants[i].ApartmentId, occupants[i].ResidentId, shares[i]));
        }
    }

    /// <summary>Locks the period and raises <see cref="UtilityBillPeriodClosed"/>.</summary>
    /// <exception cref="PeriodAlreadyClosedException">Thrown when the period is already closed.</exception>
    public void Close()
    {
        EnsureOpen();

        IsClosed = true;
        RaiseDomainEvent(new UtilityBillPeriodClosed(Id, SiteId, UtilityType));
    }

    /// <summary>Marks the given item paid.</summary>
    /// <exception cref="BillingItemNotFoundException">Thrown when the item isn't in this period.</exception>
    public void MarkItemPaid(Guid itemId)
    {
        var item = _items.FirstOrDefault(i => i.Id == itemId)
            ?? throw new BillingItemNotFoundException(itemId);

        item.MarkPaid();
    }

    private void EnsureOpen()
    {
        if (IsClosed)
        {
            throw new PeriodAlreadyClosedException(Id);
        }
    }
}
