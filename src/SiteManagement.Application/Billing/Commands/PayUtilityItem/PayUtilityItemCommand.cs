using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Billing.Commands.PayUtilityItem;

/// <summary>
/// Pays a single utility bill item by card: charges the amount through the
/// payment gateway and, only if the charge succeeds, marks the item paid. The
/// card details are passed straight through to the gateway (never stored here).
/// Admin-only for now; moves to the resident portal in W5.
/// </summary>
public sealed record PayUtilityItemCommand(
    Guid UtilityBillPeriodId,
    Guid ItemId,
    string CardNumber,
    string Cvv,
    int ExpiryYear,
    int ExpiryMonth) : ICommand;
