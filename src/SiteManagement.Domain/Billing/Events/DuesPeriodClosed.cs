using SiteManagement.Domain.Shared;

namespace SiteManagement.Domain.Billing.Events;

/// <summary>
/// Raised when a dues period is closed (locked after distribution). An
/// Application-side handler reacts by emailing each billed resident their
/// dues amount for the month.
/// </summary>
/// <param name="DuesPeriodId">The closed period.</param>
/// <param name="SiteId">The site the period belongs to.</param>
public sealed record DuesPeriodClosed(Guid DuesPeriodId, Guid SiteId) : IDomainEvent
{
    /// <inheritdoc />
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
