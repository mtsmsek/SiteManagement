using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Billing.Commands.ChangeUtilityBillAmount;

/// <summary>
/// Corrects an open utility bill period's invoice total and re-splits it across
/// the distributed items. Unpaid shares follow the new split; a paid item that
/// was over-charged credits the difference back to the resident (held as credit,
/// not refunded). Admin-only.
/// </summary>
public sealed record ChangeUtilityBillAmountCommand(Guid UtilityBillPeriodId, decimal TotalAmount) : ICommand;
