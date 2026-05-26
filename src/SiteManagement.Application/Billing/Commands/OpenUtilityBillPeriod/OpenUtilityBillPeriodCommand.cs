using SiteManagement.Application.Abstractions.Messaging;
using SiteManagement.Domain.Billing;

namespace SiteManagement.Application.Billing.Commands.OpenUtilityBillPeriod;

/// <summary>
/// Opens an empty utility bill period for a site at a fixed total amount for a
/// given utility type. Items are added later by <c>DistributeUtilityBillCommand</c>,
/// which splits the total across occupants. Admin-only.
/// </summary>
public sealed record OpenUtilityBillPeriodCommand(
    Guid SiteId,
    int Year,
    int Month,
    UtilityType UtilityType,
    decimal TotalAmount) : ICommand<OpenUtilityBillPeriodResult>, IAdminRequest;

/// <summary>Result carrying the new utility bill period's identifier.</summary>
public sealed record OpenUtilityBillPeriodResult(Guid UtilityBillPeriodId);
