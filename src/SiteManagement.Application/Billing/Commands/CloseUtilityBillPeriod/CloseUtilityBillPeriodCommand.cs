using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Billing.Commands.CloseUtilityBillPeriod;

/// <summary>
/// Locks a utility bill period after distribution. Closing raises
/// <c>UtilityBillPeriodClosed</c>, which a notification handler turns into a
/// utility bill e-mail to each billed resident. Admin-only.
/// </summary>
public sealed record CloseUtilityBillPeriodCommand(Guid UtilityBillPeriodId) : ICommand, IAdminRequest;
