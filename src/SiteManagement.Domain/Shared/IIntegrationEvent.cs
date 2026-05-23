namespace SiteManagement.Domain.Shared;

/// <summary>
/// A domain event whose handlers run as <em>side effects after</em> the
/// transaction commits (emails, real-time pushes, external publishes) rather
/// than as part of the same atomic write. The unit of work persists these to
/// the outbox inside the transaction; a background processor dispatches them
/// once the data is durably committed, so a slow or failing side effect can
/// never roll back the business operation that produced it.
/// </summary>
public interface IIntegrationEvent : IDomainEvent;
