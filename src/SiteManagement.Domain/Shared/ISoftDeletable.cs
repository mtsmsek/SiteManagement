namespace SiteManagement.Domain.Shared;

/// <summary>
/// Read-side marker for an entity that can be <em>archived</em> rather than
/// physically deleted, so dependent history (assignments, billing, …) survives.
/// A global EF query filter hides archived rows from every read unless a caller
/// explicitly opts back in with <c>IgnoreQueryFilters()</c>. Archiving itself is
/// an explicit domain operation (each aggregate exposes an <c>Archive</c>
/// method) — deletion is never silently turned into a flag flip.
/// </summary>
public interface ISoftDeletable
{
    /// <summary>True once the entity has been archived.</summary>
    bool IsDeleted { get; }

    /// <summary>When it was archived, in UTC; null while live.</summary>
    DateTime? DeletedOnUtc { get; }
}
