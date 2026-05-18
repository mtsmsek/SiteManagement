using SiteManagement.Domain.Shared;

namespace SiteManagement.Application.Abstractions.Persistence;

/// <summary>
/// Command-side persistence port for an aggregate root. Constraining
/// <typeparamref name="TRoot"/> to <see cref="AggregateRoot{TId}"/> means
/// the type system refuses to load or save inner entities on their own,
/// keeping the consistency boundary intact.
/// </summary>
/// <remarks>
/// Read-side projections (<c>SiteListItemDto</c>, <c>ResidentDetailsDto</c>,
/// ...) go through the per-context <c>I{Aggregate}Queries</c> ports instead
/// — repositories deal only in fully-hydrated aggregates so command
/// handlers never accidentally mutate a half-baked projection.
/// </remarks>
public interface IRepository<TRoot> where TRoot : AggregateRoot<Guid>
{
    /// <summary>Loads the aggregate by its identifier, eager-loading every member of the consistency boundary.</summary>
    /// <returns><c>null</c> when no aggregate with that id exists.</returns>
    Task<TRoot?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Stages a brand-new aggregate; the write happens in <see cref="IUnitOfWork.SaveChangesAsync"/>.</summary>
    Task AddAsync(TRoot root, CancellationToken ct = default);

    /// <summary>Stages an aggregate for deletion.</summary>
    void Remove(TRoot root);
}
