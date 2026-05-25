using SiteManagement.Domain.Billing;

namespace SiteManagement.Application.Abstractions.Persistence;

/// <summary>
/// Command-side repository for the <see cref="ResidentCreditAccount"/> aggregate.
/// One account per resident; the lookup-by-resident is the natural entry point
/// when crediting an over-payment or consuming credit against a new bill.
/// </summary>
public interface IResidentCreditAccountRepository : IRepository<ResidentCreditAccount>
{
    /// <summary>Loads a resident's credit account, or null when they have none yet.</summary>
    Task<ResidentCreditAccount?> GetByResidentIdAsync(Guid residentId, CancellationToken ct = default);
}
