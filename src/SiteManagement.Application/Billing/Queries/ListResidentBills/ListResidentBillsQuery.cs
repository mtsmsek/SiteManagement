using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Billing.Queries.ListResidentBills;

/// <summary>Returns every outstanding + paid line owed by a resident. Admin-only.</summary>
public sealed record ListResidentBillsQuery(Guid ResidentId) : IQuery<IReadOnlyList<ResidentBillDto>>, IAdminRequest;
