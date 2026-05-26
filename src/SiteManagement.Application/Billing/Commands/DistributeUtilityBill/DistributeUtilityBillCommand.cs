using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Billing.Commands.DistributeUtilityBill;

/// <summary>
/// Splits the period's total amount equally across every currently-occupied
/// apartment in the period's site, adding a bill item for each. Admin-only.
/// </summary>
public sealed record DistributeUtilityBillCommand(Guid UtilityBillPeriodId) : ICommand, IAdminRequest;
