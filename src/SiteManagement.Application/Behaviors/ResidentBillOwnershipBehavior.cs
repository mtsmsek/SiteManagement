using MediatR;
using SiteManagement.Application.Abstractions.Auth;
using SiteManagement.Application.Billing;
using SiteManagement.Application.Billing.Queries;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Application.Shared.Resources;

namespace SiteManagement.Application.Behaviors;

/// <summary>
/// Resource-based authorization for resident self-service: for any
/// <see cref="IOwnedBillItemRequest"/>, verifies the targeted item is among the
/// caller's own bills before the handler runs (else 403). This keeps ownership —
/// the IDOR guard — out of the handlers entirely: role is proven by
/// <see cref="AuthorizationBehavior{TRequest,TResponse}"/>, ownership here, and
/// the handler just does the work. Requests that aren't item-scoped pass
/// straight through.
/// </summary>
/// <remarks>
/// Runs after the role gate and before validation/transaction. Reads the
/// caller's bills (a query that spans both dues and utility), so a resident can
/// only ever act on an item that actually belongs to them — naming someone
/// else's item id simply fails the membership check.
/// </remarks>
public class ResidentBillOwnershipBehavior<TRequest, TResponse>(
    ICurrentUser currentUser,
    IBillingQueries billingQueries)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IBillingQueries _billingQueries = billingQueries;

    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is IOwnedBillItemRequest owned)
        {
            var residentId = _currentUser.ResidentId
                ?? throw new UnauthorizedActionException(ErrorMessageKeys.Forbidden);

            var bills = await _billingQueries.ListResidentBillsAsync(residentId, cancellationToken);
            if (bills.All(b => b.ItemId != owned.ItemId))
            {
                throw new UnauthorizedActionException(ErrorMessageKeys.PaymentNotYourBill);
            }
        }

        return await next();
    }
}
