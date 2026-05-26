using MediatR;
using SiteManagement.Application.Abstractions.Auth;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Application.Shared.Resources;

namespace SiteManagement.Application.Billing.Queries.GetMyBills;

/// <summary>
/// Resolves the caller's resident id from <see cref="ICurrentUser"/> and
/// delegates to <see cref="IBillingQueries"/> — pure read path. The
/// authorization behavior already proved the caller is a resident; the
/// <c>?? throw</c> is a defensive backstop.
/// </summary>
public sealed class GetMyBillsQueryHandler(ICurrentUser currentUser, IBillingQueries billingQueries)
    : IRequestHandler<GetMyBillsQuery, IReadOnlyList<ResidentBillDto>>
{
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IBillingQueries _billingQueries = billingQueries;

    /// <inheritdoc />
    public Task<IReadOnlyList<ResidentBillDto>> Handle(GetMyBillsQuery request, CancellationToken cancellationToken)
    {
        var residentId = _currentUser.ResidentId
            ?? throw new UnauthorizedActionException(ErrorMessageKeys.Forbidden);

        return _billingQueries.ListResidentBillsAsync(residentId, cancellationToken);
    }
}
