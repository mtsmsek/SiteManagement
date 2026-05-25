using Microsoft.EntityFrameworkCore;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Domain.Billing;

namespace SiteManagement.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core-backed <see cref="IResidentCreditAccountRepository"/>. The account is
/// a flat aggregate (just a balance), so there is nothing extra to eager-load.
/// </summary>
public sealed class ResidentCreditAccountRepository(AppDbContext dbContext) : IResidentCreditAccountRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<ResidentCreditAccount?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _dbContext.ResidentCreditAccounts.FirstOrDefaultAsync(a => a.Id == id, ct);

    /// <inheritdoc />
    public Task<ResidentCreditAccount?> GetByResidentIdAsync(Guid residentId, CancellationToken ct = default)
        => _dbContext.ResidentCreditAccounts.FirstOrDefaultAsync(a => a.ResidentId == residentId, ct);

    /// <inheritdoc />
    public async Task AddAsync(ResidentCreditAccount account, CancellationToken ct = default)
    {
        await _dbContext.ResidentCreditAccounts.AddAsync(account, ct);
    }

    /// <inheritdoc />
    public void Remove(ResidentCreditAccount account) => _dbContext.ResidentCreditAccounts.Remove(account);
}
