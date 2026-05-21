using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Billing.Commands.OpenDuesPeriod;

/// <summary>
/// Opens an empty dues period for a site at a fixed per-apartment amount.
/// Items are added later by <c>DistributeDuesCommand</c>. Admin-only.
/// </summary>
public sealed record OpenDuesPeriodCommand(
    Guid SiteId,
    int Year,
    int Month,
    decimal PerApartmentAmount) : ICommand<OpenDuesPeriodResult>;

/// <summary>Result carrying the new dues period's identifier.</summary>
public sealed record OpenDuesPeriodResult(Guid DuesPeriodId);
