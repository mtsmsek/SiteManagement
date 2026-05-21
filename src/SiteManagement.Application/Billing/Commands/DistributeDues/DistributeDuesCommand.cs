using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Billing.Commands.DistributeDues;

/// <summary>
/// Adds a dues item for every currently-occupied apartment in the period's
/// site, each at the period's fixed per-apartment amount. Admin-only.
/// </summary>
public sealed record DistributeDuesCommand(Guid DuesPeriodId) : ICommand;
