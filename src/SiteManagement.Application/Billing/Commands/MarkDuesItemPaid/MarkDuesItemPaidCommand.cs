using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Billing.Commands.MarkDuesItemPaid;

/// <summary>
/// Marks a single dues item paid within its period. Idempotent — paying an
/// already-paid item is a no-op. Admin-only.
/// </summary>
public sealed record MarkDuesItemPaidCommand(Guid DuesPeriodId, Guid ItemId) : ICommand, IAdminRequest;
