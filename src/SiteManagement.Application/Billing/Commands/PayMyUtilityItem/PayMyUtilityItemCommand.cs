using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Billing.Commands.PayMyUtilityItem;

/// <summary>
/// Resident self-service: pays one of the caller's own utility bill items by
/// card. Mirrors the admin <c>PayUtilityItemCommand</c> but is resident-scoped:
/// as an <see cref="IOwnedBillItemRequest"/>, ownership of <see cref="ItemId"/>
/// is verified by the pipeline before the handler runs, so the handler carries
/// no authorization code. Card details flow straight to the gateway, never stored.
/// </summary>
public sealed record PayMyUtilityItemCommand(
    Guid UtilityBillPeriodId,
    Guid ItemId,
    string CardNumber,
    string Cvv,
    int ExpiryYear,
    int ExpiryMonth) : ICommand, IOwnedBillItemRequest;
