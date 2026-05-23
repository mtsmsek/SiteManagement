using SiteManagement.Domain.Shared;

namespace SiteManagement.Domain.Billing.Events;

/// <summary>
/// Raised when a utility bill period is closed. An Application-side handler
/// reacts by emailing each billed resident their share for the month.
/// </summary>
/// <param name="UtilityBillPeriodId">The closed period.</param>
/// <param name="SiteId">The site the period belongs to.</param>
/// <param name="UtilityType">Which utility was billed.</param>
public sealed record UtilityBillPeriodClosed(
    Guid UtilityBillPeriodId,
    Guid SiteId,
    UtilityType UtilityType) : IIntegrationEvent
{
    /// <inheritdoc />
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
