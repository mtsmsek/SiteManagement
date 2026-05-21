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
