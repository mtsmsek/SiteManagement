using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Billing.Commands.MarkUtilityBillItemPaid;

/// <summary>
/// Marks a single utility bill item paid within its period. Idempotent — paying
/// an already-paid item is a no-op. Admin-only.
/// </summary>
public sealed record MarkUtilityBillItemPaidCommand(Guid UtilityBillPeriodId, Guid ItemId) : ICommand, IAdminRequest;
