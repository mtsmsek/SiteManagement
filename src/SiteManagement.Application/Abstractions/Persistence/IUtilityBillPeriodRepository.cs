using SiteManagement.Domain.Billing;

namespace SiteManagement.Application.Abstractions.Persistence;

/// <summary>
/// Command-side repository for the <see cref="UtilityBillPeriod"/> aggregate.
/// Read-side projections live behind <see cref="Billing.Queries.IBillingQueries"/>.
/// </summary>
public interface IUtilityBillPeriodRepository : IRepository<UtilityBillPeriod>
{
}
