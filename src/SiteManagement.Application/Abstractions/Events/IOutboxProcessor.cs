namespace SiteManagement.Application.Abstractions.Events;

/// <summary>
/// Delivers pending outbox messages: rehydrates each persisted integration
/// event and dispatches it to its in-process handlers, after the transaction
/// that produced it has committed. A background service drives this on a timer;
/// tests can call it directly for a deterministic, non-timing-dependent run.
/// </summary>
public interface IOutboxProcessor
{
    /// <summary>Processes the pending batch and returns how many were delivered.</summary>
    Task<int> ProcessPendingAsync(CancellationToken ct = default);
}
