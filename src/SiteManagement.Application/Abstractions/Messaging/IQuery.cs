using MediatR;

namespace SiteManagement.Application.Abstractions.Messaging;

/// <summary>
/// Marker for a read request (no state change). The counterpart of
/// <see cref="ICommand{TResult}"/>: queries are deliberately excluded from
/// <c>TransactionBehavior</c>, so a read never opens a write transaction.
/// Pairing both markers lets an architecture test assert that every MediatR
/// request is explicitly one or the other — a plain <see cref="IRequest{T}"/>
/// then fails the build, so "is this a command or a query?" can never be left
/// ambiguous or forgotten.
/// </summary>
/// <typeparam name="TResult">The value the query returns.</typeparam>
public interface IQuery<out TResult> : IRequest<TResult>;
