using SiteManagement.Domain.Billing;

namespace SiteManagement.Application.Billing.Services;

/// <summary>
/// Orchestrates the per-resident credit balance across Billing commands: posts
/// over-payments produced by an amount correction, and applies existing credit
/// to a freshly billed item. Keeps the find-or-create account plumbing out of
/// the command handlers so the dues and utility sides share one implementation.
/// </summary>
public interface IResidentCreditService
{
    /// <summary>
    /// Posts each over-payment to its resident's credit account, opening an
    /// account on first credit. Runs inside the caller's transaction.
    /// </summary>
    Task ApplyCreditsAsync(IReadOnlyList<OverpaymentCredit> credits, CancellationToken ct = default);

    /// <summary>
    /// Tries to settle a bill of <paramref name="owed"/> from the resident's
    /// credit. Succeeds only when the balance fully covers it (no partial
    /// settlement); on success the balance is reduced.
    /// </summary>
    /// <returns><c>true</c> when credit covered the bill and it should be marked paid.</returns>
    Task<bool> TryConsumeAsync(Guid residentId, Domain.Shared.ValueObjects.Money owed, CancellationToken ct = default);
}
