using SiteManagement.Domain.Shared.Exceptions;

namespace SiteManagement.Domain.Tenancy.Exceptions;

/// <summary>
/// Thrown when an apartment assignment that has already ended is ended again.
/// </summary>
public sealed class AssignmentAlreadyEndedException : DomainException
{
    /// <summary>Creates the exception for the already-closed assignment.</summary>
    public AssignmentAlreadyEndedException(Guid assignmentId)
        : base(TenancyMessageKeys.AssignmentAlreadyEnded, assignmentId)
    {
    }
}
