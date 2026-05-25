using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Billing.Commands.ChangeDuesAmount;

/// <summary>
/// Corrects an open dues period's per-apartment amount and re-rates its items.
/// Unpaid items follow the new rate; a paid item that was over-charged credits
/// the difference back to the resident (held as credit, not refunded). Admin-only.
/// </summary>
public sealed record ChangeDuesAmountCommand(Guid DuesPeriodId, decimal PerApartmentAmount) : ICommand;
