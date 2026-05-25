using SiteManagement.Domain.Shared.ValueObjects;

namespace SiteManagement.Domain.Billing;

/// <summary>
/// An over-payment owed back to a resident after a paid billing item was
/// corrected to a lower amount. Returned by a period's amount-change so the
/// application can post it to the resident's <see cref="ResidentCreditAccount"/>
/// in the same transaction — the period aggregate doesn't reach across into
/// another aggregate itself.
/// </summary>
/// <param name="ResidentId">The resident who over-paid.</param>
/// <param name="Amount">The amount to credit back (old amount minus new amount).</param>
public sealed record OverpaymentCredit(Guid ResidentId, Money Amount);
