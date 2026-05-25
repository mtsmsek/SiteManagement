using SiteManagement.Domain.Billing.Events;
using SiteManagement.Domain.Billing.Exceptions;
using SiteManagement.Domain.Billing.ValueObjects;
using SiteManagement.Domain.Shared;
using SiteManagement.Domain.Shared.ValueObjects;

namespace SiteManagement.Domain.Billing;

/// <summary>
/// Aggregate root for a month's dues run at a site: a fixed per-apartment
/// amount, with one <see cref="DuesItem"/> per occupied apartment. The admin
/// opens the period, the distribution command adds an item for each occupied
/// apartment, then closes it — which locks the period and raises
/// <see cref="DuesPeriodClosed"/> so residents get notified. Apartments and
/// residents are referenced by id only.
/// </summary>
public sealed class DuesPeriod : AggregateRoot<Guid>
{
    private readonly List<DuesItem> _items = [];

    /// <summary>The site this dues run belongs to (Property aggregate id).</summary>
    public Guid SiteId { get; private set; }

    /// <summary>The month being billed.</summary>
    public BillingMonth Month { get; private set; }

    /// <summary>The fixed amount each occupied apartment owes for the month.</summary>
    public Money PerApartmentAmount { get; private set; }

    /// <summary>True once the period is locked; no further items or changes.</summary>
    public bool IsClosed { get; private set; }

    /// <summary>Read-only view over the per-apartment dues items.</summary>
    public IReadOnlyCollection<DuesItem> Items => _items.AsReadOnly();

    // EF Core materialisation ctor.
    private DuesPeriod()
    {
        Month = default!;
        PerApartmentAmount = default!;
    }

    private DuesPeriod(Guid id, Guid siteId, BillingMonth month, Money perApartmentAmount) : base(id)
    {
        SiteId = siteId;
        Month = month;
        PerApartmentAmount = perApartmentAmount;
    }

    /// <summary>Opens an empty dues period at a fixed per-apartment amount.</summary>
    public static DuesPeriod Open(Guid siteId, BillingMonth month, Money perApartmentAmount)
        => new(Guid.NewGuid(), siteId, month, perApartmentAmount);

    /// <summary>
    /// Adds a dues item for an occupied apartment at the period's per-apartment
    /// amount and returns it, so the caller can register the brand-new inner
    /// entity with the persistence tracker.
    /// </summary>
    /// <returns>The freshly created dues item.</returns>
    /// <exception cref="PeriodAlreadyClosedException">Thrown when the period is closed.</exception>
    /// <exception cref="DuplicateBillingItemException">Thrown when the apartment already has an item.</exception>
    public DuesItem AddItemFor(Guid apartmentId, Guid residentId)
    {
        EnsureOpen();

        if (_items.Any(i => i.ApartmentId == apartmentId))
        {
            throw new DuplicateBillingItemException(apartmentId);
        }

        var item = DuesItem.Create(apartmentId, residentId, PerApartmentAmount);
        _items.Add(item);
        return item;
    }

    /// <summary>Locks the period and raises <see cref="DuesPeriodClosed"/>.</summary>
    /// <exception cref="PeriodAlreadyClosedException">Thrown when the period is already closed.</exception>
    public void Close()
    {
        EnsureOpen();

        IsClosed = true;
        RaiseDomainEvent(new DuesPeriodClosed(Id, SiteId));
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
    /// Corrects the per-apartment amount on an open period and re-rates every
    /// item to it. Unpaid items simply follow the new rate. A paid item also
    /// follows the new rate, but if the amount dropped, the resident over-paid:
    /// the difference is returned as an <see cref="OverpaymentCredit"/> so the
    /// application can credit it back (no cash refund). Raising the amount
    /// credits nothing — there is no mechanism to re-charge a settled item here.
    /// </summary>
    /// <returns>The over-payments to credit back, one per re-rated paid item.</returns>
    /// <exception cref="PeriodAlreadyClosedException">Thrown when the period is closed.</exception>
    public IReadOnlyList<OverpaymentCredit> ChangePerApartmentAmount(Money newAmount)
    {
        EnsureOpen();

        var credits = new List<OverpaymentCredit>();
        foreach (var item in _items)
        {
            if (item.Status == BillingItemStatus.Paid && item.Amount.Amount > newAmount.Amount)
            {
                credits.Add(new OverpaymentCredit(item.ResidentId, item.Amount.Subtract(newAmount)));
            }

            item.ChangeAmount(newAmount);
        }

        PerApartmentAmount = newAmount;
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
