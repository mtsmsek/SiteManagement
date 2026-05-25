using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Domain.Billing;
using SiteManagement.Domain.Shared.ValueObjects;

namespace SiteManagement.Application.Billing.Services;

/// <summary>
/// Default <see cref="IResidentCreditService"/>: resolves (or opens) a
/// resident's <see cref="ResidentCreditAccount"/> and delegates the balance
/// arithmetic to the aggregate. A newly opened account is staged with the
/// repository; existing accounts are already tracked, so the unit of work
/// persists them on the caller's commit.
/// </summary>
public sealed class ResidentCreditService(IResidentCreditAccountRepository accounts) : IResidentCreditService
{
    private readonly IResidentCreditAccountRepository _accounts = accounts;

    /// <inheritdoc />
    public async Task ApplyCreditsAsync(IReadOnlyList<OverpaymentCredit> credits, CancellationToken ct = default)
    {
        foreach (var credit in credits)
        {
            var account = await _accounts.GetByResidentIdAsync(credit.ResidentId, ct);
            if (account is null)
            {
                account = ResidentCreditAccount.Open(credit.ResidentId);
                account.AddCredit(credit.Amount);
                await _accounts.AddAsync(account, ct);
            }
            else
            {
                account.AddCredit(credit.Amount);
            }
        }
    }

    /// <inheritdoc />
    public async Task<bool> TryConsumeAsync(Guid residentId, Money owed, CancellationToken ct = default)
    {
        var account = await _accounts.GetByResidentIdAsync(residentId, ct);
        if (account is null)
        {
            return false;
        }

        var applied = account.Consume(owed);
        return applied.Amount > 0m;
    }
}
