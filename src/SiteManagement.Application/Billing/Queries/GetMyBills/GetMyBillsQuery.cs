using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.Application.Billing.Queries.GetMyBills;

/// <summary>
/// Returns every outstanding + paid line owed by the <em>current</em> resident.
/// Token-scoped: the resident id comes from the authenticated caller, never from
/// the request, so a resident can only ever read their own bills (no IDOR
/// surface). The admin equivalent is <c>ListResidentBillsQuery</c>.
/// </summary>
public sealed record GetMyBillsQuery : IQuery<IReadOnlyList<ResidentBillDto>>, IResidentRequest;
