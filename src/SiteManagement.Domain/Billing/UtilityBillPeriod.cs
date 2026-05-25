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
    /// creating one item each and returning them so the caller can register the
    /// brand-new inner entities with the persistence tracker. Replaces any prior
    /// distribution.
    /// </summary>
    /// <returns>The freshly created per-apartment items.</returns>
    /// <exception cref="PeriodAlreadyClosedException">Thrown when the period is closed.</exception>
    /// <exception cref="EmptyDistributionException">Thrown when there are no occupants to split between.</exception>
    public IReadOnlyList<UtilityBillItem> DistributeEqually(IReadOnlyList<(Guid ApartmentId, Guid ResidentId)> occupants)
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

        return _items.ToList();
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

    /// <summary>
    /// Corrects the invoice total on an open period and re-splits it equally
    /// across the already-distributed items (the rounding remainder still folds
    /// into the last share, so they sum back to the new total). A paid item
    /// follows the new share too, but if its share dropped, the resident
    /// over-paid: the difference comes back as an <see cref="OverpaymentCredit"/>
    /// (no cash refund). If the period was never distributed, only the total is
    /// updated.
    /// </summary>
    /// <returns>The over-payments to credit back, one per re-rated paid item.</returns>
    /// <exception cref="PeriodAlreadyClosedException">Thrown when the period is closed.</exception>
    public IReadOnlyList<OverpaymentCredit> ChangeTotalAmount(Money newTotal)
    {
        EnsureOpen();

        TotalAmount = newTotal;
        if (_items.Count == 0)
        {
            return [];
        }

        var shares = newTotal.DistributeEqually(_items.Count);
        var credits = new List<OverpaymentCredit>();
        for (var i = 0; i < _items.Count; i++)
        {
            var item = _items[i];
            var newShare = shares[i];
            if (item.Status == BillingItemStatus.Paid && item.Amount.Amount > newShare.Amount)
            {
                credits.Add(new OverpaymentCredit(item.ResidentId, item.Amount.Subtract(newShare)));
            }

            item.ChangeAmount(newShare);
        }

        return credits;
    }

    private void EnsureOpen()
    {
        if (IsClosed)
        {
            throw new PeriodAlreadyClosedException(Id);
        }
    }
}
