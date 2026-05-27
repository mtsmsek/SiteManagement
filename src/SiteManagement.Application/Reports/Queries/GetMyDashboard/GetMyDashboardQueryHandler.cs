using MediatR;
using SiteManagement.Application.Abstractions.Auth;
using SiteManagement.Application.Billing.Queries;
using SiteManagement.Application.Messaging.Queries;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Application.Shared.Resources;
using SiteManagement.Domain.Billing;

namespace SiteManagement.Application.Reports.Queries.GetMyDashboard;

/// <summary>
/// Composes the resident's landing summary from the existing Billing + Messaging
/// read sides — no new persistence. Token-scoped: everything is keyed on the
/// caller's resident id.
/// </summary>
public sealed class GetMyDashboardQueryHandler(
    ICurrentUser currentUser,
    IBillingQueries billingQueries,
    IMessagingQueries messagingQueries)
    : IRequestHandler<GetMyDashboardQuery, ResidentDashboardDto>
{
    private static readonly string Unpaid = nameof(BillingItemStatus.Unpaid);

    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IBillingQueries _billingQueries = billingQueries;
    private readonly IMessagingQueries _messagingQueries = messagingQueries;

    /// <inheritdoc />
    public async Task<ResidentDashboardDto> Handle(GetMyDashboardQuery request, CancellationToken cancellationToken)
    {
        var residentId = _currentUser.ResidentId
            ?? throw new UnauthorizedActionException(ErrorMessageKeys.Forbidden);

        var bills = await _billingQueries.ListResidentBillsAsync(residentId, cancellationToken);
        var unpaid = bills.Where(b => b.Status == Unpaid).ToList();

        var credit = await _billingQueries.GetResidentCreditAsync(residentId, cancellationToken);

        var conversations = await _messagingQueries.ListForResidentAsync(residentId, cancellationToken);
        var unread = conversations.Sum(c => c.UnreadForResident);

        return new ResidentDashboardDto(
            TotalOutstanding: unpaid.Sum(b => b.Amount),
            TotalCredit: credit,
            UnpaidCount: unpaid.Count,
            UnreadMessages: unread);
    }
}
