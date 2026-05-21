using MediatR;
using SiteManagement.Application.Abstractions.Messaging;
using SiteManagement.Application.Abstractions.Persistence;

namespace SiteManagement.Application.Behaviors;

/// <summary>
/// Wraps every <see cref="ICommand"/> / <see cref="ICommand{TResult}"/> in a
/// database transaction so a handler — and any domain-event handler it triggers
/// in a follow-up save — commit atomically or roll back together. Handlers
/// therefore never open scopes themselves; the "does this flow produce a second
/// save?" question no longer has to live in each developer's memory.
/// </summary>
/// <remarks>
/// Runs after <c>ExceptionTranslationBehavior</c> (so it sits closest to the
/// handler): the handler runs inside the scope, the scope commits only on a
/// clean return, and any exception bubbling out leaves the scope uncommitted —
/// <c>IUnitOfWorkScope.DisposeAsync</c> then rolls back. Queries don't
/// implement <see cref="ICommand"/>, so they bypass this behavior entirely.
/// </remarks>
public class TransactionBehavior<TRequest, TResponse>(IUnitOfWork unitOfWork)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not ICommand and not ICommand<TResponse>)
        {
            return await next();
        }

        await using var scope = await _unitOfWork.BeginScopeAsync(cancellationToken);
        var response = await next();
        await scope.CommitAsync(cancellationToken);
        return response;
    }
}
