using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Billing.Commands.CloseDuesPeriod;

/// <summary>
/// Locks a dues period after distribution. Closing raises
/// <c>DuesPeriodClosed</c>, which a notification handler turns into a dues
/// e-mail to each billed resident. Admin-only.
/// </summary>
public sealed record CloseDuesPeriodCommand(Guid DuesPeriodId) : ICommand;
