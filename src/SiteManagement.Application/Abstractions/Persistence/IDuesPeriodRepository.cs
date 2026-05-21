using SiteManagement.Domain.Billing;

namespace SiteManagement.Application.Abstractions.Persistence;

/// <summary>
/// Command-side repository for the <see cref="DuesPeriod"/> aggregate.
/// Read-side projections live behind <see cref="Billing.Queries.IBillingQueries"/>.
/// </summary>
public interface IDuesPeriodRepository : IRepository<DuesPeriod>
{
}
