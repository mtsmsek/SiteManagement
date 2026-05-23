namespace SiteManagement.Domain.Shared;

/// <summary>
/// An entity that records who created/last-modified it and when. The values are
/// stamped by an infrastructure save-interceptor (ambient metadata, not a
/// business operation), so the domain only exposes them for reading. Carried by
/// <see cref="AggregateRoot{TId}"/> so every aggregate root is audited.
/// </summary>
public interface IAuditableEntity
{
    /// <summary>When the entity was first persisted, in UTC.</summary>
    DateTime CreatedAtUtc { get; }

    /// <summary>The user who created it; null for system/unauthenticated writes.</summary>
    Guid? CreatedBy { get; }

    /// <summary>When the entity was last updated, in UTC; null until first modified.</summary>
    DateTime? ModifiedAtUtc { get; }

    /// <summary>The user who last updated it; null for system/unauthenticated writes.</summary>
    Guid? ModifiedBy { get; }
}
