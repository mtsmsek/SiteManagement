using SiteManagement.Application.Abstractions.Persistence;

namespace SiteManagement.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core-backed <see cref="IUnitOfWork"/>: delegates straight to the
/// <see cref="AppDbContext"/> change tracker so handlers only need one
/// dependency to commit a command.
/// </summary>
public sealed class EfUnitOfWork(AppDbContext dbContext) : IUnitOfWork
{
    private readonly AppDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _dbContext.SaveChangesAsync(ct);
}
