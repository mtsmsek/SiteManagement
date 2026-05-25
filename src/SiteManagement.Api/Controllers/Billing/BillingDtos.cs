using SiteManagement.Domain.Billing;

namespace SiteManagement.Api.Controllers.Billing;

/// <summary>Request body for <c>POST /api/dues</c>.</summary>
public sealed record OpenDuesPeriodRequest(
    Guid SiteId,
    int Year,
    int Month,
    decimal PerApartmentAmount);

/// <summary>Request body for <c>POST /api/utility-bills</c>.</summary>
public sealed record OpenUtilityBillRequest(
    Guid SiteId,
    int Year,
    int Month,
    UtilityType UtilityType,
    decimal TotalAmount);

/// <summary>Request body for <c>PUT /api/dues/{id}</c> — corrects the per-apartment amount.</summary>
public sealed record ChangeDuesAmountRequest(decimal PerApartmentAmount);

/// <summary>Request body for <c>PUT /api/utility-bills/{id}</c> — corrects the invoice total.</summary>
public sealed record ChangeUtilityBillAmountRequest(decimal TotalAmount);

/// <summary>
/// Card details for paying a billing item by card. The amount is taken from the
/// item server-side; the card is passed straight to the payment gateway and
/// never stored by this API.
/// </summary>
public sealed record PayByCardRequest(
    string CardNumber,
    string Cvv,
    int ExpiryYear,
    int ExpiryMonth);
