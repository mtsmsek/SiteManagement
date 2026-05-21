using MediatR;

namespace SiteManagement.Application.Abstractions.Messaging;

/// <summary>
/// Marker for a write request (state change). Distinguished from a plain query
/// so the <c>TransactionBehavior</c> can wrap commands — and only commands — in
/// a database transaction automatically. Handlers therefore never manage
/// transactions themselves: correctness lives in the pipeline, not in each
/// handler's memory of whether its events trigger a second save.
/// </summary>
public interface ICommand : IRequest;

/// <summary>A write request that returns a result.</summary>
/// <typeparam name="TResult">The value the command returns.</typeparam>
public interface ICommand<out TResult> : IRequest<TResult>;
